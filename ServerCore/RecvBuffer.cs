using System;

namespace ServerCore
{
    public class RecvBuffer
    {
        //최초:   [rw][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ] 
        //쓰기:   [r][ ][ ][ ][ ][ ][ ][w][ ][ ][ ][ ][ ]
        //읽기:   [ ][ ][ ][ ][ ][ ][ ][rw][ ][ ][ ][ ][ ]
        //재조정: [rw][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        private readonly ArraySegment<byte> _buffer;
        private int _readPos;
        private int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos - _readPos; } }
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset +_writePos, FreeSize); }
        }

        public void Clear()
        {
            int dataSize = DataSize; // 복사하고 Data 메서드 사용않는다. 커서변수를 수정하면, 메서드 반환값이 달라진다.
            if (dataSize == 0) // 데이터 없으면 커서만 이동.
            {
                _readPos = 0;
                _writePos = 0;
            }
            else // 남은 데이터 있으면 데이터와 커서 이동.
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
            {
                return false;
            }

            _readPos += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
            {
                return false;
            }

            _writePos += numOfBytes;
            return false;
        }
    }
}