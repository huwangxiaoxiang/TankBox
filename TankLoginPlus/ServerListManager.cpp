#include "pch.h"
#include "ServerListManager.h"

ServerList ServerListManager::servers = ServerList();

void ServerListManager::Inistance()
{
	servers.AddServer(CString("����һ��"), 1);
	servers.AddServer(CString("���϶���"), 4);
	servers.AddServer(CString("��������"), 7);
	servers.AddServer(CString("����һ��"), 2);
	servers.AddServer(CString("��������"), 3);
	servers.AddServer(CString("��������"), 6);
	servers.AddServer(CString("����һ��"), 5);
	servers.AddServer(CString("����һ��"), 5000);
	servers.AddServer(CString("ð�յ�����"), 8);
}

int ServerListManager::GetServerId(CString name)
{
	int m = servers.GetServerId(name);
	if (m < 0) return -1;
	else return m;
}

std::vector<CString> ServerListManager::GetServerNameList()
{
	return servers.GetNameList();
}
