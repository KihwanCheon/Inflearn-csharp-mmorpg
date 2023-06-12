﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // 문지기.
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 문지기 교육.
                listenSocket.Bind(endPoint);

                // 영업시작.
                // backlog: 최대 대기수.
                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening ....");

                    // 손님입장.
                    Socket client = listenSocket.Accept(); // blocking

                    // 받는다.
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = client.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Client] {recvData}");

                    // 보낸다.
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                    client.Send(sendBuff);          // blocking.

                    // 내보낸다.
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }
    }
}
