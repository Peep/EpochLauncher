#pragma once
#include "pch.h"

class ServerInfo;

class AppLauncher
{
public:
	AppLauncher();
	virtual ~AppLauncher();


	virtual bool ConnectTo(ServerInfo* to);
	virtual bool Launch();
};