﻿using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        private Socket _listenSocket;
        private Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler += onAcceptHandler;
            // 문지기 교육.
            _listenSocket.Bind(endPoint);

            // 영업시작.
            // backlog: 최대 대기수.
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;
            RegisterAccept(args); // RegisterAcceptMany(10);
        }

        /// <summary>
        /// 부하 분산을 위해 SocketAsyncEventArgs 여럿 생성 등록해서 사용가능하다.
        /// </summary>
        /// <param name="cnt"></param>
        void RegisterAcceptMany(int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAcceptCompleted;
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // clear before socket.

            bool pending = _listenSocket.AcceptAsync(args);
            if (!pending) // 호출하자마자 들어온게 있다.
            {
                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine($"{args.SocketError}");
            }

            RegisterAccept(args); // 처리후 다시 등록.
        }
    }
}
