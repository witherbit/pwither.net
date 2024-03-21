using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwither.net
{
    public static class Extensions
    {
        public static string ConvertToUTF8(this byte[] arr)
        {
            return Encoding.UTF8.GetString(arr);
        }
        public static byte[] ConvertFromUTF8(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public static string ConvertToBase64(this byte[] arr)
        {
            return Convert.ToBase64String(arr);
        }
        public static byte[] ConvertFromBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }
    }
}
