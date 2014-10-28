#pragma once
#include "pch.h"

class ServerInfo;
class ServerInfoFactory;
class ServerInfoStorage;
class Query;


class QueryFactory
{
public:
	QueryFactory();
	virtual ~QueryFactory();

	virtual Query* Create(const char* uri) = 0;
};