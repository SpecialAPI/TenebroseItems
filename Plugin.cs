using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace TenebroseItems
{
    [BepInPlugin(GUID, "Tenebrose Items", "1.0.8")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.tenebroseitems";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager man)
        {
            new Harmony(GUID).PatchAll();

            HeartOfFire.Init();
            DragonBreath.Init();
            DragonHandController.Init();
        }
    }
}
