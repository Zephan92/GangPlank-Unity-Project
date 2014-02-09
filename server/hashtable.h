#include <stdlib.h>
#include <stdint.h>

#define FNV_PRIME 16777619
#define FNV_BASE 2166136261

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

hashtable_t *ht_init();
hashtable_t *ht_init_s(size_t);

void *ht_add(hashtable_t*, const char*, void*);
void *ht_get(hashtable_t*, const char*);
void *ht_delete(hashtable_t*, const char*);
