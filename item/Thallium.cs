using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace RoR1ItemRework
{
    public class Thallium
    {
        public class ThalliumItem
        {
            public static ItemIndex ThalliumItemIndex;
            private const string ModPrefix = "@RoR1ItemRework:";
            private const string PrefabPath = ModPrefix + "Assets/Thallium.prefab";
            private const string IconPath = ModPrefix + "Assets/Thallium_Icon.png";
            private const string BuffIconPath = ModPrefix + "Assets/ThalliumBuff.png";
            private static BuffIndex ThalliumDebuff;





            public static void ThalliumItemInit()
            {
                ThalliumAsBuff();
                ThalliumAsItem();
                ThalliumItemHook();
            }

            public static void ThalliumItemHook()
            {
                On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damage, victim) =>
                {
                    if (damage.attacker)
                    {
                        CharacterBody Attacker = damage.attacker.GetComponent<CharacterBody>();
                        CharacterMaster AttackerMaster = Attacker.master;
                        CharacterBody VictimBody = victim ? victim.GetComponent<CharacterBody>() : null;
                        if (Attacker && AttackerMaster)
                        {
                            int itemcount = Attacker.inventory.GetItemCount(ThalliumItemIndex);
                            if ((itemcount > 0) && (Util.CheckRoll(5f * damage.procCoefficient, AttackerMaster)))
                            {
                                ProcChainMask procChainMask = damage.procChainMask;
                                procChainMask.AddProc(ProcType.BleedOnHit);
                                DotController.InflictDot(victim, damage.attacker, DotController.DotIndex.Blight, 3f * damage.procCoefficient, 3.33f * itemcount);
                                VictimBody.AddTimedBuff(ThalliumDebuff, 3f * damage.procCoefficient);
                            }
                        }
                    }
                    orig(self, damage, victim);
                };
                void ThalliumILHook(ILContext il)
                {
                    var c = new ILCursor(il);
                    c.GotoNext(x => x.MatchCallvirt<CharacterBody>("set_moveSpeed"));
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<CharacterBody, float>>((cb) =>
                    {
                        if (cb.HasBuff(ThalliumDebuff))
                        {
                            return 0.5f;
                        }
                        return 1.0f;
                    });
                    c.Emit(OpCodes.Mul);

                }
                IL.RoR2.CharacterBody.RecalculateStats += ThalliumILHook;

            }

            private static void ThalliumAsItem()
            {
                LanguageAPI.Add("THALLIUM_NAME_TOKEN", "Thallium");
                LanguageAPI.Add("THALLIUM_PICKUP_TOKEN", "Chance to slow and damage enemies over time.");
                LanguageAPI.Add("THALLIUM_DESCRIPTION_TOKEN", "<style=cIsDamage>5%</style> chance on hit to slow an enemy with a <style=cIsDamage>metal poisoning</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>100%</style> and dealing <style=cIsDamage>600%</style> <style=cStack>(+600% per stack)</style> TOTAL damage.");
                LanguageAPI.Add("THALLIUM_LORE_TOKEN", "Shipping Method: High Priority / Fragile\r\nOrder Details: She shouldn't notice.");
                LanguageAPI.Add("THALLIUM_NAME_TOKEN", "铊", "zh-CN");
                LanguageAPI.Add("THALLIUM_PICKUP_TOKEN", "几率减速敌人，并且使敌人中毒。", "zh-CN");
                LanguageAPI.Add("THALLIUM_DESCRIPTION_TOKEN", "有<style=cIsDamage>5%</style> 几率使得敌人<style=cIsDamage>金属中毒</style>，并<style=cIsUtility>减速</style>其<style=cIsUtility>100%</style>，造成<style=cIsDamage>600%</style> <style=cStack>(每层增加600%)</style> 总伤害。", "zh-CN");
                LanguageAPI.Add("THALLIUM_LORE_TOKEN", "运输方式：高优先级 / 脆弱\r\n商品细节：她不会注意到的。", "zh-CN");

                ItemDef ThalliumDef = new ItemDef
                {
                    name = "THALLIUM_NAME_TOKEN",
                    pickupIconPath = IconPath,
                    pickupModelPath = PrefabPath,
                    nameToken = "THALLIUM_NAME_TOKEN",
                    pickupToken = "THALLIUM_PICKUP_TOKEN",
                    descriptionToken = "THALLIUM_DESCRIPTION_TOKEN",
                    loreToken = "THALLIUM_LORE_TOKEN",
                    tier = ItemTier.Tier3,
                    tags = new ItemTag[]
                    {
                        ItemTag.Damage
                    }
                };
                ItemDisplayRule[] DisplayRules = null;
                CustomItem ThalliumItem = new CustomItem(ThalliumDef, DisplayRules);
                ThalliumItemIndex = ItemAPI.Add(ThalliumItem);

            }
            private static void ThalliumAsBuff()
            {
                BuffDef ThalliumBuffDef = new BuffDef
                {
                    iconPath = BuffIconPath,
                    canStack = false,
                    eliteIndex = EliteIndex.None,
                    isDebuff = true,
                    name = "ThalliumDeBuff"
                };
                ThalliumDebuff = BuffAPI.Add(new CustomBuff(ThalliumBuffDef));
            }
        }

    }
}
