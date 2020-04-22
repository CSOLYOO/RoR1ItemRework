using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RoR1ItemRework.item
{
    class ClassicCritThing
    {
        public static void CritInit()
        {
            On.RoR2.CharacterBody.RecalculateStats += OnCritAdd;
        }

        private static void OnCritAdd(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if(self && self.inventory)
            {
                int count1 = self.inventory.GetItemCount(ItemIndex.AttackSpeedOnCrit);
                int count2 = self.inventory.GetItemCount(ItemIndex.HealOnCrit);
                float CritIncrement = 0f;
                if (count1 > 0)
                {
                    CritIncrement += 5f * (count1 - 1);
                }
                if (count2 > 0)
                {
                    CritIncrement += 5f * (count2 - 1);
                }
                Reflection.SetPropertyValue(self, "crit", self.crit + CritIncrement);
            }
        }
    }
}