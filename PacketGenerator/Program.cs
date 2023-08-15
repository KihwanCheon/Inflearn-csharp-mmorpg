using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    internal class Program
    {
        private static string _genPackets = "";
        private static ushort _packetId = 0;
        private static string _packetEnums = "";

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true, IgnoreWhitespace = true
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", settings))
            {
                r.MoveToContent();
                while (r.Read())
                {
                    // <packet> 은 depth 가 1이고 여는 태그이다.
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);
                    
                    // Console.WriteLine(r.Name +" "+ r["name"]);
                }

                _genPackets = string.Format(PacketFormats.FileFormat, _packetEnums, _genPackets);

                File.WriteAllText("GenPackets.cs", _genPackets);
            }
        }


        /// <see cref="PacketFormats.PacketFormat"/>
        /// {0} 패킷이름
        private static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine($"invalid packet node: {r.Name}");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            

            _genPackets += string.Format(PacketFormats.PacketFormat, packetName, t.Item1, t.Item2, t.Item3);
            _packetEnums += string.Format(PacketFormats.EnumFormat, packetName, ++_packetId) + Environment.NewLine + "\t";
        }

        /// <see cref="PacketFormats.MemberFormat"/>
        /// {1} 멤버이름
        /// {2} 멤버변수 Read
        /// {3} 멤버변수 Write
        private static Tuple<string, string, string> ParseMembers(XmlReader r)
        {
            string packetName = r["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine($"member without name ${r.Name}");
                    return null;
                }

                if (!string.IsNullOrEmpty(memberCode)) memberCode += Environment.NewLine;
                if (!string.IsNullOrEmpty(readCode)) readCode += Environment.NewLine;
                if (!string.IsNullOrEmpty(writeCode)) writeCode += Environment.NewLine;


                string memberType = r.Name.ToLower();

                switch (memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormats.MemberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormats.ReadByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormats.WriteByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double": 
                        memberCode += string.Format(PacketFormats.MemberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormats.ReadFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormats.WriteFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormats.MemberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormats.ReadStringFormat, memberName);
                        writeCode += string.Format(PacketFormats.WriteStringFormat, memberName);
                        break;
                    case "list":
                        var t = ParseList(r);
                        if (t != null)
                        {
                            memberCode += t.Item1;
                            readCode += t.Item2;
                            writeCode += t.Item3;
                        }
                        
                        break;

                    default: break;
                }
            }

            memberCode = memberCode.Replace("\n", "\n    ");
            readCode = readCode.Replace("\n", "\n        ");
            writeCode = writeCode.Replace("\n", "\n        ");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        private static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listType = r["type"];
            string listName = r["name"];

            if (string.IsNullOrEmpty(listName) || string.IsNullOrEmpty(listType))
            {
                Console.WriteLine($"List without name({listName}) or type({listType})");
                return null;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            string memberCode = string.Format(PacketFormats.MemberListFormat, listType, listName, t.Item1, t.Item2, t.Item3);
            string readCode = string.Format(PacketFormats.ReadListFormat, listType, listName);
            string writeCode = string.Format(PacketFormats.WriteListFormat, listType, listName);

            return Tuple.Create(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool": return "ToBoolean";
                case "short": return "ToInt16";
                case "ushort": return "ToUInt16";
                case "int": return "ToInt32";
                case "long": return "ToInt64";
                case "float": return "ToSingle";
                case "double": return "ToDouble";
                default: return "";
            }
        }

        public static string ToUpperFirstChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string ToLowerFirstChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
