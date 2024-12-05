using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace TenebroseItems
{
    [HarmonyPatch]
    public class ProjectileExt : MonoBehaviour
    {
        public Action<Projectile, HealthHaver, HealthHaver.ModifyDamageEventArgs> ModifyDealtDamage;

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.HandleDamage))]
        [HarmonyILManipulator]
        public static void ModifyProjectileDamage_Transpiler(ILContext ctx)
        {
            var crs = new ILCursor(ctx);

            if (!crs.TryGotoNext(x => x.MatchStloc(4)))
                return;

            var mpd_m = AccessTools.Method(typeof(ProjectileExt), nameof(ModifyProjectileDamage_Modify));

            crs.Emit(OpCodes.Ldarg_1);
            crs.Emit(OpCodes.Ldarg_0);
            crs.Emit(OpCodes.Call, mpd_m);
        }

        public static float ModifyProjectileDamage_Modify(float dmg, SpeculativeRigidbody rb, Projectile proj)
        {
            if (rb == null)
                return dmg;

            var hh = rb.healthHaver;

            if (hh == null)
                return dmg;

            var projExt = proj.Ext();
            if (projExt != null && projExt.ModifyDealtDamage != null)
            {
                var args = new HealthHaver.ModifyDamageEventArgs()
                {
                    InitialDamage = dmg,
                    ModifiedDamage = dmg
                };

                projExt.ModifyDealtDamage?.Invoke(proj, hh, args);
                dmg = args.ModifiedDamage;
            }

            return dmg;
        }
    }
    public static class ExtTools
    {
        public static T AddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component c) where T : Component
        {
            return c.gameObject.GetOrAddComponent<T>();
        }

        public static ProjectileExt Ext(this Projectile proj)
        {
            if (proj == null)
                return null;

            return proj.GetOrAddComponent<ProjectileExt>();
        }
    }
}
