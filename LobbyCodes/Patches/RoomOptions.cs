using HarmonyLib;
using Photon.Realtime;
using System;
using UnboundLib;
namespace LobbyCodes.Patches
{
    // patch to forcefully set RoomOptions.PublishUserId = true for rooms created by us
    [HarmonyPatch(typeof(Photon.Realtime.RoomOptions), MethodType.Constructor)]
    class RoomOptions_Patch_Constructor
    {
        static void Postfix(RoomOptions __instance)
        {
            __instance.SetPropertyValue(nameof(__instance.PublishUserId), true);
        }
    }
}
