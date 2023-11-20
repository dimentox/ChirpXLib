using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Game.Achievements;
using Unity.Entities;

namespace ChirpXLib.Hooks
{
    public delegate void OnUpdateEventHandler(World world);
    [HarmonyPatch(typeof(EventAchievementTriggerSystem), "OnUpdate")]
    public class EATSHook
    {
        public static event OnUpdateEventHandler OnUpdate;
        private static void Postfix(EventAchievementTriggerSystem __instance)
        {
            try
            {
                OnUpdate?.Invoke(__instance.World);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError(e);
            }
        }
    }
}
