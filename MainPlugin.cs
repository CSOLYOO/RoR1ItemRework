using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System.Reflection;
using UnityEngine;

namespace RoR1ItemRework
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [R2APISubmoduleDependency(new string[]
    {
        "ItemAPI",
        "ItemDropAPI",
        "ResourcesAPI",
        "AssetPlus",
        "BuffAPI",
        "LanguageAPI",
    })]
    [BepInPlugin(ModGuid, ModName, ModVer)]

    public class RoR1ItemRework : BaseUnityPlugin
    {
        private const string ModVer = "1.0.6";
        private const string ModName = "RoR1ItemRework";
        private const string ModGuid = "com.NetherCrowCSOLYOO.RoR1ItemRework";
        public static AssetBundleResourcesProvider Provider;
        public static AssetBundle Bundle;
        private const string ModPrefix = "@RoR1ItemRework:";
        public static ConfigFile RoRConfig { get; set; }
        public static ConfigEntry<bool> cfgEnableVial;


        public void Awake()
        {
            RoRConfig = new ConfigFile(Paths.ConfigPath + "\\RoR1ItemRework.cfg", true);
            cfgEnableVial = RoRConfig.Bind<bool>(
                "Vial",
                "EnablePermanentRegen",
                true,
                "If Enable, Vial's regen increment wont be affected by OnFire Buff or Difficulty"
                );


            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RoR1ItemRework.ror1item"))
            {
                Bundle = AssetBundle.LoadFromStream(stream);
                Provider = new AssetBundleResourcesProvider(ModPrefix.Trim(':'), Bundle);
                ResourcesAPI.AddProvider(Provider);
            };
            ArmsRace.ArmsRaceItem.ArmsRaceItemInit();
            ArmsRace.ArmsRaceItem.ArmsRaceItemHook();
            Vial.VialItem.VialItemInit();
            Thallium.ThalliumItem.ThalliumItemInit();
            item.ClassicCritThing.CritInit();
            ToughTimes.ToughTimesItemInit();
        }

    }


}
    
    