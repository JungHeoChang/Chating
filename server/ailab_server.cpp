/* gcc -o new_msg_server new_msg_server.cpp -lmysqlclient -lpthread */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>
#include "/usr/include/mysql/mysql.h"
#include <errno.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <time.h>
#include <pthread.h>
#include <sys/shm.h>
#include <sys/ipc.h>
#include <sys/wait.h>
#include <sys/signal.h>

#define BUF_SIZE 1024
#define PORT 11100
#define DB_HOST "127.0.0.1"
#define DB_USER "root"
#define DB_PASS "Ailabpi" // 교수님 방에서 바꾸기
#define DB_NAME "talk"
#define DB_NAME_USER "user"
#define LISTENQ 100
#define userBuf 15
#define textBuf 257
#define stateBuf 9
#define verBuf 10
#define timeBuf 23

void Error_Handling(const char *);

void TCP();
void Send_Msg(int, char *, char *, char *);
int Recv_Msg(int);

void Mysql_server();
void User_Mysql_server();
void Send_Sql(char *, char *, char *);
void Recv_Sql(int);

void Thread(int);
void * Send_Function(void *);
void * Recv_Function(void *);

void Pid_Handling();
void Error_Handling(const char *);
void z_handler(int);

int CharToInt(char);
char * read_count(const char *);
char * send_count(const char *, int);

void Current_user(int, char *);
void Current_user_add(int, char *);
void Current_user_remove(int, char *);
char * Current_user_count(int, int);

void Check_Update(int);

void Send_Before_Msg(int , char * , char * , char * , char *);
void Before_Message(int);

typedef struct client_info {
    char clientAddr[32];
    int clientPort;
} Info;

typedef struct {
    char ID[userBuf];
    char msg[textBuf];
    char time[20];
    char state[6];
} db_info;

struct check_data {
    int check;
    char data[BUF_SIZE];
} *shared_data;

MYSQL * connection = NULL, conn;
MYSQL * user_connection = NULL, user_conn;
pthread_mutex_t query_mutex; // query문에 대한 상호배제
pthread_mutex_t user_query_mutex;

char msg_time[20];
char query[355];
char ID[userBuf];
char out_user[userBuf];
void * shared_memory = (void *)0;
void * count_memory = (void *)0;
char * user_list;
db_info db;

int * user_count;
bool first = true;
bool prog_exit = false;
bool user_add = true;

int main()
{
    TCP();
    return 0;
}

void TCP()
{
    int server_sd, client_sd;
    struct sockaddr_in server_adr;
    struct sockaddr_in client_adr;
    Info client_list[20];
    int conNum = 0;
    int pid;
    int shmid;
    int shmid2;
    int shmid3;
    struct sigaction act;
    int state;

    socklen_t clnt_adr_sz;
    server_sd = socket(PF_INET, SOCK_STREAM, 0);

    if(server_sd == -1)
        Error_Handling("socket() : Error");

    memset(&server_adr, 0, sizeof(server_adr));

    server_adr.sin_family = AF_INET;
    server_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    server_adr.sin_port = htons(PORT);

    if(bind(server_sd, (struct sockaddr *)&server_adr, sizeof(server_adr))
                == -1)
    {
        Error_Handling("bind() : error");
    }
    
    listen(server_sd, LISTENQ);

    if(listen(server_sd, 5) == -1)
        Error_Handling("listen()  error");

    act.sa_handler = z_handler;
    sigemptyset(&act.sa_mask);
    act.sa_flags = 0;
    state = sigaction(SIGCHLD, &act, 0);

    if(state != 0)
        puts("sigaction() error\n");

    /* db_info */
    shmid = shmget((key_t)1233, 4048, 0666|IPC_CREAT);

    if(shmid == -1)
    {
        perror("shmget failed : ");
    }

    shared_memory = shmat(shmid, (void *)NULL, 0);

    db = *(db_info*)shared_memory;

    if(shared_memory == (void *) -1)
    {
        perror("shmat failed : ");
    }

    /* user_list */    
    shmid2 = shmget((key_t)1234, 2048, 0666|IPC_CREAT);
        
    if(shmid2 == -1)
    {
        perror("shmget failed : ");
    }    

    user_list = (char*)shmat(shmid2, (char *)NULL, 0);
    
    /* user count */
    shmid3 = shmget((key_t)1235, sizeof(int), 0666|IPC_CREAT);

    if(shmid3 == -1)
    {
        perror("shmget failed : ");
    }

    user_count = (int *)shmat(shmid3, NULL, 0);

    if(user_count == (int*)-1)
    {
        perror("shmat2 falied : ");
    }
    *user_count = 0;

    while(1)
    {
        clnt_adr_sz = sizeof(client_adr);

        client_sd = accept(server_sd, (struct sockaddr *)&client_adr,
                &clnt_adr_sz);

        if(conNum < 20)
        {
            strcpy(client_list[conNum].clientAddr, inet_ntoa(client_adr.sin_addr));
            client_list[conNum].clientPort = ntohs(client_adr.sin_port);
            conNum++;
        }

        printf("Client IP : %s, Port : %d\n", client_list[conNum-1].clientAddr,
                client_list[conNum-1].clientPort);

        if(client_sd == -1)
        {
            Error_Handling("accept() : error");
            continue;
        }

        printf("client : %d\n", client_sd);

        pid = fork();
        if(pid == -1)
        {
            close(client_sd);
            continue;
        }
        else if(pid > 0)
        {
            close(client_sd);
            continue;
        }
        else if(pid == 0)
        {
            printf("child pid : %d parent : %d\n", getpid(), getppid());

            /* 업데이트 확인 */
            Check_Update(client_sd); 

            Mysql_server();
            User_Mysql_server();

            Before_Message(client_sd);

            Thread(client_sd);
            
            conNum--;
            close(client_sd);    
            mysql_close(connection);        
            exit(0);
        }
    }
    
    if(shmctl(shmid, IPC_RMID, 0) == -1)
    {
        perror("shmctl failed");
    }
    
    if(shmctl(shmid2, IPC_RMID, 0) == -1)
    {
        perror("shmctl2 failed");
    }
    
    if(shmctl(shmid3, IPC_RMID, 0) == -1)
    {
        perror("shmctl3 failed");
    }
}

/* server -> client */
void Send_Msg(int client, char * user, char * text, char * send_true)
{
    char buf[260];
     
    if(strcmp(send_true, "TRUE") == 0)
    {
        strncpy(buf, send_count(send_true, stateBuf), stateBuf);
        write(client, buf, stateBuf);
        memset(buf, 0, sizeof(buf));
        
        strncpy(buf, send_count(user, userBuf), userBuf);
        write(client, buf, userBuf); // sizeof(user) = 4?
        memset(buf, 0, sizeof(buf));

        strncpy(buf, send_count(text, textBuf), textBuf);
        write(client, buf, textBuf);
        memset(buf, 0, sizeof(buf));
    }
    else if(strcmp(send_true, "EXIT") == 0)
    {
        strncpy(buf, send_count(send_true, stateBuf), stateBuf);
        write(client, buf, stateBuf);
        memset(buf, 0, sizeof(buf));
        
        strncpy(buf, send_count(user, userBuf), userBuf);
        write(client, buf, userBuf);
        memset(buf, 0, sizeof(buf));

        if(pthread_mutex_lock(&user_query_mutex) != EBUSY)        
        {
        Current_user_remove(client, user);
        pthread_mutex_unlock(&user_query_mutex);
        }
    }
    else if(strcmp(send_true, "UNEX") == 0)
    {
        strncpy(buf, send_count(send_true, stateBuf), stateBuf);
        write(client, buf, stateBuf);
        memset(buf, 0, sizeof(buf));

        strncpy(buf, send_count(user, userBuf), userBuf);
        write(client, buf, userBuf);
        memset(buf, 0, sizeof(buf));

        if(pthread_mutex_lock(&user_query_mutex) != EBUSY)        
        {
        Current_user_remove(client, user);
        pthread_mutex_unlock(&user_query_mutex);
        }
    }
    else if(strcmp(send_true, "ENTER") == 0)
    {
        strncpy(buf, send_count(send_true, stateBuf), stateBuf);
        write(client, buf, stateBuf);
        memset(buf, 0, sizeof(buf));
        
        strncpy(buf, send_count(user, userBuf), userBuf);
        write(client, buf, userBuf);
        memset(buf, 0, sizeof(buf));

        if(pthread_mutex_lock(&user_query_mutex) != EBUSY)        
        {
        Current_user(client, user);
        pthread_mutex_unlock(&user_query_mutex);        
        }
    }
    else
    {
        strncpy(buf, send_count(send_true, stateBuf), stateBuf);
        write(client, buf, stateBuf);
        memset(buf, 0, sizeof(buf));
    }
}

/* client -> server */
int Recv_Msg(int client)
{
    int read_cnt = 0;
    const char exit_prog[5] = "exit";
    int socket_check = 0;
    char text[textBuf];
    char user[userBuf];
    char recv_true[stateBuf];
    char Buf[260];

    socket_check = read(client, Buf, stateBuf);
    strncpy(recv_true, read_count(Buf), stateBuf);
    memset(Buf, 0, sizeof(Buf));
    
    if(socket_check <= 0)
    {
        printf("socket error\n");
        strcpy(recv_true, "UNEX");
        strcpy(text, "@CONNECTED@ENDED@UNEXPECTEDLY@");
        Send_Sql(out_user, text, recv_true);
        prog_exit = true;
    }

    if(strncmp("TRUE", recv_true, 4) == 0) // user[10]
    {
        read(client, Buf, userBuf);
        strncpy(user, read_count(Buf), userBuf);
        memset(Buf, 0, sizeof(Buf));
        
        read(client, Buf, textBuf);
        strncpy(text, read_count(Buf), textBuf);
        memset(Buf, 0, sizeof(Buf));
        
        if(pthread_mutex_lock(&query_mutex) != EBUSY)        
        {
            Send_Sql(user, text, recv_true);
            pthread_mutex_unlock(&query_mutex);
        }
    }
    else if(strcmp(recv_true, "ENTER") == 0)
    {
        read(client, Buf, userBuf);
        strncpy(user, read_count(Buf), userBuf);
        memset(Buf, 0, sizeof(Buf));
        strcpy(out_user, user);
        
        if(pthread_mutex_lock(&query_mutex) != EBUSY)        
        {
            strcpy(text, "@ENTER@");
            Send_Sql(user, text, recv_true);
            pthread_mutex_unlock(&query_mutex);
            
            if(pthread_mutex_lock(&user_query_mutex) != EBUSY)
            {
                Current_user_add(client, user);
                pthread_mutex_unlock(&user_query_mutex);
            }
        }
    }
    else if(strcmp(recv_true, "EXIT") == 0)
    {
        read(client, Buf, userBuf);
        strncpy(user, read_count(Buf), userBuf);
        memset(Buf, 0, sizeof(Buf));
        strcpy(text, "@EXIT@");

        if(pthread_mutex_lock(&query_mutex) != EBUSY)
        {
            Send_Sql(user, text, recv_true);
            pthread_mutex_unlock(&query_mutex);
        }
        close(client);
        prog_exit = true;
    }
    memset(user, 0, sizeof(user));
    memset(text, 0, sizeof(text));
    memset(recv_true, 0, sizeof(recv_true));
    memset(Buf, 0, sizeof(Buf));
}

void Mysql_server()
{  
    mysql_init(&conn);
    mysql_options(&conn, MYSQL_SET_CHARSET_NAME, "utf8");

    connection = mysql_real_connect(&conn, DB_HOST, DB_USER, DB_PASS,
            DB_NAME, 3306, (char *)NULL, 0);

    if(connection == NULL)
    {
        fprintf(stderr, "Mysql connection error : %s", mysql_error(&conn));
    }
}

void User_Mysql_server()
{  
    mysql_init(&user_conn);
    mysql_options(&user_conn, MYSQL_SET_CHARSET_NAME, "utf8");

    user_connection = mysql_real_connect(&user_conn, DB_HOST, DB_USER, DB_PASS,
            DB_NAME_USER, 3306, (char *)NULL, 0);

    if(user_connection == NULL)
    {
        fprintf(stderr, "Mysql user_connection error : %s", mysql_error(&user_conn));
    }
}

/* server -> DB */
void Send_Sql(char * user, char * buf, char * msg_state)
{
    MYSQL_RES * sql_result;
    MYSQL_ROW sql_row;

    int query_stat = 0;
    char send_time[20];
    time_t timer;
    struct tm * t;
    strcpy(ID, user);
    time(&timer);
    t = localtime(&timer);
    strftime(send_time, 20, "%Y%m%d%H%M%S", t); // 시간 저장
    
    strcpy((*(db_info*)shared_memory).ID, user);
    strcpy((*(db_info*)shared_memory).msg, buf);
    strcpy((*(db_info*)shared_memory).time, send_time);
    strcpy((*(db_info*)shared_memory).state, msg_state);

    sprintf(query, "insert into message values " 
            "('%s', '%s', '%s', '%s')",
            (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg,
            (*(db_info*)shared_memory).time, (*(db_info*)shared_memory).state);

    
    query_stat = mysql_query(connection, query);
    
    if(query_stat != 0)     
    {
        fprintf(stderr, "Send_Mysql query error : %s", mysql_error(&conn));
    }
}

/* DB -> server */
void Recv_Sql(int client)
{
    MYSQL_RES * sql_result;
    MYSQL_ROW sql_row;
    
    int query_stat;
    char send_true[6];
    
    sprintf(query, "select * from message where time = " 
           "(select time from message order by time desc limit 1)");

    query_stat = mysql_query(connection, query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Recv_Mysql query error : %s", mysql_error(&conn));
    }
    
    sql_result = mysql_store_result(connection);

    if(first == true)
    {   
        strcpy(msg_time, (*(db_info*)shared_memory).time);
        first = false;
    }

    if((!(strcmp(msg_time, (*(db_info*)shared_memory).time) == 0)))
    {
        while((sql_row = mysql_fetch_row(sql_result)) != NULL)
        {
            strcpy((*(db_info*)shared_memory).ID, sql_row[0]);
            strcpy((*(db_info*)shared_memory).msg, sql_row[1]);
            strcpy((*(db_info*)shared_memory).time, sql_row[2]);
            strcpy((*(db_info*)shared_memory).state, sql_row[3]);

            if(strncmp((*(db_info*)shared_memory).state, "EXIT", 4) == 0)
            {
                Send_Msg(client, (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg, (*(db_info*)shared_memory).state);
            }
            else if((strcmp((*(db_info*)shared_memory).state, "UNEX") == 0))
            {
                Send_Msg(client, (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg, (*(db_info*)shared_memory).state);
            }
            else if((strcmp((*(db_info*)shared_memory).state, "ENTER") == 0))
            {
                Send_Msg(client, (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg, (*(db_info*)shared_memory).state);
            }
            else
            {
                Send_Msg(client, (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg, (*(db_info*)shared_memory).state);
            }
        }
        strcpy(msg_time, (*(db_info*)shared_memory).time);
    }    
    else
    {
        strcpy(send_true, "FALSE");
        Send_Msg(client, (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg, send_true);
    }
    mysql_free_result(sql_result);
}

void Thread(int client)
{
    int * t_client = &client;
  
    pthread_t pt_send, pt_recv;
    
    pthread_mutex_init(&query_mutex, NULL);
    pthread_mutex_init(&user_query_mutex, NULL);

    pthread_create(&pt_send, NULL, *Send_Function, t_client);
    pthread_create(&pt_recv, NULL, *Recv_Function, t_client);
    
    pthread_join(pt_send, NULL);
    pthread_join(pt_recv, NULL);

    pthread_mutex_destroy(&query_mutex);
    pthread_mutex_destroy(&user_query_mutex);
}

/* DB -> server */
void * Send_Function(void * arg)
{
    int client = *(int *)arg;
    
    while(1)
    {
        usleep(400000);

        if(pthread_mutex_lock(&query_mutex) != EBUSY)
        {
            Recv_Sql(client);
            pthread_mutex_unlock(&query_mutex);
        }

        if(prog_exit == true)
            break;
    }
}

/* client -> server */
void * Recv_Function(void * arg)
{
    int client = *(int *)arg;
    
    while(1)
    {
        usleep(200000);
        
        Recv_Msg(client);
        
        if(prog_exit == true)
            break;
    }
}

char * read_count(const char * text)
{
    int i=0;
    int j=0;
    int k=0;
    int read_num = 0;
    int num = 0;
    static char _text[260];
    char test[260];
    memset(_text, 0, sizeof(_text));
   
    for(i=0; i<3; i++)
    {
        if('0' <= text[i] && text[i] <= '9')
        {
            read_num = CharToInt(text[i]);
        }

        if(i == 0)
        {
            num = 100 * read_num;
        }
        else if(i == 1)
        {
            num += 10 * read_num;
        }
        else if(i == 2)
        {
            num += read_num;
        }
        read_num = 0;
    }
    
    i=3;
    for(j=3; i < (num + 3); j++)
    {
        if(text[j] & 0x80)
        {
            k = j;
            for(j=j; j<k+3; j++)
                _text[j-3] = text[j];
            j--;
        }
        else
        {
            _text[j-3] = text[j];
        }
        i++;
    } 
    return _text;
}

char * send_count(const char * text, int size)
{
    static char _text[260];
    int i=0;
    char num[3];
    int count = 0;
    memset(_text, 0, sizeof(_text));
    
    for(i=0; i < strlen(text); i++)
    {
        if(text[i] & 0x80) // 3개씩 읽기
            i += 2;
        count++;
    }
    
    if(count < 10)
    {
        strcat(_text, "00");
        sprintf(num, "%d", count);
        strcat(_text, num);
        memset(num, 0, sizeof(num));
    }
    else if(count < 100)
    {
        strcat(_text, "0");
        sprintf(num, "%d", count);
        strcat(_text, num);
        memset(num, 0, sizeof(num));
    }
    else
    {
        sprintf(num, "%d", count);
        strcat(_text, num);
        memset(num, 0, sizeof(num));
    }
    strcat(_text, text);

    for(i=0; i < (size-3) - strlen(text); i++)
    {
        strcat(_text, " ");
    }
    return _text;
}

int CharToInt(char c)
{
    static int diff = 1 - '1';
    return c + diff;
}

void Current_user_add(int client, char * id)
{
    int i=0;
    int count=0;
    char count_buf[3];
    char list_buf[userBuf+1];
    char ID[12];
    char number[3];
    char user_query[100];
    int query_stat = 0;
    MYSQL_RES * sql_result;
    MYSQL_ROW sql_row;
   
    sprintf(user_query, "insert into users values " 
            "('%s')", id);
     
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)     
    {
        fprintf(stderr, "Current_user_add query error : %s", mysql_error(&user_conn));
    }

    sprintf(user_query, "select count(*) from users");
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Current_user_add query error : %s", mysql_error(&user_conn));
    }
    
    sql_result = mysql_store_result(user_connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        count = atoi(sql_row[0]);
    }
     
    strncpy(count_buf, Current_user_count(count, 3), 3);
    write(client, count_buf, 2);
    memset(count_buf, 0, sizeof(count_buf));

    sprintf(user_query, "select * from users");    
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Current_user_add query error : %s", mysql_error(&user_conn));
    }
    
    sql_result = mysql_store_result(user_connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        strcpy(user_list, sql_row[0]);
        strncpy(list_buf, send_count(user_list, userBuf), userBuf);
        write(client, list_buf, userBuf);
        
        memset(list_buf, 0, sizeof(list_buf));
        memset(user_list, 0, sizeof(user_list));
    }
}

void Current_user_remove(int client, char * id)
{
    MYSQL_RES * sql_result;
    MYSQL_ROW sql_row;
    int count=0;
    char count_buf[3];
    char list_buf[userBuf+1];
    char number[3];
    char user_query[100];
    int query_stat = 0;

    sprintf(user_query, "delete from users where id = " 
            "('%s')", id);
     
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)     
    {
        fprintf(stderr, "Current_user_remove query error : %s", mysql_error(&user_conn));
    }

    sprintf(user_query, "select count(*) from users");
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Current_user_remove query error : %s", mysql_error(&user_conn));
    }

    sql_result = mysql_store_result(user_connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        count = atoi(sql_row[0]);
    }

    strncpy(count_buf, Current_user_count(count, 3), 3);
    write(client, count_buf, 2);
    memset(count_buf, 0, sizeof(count_buf));

    sprintf(user_query, "select * from users");    
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Current_user_remove query error : %s", mysql_error(&user_conn));
    }
    
    sql_result = mysql_store_result(user_connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        strcpy(user_list, sql_row[0]);
        strncpy(list_buf, send_count(user_list, userBuf), userBuf);
        write(client, list_buf, userBuf);
        memset(list_buf, 0, sizeof(list_buf));
    }
}

void Current_user(int client, char * id)
{
    MYSQL_RES * sql_result;
    MYSQL_ROW sql_row;
    int count=0;
    char count_buf[3];
    char list_buf[userBuf+1];
    char number[3];
    char user_query[100];
    int query_stat = 0;

    sprintf(user_query, "select count(*) from users");
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Current_user query error : %s", mysql_error(&user_conn));
    }

    sql_result = mysql_store_result(user_connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        count = atoi(sql_row[0]);
    }

    strncpy(count_buf, Current_user_count(count, 3), 3);
    write(client, count_buf, 2);
    memset(count_buf, 0, sizeof(count_buf));

    sprintf(user_query, "select * from users");    
    query_stat = mysql_query(user_connection, user_query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Current_user query error : %s", mysql_error(&user_conn));
    }
    
    sql_result = mysql_store_result(user_connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        strcpy(user_list, sql_row[0]);
        strncpy(list_buf, send_count(user_list, userBuf), userBuf);
        write(client, list_buf, userBuf);
        memset(list_buf, 0, sizeof(list_buf));
    }
}

/* 현재 유저 수 세기 */
char * Current_user_count(int count, int size)
{
    static char _count[3];
    int i=0;
    char number[2];
    
    memset(_count, 0, sizeof(_count));
    
    if(count < 10)
    {
        strcat(_count, "0");
        sprintf(number, "%d", count);
        strcat(_count, number);
        memset(number, 0, sizeof(number));
    }
    else
    {
        sprintf(number, "%d", count);
        strcat(_count, number);
        memset(number, 0, sizeof(number));
    }    
    return _count;
}

void Check_Update(int client)
{
    char check[verBuf];
    char buf[verBuf];
    char version[verBuf];
    char client_ver[verBuf];
    char server_ver[verBuf+3];
    FILE * fp = fopen("update.txt", "r"); //위치 조정

    fgets(check, sizeof(check), fp);
    fclose(fp);
        
    /* server 업데이트 보내기 */
    strncpy(server_ver, send_count(check, verBuf), verBuf);
    write(client, server_ver, verBuf);
    memset(server_ver, 0, sizeof(server_ver));
}

void Before_Message(int client)
{
    MYSQL_RES * sql_result;
    MYSQL_ROW sql_row;
    
    int query_stat;
    char send_true[6];
    
    sprintf(query, "select * from (select * from message order by time desc limit 100) as a group by a.time");
    
    query_stat = mysql_query(connection, query);
    
    if(query_stat != 0)
    {
        fprintf(stderr, "Before_Message query error : %s", mysql_error(&conn));
    }
    
    sql_result = mysql_store_result(connection);
    
    while((sql_row = mysql_fetch_row(sql_result)) != NULL)
    {
        strcpy((*(db_info*)shared_memory).ID, sql_row[0]);
        strcpy((*(db_info*)shared_memory).msg, sql_row[1]);
        strcpy((*(db_info*)shared_memory).time, sql_row[2]);
        strcpy((*(db_info*)shared_memory).state, sql_row[3]);

        Send_Before_Msg(client, (*(db_info*)shared_memory).ID, (*(db_info*)shared_memory).msg, (*(db_info*)shared_memory).time, (*(db_info*)shared_memory).state);
    }
}

/* server -> client */
void Send_Before_Msg(int client, char * user, char * text, char * time, char * send_true)
{
    char buf[260];
    
    strncpy(buf, send_count(time, timeBuf), timeBuf);
    write(client, buf, timeBuf);
    memset(buf, 0, sizeof(buf));
    
    strncpy(buf, send_count(send_true, stateBuf), stateBuf);
    write(client, buf, stateBuf);
    memset(buf, 0, sizeof(buf));
    
    strncpy(buf, send_count(user, userBuf), userBuf);
    write(client, buf, userBuf);
    memset(buf, 0, sizeof(buf));
    
    if(strcmp(send_true, "TRUE") == 0)
    {
        strncpy(buf, send_count(text, textBuf), textBuf);
        write(client, buf, textBuf);
        memset(buf, 0, sizeof(buf));
    }
}

void Error_Handling(const char * message)
{
    fputs(message, stderr);
    fputc('\n', stderr);
}

void z_handler(int signum)
{
    pid_t child;
    int state;

    child = waitpid(-1, &state, WNOHANG);

    printf("\t 소멸된 자식 프로세스 ID = %d\n", child);
    printf("\t 소멸된 자식 프로세스의 리턴 값 = %d\n", WEXITSTATUS(state));
}
