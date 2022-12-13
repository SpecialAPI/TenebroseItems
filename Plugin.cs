using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using ItemAPI;

namespace TenebroseItems
{
    [BepInPlugin("spapi.etg.tenebroseitems", "Tenebrose Items", "1.0.0")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager man)
        {
            FakePrefabHooks.Init();
            ItemBuilder.Init();
            HeartOfFire.Init();
            DragonBreath.Init();
            DragonHandController.Init();
        }
    }
}
