using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;

namespace TenebroseItems
{
    public class HeartOfFire : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Heart of Fire"; 
            string resourceName = "TenebroseItems/Resources/HeartOfFire"; 
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<HeartOfFire>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Heat in Every Beat";
            string longDesc = "Grants the user immunity to fire and the ability to double jump.\n\nEvery dragon is born with one and Tenebrose is no exception. Within the gungeon dragons are exceedingly rare in the upper levels which normally will make something like it's heart a commodity that would most likely sell for a small fortune. With that knowledge in retrospect, it wasn't such a good idea to come down here was it?";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.SPECIAL;
            Hook hook = new Hook(
                typeof(PlayerController).GetMethod("CheckDodgeRollDepth", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(HeartOfFire).GetMethod("CheckDodgeRollDepthHook")
            );
        }

        public static bool CheckDodgeRollDepthHook(Func<PlayerConsumables, bool> orig, PlayerController self)
        {
            if (self.IsSlidingOverSurface && !self.DodgeRollIsBlink)
            {
                return !self.CurrentRoom.IsShop && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.TUTORIAL;
            }
            bool flag = PassiveItem.IsFlagSetForCharacter(self, typeof(PegasusBootsItem));
            int num = (!flag) ? 1 : 2;
            if (flag && self.HasActiveBonusSynergy(CustomSynergyType.TRIPLE_JUMP, false))
            {
                num++;
            }
            foreach(PassiveItem passive in self.passiveItems)
            {
                if(passive != null && passive is HeartOfFire)
                {
                    num++;
                }
            }
            if (self.DodgeRollIsBlink)
            {
                num = 1;
            }
            return !self.IsDodgeRolling || (int)info.GetValue(self) < num;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if(player.healthHaver != null)
            {
                this.m_fireImmunity = new DamageTypeModifier
                {
                    damageType = CoreDamageTypes.Fire,
                    damageMultiplier = 0f
                };
                player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if (this.m_fireImmunity != null && player.healthHaver != null)
            {
                player.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
                this.m_fireImmunity = null;
            }
            return base.Drop(player);
        }

        private DamageTypeModifier m_fireImmunity;
        public static FieldInfo info = typeof(PlayerController).GetField("m_currentDodgeRollDepth", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
