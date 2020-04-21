using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Reflection;
using UnityEngine;

namespace RoR1ItemRework
{
    public class ArmsRace
    {
        public class ArmsRaceItem
        {
            public static ItemIndex ArmsRaceItemIndex;
            private const string ModPrefix = "@RoR1ItemRework:";
            private const string PrefabPath = ModPrefix + "Assets/ArmsRace.prefab";
            private const string IconPath = ModPrefix + "Assets/ArmsRace_Icon.png";




            public static void ArmsRaceItemInit()
            {
                ArmsRaceAsItem();
            }

            public static void ArmsRaceItemHook()
            {
                On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damage, victim) =>
                {
                    if (damage.attacker)
                    {
                        CharacterBody Attacker = damage.attacker.GetComponent<CharacterBody>();
                        CharacterMaster AttackerMaster = Attacker.master;
                        if (Attacker.name.ToLower().Contains("drone") || Attacker.name.ToLower().Contains("turret"))
                        {
                            if (!damage.procChainMask.HasProc(ProcType.Missile))
                            {
                                var inventory = AttackerMaster.minionOwnership.ownerMaster.inventory;
                                int itemCount = inventory.GetItemCount(ArmsRaceItemIndex);
                                if (itemCount > 0)
                                {
                                    GameObject gameObject = Attacker.gameObject;
                                    InputBankTest component = gameObject.GetComponent<InputBankTest>();
                                    Vector3 position = component ? component.aimOrigin : gameObject.transform.position;
                                    Vector3 vector = component ? component.aimDirection : gameObject.transform.forward;
                                    Vector3 up = Vector3.up;
                                    if (Util.CheckRoll(9f, AttackerMaster))
                                    {
                                        float damageCoefficient = 3f * itemCount;
                                        float dealdamage = Util.OnHitProcDamage(damage.damage, Attacker.damage, damageCoefficient);
                                        ProcChainMask procChainMask2 = damage.procChainMask;
                                        procChainMask2.AddProc(ProcType.Missile);
                                        FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                                        {
                                            projectilePrefab = GlobalEventManager.instance.missilePrefab,
                                            position = position,
                                            rotation = Util.QuaternionSafeLookRotation(up),
                                            procChainMask = procChainMask2,
                                            target = victim,
                                            owner = gameObject,
                                            damage = dealdamage,
                                            crit = damage.crit,
                                            force = 200f,
                                            damageColorIndex = DamageColorIndex.Item
                                        };
                                        ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                                    }
                                }
                            }
                        }
                        orig(self, damage, victim);
                    }


                };
            }

            private static void ArmsRaceAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_NAME_TOKEN", "Arms Race");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_PICKUP_TOKEN", "Drones are equipped with explosive weaponry.");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_DESCRIPTION_TOKEN", "On drone action: 9% chance for drones and turrets to fire missiles and mortars,deals <style=cIsDamage>300%</style><style=cStack>(300% per stack)</style>damage.");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_LORE_TOKEN", "Drones are equipped with explosive weaponry.");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_NAME_TOKEN", "军备竞赛", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_PICKUP_TOKEN", "无人机装备了爆炸性武器", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_DESCRIPTION_TOKEN", "无人机和炮塔有9%概率发射导弹，造成<style=cIsDamage>300%</style><style=cStack>（每层增加300%）</style>的伤害。", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_LORE_TOKEN", "无人机被爆炸性武器武装了起来。", "zh-CN");

                ItemDef ArmsRaceDef = new ItemDef
                {
                    name = "ARMSRACE_NAME_TOKEN",
                    pickupIconPath = IconPath,
                    pickupModelPath = PrefabPath,
                    nameToken = "ARMSRACE_NAME_TOKEN",
                    pickupToken = "ARMSRACE_PICKUP_TOKEN",
                    descriptionToken = "ARMSRACE_DESCRIPTION_TOKEN",
                    loreToken = "ARMSRACE_LORE_TOKEN",
                    tier = ItemTier.Tier2,
                    tags = new ItemTag[]
                    {
                        ItemTag.Damage,
                        ItemTag.Utility
                    }
                };
                ItemDisplayRule[] DisplayRules = null;
                CustomItem ArmsRaceItem = new CustomItem(ArmsRaceDef, DisplayRules);
                ArmsRaceItemIndex = ItemAPI.Add(ArmsRaceItem);

            }
        }

    }

}
