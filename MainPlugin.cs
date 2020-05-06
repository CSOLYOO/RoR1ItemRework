using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
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
        private const string ModVer = "1.2.0";
        private const string ModName = "RoR1ItemRework";
        private const string ModGuid = "com.NetherCrowCSOLYOO.RoR1ItemRework";
        public static AssetBundleResourcesProvider Provider;
        public static AssetBundle Bundle;
        private const string ModPrefix = "@RoR1ItemRework:";
        public static ConfigFile RoRConfig { get; set; }


        public void Awake()
        {
            Setconfig();
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RoR1ItemRework.ror1item"))
            {
                Bundle = AssetBundle.LoadFromStream(stream);
                Provider = new AssetBundleResourcesProvider(ModPrefix.Trim(':'), Bundle);
                ResourcesAPI.AddProvider(Provider);
            };
            if (cfgArmsrace.Value)
            {
                ArmsRace.ArmsRaceItem.ArmsRaceItemInit();
                ArmsRace.ArmsRaceItem.ArmsRaceItemHook();
            }
            if (cfgVial.Value)
            {
                Vial.VialItem.VialItemInit();
            }
            if (cfgLeech.Value)
            {
                Leech.LeechInit();
            }
            if (cfgThallium.Value)
            {
                Thallium.ThalliumItem.ThalliumItemInit();
            }
            ClassicCritThing.CritInit();
            if (cfgToughtime.Value)
            {
                ToughTimes.ToughTimesItemInit();
            }


        }
        private void Setconfig()
        {
            RoRConfig = new ConfigFile(Paths.ConfigPath + "\\RoR1ItemRework.cfg", true);
            cfgEnableVial = RoRConfig.Bind<bool>(
                "Mysterious Vial",
                "EnablePermanentRegen",
                true,
                "If Enable, Vial's regen increment wont be affected by OnFire Buff or Difficulty"
                );
            cfgEnableRoR1Crit = RoRConfig.Bind<bool>(
                "General",
                "EnableRoR1CritStack",
                true,
                "If Enable, Predatory Instincts and Harvester's Scythe will add crit by stacking"
                );
            cfgVial = RoRConfig.Bind<bool>(
                "Mysterious Vial",
                "Enable",
                true,
                "Enable the Mysterious Vial"
                );
            cfgLeech = RoRConfig.Bind<bool>(
                "Massive Leech",
                "Enable",
                true,
                "Enable the Massive Leech"
                );
            cfgArmsrace = RoRConfig.Bind<bool>(
                "Arms Race",
                "Enable",
                true,
                "Enable the Arms Race"
                );
            cfgThallium = RoRConfig.Bind<bool>(
                "Thallium",
                "Enable",
                true,
                "Enable the Thallium"
                );
            cfgToughtime = RoRConfig.Bind<bool>(
                "Tough Times",
                "Enable",
                true,
                "Enable the Tough Times"
                );
        }

        public static ConfigEntry<bool> cfgEnableVial;
        public static ConfigEntry<bool> cfgEnableRoR1Crit;
        public static ConfigEntry<bool> cfgVial;
        public static ConfigEntry<bool> cfgLeech;
        public static ConfigEntry<bool> cfgArmsrace;
        public static ConfigEntry<bool> cfgToughtime;
        public static ConfigEntry<bool> cfgThallium;
    }


}
    
    