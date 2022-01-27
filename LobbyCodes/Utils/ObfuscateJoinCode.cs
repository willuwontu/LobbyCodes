using System;
using System.Collections.Generic;
using LobbyCodes.Networking;
using UnityEngine;
using System.Linq;

namespace LobbyCodes.Utils
{
    public static class ObfuscateJoinCode
    {
        private static readonly char[] BaseChars =
         "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly Dictionary<char, int> CharValues = BaseChars
                   .Select((c, i) => new { Char = c, Index = i })
                   .ToDictionary(c => c.Char, c => c.Index);
        private static readonly char[] Delimeters = ":#$?&=@".ToCharArray();

        private static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        private static int GetRegionNumber(string region)
        {
            return LobbyCodeHandler.Regions.Length * UnityEngine.Random.Range(0, LobbyCodeHandler.Regions.Length) + Array.IndexOf(LobbyCodeHandler.Regions, region); 
        }
        private static string GetRegionCode(int region)
        {
            return LobbyCodeHandler.Regions[mod(region, LobbyCodeHandler.Regions.Length)];
        }
        public static string LongToBase(long value)
        {
            long targetBase = BaseChars.Length;
            // Determine exact number of characters to use.
            char[] buffer = new char[Math.Max(
                       (int)Math.Ceiling(Math.Log(value + 1, targetBase)), 1)];

            var i = buffer.Length;
            do
            {
                buffer[--i] = BaseChars[value % targetBase];
                value /= targetBase;
            }
            while (value > 0);

            return new string(buffer, i, buffer.Length - i);
        }

        public static long BaseToLong(string number)
        {
            char[] chrs = number.ToCharArray();
            int m = chrs.Length - 1;
            int n = BaseChars.Length, x;
            long result = 0;
            for (int i = 0; i < chrs.Length; i++)
            {
                x = CharValues[chrs[i]];
                result += x * (long)Math.Pow(n, m--);
            }
            return result;
        }
        public static string Obfuscate(string code)
        {
            // split into region + room number
            string[] reg_room = code.Split(':');
            string region = reg_room[0];
            string room = reg_room[1];
            // convert region to (random, but reversible) int
            int regionNum = GetRegionNumber(region);
            long roomNum = long.Parse(room);
            // convert both to readible base
            string newRegion = LongToBase(regionNum);
            string newRoom = LongToBase(roomNum);
            // join them with a random separator
            return $"{newRegion}{Delimeters[UnityEngine.Random.Range(0, Delimeters.Length)]}{newRoom}";

        }
        public static string DeObfuscate(string code)
        {
            // the obfuscation process in reverse

            // split into region + room number
            string[] reg_room = code.Split(Delimeters);
            string regionLong = reg_room[0];
            string roomLong = reg_room[1];
            // convert both back to decimal
            int newRegion = (int)BaseToLong(regionLong);
            long newRoom = BaseToLong(roomLong);
            // convert region number to region code
            string region = GetRegionCode(newRegion);
            // convert both to string and join them with ':'
            return $"{region}:{newRoom}";

        }
    }
}
