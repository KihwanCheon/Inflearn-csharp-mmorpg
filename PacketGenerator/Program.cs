using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    internal class Program
    {
        private static string genPackets = "";

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

                File.WriteAllText("GenPackets.cs", genPackets);
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
            

            genPackets += string.Format(PacketFormats.PacketFormat, packetName, t.Item1, t.Item2, t.Item3);
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
                    case "list": break;

                    default: break;
                }
            }

            memberCode = memberCode.Replace("\n", "\n    ");
            readCode = readCode.Replace("\n", "\n        ");
            writeCode = writeCode.Replace("\n", "\n        ");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
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
    }
}
