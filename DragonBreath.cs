using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace TenebroseItems
{
    public class DragonBreath : PlayerItem
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
            item.flamesVfx = (PickupObjectDatabase.GetById(384) as Gun).muzzleFlashEffects.effects[0].effects[0].effect;
            item.fireEffect = (PickupObjectDatabase.GetById(295) as BulletStatusEffectItem).FireModifierEffect;
        }

        public override void Update()
        {
            base.Update();
            if (m_pickedUp && m_isCurrentlyActive && LastOwner != null)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(LastOwner.PlayerIDX);
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
                        GameObject obj = SpawnManager.SpawnVFX(flamesVfx, LastOwner.CenterPosition, Quaternion.Euler(0f, 0f,
                            BraveMathCollege.Atan2Degrees(LastOwner.unadjustedAimPoint.XY() - LastOwner.CenterPosition) + z));
                        obj.transform.localScale = new Vector3(2.4f, 0.5f, 0);
                        obj.transform.parent = LastOwner.transform;
                        SpeculativeRigidbody hitRigidbody = IterativeRaycast(LastOwner.CenterPosition, BraveMathCollege.DegreesToVector(BraveMathCollege.Atan2Degrees(LastOwner.unadjustedAimPoint.XY() - LastOwner.CenterPosition) + z), 
                            11.2f, int.MaxValue, LastOwner.specRigidbody);
                        if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                        {
                            hitRigidbody.aiActor.ApplyEffect(fireEffect, 1, null);
                        }
                    }
                    LastOwner.IsGunLocked = true;
                }
                if (LastOwner.IsDodgeRolling)
                {
                    DoActiveEffect(LastOwner);
                }
            }
        }

        protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            int num = 0;
            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out var raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, null, ignoreRigidbody))
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

        public override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            if (m_isCurrentlyActive)
            {
                DoActiveEffect(user);
            }
        }

        public override void DoActiveEffect(PlayerController user)
        {
            base.DoActiveEffect(user);
            m_isCurrentlyActive = false;
            user.IsGunLocked = false;
        }

        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_BOSS_DragunGold_Roar_01", base.gameObject);
            m_isCurrentlyActive = true;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user != null && !user.IsDodgeRolling;
        }

        public GameObject flamesVfx;
        public GameActorFireEffect fireEffect;
    }
}
