namespace LobbyCodes.Extensions
{
    public static class PhotonPlayerExtensions
    {
        public static void SetProperty(this Photon.Realtime.Player instance, string key, object value) {
            var propKey = LobbyCodes.GetConfigKey(key);
            var props = instance.CustomProperties;

            if (!props.ContainsKey(propKey)) {
                props.Add(propKey, value);
            } else {
                props[propKey] = value;
            }

            instance.SetCustomProperties(props);
        }

        public static T GetProperty<T>(this Photon.Realtime.Player instance, string key) {
            var propKey = LobbyCodes.GetConfigKey(key);
            var props = instance.CustomProperties;

            if (!props.ContainsKey(propKey)) {
                return default;
            }

            return (T)props[propKey];
        }

        public static void SetModded(this Photon.Realtime.Player instance) {
            instance.SetProperty("modded", true);
        }

        public static bool IsModded(this Photon.Realtime.Player instance) {
            return instance.GetProperty<bool>("modded");
        }

        private static readonly string HostInvitedKey = LobbyCodes.GetConfigKey("InviteFromHost");
        public static bool WasInvitedByHost(this Photon.Realtime.Player instance)
        {
            return instance.GetProperty<bool>(PhotonPlayerExtensions.HostInvitedKey);
        }
        public static void SetWasInvitedByHost(this Photon.Realtime.Player instance, bool wasInvitedByHost)
        {
            instance.SetProperty(PhotonPlayerExtensions.HostInvitedKey, wasInvitedByHost);
        }

        public const string OnlyHostCanInviteKey = "OnlyHostCanInvite";
        public static bool OnlyHostCanInvite(this Photon.Realtime.Player instance)
        {
            return instance.GetProperty<bool>(PhotonPlayerExtensions.OnlyHostCanInviteKey);
        }
        public static void SetOnlyHostCanInvite(this Photon.Realtime.Player instance, bool onlyHostCanInvite)
        {
            instance.SetProperty(PhotonPlayerExtensions.OnlyHostCanInviteKey, onlyHostCanInvite);
        }
    }
}
