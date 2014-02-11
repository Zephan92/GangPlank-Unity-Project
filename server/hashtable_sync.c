#include "hashtable_sync.h"

void ht_sync_init_s(hashtable_sync_t *table, size_t capacity){
	table->t = malloc(sizeof(hashtable_t));
	ht_init_s(table->t, capacity);
	sem_init(&table->mutex, 0, 1);
}

void ht_sync_init(hashtable_sync_t *table){
	ht_sync_init_s(table,16);
}

void *do_locked(hashtable_sync_t* table, const char* key, void *(*do_op)(hashtable_t*,const char*)){
	void *ret;
	sem_wait(&table->mutex);
	ret = do_op(table->t, key);
	sem_post(&table->mutex);
	return ret;
}

void *ht_sync_get(hashtable_sync_t *table, const char *key){
	return do_locked(table, key, ht_get);
}

void *ht_sync_delete(hashtable_sync_t *table, const char *key){
	return do_locked(table, key, ht_delete);
}

void *ht_sync_add(hashtable_sync_t* table, const char* key, void *val){
	void *ret;
	sem_wait(&table->mutex);
	ret = ht_add(table->t, key, val);
	sem_post(&table->mutex);
	return ret;
}

void ht_sync_dispose(hashtable_sync_t *table){
	ht_dispose(table->t);
	free(table->t);
	sem_destroy(&table->mutex);
}
