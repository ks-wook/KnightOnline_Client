using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;

/*
 * 서버로 패킷을 전송하기 위해 사용하는 네트워크 매니저 스크립트
 */

public class NetworkManager
{
	ServerSession _session = new ServerSession();

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	// 로그인 완료후 게임 접속 처리 함수
	public void ConnectToGame()
	{
		// 서버 주소 설정
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);
		IPAddress ipAddr = ipHost.AddressList[1];
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Connector connector = new Connector();

		// 설정된 주소로 접속
		connector.Connect(endPoint,
			() => { return _session; },
			1);
	}

	// 네트워크 매니저 업데이트 함수
	public void Update()
	{
		// 서버로부터 받은 패킷들은 패킷 큐에 쌓인다.
		List<PacketMessage> list = PacketQueue.Instance.PopAll();

		// 유니티 게임 오브젝트와 관련된 모든 처리는 유니티 주 쓰레드에서 담당해야 하므로 일감을 넘긴다.
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}
	}

}
