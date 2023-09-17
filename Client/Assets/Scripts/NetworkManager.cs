﻿using System.Collections;
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

        StartCoroutine("CoSendPacket");
    }

    // Update is called once per frame
    void Update()
    {
        var packet = PacketQueue.Instance.Pop();
        if (packet == null)
            return;

        PacketManager.Instance.HandlePacket(_session, packet);
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            C_Chat chatPacket = new C_Chat { chat = "Hello unity" };
            var segment = chatPacket.Write();

            _session.Send(segment);
        }
    }
}
