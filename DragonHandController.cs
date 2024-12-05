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
            var gun = ETGMod.Databases.Items.NewGun("Firedrake Hand", "dragon_hand");
            Game.Items.Rename("outdated_gun_mods:firedrake_hand", "spapi:firedrake_hand");

            gun.gameObject.AddComponent<DragonHandController>();
            gun.SetShortDescription("Malevolent Flames");
            gun.SetLongDescription("Tenebrose has always had a particular affinity for setting things ablaze, weather it be because he's biologically predisposed to, or perhaps he has an innate love for destruction. We would ask him, but we prefer " +
                "our faces not to be melted off via purplish flame.");

            gun.SetupSprite(null, "dragon_hand_idle_001", 8);
            gun.SetAnimationFPS(gun.shootAnimation, 16);
            gun.SetAnimationFPS(gun.reloadAnimation, 8);

            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(125) as Gun, true, false);
            var projectile = Instantiate((PickupObjectDatabase.GetById(125) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            DontDestroyOnLoad(projectile);

            projectile.damageTypes = CoreDamageTypes.None;
            projectile.DefaultTintColor = new Color(0.5f, 0f, 1f);
            projectile.HasDefaultTint = true;
            projectile.GetComponent<PierceProjModifier>().penetration = 0;
            projectile.FireApplyChance = 0.25f;
            projectile.baseData.damage += 0.8f;

            gun.DefaultModule.projectiles[0] = projectile;
            gun.DefaultModule.numberOfShotsInClip = 7;
            gun.reloadTime = 1.4f;
            gun.StarterGunForAchievement = true;
            gun.gunHandedness = GunHandedness.HiddenOneHanded;
            gun.InfiniteAmmo = true;
            gun.muzzleFlashEffects.type = VFXPoolType.None;

            gun.barrelOffset.transform.localPosition = new Vector3(0.1875f, 0.1875f, 0f);
            gun.gunSwitchGroup = "BurningHand";
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.gunClass = GunClass.FIRE;

            var animator = gun.GetComponent<tk2dSpriteAnimator>();
            var emptyAnim = animator.GetClipByName(gun.emptyAnimation);
            var reloadAnim = animator.GetClipByName(gun.reloadAnimation);
            var shootAnim = animator.GetClipByName(gun.shootAnimation);

            emptyAnim.wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            emptyAnim.loopStart = 1;
            foreach (var frame in emptyAnim.frames)
            {
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                MakeOffset(def, new Vector2(0.1875f, 0.125f));
            }

            for(var i = 0; i < reloadAnim.frames.Length; i++)
            {
                var frame = reloadAnim.frames[i];

                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                MakeOffset(def, new Vector2(0.1875f, 0.125f));

                if (i != 1 && i != 6)
                    continue;

                frame.triggerEvent = true;
                frame.eventAudio = "Play_WPN_blasphemy_shot_01";
            }

            foreach (var frame in shootAnim.frames)
            {
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                MakeOffset(def, new Vector2(0.125f, 0f));
            }

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            const float MinDamageMultiplier = 1f;
            const float MaxDamageMultiplier = 5f;
            const float MinScale = 0.5f;
            const float MaxScale = 1.5f;
            const float MaxRoll = 13f;

            var roll = UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);

            var t = roll / MaxRoll;
            var scaleMult = Mathf.Lerp(MinScale, MaxScale, t);
            var dmgMult = Mathf.Lerp(MinDamageMultiplier, MaxDamageMultiplier, t);

            projectile.AdditionalScaleMultiplier *= scaleMult;
            projectile.baseData.damage *= dmgMult;
        }

        public static void MakeOffset(tk2dSpriteDefinition def, Vector2 offset)
        {
            var xOffset = offset.x;
            var yOffset = offset.y;

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
