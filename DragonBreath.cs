using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace TenebroseItems
{
    class DragonBreath : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Fire Breath";
            string resourceName = "TenebroseItems/Resources/DragonBreath"; 
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<DragonBreath>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Dragon's Fury";
            string longDesc = "Allows the holder to breathe a stream of controllable flames.\n\nIs the ability to breathe fire inherently magical, or purely biological in nature? Who knows... What is known however, is that breathing fire is really, " +
                "really cool.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi"); 
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 5.5f);
            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;
        }

        public override void Update()
        {
            base.Update();
            if (this.m_pickedUp && this.m_isCurrentlyActive && this.LastOwner != null)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.LastOwner.PlayerIDX);
                bool flag2 = instanceForPlayer == null;
                if (!flag2)
                {
                    for(int i = 0; i < 3; i++)
                    {
                        float z = 0f;
                        if(i == 1f)
                        {
                            z = 5f;
                        }
                        else if(i == 2f)
                        {
                            z = -5f;
                        }
                        GameObject obj = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(384) as Gun).muzzleFlashEffects.effects[0].effects[0].effect, this.LastOwner.CenterPosition, Quaternion.Euler(0f, 0f,
                            BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.LastOwner.CenterPosition) + z));
                        obj.transform.localScale = new Vector3(2.4f, 0.5f, 0);
                        obj.transform.parent = this.LastOwner.transform;
                        SpeculativeRigidbody hitRigidbody = this.IterativeRaycast(this.LastOwner.CenterPosition, BraveMathCollege.DegreesToVector(BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.LastOwner.CenterPosition) + z), 
                            11.2f, int.MaxValue, this.LastOwner.specRigidbody);
                        if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                        {
                            hitRigidbody.aiActor.ApplyEffect((PickupObjectDatabase.GetById(295) as BulletStatusEffectItem).FireModifierEffect, 1, null);
                        }
                    }
                    this.LastOwner.IsGunLocked = true;
                }
                if (this.LastOwner.IsDodgeRolling)
                {
                    this.DoActiveEffect(this.LastOwner);
                }
            }
        }

        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            RaycastResult raycastResult;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, null, ignoreRigidbody))
            {
                num++;
                SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (num < 3 && speculativeRigidbody != null)
                {
                    MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (component != null)
                    {
                        component.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }
                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }
            return null;
        }

        protected override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            if (this.m_isCurrentlyActive)
            {
                this.DoActiveEffect(user);
            }
        }

        protected override void DoActiveEffect(PlayerController user)
        {
            base.DoActiveEffect(user);
            this.m_isCurrentlyActive = false;
            user.IsGunLocked = false;
        }

        protected override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Roar_01", base.gameObject);
            this.m_isCurrentlyActive = true;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && !user.IsDodgeRolling;
        }
    }
}
