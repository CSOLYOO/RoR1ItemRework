using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using R2API.Utils;

namespace RoR1ItemRework
{
    class RepulsionArmor
    {
        private static ItemIndex RepulsionArmorItemIndex;
        private const string ModPrefix = "@RoR1ItemRework:";
        private const string PrefabPath = ModPrefix + "Assets/RepulsionArmor.prefab";
        private const string IconPath = ModPrefix + "Assets/RepulsionArmor_Icon.png";
        private const string BuffIconPath = ModPrefix + "Assets/RepulsionArmor_Icon.png";
        private static BuffIndex RepulsionArmorStackbuff;
        private static BuffIndex RepulsionArmorArmorbuff;

        private static void RepulsionArmorAsItem()
        {
            LanguageAPI.Add("REPULSIONARMOR_NAME_TOKEN", "Repulsion Armor");
            LanguageAPI.Add("REPULSIONARMOR_PICKUP_TOKEN", "After taking damage, reflect all attacks for seconds.");
            LanguageAPI.Add("REPULSIONARMOR_DESCRIPTION_TOKEN", "After <style=cIsUtility>6</style> hits <style=cIsHealing>Increase 500 armors</style> for <style=cIsUtility>3</style> <style=cStack>(+1 per stack)</style> seconds.");
            LanguageAPI.Add("REPULSIONARMOR_LORE_TOKEN", @"- Shipping Method:  High Priority / Fragile
- Order Details :  Prototype repulsion armor: it doesn't quite work as advertised. The protective shielding will activate, but only after... repeated triggers. You'll have to get hit a lot of times before it activates. We have tried increasing the sensitivity, but it lead to the armor triggering from simple movements. We will send an improved prototype next month.
");
            LanguageAPI.Add("REPULSIONARMOR_NAME_TOKEN", "斥力装甲", "zh-CN");
            LanguageAPI.Add("REPULSIONARMOR_PICKUP_TOKEN", "受到伤害后，在几秒内降低受到的攻击。", "zh-CN");
            LanguageAPI.Add("REPULSIONARMOR_DESCRIPTION_TOKEN", "受到<style=cIsUtility>6</style>次攻击后，增加<style=cIsHealing>500护甲</style>, 持续<style=cIsUtility>3秒</style> <style=cStack>（每层增加1秒）</style>。", "zh-CN");
            LanguageAPI.Add("REPULSIONARMOR_LORE_TOKEN", "运输方式：高优先级 / 脆弱\r\n商品细节：原型排斥装甲：它并没有像宣传的那样起作用。保护性屏蔽将被激活，但仅在...反复触发后才能激活。在激活之前，您必须被击中很多次。我们已经尝试提高灵敏度，但是它会导致装甲由简单的动作触发。我们将在下个月发送改进的原型。", "zh-CN");

            ItemDef RepulsionArmorDef = new ItemDef
            {
                name = "REPULSIONARMOR_NAME_TOKEN",
                pickupIconPath = IconPath,
                pickupModelPath = PrefabPath,
                nameToken = "REPULSIONARMOR_NAME_TOKEN",
                pickupToken = "REPULSIONARMOR_PICKUP_TOKEN",
                descriptionToken = "REPULSIONARMOR_DESCRIPTION_TOKEN",
                loreToken = "REPULSIONARMOR_LORE_TOKEN",
                tier = ItemTier.Tier3,
                tags = new ItemTag[]
                    {
                        ItemTag.Healing,
                        ItemTag.Utility
                    }
            };
            ItemDisplayRule[] DisplayRules = null;
            CustomItem RepulsionArmorItem = new CustomItem(RepulsionArmorDef, DisplayRules);
            RepulsionArmorItemIndex = ItemAPI.Add(RepulsionArmorItem);

            BuffDef RepulsionArmorStackbuffDef = new BuffDef
            {
                iconPath = BuffIconPath,
                canStack = true,
                eliteIndex = EliteIndex.None,
                isDebuff = false,
                name = "RepulsionArmorStackbuff"
            };
            RepulsionArmorStackbuff = BuffAPI.Add(new CustomBuff(RepulsionArmorStackbuffDef));

            BuffDef RepulsionArmorArmorbuffDef = new BuffDef
            {
                iconPath = "Textures/BuffIcons/texBuffGenericShield",
                buffColor = new Color(0.9137255f, 0.37254903f, 0.1882353f),
                canStack = false,
                eliteIndex = EliteIndex.None,
                isDebuff = false,
                name = "RepulsionArmorArmorbuff"
            };
            RepulsionArmorArmorbuff = BuffAPI.Add(new CustomBuff(RepulsionArmorArmorbuffDef));
        }

        private static void RepulsionArmorHook()
        {
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damage) =>
            {
                orig(self,damage);
                CharacterMaster master = self.body.master;
                if (master && master.inventory)
                {
                    int itemCount = master.inventory.GetItemCount(RepulsionArmorItemIndex);
                    if (itemCount > 0 && !self.body.HasBuff(RepulsionArmorArmorbuff))
                    {
                        self.body.AddTimedBuff(RepulsionArmorStackbuff, 3f);
                        if (self.body.GetBuffCount(RepulsionArmorStackbuff) >= 6)
                        {
                            self.body.ClearTimedBuffs(RepulsionArmorStackbuff);
                            self.body.AddTimedBuff(RepulsionArmorArmorbuff, 2f + (float)itemCount); 
                        }
                    }
                }
            };
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
             {
                 orig(self);
                 if (self && self.inventory)
                 {
                     if (self.HasBuff(RepulsionArmorArmorbuff))
                     {
                         Reflection.SetPropertyValue(self, "armor", self.armor + 500f);
                     }
                 }
             };
        }

        public static void RepulsionArmorItemInit()
        {
            RepulsionArmorAsItem();
            RepulsionArmorHook();
        }
    }
}
