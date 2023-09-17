using System;
using System.Net;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    readonly ServerSession _session = new ServerSession(0);

    // Start is called before the first frame update
    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();
        connector.Connect(endPoint, () => _session);
    }

    // Update is called once per frame
    void Update()
    {
        var packets = PacketQueue.Instance.PopAll();
        if (packets == null || packets.Count == 0)
            return;

        foreach (var packet in packets)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }
}
