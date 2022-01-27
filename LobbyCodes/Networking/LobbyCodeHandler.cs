using Photon.Pun;
using LobbyCodes.Utils;
using System;
using System.Linq;

namespace LobbyCodes.Networking
{
    public static class LobbyCodeHandler
    {
        private readonly static string[] Regions = new string[] { "asia", "au", "cae", "cn", "eu", "in", "jp", "ru", "rue", "za", "sa", "kr", "tr", "us", "usw" };
        private static string GetPureCode()
        {
            if (PhotonNetwork.OfflineMode || PhotonNetwork.CurrentRoom == null) { return ""; }
            return $"{PhotonNetwork.CloudRegion}:{PhotonNetwork.CurrentRoom.Name}";
        }
        public static string GetCode()
        {
            return ObfuscateJoinCode.Obfuscate(GetPureCode());
        }
        private static ExitCode PureConnectToRoom(string pureCode)
        {
            ExitCode exitCode = ExitCode.Success;

            try
            {
                string[] reg_room = pureCode.Split(':');
                string region = reg_room[0];
                string room = reg_room[1];
                if (!Regions.Contains(region) || !room.All(char.IsDigit))
                {
                    throw new FormatException();
                }
                NetworkConnectionHandler.instance.ForceRegionJoin(region, room);
            }
            catch (IndexOutOfRangeException)
            {
                exitCode = ExitCode.Invalid;
            }
            catch (FormatException)
            {
                exitCode = ExitCode.Invalid;
            }
            catch (Exception)
            {
                exitCode = ExitCode.UnknownError;
            }
            return exitCode;
        }
        public static ExitCode ConnectToRoom(string obfuscatedCode)
        {
            return PureConnectToRoom(ObfuscateJoinCode.DeObfuscate(obfuscatedCode));
        }

        public enum ExitCode
        {
            Success,
            Invalid,
            UnknownError
        }

    }
}
