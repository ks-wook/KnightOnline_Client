using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Ŭ�󿡼� JsonUtility�� �̿��ؼ� �Ľ��� �Ҷ� get set�� ������ �ν��� �ȵ�.
public class CreateAccountPacketReq
{
    public string AccountName;
    public string Password;
}

// ������ ���� -> Res
public class CreateAccountPacketRes
{
    public bool CreateOk;
}

public class LoginAccountPacketReq
{
    public string AccountName;
    public string Password;
}

public class ServerInfo
{
    public string Name;
    public string IpAddress;
    public int Port;
    public int ByshScore;
}


public class LoginAccountPacketRes
{
    public bool LoginOk { get; set; }
    public int AccountId { get; set; }
    public int Token { get; set; }
    public List<ServerInfo> ServerList { get; set; } = new List<ServerInfo>();
}