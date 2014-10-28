#pragma once
#include "pch.h"

class ServerInfo;
class ServerInfoStorage;

class ServerInfoFactory
{
public:
	ServerInfoFactory();
	virtual ~ServerInfoFactory();



	ServerInfo* Create();

protected:

};