using System.Text;
using System.Linq;

namespace LobbyCodes.Utils
{
    public static class ObfuscateJoinCode
    {
        // reversible, in no way secure, cypher for printable ascii
        private static string ROT47(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            char[] ca = new char[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c >= 33 && c <= 126)
                {
                    int j = c + 47;
                    if (j > 126) j -= 94;
                    ca[i] = (char)j;
                }
                else
                {
                    ca[i] = c;
                }
            }
            return new string(ca);
        }
        public static string Obfuscate(string code)
        {
            return ROT47(code);
        }
        public static string DeObfuscate(string code)
        {
            return ROT47(code);
        }
    }
}
