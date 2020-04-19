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
                VialAsItem();
            }

            public static void VialItemHook()
            {
                void VialHook(ILContext li)
                {
                    ILCursor c = new ILCursor(li);
                    c.GotoNext(
                        x => x.MatchLdloc(44),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(45),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(46),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(47),
                        x => x.MatchAdd(),
                        x => x.MatchLdloc(48),
                        x => x.MatchMul()
                    );
                    c.Index += 8;
                    c.EmitDelegate<Func<RoR2.CharacterBody, float>>((self) =>
                        {
                            if (self.inventory)
                            {
                                float RegenStats = self.inventory.GetItemCount(VialItemIndex) * 1.2f;
                                return RegenStats;
                            }
                            else return 0f;
                        });
                    c.Emit(OpCodes.Add);

                };
                IL.RoR2.CharacterBody.RecalculateStats += VialHook;
            }

            private static void VialAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("VIAL_NAME_TOKEN", "Mysterious Vial");
                R2API.AssetPlus.Languages.AddToken("VIAL_PICKUP_TOKEN", "+1.2 HPregen/sec");
                R2API.AssetPlus.Languages.AddToken("VIAL_DESCRIPTION_TOKEN", "+1.2 <style=cStack>(+1.2 per stack)</style>HPregen.");
                R2API.AssetPlus.Languages.AddToken("VIAL_LORE_TOKEN", "WOW");
                R2API.AssetPlus.Languages.AddToken("VIAL_NAME_TOKEN", "神秘药剂", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("VIAL_PICKUP_TOKEN", "增加回血速度", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("VIAL_DESCRIPTION_TOKEN", "+1.2<style=cStack>（每层增加1.2）</style>回血速度。", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("VIAL_LORE_TOKEN", "哇哦", "zh-CN");

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
