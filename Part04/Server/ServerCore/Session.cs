using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly ushort HeaderSize = 2;

        // [size(2)][packetId(2)][.....][size(2)][packetId(2)][.....]
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int orgCnt = buffer.Count;
            int processLen = 0;
            int packetCount = 0;
            while (true)
            {
                if (buffer.Count < HeaderSize)
                {
                    // Console.WriteLine($"OnRecv(buffer), buffer({buffer.Count}) is less than headerSize");
                    break;
                }

                if (buffer.Array == null)
                {
                    // Console.WriteLine($"OnRecv(buffer), buffer is null. why buffer({buffer.Count})?");
                    break;
                }

                ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < size)
                {
                    // Console.WriteLine($"OnRecv(buffer), buffer({buffer.Count}) is less than packet size({size})");
                    break;
                }

                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, size)); //packet 1 ea.
                processLen += size;
                packetCount++;

                if (orgCnt == processLen || orgCnt < processLen + HeaderSize)
                {
                    // Console.WriteLine($"OnRecv(buffer), all of buffer({orgCnt}) is proceeded({processLen})");
                    break;
                }

                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + size, buffer.Count - size);
            }

            if (packetCount > 1)
                Console.WriteLine($"packet count: {packetCount}");

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        private int _disconnected;

        readonly RecvBuffer _recvBuffer = new RecvBuffer(ushort.MaxValue);

        readonly object _lock = new object();
        readonly SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        readonly SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        readonly List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        readonly Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        #region inheritable interface

        public abstract void OnConnected(EndPoint endPoint);
        // 처리중 문제가 있으면 음수를 반환한다.
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        #endregion

        void Clear()
        {
            lock (_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += OnRecvCompleted;
            // recvArgs.UserToken = 뭔가 구분할 값을 넣어서 쓸수 있음.
            _sendArgs.Completed += OnSendCompleted;

            RegisterRecv();
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Send(List<ArraySegment<byte>> sendBuffs)
        {
            if (sendBuffs == null || sendBuffs.Count == 0)
                return;

            lock (_lock)
            {
                sendBuffs.ForEach(b => _sendQueue.Enqueue(b));

                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        public void Disconnect()
        {
            // 중복 호출로 인한 예외 방지.
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) 
                return;

            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();

            Clear();
        }

        #region 네트워크 통신 PRIVATE_INTERNAL

        void RegisterSend()
        {
            if (_disconnected == 1)
                return;

            /* //_sendArgs.BufferList 와 같이 쓰면 안됨.
            byte[] buff = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buff, 0, buff.Length);
            */
            while (_sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = _sendQueue.Dequeue();

                // BufferList 에 개별로 넣으면 안됨
                // _sendArgs.BufferList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
                _pendingList.Add(buff);
            }

            _sendArgs.BufferList = _pendingList;

            try
            {
                bool pending = _socket.SendAsync(_sendArgs);
                if (!pending)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterSend Fail {e}");
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
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);
                        
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
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;

            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (!pending) // 등록하자마자 받은게 있으면.
                    OnRecvCompleted(null, _recvArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterRecv failed {e}");
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // Write 커서 이동.
                    if (!_recvBuffer.OnWrite(args.BytesTransferred))
                    {
                        Console.WriteLine("OnRecvCompleted failed : recvBuffer.OnWrite");
                        Disconnect();
                        return;
                    }

                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if (processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Console.WriteLine("OnRecvCompleted failed : OnRecv");
                        Disconnect();
                        return;
                    }

                    if (!_recvBuffer.OnRead(processLen))
                    {
                        Console.WriteLine("OnRecvCompleted failed : recvBuffer.OnRead");
                        Disconnect();
                        return;
                    }
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
