using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace TenebroseItems
{
    [BepInPlugin(MOD_GUID, "Tenebrose Items", "1.0.8")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInDependency(Alexandria.Alexandria.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "spapi.etg.tenebroseitems";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager man)
        {
            new Harmony(MOD_GUID).PatchAll();
            ETGMod.Assets.SetupSpritesFromAssembly(typeof(Plugin).Assembly, "TenebroseItems/Resources/MTGAPISpriteRoot");

            HeartOfFire.Init();
            DragonBreath.Init();
            DragonHandController.Init();
        }
    }
}
