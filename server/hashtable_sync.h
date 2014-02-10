#include <semaphore.h>
#include "hashtable.h"

typedef struct{
	hashtable_t *t;
	sem_t mutex;
} hashtable_sync_t;

void ht_sync_init(hashtable_sync_t*);
void ht_sync_init_s(hashtable_sync_t*,size_t);

void *ht_sync_add(hashtable_sync_t*, const char*, void*);
void *ht_sync_get(hashtable_sync_t*, const char*);
void *ht_sync_delete(hashtable_sync_t*, const char*);
void ht_sync_dispose(hashtable_sync_t*);
