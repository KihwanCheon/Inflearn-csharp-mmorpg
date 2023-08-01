using System;
using System.Threading;

namespace ServerCore
{

    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => null);

        public static int ChunkSize { get; set; } = 4096 * 100;

        public static ArraySegment<byte> Open(int reservedSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if (CurrentBuffer.Value.FreeSize < reservedSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reservedSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    /// <summary>
    /// 일회용으로 만들고 사용.
    /// </summary>
    public class SendBuffer
    {
        // [u][ ][ ][ ][ ][ ][ ][ ]
        // [ ][ ][ ][U][ ][ ][ ][ ]
        private byte[] _buffer;
        private int _usedSize = 0;

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }
        
        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        /// <summary>
        /// 사용하려는 버퍼를 요청한다, 크기가 모자라면 null 을 반환한다.
        /// </summary>
        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize)
            {
                return null;
            }

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        /// <summary>
        /// 사용했다고 확정한다, 사용한 범위를 돌려준다.
        /// </summary>
        public ArraySegment<byte> Close(int usedSize)
        {
            var segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}