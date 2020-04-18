﻿using R2API;
using RoR2;
using RoR2.Projectile;
using System.Reflection;
using UnityEngine;

namespace RoR1ItemRework
{
    public class ArmsRace
    {
        public class ArmsRaceItem
        {
            public static GameObject ArmsRacePrefab;
            public static ItemIndex ArmsRaceItemIndex;
            public static AssetBundleResourcesProvider ArmsRaceProvider;
            public static AssetBundle ArmsRaceBundle;
            private const string ModPrefix = "@RoR1ItemRework:";
            private const string PrefabPath = ModPrefix + "Assets/ArmsRace.prefab";
            private const string IconPath = ModPrefix + "Assets/ArmsRace_Icon.png";




            public static void ArmsRaceItemInit()
            {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RoR1ItemRework.armsrace"))
                {
                    ArmsRaceBundle = AssetBundle.LoadFromStream(stream);
                    ArmsRaceProvider = new AssetBundleResourcesProvider(ModPrefix.Trim(':'), ArmsRaceBundle);
                    ResourcesAPI.AddProvider(ArmsRaceProvider);
                    ArmsRacePrefab = ArmsRaceBundle.LoadAsset<GameObject>("Assets/ArmsRace.prefab");
                };

                ArmsRaceAsItem();
            }

            public static void ArmsRaceItemHook()
            {
                CharacterBody drone1 = Resources.Load<GameObject>("Prefabs/CharacterBodies/Drone1Body").GetComponent<CharacterBody>();
                CharacterBody drone2 = Resources.Load<GameObject>("Prefabs/CharacterBodies/Drone2Body").GetComponent<CharacterBody>();
                CharacterBody drone3 = Resources.Load<GameObject>("Prefabs/CharacterBodies/EmergencyDroneBody").GetComponent<CharacterBody>();
                CharacterBody drone4 = Resources.Load<GameObject>("Prefabs/CharacterBodies/MegaDroneBody").GetComponent<CharacterBody>();
                CharacterBody drone5 = Resources.Load<GameObject>("Prefabs/CharacterBodies/Turret1Body").GetComponent<CharacterBody>();
                CharacterBody drone6 = Resources.Load<GameObject>("Prefabs/CharacterBodies/MissileDroneBody").GetComponent<CharacterBody>();
                CharacterBody drone7 = Resources.Load<GameObject>("Prefabs/CharacterBodies/FlameDroneBody").GetComponent<CharacterBody>();
                CharacterBody drone8 = Resources.Load<GameObject>("Prefabs/CharacterBodies/BackupDroneBody").GetComponent<CharacterBody>();
                CharacterBody drone9 = Resources.Load<GameObject>("Prefabs/CharacterBodies/BackupDroneOldBody").GetComponent<CharacterBody>();


                On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damage, victim) =>
                {
                    var Attacker = damage.attacker.GetComponent<CharacterBody>();
                    var AttackerMaster = damage.attacker.GetComponent<CharacterMaster>();
                    if (!Attacker == drone1 || drone2 || drone3 || drone4 || drone5 || drone6 || drone7 || drone8 || drone9)
                    {
                        return;
                    }

                    if (!damage.procChainMask.HasProc(ProcType.Missile))
                    {
                        Inventory inventory = AttackerMaster.minionOwnership.ownerMaster.inventory;
                        int itemCount = inventory.GetItemCount(ArmsRaceItemIndex);
                        if (itemCount > 0)
                        {
                            GameObject gameObject = Attacker.gameObject;
                            InputBankTest component = gameObject.GetComponent<InputBankTest>();
                            Vector3 position = component ? component.aimOrigin : gameObject.transform.position;
                            Vector3 vector = component ? component.aimDirection : gameObject.transform.forward;
                            Vector3 up = Vector3.up;
                            if (Util.CheckRoll(9f * damage.procCoefficient, AttackerMaster))
                            {
                                float damageCoefficient = 3f * itemCount;
                                float dealdamage = Util.OnHitProcDamage(damage.damage, Attacker.damage, damageCoefficient);
                                ProcChainMask procChainMask2 = damage.procChainMask;
                                procChainMask2.AddProc(ProcType.Missile);
                                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                                {
                                    projectilePrefab = self.missilePrefab,
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
                        orig(self, damage, victim);
                    }


                };
            }

            private static void ArmsRaceAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_NAME_TOKEN", "Arms Race", "en");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_PICKUP_TOKEN", "On drone action: 9% chance for drones to fire missiles and mortars", "en");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_DESCRIPTION_TOKEN", "On drone action: 9% chance for drones to fire missiles and mortars,deals 300%<style=cStack>（300% per stack）damage.</style>", "en");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_LORE_TOKEN", "Drones are equipped with explosive weaponry.", "en");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_NAME_TOKEN", "军备竞赛", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_PICKUP_TOKEN", "无人机有概率发射导弹", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_DESCRIPTION_TOKEN", "无人机有9%概率发射导弹，造成300%<style=cStack>（每层增加300%）</style>的伤害。", "zh-CN");
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
                    tier = ItemTier.Tier2
                };
                ItemDisplayRule[] DisplayRules = null;
                CustomItem ArmsRaceItem = new CustomItem(ArmsRaceDef, DisplayRules);
                ArmsRaceItemIndex = ItemAPI.Add(ArmsRaceItem);

            }
        }

    }

}
