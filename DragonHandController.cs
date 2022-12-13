using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using ItemAPI;
using UnityEngine;
using Dungeonator;

namespace TenebroseItems
{
    public class DragonHandController : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Firedrake Hand", "dragon_hand");
            Game.Items.Rename("outdated_gun_mods:firedrake_hand", "spapi:firedrake_hand");
            gun.gameObject.AddComponent<DragonHandController>();
            GunExt.SetShortDescription(gun, "Malevolent Flames");
            GunExt.SetLongDescription(gun, "Tenebrose has always had a particular affinity for setting things ablaze, weather it be because he's biologically predisposed to, or perhaps he has an innate love for destruction. We would ask him, but we prefer " +
                "our faces not to be melted off via purplish flame.");
            GunExt.SetupSprite(gun, null, "dragon_hand_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(125) as Gun, true, false);
            Projectile projectile = Instantiate((PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            projectile.damageTypes = CoreDamageTypes.None;
            projectile.DefaultTintColor = new Color(0.5f, 0f, 1f);
            projectile.HasDefaultTint = true;
            projectile.GetComponent<PierceProjModifier>().penetration = 0;
            DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.DefaultModule.numberOfShotsInClip = 7;
            gun.reloadTime = 1.4f;
            gun.StarterGunForAchievement = true;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.InfiniteAmmo = true;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.barrelOffset.transform.localPosition = new Vector3(0.1875f, 0.1875f, 0f);
            gun.gunSwitchGroup = "BurningHand";
            gun.gunClass = GunClass.FIRE;
            projectile.FireApplyChance = 0.25f;
            projectile.baseData.damage += 0.8f;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                MakeOffset(def, new Vector2(0.1875f, 0.125f));
            }
            int i = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames)
            {
                if (i == 1 || i == 6)
                {
                    frame.triggerEvent = true;
                    frame.eventAudio = "Play_WPN_blasphemy_shot_01";
                }
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                MakeOffset(def, new Vector2(0.1875f, 0.125f));
                i++;
            }
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                MakeOffset(def, new Vector2(0.125f, 0f));
            }
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames[0].eventInfo = "extinguish";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).loopStart = 1;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            float MinDamageMultiplier = 1f;
            float MaxDamageMultiplier = 5f;
            float MinScale = 0.5f;
            float MaxScale = 1.5f;
            float MaxRoll = 13f;
            int num = UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);
            int num3 = Mathf.Clamp(num, 1, 100);
            float num5 = Mathf.Lerp(MinScale, MaxScale, Mathf.Clamp01((float)num3 / MaxRoll));
            float num6 = Mathf.Lerp(MinDamageMultiplier, MaxDamageMultiplier, Mathf.Clamp01((float)num3 / MaxRoll));
            projectile.AdditionalScaleMultiplier *= num5;
            projectile.baseData.damage *= num6;
        }

        public static void MakeOffset(tk2dSpriteDefinition def, Vector2 offset)
        {
            float xOffset = offset.x;
            float yOffset = offset.y;
            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);
            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
        }
    }
}
