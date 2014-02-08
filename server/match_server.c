#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string.h>
#include <pthread.h>
#include <sys/socket.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <signal.h>

typedef void handler_t(int);
handler_t *Signal(int, handler_t*);

void do_server(uint16_t port);
void *(client_connect)(void*);

void sigterm_handler(int);

int server_fd;

int main(int argc, char *argv[]){
	uint16_t port = 5555;
	if(argc > 1){
		port = atoi(argv[1]);
	}

	Signal(SIGTERM, sigterm_handler);

	do_server(port);

	return 0;
}

typedef struct{
	int client_fd;
	struct sockaddr_in client_addr;
	int addrlen;
} client_args;

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
	int flag = 1;
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

void *(client_connect)(void *c_arg){
	client_args *args = (client_args*)c_arg;
	char client_host[128];
	printf("client '%s' connected\n",inet_ntoa(args->client_addr.sin_addr));

	char buf[128];
	char sendbuf[128] = "sending\nsome\nmessages\n";
	int rec_size;
	if((rec_size = recv(args->client_fd, buf, 128, 0)) < 0){
		printf("error in reading from client\n");
	}
	else{
		while(rec_size){
			printf("received '%s'\n",buf);
			printf("sending '%s'\n",sendbuf);

			send(args->client_fd, sendbuf, strlen(sendbuf), 0);

			printf("sent '%s'\n",sendbuf);
			if((rec_size = recv(args->client_fd, buf, 128, 0)) < 0){
				break;
			}
		}
	}

	printf("client finished\n");
	free(args);
	return NULL;
}

void sigterm_handler(int sig){
	close(server_fd);
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
