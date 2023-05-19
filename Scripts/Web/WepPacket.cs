using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 클라에서 JsonUtility를 이용해서 파싱을 할땐 get set이 있으면 인식이 안됨.
public class CreateAccountPacketReq
{
    public string AccountName;
    public string Password;
}

// 서버의 응답 -> Res
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