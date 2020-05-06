using R2API;
using RoR2;

namespace RoR1ItemRework
{
    class Leech
    {
        private static EquipmentIndex MassiveLeechIndex;
        private static BuffIndex MassiveLeechbuff;
        private const string ModPrefix = "@RoR1ItemRework:";
        private const string PrefabPath = ModPrefix + "Assets/Massive_Leech.prefab";
        private const string IconPath = ModPrefix + "Assets/Massive_Leech_Icon.png";

        public static void LeechInit()
        {
            LeechAsBuff();
            LeechAsEquip();
            LeechHook();
        }

        private static void LeechAsEquip()
        {
            LanguageAPI.Add("MASSIVELEECH_NAME_TOKEN", "Massive Leech");
            LanguageAPI.Add("MASSIVELEECH_PICKUP_TOKEN", "Grant massive life on hit for 10 seconds.Heal will increase with your level up.");
            LanguageAPI.Add("MASSIVELEECH_DESCRIPTION_TOKEN", "For 10 seconds, every hit <style=cIsHealing>heals</style> you for <style=cIsHealing>10 health</style>. Each level will gain <style=cIsHealing>1 extra health</style>.");
            LanguageAPI.Add("MASSIVELEECH_LORE_TOKEN", @"- Shipping Method:  Volatile
- Order Details:  Giant leeches found in the pools of HYPERION-5. Very similar to its counterpart on Earth, but it seems to have developed teeth as well, allowing it to eat meat as well as siphon blood. A few have been spotted to ballon to enormous proportions,up to the size of a small dog. Like the common leech, this has obvious medical implications. You will just have to be extra careful, or you may come back to no patient and a giant bloody leech.
");
            LanguageAPI.Add("MASSIVELEECH_NAME_TOKEN", "庞大水蛭", "zh-CN");
            LanguageAPI.Add("MASSIVELEECH_PICKUP_TOKEN", "在10秒内，攻击汲取巨额生命。吸血值随等级上涨。", "zh-CN");
            LanguageAPI.Add("MASSIVELEECH_DESCRIPTION_TOKEN", "10秒内，每次攻击<style=cIsHealing>回复10点生命值</style>。人物每提升一级，就额外获得<style=cIsHealing>1点生命值</style>。", "zh-CN");
            LanguageAPI.Add("MASSIVELEECH_LORE_TOKEN", @"- 邮寄方式：易挥发
- 订单详细信息：在HYPERION-5的水池中发现了巨型水蛭。 它与地球上的对应物非常相似，但它似乎也有牙齿，可以吃肉和虹吸血。已经发现他们可以按比例放大，甚至可以达到狗的大小。 像普通的水蛭一样，这也具有明显的医学含义。 您只需要格外小心。
", "zh-CN");

            EquipmentDef LeechEquipmentDef = new EquipmentDef
            {
                name = "MASSIVELEECH_NAME_TOKEN",
                cooldown = 45f,
                pickupModelPath = PrefabPath,
                pickupIconPath = IconPath,
                nameToken = "MASSIVELEECH_NAME_TOKEN",
                pickupToken = "MASSIVELEECH_PICKUP_TOKEN",
                descriptionToken = "MASSIVELEECH_DESCRIPTION_TOKEN",
                loreToken = "MASSIVELEECH_LORE_TOKEN",
                canDrop = true,
                enigmaCompatible = true
            };

            ItemDisplayRule[] LeechDisplayRules = null;
            CustomEquipment LeechEquipment = new CustomEquipment(LeechEquipmentDef, LeechDisplayRules);
            MassiveLeechIndex = ItemAPI.Add(LeechEquipment);

        }

        private static void LeechAsBuff()
        {
            BuffDef LeechBuffDef = new BuffDef
            {
                iconPath = IconPath,
                canStack = false,
                eliteIndex = EliteIndex.None,
                isDebuff = false,
                name = "LeechBuff"
            };
            MassiveLeechbuff = BuffAPI.Add(new CustomBuff(LeechBuffDef));
        }
        private static void LeechHook()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += (orig, self, equipmentIndex) =>
            {
                if (equipmentIndex == MassiveLeechIndex)
                {
                    if (self.characterBody)
                    {
                        self.characterBody.AddTimedBuff(MassiveLeechbuff, 10f);
                        return true;
                    }
                }
                return orig(self, equipmentIndex);
            };

            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damage, victim) =>
            {
                if (damage.attacker)
                {
                    CharacterBody Attacker = damage.attacker.GetComponent<CharacterBody>();
                    CharacterMaster AttackerMaster = Attacker.master;
                    if (Attacker && AttackerMaster)
                    {
                        if (Attacker.HasBuff(MassiveLeechbuff))
                        {
                            ProcChainMask procChainMask = damage.procChainMask;
                            HealthComponent component = Attacker.GetComponent<HealthComponent>();
                            procChainMask.AddProc(ProcType.HealOnHit);
                            float heal = Attacker.level + 10f;
                            component.Heal(heal * damage.procCoefficient, procChainMask, true);
                        }
                    }
                }
                orig(self, damage, victim);
            };
        }
    }
}
