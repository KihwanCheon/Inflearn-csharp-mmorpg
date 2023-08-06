using System;
using System.Xml;

namespace PacketGenerator
{
    internal class Program
    {
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
            }
        }

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

            ParseMembers(r);
        }

        private static void ParseMembers(XmlReader r)
        {
            string packetName = r["name"];

            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine($"member without name ${r.Name}");
                    return;
                }

                string memberType = r.Name.ToLower();

                switch (memberType)
                {
                    case "bool": break;
                    case "byte": break;
                    case "short": break;
                    case "ushort": break;
                    case "int": break;
                    case "long": break;
                    case "float": break;
                    case "double": break;
                    case "string": break;
                    case "list": break;
                    default: break;
                }
            }
        }
    }
}
