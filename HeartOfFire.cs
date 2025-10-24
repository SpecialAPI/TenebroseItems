using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using HarmonyLib;
using MonoMod.Cil;
using Mono.Cecil.Cil;


namespace TenebroseItems
{
    [HarmonyPatch]
    public class HeartOfFire : PassiveItem
    {
        private DamageTypeModifier m_fireImmunity;

        public static void Init()
        {
            var itemName = "Heart of Fire"; 
            var resourceName = "TenebroseItems/Resources/HeartOfFire"; 

            var obj = new GameObject(itemName);
            var item = obj.AddComponent<HeartOfFire>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            var shortDesc = "Heat in Every Beat";
            var longDesc = "Grants the user immunity to fire, the ability to double jump and increases damage to enemies on fire.\n\nEvery dragon is born with one and Tenebrose is no exception. Within the gungeon dragons are exceedingly rare in the upper levels which normally will make something like it's heart a commodity that would most likely sell for a small fortune. With that knowledge in retrospect, it wasn't such a good idea to come down here was it?";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");

            item.quality = ItemQuality.SPECIAL;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

            player.PostProcessProjectile += PostProcessProjectile;
            if (player.healthHaver == null)
                return;

            m_fireImmunity = new DamageTypeModifier
            {
                damageType = CoreDamageTypes.Fire,
                damageMultiplier = 0f
            };
            player.healthHaver.damageTypeModifiers.Add(m_fireImmunity);
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            proj.Ext().ModifyDealtDamage += DouleFireDamage;
        }

        public void DouleFireDamage(Projectile proj, HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if(hh == null || hh.gameActor == null || hh.gameActor.GetEffect("fire") == null)
                return;

            args.ModifiedDamage *= 1.5f;
        }

        public override void DisableEffect(PlayerController player)
        {
            base.DisableEffect(player);

            if (player == null)
                return;

            player.PostProcessProjectile -= PostProcessProjectile;
            if (m_fireImmunity == null || player.healthHaver == null)
                return;

            player.healthHaver.damageTypeModifiers.Remove(m_fireImmunity);
            m_fireImmunity = null;
        }

        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.CheckDodgeRollDepth))]
        [HarmonyILManipulator]
        public static void ExtraDodgeRoll_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryGotoNext(MoveType.After, x => x.MatchStloc(1)))
                return;

            var edr_a = AccessTools.Method(typeof(HeartOfFire), nameof(ExtraDodgeRoll_Add));

            crs.Emit(OpCodes.Ldloca, 1);
            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, edr_a);
        }

        public static void ExtraDodgeRoll_Add(ref int current, PlayerController player)
        {
            if (player == null || player.passiveItems == null)
                return;

            foreach (PassiveItem passive in player.passiveItems)
            {
                if (passive == null || passive is not HeartOfFire)
                    continue;

                current++;
            }
        }
    }
}
