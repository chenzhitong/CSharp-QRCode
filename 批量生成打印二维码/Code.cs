using System;
using System.Text;

namespace 批量生成打印二维码
{
    public static class Code
    {
        public static string Sha256(this string input)
        {
            System.Security.Cryptography.SHA256 obj = System.Security.Cryptography.SHA256.Create();
            return BitConverter.ToString(obj.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "");
        }

        public static string Sha256(this byte[] input)
        {
            System.Security.Cryptography.SHA256 obj = System.Security.Cryptography.SHA256.Create();
            return BitConverter.ToString(obj.ComputeHash(input)).Replace("-", "");
        }

        public static string Create(string input) => input + input.Sha256().Substring(0, 3);

        public static bool Check(string address)
        {
            if (address.Length != 8)
                return false;
            var input = address.Substring(0, 5);
            var check = address.Substring(5, 3);
            return input.Sha256().Substring(0, 3) == check;
        }
    }
}
