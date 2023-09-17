namespace PacketGenerator
{
    public class PacketFormats
    {
        // {0} 패킷 등록.
        public static string ManagerFormat =
@"using System;
using System.Collections.Generic;
using ServerCore;


public class PacketManager
{{
    #region Singleton

    public static PacketManager Instance {{ get; }} = new PacketManager();

    PacketManager()
    {{
        Register();
    }}

    #endregion

    
    private Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv 
        = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    private Dictionary<ushort, Action<PacketSession, IPacket>> _handler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();


    public void Register()
    {{
{0}
    }}
    
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (_onRecv.TryGetValue(id, out var action))
            action.Invoke(session, buffer);
        else
            Console.WriteLine($""packet id not found: {{id}}"");
    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        var packet = new T();
        packet.Read(buffer);

        if (_handler.TryGetValue(packet.Protocol, out var action))
            action.Invoke(session, packet);
        else
            Console.WriteLine($""packet handler not found: {{packet.GetType()}}"");
    }}
}}
";

        // {0} 패킷 이름.
        public static string ManagerRegisterFormat =
@"        _onRecv.Add((ushort) PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort) PacketID.{0}, PacketHandler.{0}Handler);
";


        // {0} enum 변수.
        // {1} 패킷 정의.
        public static string FileFormat =
@"using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

public enum PacketID
{{
    {0}
}}

public interface IPacket
{{
    ushort Protocol {{ get; }}
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}}

{1}

";
        // {0} 패킷 이름.
        // {1} 패킷 번호.
        public static string EnumFormat =
@"{0} = {1},";

        // {0} 패킷 이름
        // {1} 멤버 이름
        // {2} 멤버변수 Read
        // {3} 멤버변수 Write
        public static string PacketFormat =
@"
public class {0} : IPacket
{{
    {1}

    public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        count += sizeof(ushort);    // for size
        count += sizeof(ushort);    // for PacketID
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        
        count += sizeof(ushort);    // for size
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.{0}), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);    // for PacketID

        {3}

        // write count at last, after packet counted
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        
        return SendBufferHelper.Close(count);
    }}
}}
";

        // {0} 변수 형식.
        // {1} 변수 이름.
        public static string MemberFormat = 
@"public {0} {1};";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2} 멤버이름
        // {3} 멤버변수 Read
        // {4} 멤버변수 Write
        public static string MemberListFormat =
@"public class {0}
{{
    {2}

    public void Read(ArraySegment<byte> segment, ref ushort count)
    {{
        {3}
    }}

    public bool Write(ArraySegment<byte> segment, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}
}}

public List<{0}> {1} = new List<{0}>();";

        // {0} 변수 이름.
        // {1} To 변수 형식.
        // {2} 변수 형식
        public static string ReadFormat =
@"this.{0} = BitConverter.{1}(segment.Array, segment.Offset + count);
count += sizeof({2});";

        // {0} 변수 이름.
        // {1} 변수 형식
        public static string ReadByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";


        // {0} 변수 이름.
        public static string ReadStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);    // for {0}Len
this.{0} = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;           // for this.{0}";

        // {0} 리스트 이름[대문자].
        // {1} 리스트 이름[소문자].
        public static string ReadListFormat =
@"// {1} list
this.{1}.Clear();
ushort {1}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);

for (int i = 0; i < {1}Len; i++)
{{
    {0} element = new {0}();
    element.Read(s, ref count);
    this.{1}.Add(element);
}}";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string WriteFormat =
@"Array.Copy(BitConverter.GetBytes({0}), 0, segment.Array, segment.Offset + count, sizeof({1}));
count += sizeof({1});";

        // {0} 변수 이름
        // {1} 변수 형식
        public static string WriteByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";

        // {0} 변수 이름
        public static string WriteStringFormat =
@"// string
ushort {0}Len = (ushort)Encoding.Unicode.GetBytes({0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(ushort));;
count += sizeof(ushort);
count += {0}Len;";

        // {0} 리스트 이름[대문자].
        // {1} 리스트 이름[소문자].
        public static string WriteListFormat =
@"// {1} list
Array.Copy(BitConverter.GetBytes((ushort)this.{1}.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
foreach ({0} element in this.{1})
    element.Write(segment, ref count);";

    }
}