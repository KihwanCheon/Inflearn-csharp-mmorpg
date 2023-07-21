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
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        private Queue<byte[]> _sendQueue = new Queue<byte[]>();

        public void Init(Socket socket)
        {
            _socket = socket;

            // 수신.
            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);
            // recvArgs.UserToken = 뭔가 구분할 값을 넣어서 쓸수 있음.

            // 송신.
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
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
            /* //_sendArgs.BufferList 와 같이 쓰면 안됨.
            byte[] buff = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buff, 0, buff.Length);
            */
            while (_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();

                // BufferList 에 개별로 넣으면 안됨
                // _sendArgs.BufferList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
                _pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }

            _sendArgs.BufferList = _pendingList;

            bool pending = _socket.SendAsync(_sendArgs);
            if (!pending)
                OnSendCompleted(null, _sendArgs);
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();
                        Console.WriteLine($"Transferred bytes: {_sendArgs.BytesTransferred}");

                        if (_sendQueue.Count > 0)
                            RegisterSend();
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

        private void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (!pending) // 등록하자마자 받은게 있으면.
            {
                OnRecvCompleted(null, _recvArgs);
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

                    RegisterRecv();
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
