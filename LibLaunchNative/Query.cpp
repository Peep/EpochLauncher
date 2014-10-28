#include "pch.h"
#include "Query.h"
#include "QueryStorage.h"

Query::Query(const char* uri, QueryFactory* factory, QueryStorage* storage)
{
	_handle = ++storage->_nextId;
	storage->_livingQueries.emplace(_handle, std::unique_ptr<Query>(this));
}

Query::~Query()
{

}

