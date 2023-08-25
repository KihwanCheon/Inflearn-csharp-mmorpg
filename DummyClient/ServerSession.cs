using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    /// <summary>서버 대리자</summary>
    public class ServerSession : Session
    {
        /**
         * C++ 처럼 포인터를 직접적으로 사용하기.
         * TryWriteBytes 대용으로...
         * unsafe!
         */
        static unsafe void ToBytes(byte[] array, int offset, ulong value)
        {
            fixed (byte* ptr = &array[offset])
                *(ulong*)ptr = value;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            var packet = new C_PlayerInfoReq { playerId = 333, name = "TestId"};
            var skill = new C_PlayerInfoReq.Skill{id = 101, level = 1, duration = 3.0f};
            skill.attrs.Add(new C_PlayerInfoReq.Skill.Attribute{fire = 33});
            packet.skills.Add(skill);
            packet.skills.Add(new C_PlayerInfoReq.Skill{id = 201, level = 2, duration = 4.0f});
            packet.skills.Add(new C_PlayerInfoReq.Skill{id = 301, level = 3, duration = 5.0f});
            packet.skills.Add(new C_PlayerInfoReq.Skill{id = 401, level = 4, duration = 6.0f});

            ArraySegment<byte> s = packet.Write();

            if (s != null)
                Send(s);
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            if (buffer == null || buffer.Array == null)
            {
                Console.WriteLine($"[From Server] null array");
                return -1;
            }
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"To Server Transferred bytes: {numOfBytes}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
    }
}