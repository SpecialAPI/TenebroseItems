using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;

namespace TenebroseItems
{
    public class DragonBreath : PlayerItem
    {
        public static void Init()
        {
            var itemName = "Fire Breath";
            var resourceName = "TenebroseItems/Resources/DragonBreath"; 

            var obj = new GameObject(itemName);
            var item = obj.AddComponent<DragonBreath>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            var shortDesc = "Dragon's Fury";
            var longDesc = "Allows the holder to breathe a stream of controllable flames.\n\nIs the ability to breathe fire inherently magical, or purely biological in nature? Who knows... What is known however, is that breathing fire is really, really cool.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi"); 
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 5.5f);

            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;

            item.flamesVfx = (PickupObjectDatabase.GetById(384) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
            item.fireEffect = (PickupObjectDatabase.GetById(295) as BulletStatusEffectItem).FireModifierEffect;
        }

        public override void Update()
        {
            base.Update();

            if (!PickedUp || !IsCurrentlyActive || LastOwner == null)
                return;

            var owner = LastOwner;
            var input = BraveInput.GetInstanceForPlayer(owner.PlayerIDX);

            owner.IsGunLocked = true;
            if (owner.IsDodgeRolling)
                DoActiveEffect(owner);

            if (input == null)
                return;

            for (var i = -1; i <= 1; i++)
            {
                var angleOffset = i * 5f;
                var angle = (owner.unadjustedAimPoint.XY() - owner.CenterPosition).ToAngle() + angleOffset;

                var fire = SpawnManager.SpawnVFX(flamesVfx, owner.CenterPosition, Quaternion.Euler(0f, 0f, angle));
                fire.transform.localScale = new Vector3(2.4f, 0.5f, 0);
                fire.transform.parent = owner.transform;

                var hitRigidbody = IterativeRaycast(owner.CenterPosition, BraveMathCollege.DegreesToVector(angle), 11.2f, int.MaxValue, owner.specRigidbody);

                if (!hitRigidbody || !hitRigidbody.aiActor || !hitRigidbody.aiActor.IsNormalEnemy)
                    continue;

                hitRigidbody.aiActor.ApplyEffect(fireEffect, 1, null);
            }
        }

        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            var hitRBs = 0;

            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out var raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, null, ignoreRigidbody))
            {
                hitRBs++;
                var speculativeRigidbody = raycastResult.SpeculativeRigidbody;

                if (hitRBs < 3 && speculativeRigidbody != null)
                {
                    var breakable = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (breakable != null)
                    {
                        breakable.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);

                        continue;
                    }
                }

                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }

        public override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);

            if (IsCurrentlyActive)
                DoActiveEffect(user);
        }

        public override void DoActiveEffect(PlayerController user)
        {
            base.DoActiveEffect(user);

            IsCurrentlyActive = false;
            user.IsGunLocked = false;
        }

        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Roar_01", gameObject);
            IsCurrentlyActive = true;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && !user.IsDodgeRolling;
        }

        public GameObject flamesVfx;
        public GameActorFireEffect fireEffect;
    }
}
