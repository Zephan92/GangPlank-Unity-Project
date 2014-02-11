#include "hashtable.h"

static uint32_t hash_code(const char* key);
static node_t *ht_get_node(hashtable_t*,const char*);
static node_t *ht_get_node_i(hashtable_t*,const char*,uint32_t);
static void ht_rehash(hashtable_t*, size_t);

void ht_init_s(hashtable_t *t, size_t capacity){
	uint32_t cap = 1 << (31 - __builtin_clz(capacity));
	if(capacity > cap) cap <<= 1;

	t->cap = cap;
	t->arr = (node_t**)calloc(t->cap, sizeof(node_t*));
	t->size = 0;
}

void ht_init(hashtable_t *t){
	ht_init_s(t, 16);
}

void *ht_add(hashtable_t *t, const char* key, void *val){
	if(t->size >= (t->cap >> 1) + (t->cap >> 2)){
		ht_rehash(t, t->cap << 1);
	}

	uint32_t index = hash_code(key) & (t->cap - 1);
	void *ret = NULL;
	node_t *n = ht_get_node_i(t, key, index);
	if(!n){
		n = (node_t*)malloc(sizeof(node_t));
		n->next = t->arr[index];
		n->key = key;

		t->arr[index] = n;
		t->size++;
	}
	else{
		ret = n->val;
	}
	n->val = val;

	return ret;
}

void *ht_get(hashtable_t *t, const char* key){
	node_t *n = ht_get_node(t, key);
	return n?n->val:NULL;
}

node_t *ht_get_node(hashtable_t *t, const char* key){
	return ht_get_node_i(t, key, hash_code(key) & (t->cap - 1));
}

node_t *ht_get_node_i(hashtable_t *t, const char* key, uint32_t index){
	node_t *n;
	for(n = t->arr[index]; n && strcmp(key, n->key); n = n->next);
	return n;
}

void *ht_delete(hashtable_t *t, const char*key){
	uint32_t index = hash_code(key) & (t->cap-1);
	void *ret = NULL;
	node_t *n = t->arr[index];
	if(!n) return ret;
	if(strcmp(key,n->key) == 0){
		t->arr[index] = n->next;
		ret = n->val;
		free(n);
		t->size--;
	}
	else{
		for(; n->next && strcmp(key, n->next->key); n = n->next);
		if(n->next){
			node_t *target = n->next;
			n->next = target->next;
			ret = target->val;
			free(target);
			t->size--;
		}
	}
	if(t->size <= t->cap >> 2 && t->cap > 16){
		ht_rehash(t, t->cap >> 1);
	}
	return ret;
}

void subrehash(node_t **newarr, size_t newcap, node_t *n){
	if(n->next){
		subrehash(newarr, newcap, n->next);
	}
	uint32_t index = hash_code(n->key) & (newcap-1);
	n->next = newarr[index];
	newarr[index] = n;
}

void ht_rehash(hashtable_t *t, size_t newcap){
	node_t **newarr = (node_t**)calloc(newcap, sizeof(node_t*));
	uint32_t i;
	for(i = 0; i < t->cap; i++){
		if(t->arr[i]){
			subrehash(newarr, newcap, t->arr[i]);		
		}
	}
	free(t->arr);
	t->arr = newarr;
	t->cap = newcap;
}

void subdispose(node_t *n){
	if(n->next){
		subdispose(n->next);
	}
	free(n);
}

void ht_dispose(hashtable_t *t){
	node_t *n;
	uint32_t i;
	for(i = 0; i < t->cap; i++){
		n = t->arr[i];
		if(n){
			subdispose(n);
		}
	}
	free(t->arr);
}

#define FNV_PRIME 16777619
#define FNV_BASE 2166136261

uint32_t hash_code(const char* key){
	uint32_t i, hc = FNV_BASE;
	for(i = 0; key[i]; i++){
		hc ^= key[i];
		hc *= FNV_PRIME;
	}
	return hc;
}

/*
void ht_print(hashtable_t *t){
	node_t *n;
	uint32_t i;
	for(i = 0; i < t->cap; i++){
		printf("%03u: ",i);
		n = t->arr[i];
		while(1){
			if(n){
				printf("k:'%s' v:%p -> ",n->key,n->val);
				n = n->next;
			}
			else{
				printf("null\n");
				break;
			}
		}
	}
	printf("\n");
}
//*/
