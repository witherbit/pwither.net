using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pwither.net.Enums;

namespace pwither.net.Nodes
{
    public static class NodeUtils
    {
        public static byte[] RandomBytes(int size)
        {
            Random rnd = new Random((short)DateTime.Now.Ticks);
            byte[] buffer = new byte[size];
            rnd.NextBytes(buffer);
            return buffer;
        }

        public static Node GetChild(this Node node, string tag, ContentStorageStyle contentStorageStyle = ContentStorageStyle.NodeTree)
        {
            if (contentStorageStyle == ContentStorageStyle.NodeTree)
            {
                var temp = node.Content as Node;
                while (true)
                {
                    if (temp == null || temp.Content == null)
                        break;
                    if (temp.Tag == tag)
                        return temp;
                    else
                        temp = temp.Content as Node;
                }
            }
            else if (contentStorageStyle == ContentStorageStyle.NodeArray)
            {
                if (node.Content is Node[] nodes)
                {
                    return nodes.FirstOrDefault(x => x.Tag == tag);
                }
                return null;
            }
            else if (contentStorageStyle == ContentStorageStyle.NodeList)
            {
                if (node.Content is List<Node> nodes)
                {
                    return nodes.FirstOrDefault(x => x.Tag == tag);
                }
                return null;
            }
            else if (contentStorageStyle == ContentStorageStyle.ObjectArray)
            {
                if (node.Content is object[] nodes)
                {
                    var cntnt = node.Content as object[];
                    foreach (object obj in cntnt)
                    {
                        if (obj is Node)
                            if (((Node)obj).Tag == tag)
                                return obj as Node;
                    }
                }
                return null;
            }
            else if (contentStorageStyle == ContentStorageStyle.ObjectList)
            {
                if (node.Content is List<object> nodes)
                {
                    var cntnt = node.Content as List<object>;
                    foreach (object obj in cntnt)
                    {
                        if (obj is Node)
                            if (((Node)obj).Tag == tag)
                                return obj as Node;
                    }
                }
                return null;
            }
            return null;
        }
    }
}
