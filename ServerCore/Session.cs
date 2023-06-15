﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        private int _disconnected = 0;

        public void Init(Socket socket)
        {
            _socket = socket;
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += OnRecvCompleted;
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            // recvArgs.UserToken = 뭔가 구분할 값을 넣어서 쓸수 있음.

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            // _socket.Send(sendBuff);
            SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSendCompleted;
            sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);

            RegisterSend(sendArgs);
        }

        public void Disconnect()
        {
            // 중복 호출로 인한 예외 방지.
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) 
                return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신 PRIVATE_INTERNAL

        void RegisterSend(SocketAsyncEventArgs args)
        {
            bool pending = _socket.SendAsync(args);
            if (!pending)
            {
                OnSendCompleted(null, args);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnSendCompleted {e}");

                }
            }
            else
            {
                Disconnect();
            }
        }

        private void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (!pending) // 등록하자마자 받은게 있으면.
            {
                OnRecvCompleted(null, args);
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");

                    RegisterRecv(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        #endregion
    }
}
