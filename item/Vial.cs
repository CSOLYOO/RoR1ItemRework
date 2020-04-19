using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Reflection;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace RoR1ItemRework
{
    public class Vial
    {
        public class VialItem
        {
            public static GameObject VialPrefab;
            public static ItemIndex VialItemIndex;
            public static AssetBundleResourcesProvider VialProvider;
            public static AssetBundle VialBundle;
            private const string ModPrefix = "@RoR1ItemRework:";
            private const string PrefabPath = ModPrefix + "Assets/Vial.prefab";
            private const string IconPath = ModPrefix + "Assets/Vial_Icon.png";




            public static void VialItemInit()
            {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RoR1ItemRework.vial"))
                {
                    VialBundle = AssetBundle.LoadFromStream(stream);
                    VialProvider = new AssetBundleResourcesProvider(ModPrefix.Trim(':'), VialBundle);
                    ResourcesAPI.AddProvider(VialProvider);
                    VialPrefab = VialBundle.LoadAsset<GameObject>("Assets/Vial.prefab");
                };

                VialAsItem();

                IL.RoR2.CharacterBody.RecalculateStats += (li) =>
                {
                    ILCursor c = new ILCursor(li);
                    c.GotoNext(
                        x => x.MatchLdcI4(0),
                        x => x.MatchStloc(30)
                    );
                    c.Index += 2;
                    c.Emit(OpCodes.Ldloc, 30);
                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate<Func<float, RoR2.CharacterBody, float>>(
                        (RegenStat, self) =>
                        {
                            if (self.inventory)
                                RegenStat = 1.2f * self.inventory.GetItemCount(VialItemIndex);
                            return RegenStat;
                        }
                        );
                    c.Emit(OpCodes.Stloc, 30);
                    c.GotoNext(
                        x => x.MatchLdloc(44),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(45),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(46),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(47),
                        x => x.MatchAdd()
                    );
                    c.Index += 8;
                    c.Emit(OpCodes.Ldloc, 30);
                    c.Emit(OpCodes.Add);

                };
            }



            private static void VialAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("Vial_NAME_TOKEN", "Mysterious Vial");
                R2API.AssetPlus.Languages.AddToken("Vial_PICKUP_TOKEN", "+1.2 HPregen/sec");
                R2API.AssetPlus.Languages.AddToken("Vial_DESCRIPTION_TOKEN", "+1.2 <style=cStack>(+1.2 per stack)</style>HPregen.");
                R2API.AssetPlus.Languages.AddToken("Vial_LORE_TOKEN", "WOW");
                R2API.AssetPlus.Languages.AddToken("Vial_NAME_TOKEN", "神秘药剂", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("Vial_PICKUP_TOKEN", "增加回血速度", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("Vial_DESCRIPTION_TOKEN", "+1.2<style=cStack>（每层增加1.2）</style>回血速度。", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("Vial_LORE_TOKEN", "哇哦", "zh-CN");

                ItemDef VialDef = new ItemDef
                {
                    name = "VIAL_NAME_TOKEN",
                    pickupIconPath = IconPath,
                    pickupModelPath = PrefabPath,
                    nameToken = "VIAL_NAME_TOKEN",
                    pickupToken = "VIAL_PICKUP_TOKEN",
                    descriptionToken = "VIAL_DESCRIPTION_TOKEN",
                    loreToken = "VIAL_LORE_TOKEN",
                    tier = ItemTier.Tier1
                };
                ItemDisplayRule[] DisplayRules = null;
                CustomItem VialItem = new CustomItem(VialDef, DisplayRules);
                VialItemIndex = ItemAPI.Add(VialItem);

            }
        }

    }

}
