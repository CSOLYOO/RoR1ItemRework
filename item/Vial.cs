using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace RoR1ItemRework
{
    public class Vial
    {
        public class VialItem
        {
            public static ItemIndex VialItemIndex;
            private const string ModPrefix = "@RoR1ItemRework:";
            private const string PrefabPath = ModPrefix + "Assets/Vial.prefab";
            private const string IconPath = ModPrefix + "Assets/Vial_Icon.png";


            public static void VialItemInit()
            {
                VialAsItem();
                VialItemHook();
            }

            
            public static void VialItemHook()
            {
                On.RoR2.CharacterBody.RecalculateStats += OnRegenAdd;

                if (!RoR1ItemRework.cfgEnableVial.Value)
                {
                    On.RoR2.CharacterBody.RecalculateStats -= OnRegenAdd;
                    IL.RoR2.CharacterBody.RecalculateStats += VialHook;
                }

            }

            private static void VialAsItem()
            {
                R2API.AssetPlus.Languages.AddToken("VIAL_NAME_TOKEN", "Mysterious Vial");
                R2API.AssetPlus.Languages.AddToken("VIAL_PICKUP_TOKEN", "Increased health regeneration.");
                R2API.AssetPlus.Languages.AddToken("VIAL_DESCRIPTION_TOKEN", "Gain <style=clsHealing>1.2</style> <style=cStack>(+1.2 per stack)</style>HP regen/s.");
                R2API.AssetPlus.Languages.AddToken("VIAL_LORE_TOKEN", "Apply to skin for a rapidly acting gel that contains both antiseptics and an agent to encourage protein synthesis!");
                R2API.AssetPlus.Languages.AddToken("VIAL_NAME_TOKEN", "神秘药剂", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("VIAL_PICKUP_TOKEN", "增加生命值再生速度", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("VIAL_DESCRIPTION_TOKEN", "使<style=cIsHealing>基础生命值再生速度</style>提高<style=cIsHealing>1.2hp/s</style><style=cStack>（每层增加1.2hp/s）</style>。", "zh-CN");
                R2API.AssetPlus.Languages.AddToken("VIAL_LORE_TOKEN", "涂在皮肤上，可快速作用，同时含有防腐剂和促进蛋白质合成的物质！", "zh-CN");

                ItemDef VialDef = new ItemDef
                {
                    name = "VIAL_NAME_TOKEN",
                    pickupIconPath = IconPath,
                    pickupModelPath = PrefabPath,
                    nameToken = "VIAL_NAME_TOKEN",
                    pickupToken = "VIAL_PICKUP_TOKEN",
                    descriptionToken = "VIAL_DESCRIPTION_TOKEN",
                    loreToken = "VIAL_LORE_TOKEN",
                    tier = ItemTier.Tier1,
                    tags = new ItemTag[]
                    {
                        ItemTag.Healing
                    }
                };
                ItemDisplayRule[] DisplayRules = null;
                CustomItem VialItem = new CustomItem(VialDef, DisplayRules);
                VialItemIndex = ItemAPI.Add(VialItem);

            }
            private static void OnRegenAdd(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
            {
                orig(self);
                if (self && self.inventory)
                {
                    int count1 = self.inventory.GetItemCount(VialItemIndex);
                    float RegenIncrement = 0f;
                    if (count1 > 0)
                    {
                        RegenIncrement += 1.2f * count1;
                    }
                    Reflection.SetPropertyValue(self, "regen", self.regen + RegenIncrement);
                }
            }
            private static void VialHook(ILContext li)
            {
                var c = new ILCursor(li);
                c.IL.Body.Variables.Add(new VariableDefinition(c.IL.Body.Method.Module.TypeSystem.Single));
                int locCount = c.IL.Body.Variables.Count - 1;
                bool ILfound = c.TryGotoNext(MoveType.After,
                    x => x.OpCode == OpCodes.Ldloc_S,
                    x => x.MatchMul(),
                    x => x.OpCode == OpCodes.Stloc_S,
                    x => x.MatchLdcR4(1f),
                    x => x.OpCode == OpCodes.Stloc_S
                    );
                if (ILfound)
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<RoR2.CharacterBody, float>>((self) =>
                    {
                        if (self.inventory)
                        {
                            float RegenStats = self.inventory.GetItemCount(VialItemIndex) * 1.2f;
                            return RegenStats;
                        }
                        else return 0f;
                    });
                    c.Emit(OpCodes.Stloc, locCount);
                }
                else
                {
                    Debug.LogError("RoR1ItemRework fail IL of Vial(Load Inventory)");
                    return;
                }
                ILfound = c.TryGotoNext(
                    x => x.OpCode == OpCodes.Ldloc_S,
                    x => x.MatchAdd(),
                    x => x.OpCode == OpCodes.Ldloc_S,
                    x => x.MatchAdd(),
                    x => x.OpCode == OpCodes.Ldloc_S,
                    x => x.MatchAdd(),
                    x => x.OpCode == OpCodes.Ldloc_S,
                    x => x.MatchAdd(),
                    x => x.OpCode == OpCodes.Ldloc_S,
                    x => x.MatchMul(),
                    x => x.OpCode == OpCodes.Stloc_S
                );
                if (ILfound)
                {
                    c.Index += 8;
                    c.Emit(OpCodes.Ldloc, locCount);
                    c.Emit(OpCodes.Add);
                }
                else
                {
                    Debug.LogError("RoR1ItemRework fail IL of Vial(Add stats)");
                    return;
                }

            }
        }

    }

}
