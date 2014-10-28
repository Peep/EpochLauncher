#pragma once
#include "pch.h"

class ServerInfoFactory;
class ServerInfoStorage;

class ServerInfo
{
public:
	ServerInfo(ServerInfoFactory* factory, ServerInfoStorage* storage);
	virtual ~ServerInfo();
};