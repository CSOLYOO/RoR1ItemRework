using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
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


            internal static void Init()
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

            private static void ArmsRaceAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_NAME_TOKEN", "Arms Race", "English");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_PICKUP_TOKEN", "On drone action: 9% chance for drones to fire missiles and mortars", "English");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_DESCRIPTION_TOKEN", "On drone action: 9% chance for drones to fire missiles and mortars,deals 300%<style=cStack>（300% per stack）damage.</style>", "English");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_LORE_TOKEN", "Drones are equipped with explosive weaponry.", "English");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_NAME_TOKEN", "军备竞赛", "sChinese");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_PICKUP_TOKEN", "无人机有概率发射导弹", "sChinese");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_DESCRIPTION_TOKEN", "无人机有9%概率发射导弹，造成300%<style=cStack>（每层增加300%）</style>的伤害。", "sChinese");
                R2API.AssetPlus.Languages.AddToken("ARMSRACE_LORE_TOKEN", "无人机被爆炸性武器武装了起来。.", "sChinese");

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
