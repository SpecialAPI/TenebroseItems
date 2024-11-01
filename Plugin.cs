using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;
using ItemAPI;

namespace TenebroseItems
{
    [BepInPlugin(GUID, "Tenebrose Items", "1.0.5")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.tenebroseitems";

        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager man)
        {
            new Harmony(GUID).PatchAll();

            FakePrefabHooks.Init();
            ItemBuilder.Init();
            HeartOfFire.Init();
            DragonBreath.Init();
            DragonHandController.Init();
        }
    }
}
