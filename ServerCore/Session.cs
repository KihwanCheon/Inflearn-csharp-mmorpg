using System;
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

        object _lock = new object();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        private bool _pending = false;
        private Queue<byte[]> _sendQueue = new Queue<byte[]>();

        public void Init(Socket socket)
        {
            _socket = socket;

            // 수신.
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += OnRecvCompleted;
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            // recvArgs.UserToken = 뭔가 구분할 값을 넣어서 쓸수 있음.

            // 송신.
            _sendArgs.Completed += OnSendCompleted;

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (!_pending)
                {
                    RegisterSend();
                }
            }
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

        void RegisterSend()
        {
            _pending = true;
            byte[] buff = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buff, 0, buff.Length);

            bool pending = _socket.SendAsync(_sendArgs);
            if (!pending)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if (_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        else
                        {
                            _pending = false;
                        }
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
