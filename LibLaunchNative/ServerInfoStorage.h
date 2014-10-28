#pragma once
#include "pch.h"

class ServerInfo;
class ServerInfoFactory;

class ServerInfoStorage
{
public:
	ServerInfoStorage();
	virtual ~ServerInfoStorage();

	unsigned InfoCount() const;
	ServerInfo* Get(unsigned long long handle);

protected:
	std::unordered_map<unsigned long long, std::unique_ptr<ServerInfo>> _livingInfos;
	unsigned long long _nextId;

	friend class ServerInfo;
};
