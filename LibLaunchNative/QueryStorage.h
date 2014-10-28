#pragma once
#include "pch.h"


class Query;

using QueryId = unsigned long long;

class QueryStorage
{
public:
	QueryStorage();
	virtual ~QueryStorage();

	Query* Get(unsigned long long handle) const;

	unsigned QueryCount() const;

protected:
	std::unordered_map<unsigned long long, std::unique_ptr<Query>> _livingQueries;
	unsigned long long _nextId;

	friend class Query;
};


