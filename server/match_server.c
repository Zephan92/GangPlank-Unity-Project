#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
#include <pthread.h>
#include <sys/socket.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <signal.h>

#include "hashtable_sync.h"

typedef void handler_t(int);
handler_t *Signal(int, handler_t*);

void do_server(uint16_t port);
void *client_connect(void*);

void sigterm_handler(int);

int char_count(char*, char);
int str_split(char*, char, char**);

typedef struct{
	int client_fd;
	struct sockaddr_in client_addr;
	int addrlen;
} client_args;

typedef struct{
	char *name;
	char ip[INET_ADDRSTRLEN];
} user_t;

void add_user(int, char**, char*, client_args*);
void get_user(int, char**, char*, client_args*);
void rm_user(int, char**, char*, client_args*);
void count_users(int, char**, char*, client_args*);
void list_users(int, char**, char*, client_args*);

int server_fd;

hashtable_t command_table;
hashtable_sync_t user_table;

int main(int argc, char *argv[]){
	uint16_t port = 5555;
	if(argc > 1){
		port = atoi(argv[1]);
	}

	Signal(SIGTERM, sigterm_handler);

	ht_init(&command_table);
	ht_add(&command_table, "add", add_user);
	ht_add(&command_table, "get", get_user);
	ht_add(&command_table, "rm", rm_user);
	ht_add(&command_table, "count", count_users);
	ht_add(&command_table, "list", list_users);

	ht_sync_init(&user_table);

	do_server(port);

	ht_dispose(&command_table);
	ht_sync_dispose(&user_table);

	return 0;
}

void do_server(uint16_t port){
	int client_fd, sin_size;
	struct sockaddr_in server_addr, client_addr;

	printf("Starting server listening on port %i\n", port);
	
	if((server_fd = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP)) < 0){
		printf("failed to establish server socket. exiting\n");
		return;
	}

	server_addr.sin_family = AF_INET;
	server_addr.sin_port = htons(port);
	server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
	memset(&(server_addr.sin_zero), 0, 8);

	if(bind(server_fd, (struct sockaddr*)&server_addr, sizeof(struct sockaddr)) < 0){
		printf("failed to bind server socket. exiting\n");
		close(server_fd);
		return;
	}
	if(listen(server_fd, 10) < 0){
		printf("failed to listen on server socket. exiting\n");
		close(server_fd);
		return;
	}

	pthread_attr_t thread_attr;
	pthread_attr_init(&thread_attr);
	pthread_attr_setdetachstate(&thread_attr, PTHREAD_CREATE_DETACHED);
	pthread_t thread;
	while(1){
		sin_size = sizeof(struct sockaddr_in);
		if((client_fd = accept(server_fd, (struct sockaddr*)&client_addr, &sin_size)) < 0){
			printf("error occurred in accepting connection\n");
			continue;
		}
		
		client_args *args = malloc(sizeof(client_args));
		args->client_fd = client_fd;
		args->client_addr = client_addr;
		args->addrlen = sin_size;
		pthread_create(&thread, &thread_attr, client_connect, args);
	}

}

#define BUF_SIZE 128
void *client_connect(void *c_arg){
	client_args *args = (client_args*)c_arg;
	printf("client '%s' connected\n",inet_ntoa(args->client_addr.sin_addr));

	char recbuf[BUF_SIZE], sendbuf[BUF_SIZE];
	int rec_size;
	if((rec_size = recv(args->client_fd, recbuf, BUF_SIZE, 0)) < 0){
		printf("error in reading from client\n");
	}
	else{
		while(rec_size){
			if(recbuf[rec_size-1] == '\x17') recbuf[rec_size-1] = '\0';
			//printf("rec:{%s}\n",recbuf);
			int argc = char_count(recbuf, '\x1F') + 1;
			char **argv = malloc(sizeof(char*) * argc);
			str_split(recbuf, '\x1F', argv);

			void (*command)(int, char**, char*, client_args*) = ht_get(&command_table, argv[0]);
			if(command){
				command(argc, argv, sendbuf, args);
			}
			else{
				snprintf(sendbuf, BUF_SIZE, "err\x1F""unknown command '%s'\x17", argv[0]);
			}

			send(args->client_fd, sendbuf, strlen(sendbuf), 0);
			//printf("sending {%s}\n", sendbuf);
			free(argv);

			if((rec_size = recv(args->client_fd, recbuf, BUF_SIZE, 0)) < 0){
				break;
			}
		}
	}

	printf("client '%s' disconnected\n", inet_ntoa(args->client_addr.sin_addr));
	free(args);
	return NULL;
}

inline void send_too_few_args(char* sendbuf, const char *cmd){
	snprintf(sendbuf, BUF_SIZE, "err\x1F""too few args for '%s'\x17", cmd);	
}

void add_user(int argc, char **argv, char *sendbuf, client_args *c_args){
	if(argc < 2){
		send_too_few_args(sendbuf, argv[0]);
		return;
	}
	if(ht_sync_get(&user_table, argv[1])){
		snprintf(sendbuf, BUF_SIZE, "fail\x1F""username taken\x17");
		return;
	}

	user_t *newuser = malloc(sizeof(user_t));
	newuser->name = malloc(sizeof(char) * strlen(argv[1]));

	strcpy(newuser->name, argv[1]);
	inet_ntop(AF_INET, &c_args->client_addr.sin_addr, newuser->ip, INET_ADDRSTRLEN);

	ht_sync_add(&user_table, newuser->name, newuser);
	snprintf(sendbuf, BUF_SIZE, "ok\x1F""added user '%s' %s\x17", newuser->name, newuser->ip);
}

void get_user(int argc, char **argv, char *sendbuf, client_args *c_args){
	if(argc < 2){
		send_too_few_args(sendbuf, argv[0]);
		return;
	}
	user_t *user;
	if(user = ht_sync_get(&user_table, argv[1])){
		snprintf(sendbuf, BUF_SIZE, "ok\x1F""user '%s' %s\x17", user->name, user->ip);
	}
	else{
		snprintf(sendbuf, BUF_SIZE, "fail\x1F""user not found\x17");
	}
}
void rm_user(int argc, char **argv, char *sendbuf, client_args *c_args){
	if(argc < 2){
		send_too_few_args(sendbuf, argv[0]);
		return;
	}
	user_t *user;
	if(user = ht_sync_get(&user_table, argv[1])){
		ht_sync_delete(&user_table, argv[1]);
		snprintf(sendbuf, BUF_SIZE, "ok\x1F""removed '%s'\x17", user->name);
		free(user->name);
		free(user);
	}
	else{
		snprintf(sendbuf, BUF_SIZE, "fail\x1F""user not found\x17");
	}
	
}

void count_users(int argc, char **argv, char *sendbuf, client_args *c_args){
	snprintf(sendbuf, BUF_SIZE, "ok\x1F""%lu\x17", user_table.t->size);
}

void list_users(int argc, char **argv, char *sendbuf, client_args *c_args){
	if(argc < 2){
		send_too_few_args(sendbuf, argv[0]);
		return;
	}
	//int num = atoi(argv[1]), i;
	size_t num, i;
	if(strcmp(argv[1], "all") == 0){
		num = user_table.t->size;
	}
	else{
		num = atoi(argv[1]);
	}
	if(num > 0){
		kvp_t *kvps = malloc(sizeof(kvp_t)*user_table.t->size);
		get_all_kvps(user_table.t,kvps);
		for(i = 0; i < num-1; i++){
			if(i < user_table.t->size){
				snprintf(sendbuf, BUF_SIZE, "ok\x1F""%s\x1E", (char*)(kvps[i].key));
			}
			else{
				snprintf(sendbuf, BUF_SIZE, "fail\x1F""over user count\x1E");
			}
			send(c_args->client_fd, sendbuf, strlen(sendbuf), 0);
			//printf("sending {%s}\n", sendbuf);
		}
		if(num-1 < user_table.t->size){
			snprintf(sendbuf, BUF_SIZE, "ok\x1F""%s\x17", (char*)(kvps[num-1].key));
		}
		else{
			snprintf(sendbuf, BUF_SIZE, "fail\x1F""over user count\x17");
		}
		free(kvps);
	}
	else{
		snprintf(sendbuf, BUF_SIZE, "fail\x1F""no users\x17");
	}
}

int char_count(char* str, char c){
	int ret = 0;
	while(*str){
		if(*str++ == c) ret++;
	}
	return ret;
}

int str_split(char* str, char c, char **arr){
	size_t i = 1;
	arr[0] = str;
	while(*str){
		if(*str == c){
			*str = '\0';
			arr[i++] = ++str;
		}
		else{
			str++;
		}
	}
	return i;
}

void sigterm_handler(int sig){
	close(server_fd);
	exit(0);
}

handler_t *Signal(int sig, handler_t *handler){
	struct sigaction action, prev;
	action.sa_handler = handler;
	sigemptyset(&action.sa_mask);
	action.sa_flags = SA_RESTART;
	if(sigaction(sig, &action, &prev) < 0){
		printf("signal error\n");
	}
	return prev.sa_handler;
}
