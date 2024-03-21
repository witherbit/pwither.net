using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pwither.net.Enums;

namespace pwither.net.Nodes
{
    public class Node
    {
        public string Tag { get; set; }
        public object Content { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public Node(string tag, object content, Dictionary<string, string> attributes)
        {
            Tag = tag;
            Content = content;
            Attributes = attributes;
        }
        public Node(string tag, object content)
        {
            Tag = tag;
            Content = content;
            Attributes = new Dictionary<string, string>();
        }
        public Node(string tag, Dictionary<string, string> attributes)
        {
            Tag = tag;
            Attributes = attributes;
        }
        public Node(string tag)
        {
            Tag = tag;
            Attributes = new Dictionary<string, string>();
        }
        public Node() { }

        public virtual byte[] Pack()
        {
            return JsonConvert.SerializeObject(this).ConvertFromUTF8();
        }

        public static Node Unpack(byte[] arr)
        {
            return JsonConvert.DeserializeObject<Node>(arr.ConvertToUTF8());
        }

        public string GetAttribute(string attribute)
        {
            if (Attributes.ContainsKey(attribute))
            {
                return Attributes[attribute];
            }
            return default;
        }
        public void SetAttribute(string attribute, string value)
        {
            if (!Attributes.ContainsKey(attribute))
            {
                Attributes.Add(attribute, value);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
