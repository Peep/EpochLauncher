#pragma once
#include "pch.h"

class QueryFactory;
class QueryStorage;
class ServerInfo;


class Query
{
public:
	bool IsAlive;
	bool IsDirty;

	Query(const char* uri, QueryFactory* factory, QueryStorage* storage);
	virtual ~Query();

	unsigned long long Handle() const;
	virtual bool UpdateInfo(ServerInfo* info) = 0;

private:
	unsigned long long _handle;
};