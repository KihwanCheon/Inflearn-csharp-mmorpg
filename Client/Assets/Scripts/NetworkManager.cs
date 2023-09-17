using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    ServerSession _session = new ServerSession(0);

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
        
    }
}
