#include "QueryStorage.h"
#include "Query.h"

QueryStorage::QueryStorage()
{

}

QueryStorage::QueryStorage()
{

}

Query* QueryStorage::Get(unsigned long long handle) const
{
	auto f = _livingQueries.find(handle);
	if (f == _livingQueries.end())
		return nullptr;
	return f->second.get();
}

unsigned QueryStorage::QueryCount() const
{
	return _livingQueries.size();
}

