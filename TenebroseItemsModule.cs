using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemAPI;

namespace TenebroseItems
{
    public class TenebroseItemsModule : ETGModule
    {
        public override void Init()
        {
        }

        public override void Start()
        {
            FakePrefabHooks.Init();
            ItemBuilder.Init();
            HeartOfFire.Init();
            DragonBreath.Init();
            DragonHandController.Init();
        }

        public override void Exit()
        {
        }
    }
}
