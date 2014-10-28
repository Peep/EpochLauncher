#pragma once
#include "pch.h"

class ServerInfo;
class ServerInfoStorage;

class ServerInfoFactory
{
public:
	ServerInfoFactory();
	virtual ~ServerInfoFactory();

	virtual ServerInfo* Create() = 0;

};