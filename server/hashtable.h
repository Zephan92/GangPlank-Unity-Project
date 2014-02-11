#include <stdlib.h>
#include <stdint.h>

typedef struct node{
	struct node *next;
	const char *key;
	void *val;
} node_t;

typedef struct{
	node_t **arr;
	size_t cap;
	size_t size;
} hashtable_t;

void ht_init(hashtable_t*);
void ht_init_s(hashtable_t*, size_t);

void *ht_add(hashtable_t*, const char*, void*);
void *ht_get(hashtable_t*, const char*);
void *ht_delete(hashtable_t*, const char*);
void ht_dispose(hashtable_t*);
