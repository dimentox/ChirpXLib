
using BepInEx;
using BepInEx.Logging;
using ChirpXLib.Utils;
using HarmonyLib;
using System.Reflection;
using System.IO;
using System.Reflection;
using ATL.Logging;
using Unity.Entities;
using UnityEngine;
using Colossal.Plugins;


namespace ChirpXLib
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Harmony harmony;
        public static bool isInitialized = false;

        public static ManualLogSource Logger;

        public void Awake()
        {

            Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
            harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            TaskRunner.Initialize();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is Initialized!");
        }

      
    }
}