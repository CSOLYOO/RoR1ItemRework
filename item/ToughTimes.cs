using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using R2API.Utils;

namespace RoR1ItemRework
{
    class ToughTimes
    {
        public static ItemIndex ToughTimesItemIndex;
        private const string ModPrefix = "@RoR1ItemRework:";
        private const string PrefabPath = ModPrefix + "Assets/ToughTimes.prefab";
        private const string IconPath = ModPrefix + "Assets/ToughTimes_Icon.png";

        public static void ToughTimesItemInit()
        {
            ToughTimesAsItem();
            ToughTimesItemHook();
        }

        public static void ToughTimesItemHook()
        {
            On.RoR2.CharacterBody.RecalculateStats += OnArmorAdd;
        }
        private static void ToughTimesAsItem()
        {
            LanguageAPI.Add("TOUGHTIMES_NAME_TOKEN", "Tough Times");
            LanguageAPI.Add("TOUGHTIMES_PICKUP_TOKEN", "Add Armor to reduce incoming damage.");
            LanguageAPI.Add("TOUGHTIMES_DESCRIPTION_TOKEN", "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>14</style> <style=cStack>(+14 per stack)</style>.");
            LanguageAPI.Add("TOUGHTIMES_LORE_TOKEN", "- Order Details:\"Bears are just about the only toy that can lose just about everything and still maintain their dignity and worth.\" \r\n- Samantha Armstrong.Don't forget, hon. I'm coming home soon.Stay strong.");
            LanguageAPI.Add("TOUGHTIMES_NAME_TOKEN", "艰难时光", "zh-CN");
            LanguageAPI.Add("TOUGHTIMES_PICKUP_TOKEN", "提高自身护甲", "zh-CN");
            LanguageAPI.Add("TOUGHTIMES_DESCRIPTION_TOKEN", "<style=cIsHealing>提高护甲14点</style><style=cStack>（每层增加14点）</style>。", "zh-CN");
            LanguageAPI.Add("TOUGHTIMES_LORE_TOKEN", "-商品细节：“熊是唯一可以失去几乎所有东西并仍然保持其尊严和价值的玩具”。\r\n-萨曼莎·阿姆斯特朗：别忘了，亲爱的。我很快就要回家了。", "zh-CN");

            ItemDef ToughTimesDef = new ItemDef
            {
                name = "TOUGHTIMES_NAME_TOKEN",
                pickupIconPath = IconPath,
                pickupModelPath = PrefabPath,
                nameToken = "TOUGHTIMES_NAME_TOKEN",
                pickupToken = "TOUGHTIMES_PICKUP_TOKEN",
                descriptionToken = "TOUGHTIMES_DESCRIPTION_TOKEN",
                loreToken = "TOUGHTIMES_LORE_TOKEN",
                tier = ItemTier.Tier2,
                tags = new ItemTag[]
                {
                        ItemTag.Utility
                }
            };
            ItemDisplayRule[] DisplayRules = null;
            CustomItem ToughTimesItem = new CustomItem(ToughTimesDef, DisplayRules);
            ToughTimesItemIndex = ItemAPI.Add(ToughTimesItem);

        }

        private static void OnArmorAdd(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                int count1 = self.inventory.GetItemCount(ToughTimesItemIndex);
                float ArmorIncrement = 0f;
                if (count1 > 0)
                {
                    ArmorIncrement += 14f * count1;
                }
                Reflection.SetPropertyValue(self, "armor", self.armor + ArmorIncrement);
            }
        }
    }
}

