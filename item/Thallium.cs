using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace RoR1ItemRework
{
    public class Thallium
    {
        public class ThalliumItem
        {
            public static GameObject ThalliumPrefab;
            public static ItemIndex ThalliumItemIndex;
            public static AssetBundleResourcesProvider ThalliumProvider;
            public static AssetBundle ThalliumBundle;
            private const string ModPrefix = "@RoR1ItemRework:";
            private const string PrefabPath = ModPrefix + "Assets/Thallium.prefab";
            private const string IconPath = ModPrefix + "Assets/Thallium_Icon.png";
            private const string BuffIconPath = ModPrefix + "Assets/ThalliumBuff.png";
            private static BuffIndex ThalliumDebuff;





            public static void ThalliumItemInit()
            {
                ThalliumAsBuff();
                ThalliumAsItem();
                
            }

            public static void ThalliumItemHook()
            {
                
            }

            private static void ThalliumAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("THALLIUM_NAME_TOKEN", "Thallium");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_PICKUP_TOKEN", "Chance to slow and damage enemies over time.");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_DESCRIPTION_TOKEN", "<style=cIsDamage>5%</style> chance on hit to slow an enemy with a <style=cIsDamage>metal poisoning</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>100%</style> and dealing <style=cIsDamage>600%</style> <style=cStack>(+600% per stack)</style> TOTAL damage.");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_LORE_TOKEN", "Shipping Method: High Priority / Fragile\r\nOrder Details: She shouldn't notice.");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_NAME_TOKEN", "铊", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_PICKUP_TOKEN", "几率减速敌人，并且使敌人中毒。", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_DESCRIPTION_TOKEN", "有<style=cIsDamage>5%</style> 几率使得敌人<style=cIsDamage>金属中毒</style>，并<style=cIsUtility>减速</style>其<style=cIsUtility>100%</style>，造成<style=cIsDamage>600%</style> <style=cStack>(+600% per stack)</style> 总伤害。", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("THALLIUM_LORE_TOKEN", "运输方式：高优先级 / 脆弱\r\n商品细节：她不会注意到的。", "zh-CN");

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
                    canStack = true,
                    eliteIndex = EliteIndex.None,
                    isDebuff = true,
                    name = "ThalliumDeBuff"
                };
                ThalliumDebuff = ItemAPI.Add(new CustomBuff(ThalliumBuffDef.name, ThalliumBuffDef));
            }
        }

    }
}
