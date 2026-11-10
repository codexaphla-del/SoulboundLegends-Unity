using System;
using System.Collections.Generic;

namespace SL.Systems
{
    /// <summary>Item grades from the design (section 8). Divine is never a normal beast drop.</summary>
    public enum ItemGrade { Common = 1, Uncommon = 2, Rare = 3, Epic = 4, Legendary = 5, Mythic = 6, Divine = 7 }

    /// <summary>Ten gods plus the hidden Shadow God (design: ten gods, Omnigod, hidden Shadow God).</summary>
    public enum GodId { Water = 0, Fire = 1, Earth = 2, Void = 3, Thunder = 4, Nature = 5, Light = 6, Air = 7, Ice = 8, Omni = 9, HiddenShadow = 10 }

    public struct SpeciesLoot
    {
        public string ItemId; public float Chance; public int Min, Max; public ItemGrade Grade;
        public SpeciesLoot(string id, float chance, int min, int max, ItemGrade grade) { ItemId = id; Chance = chance; Min = min; Max = max; Grade = grade; }
    }

    /// <summary>One creature record (design section 7). Server-owned data; the client only needs id, mesh and look.</summary>
    public sealed class SpeciesRecord
    {
        public string Id, Name, Family, Habitat, BaseMesh, Plan, Pattern;
        public int Level, LevelMin, LevelMax;
        public float SizeMeters, Speed, Aggro, Range, AttackCooldown;
        public float HpMul = 1f, DmgMul = 1f;
        public int Weak, Resist;                // GodId values
        public int ColorA, ColorB, ColorC;      // 0xRRGGBB
        public bool Ranged, Flying, Boss;
        public ItemGrade Grade = ItemGrade.Common;
        public string[] Actions;                // ids from CombatActionLibrary
        public List<SpeciesLoot> Loot = new List<SpeciesLoot>();

        public int Hp { get { return (int)((36 + Level * 30 + Level * Level * 4) * HpMul); } }
        public int Dmg { get { return (int)((4 + Level * 2.4f) * DmgMul); } }
        public int Xp { get { return 8 + Level * 9; } }
        public int GoldMin { get { return Level; } }
        public int GoldMax { get { return Level * 3 + 2; } }
    }

    /// <summary>The twenty beginner species: two per level, Levels 1 to 10, each built from one of the owner's base meshes.</summary>
    public static class BeginnerSpecies
    {
        public static readonly List<SpeciesRecord> All = new List<SpeciesRecord>();
        static readonly Dictionary<string, SpeciesRecord> map = new Dictionary<string, SpeciesRecord>();

        static SpeciesRecord S(string id, string name, int level, string family, string mesh, string plan, float size, float speed,
                               int weak, int resist, int a, int b, int c, string habitat, string pattern, string[] actions,
                               float hpMul, float dmgMul, bool flying, bool ranged, ItemGrade grade)
        {
            SpeciesRecord s = new SpeciesRecord();
            s.Id = id; s.Name = name; s.Level = level; s.LevelMin = Math.Max(1, level - 1); s.LevelMax = level + 1;
            s.Family = family; s.BaseMesh = mesh; s.Plan = plan; s.SizeMeters = size; s.Speed = speed;
            s.Weak = weak; s.Resist = resist; s.ColorA = a; s.ColorB = b; s.ColorC = c; s.Habitat = habitat; s.Pattern = pattern;
            s.Actions = actions; s.HpMul = hpMul; s.DmgMul = dmgMul; s.Flying = flying; s.Ranged = ranged; s.Grade = grade;
            s.Aggro = 9f + level * 0.3f + (flying ? 3f : 0f);
            s.Range = ranged ? 11f : 2.0f;
            s.AttackCooldown = 1.6f;
            All.Add(s); map[id] = s;
            return s;
        }

        static void L(SpeciesRecord s, string item, float chance, int min, int max, ItemGrade g)
        {
            s.Loot.Add(new SpeciesLoot(item, chance, min, max, g));
        }

        static BeginnerSpecies()
        {
            const int Water = 0, Fire = 1, Earth = 2, Thunder = 4, Nature = 5, Light = 6, Air = 7, Ice = 8, Void = 3;
            string[] bite = { "bite" }, claw = { "claw" }, bc = { "bite", "claw" }, pounce = { "bite", "pounce" }, charge = { "charge", "bite" },
                     sting = { "bite", "tail_whip" }, slam = { "slam", "claw" }, blob = { "pounce" };
            SpeciesRecord s;

            s = S("b01_slime", "Mossy Slime", 1, "Slime", "monster_slime", "blob", 1.1f, 2.6f, Fire, Water, 0x5fb85a, 0x9be08a, 0x2f6b34,
                  "Meadow edges beside the village", "Hops at the player and bounces away when hurt.", blob, 0.9f, 0.8f, false, false, ItemGrade.Common);
            L(s, "herb_red", 0.5f, 1, 2, ItemGrade.Common); L(s, "herb_green", 0.2f, 1, 1, ItemGrade.Common);
            s = S("b01_mole", "Field Mole", 1, "Beast", "monster_mole", "quad", 0.9f, 3.2f, Water, Earth, 0x6b4a2e, 0xc79a6a, 0x3a2a1c,
                  "Farm fields and dirt paths", "Digs at the player's feet, then retreats.", claw, 0.9f, 0.8f, false, false, ItemGrade.Common);
            L(s, "hide", 0.45f, 1, 2, ItemGrade.Common); L(s, "herb_green", 0.2f, 1, 1, ItemGrade.Common);

            s = S("b02_mudslime", "Mud Slime", 2, "Slime", "monster_slime", "blob", 1.2f, 2.6f, Water, Earth, 0x7a5a3a, 0xb08a5a, 0x3a2a1a,
                  "Riverbanks", "Slow, sticky hops. Splits damage across nearby slimes.", blob, 1.0f, 0.9f, false, false, ItemGrade.Common);
            L(s, "herb_red", 0.4f, 1, 2, ItemGrade.Common); L(s, "wood", 0.2f, 1, 1, ItemGrade.Common);
            s = S("b02_boar", "Forest Boar", 2, "Beast", "monster_boar", "quad", 1.6f, 5.0f, Fire, Nature, 0x4a3a2e, 0x8a6a4a, 0x201810,
                  "The forest road", "Charges in a straight line, then needs a moment to turn.", charge, 1.1f, 1.0f, false, false, ItemGrade.Common);
            L(s, "hide", 0.5f, 1, 2, ItemGrade.Common); L(s, "bone", 0.25f, 1, 1, ItemGrade.Common);

            s = S("b03_greywolf", "Grey Wolf", 3, "Beast", "monster_wolf", "quad", 1.8f, 5.8f, Thunder, Air, 0x8a8f99, 0xd9dde6, 0x3b3f48,
                  "Forest edge", "Circles the player, then bites. Calls nearby wolves.", pounce, 1.0f, 1.0f, false, false, ItemGrade.Common);
            L(s, "hide", 0.5f, 1, 2, ItemGrade.Common); L(s, "bone", 0.3f, 1, 1, ItemGrade.Common);
            s = S("b03_cavebat", "Cave Bat", 3, "Flying", "monster_bat", "flyer", 1.3f, 5.0f, Light, Air, 0x3a3040, 0x6a5a70, 0x1a1420,
                  "Cave mouths and dusk skies", "Swoops in, bites, and flies back out of reach.", bite, 0.8f, 0.9f, true, false, ItemGrade.Common);
            L(s, "bone", 0.3f, 1, 1, ItemGrade.Common); L(s, "herb_blue", 0.2f, 1, 1, ItemGrade.Common);

            s = S("b04_woodbeetle", "Wood Beetle", 4, "Insect", "monster_beetle", "quad", 1.3f, 3.6f, Fire, Earth, 0x4a5a2a, 0x8aa04a, 0x202810,
                  "Old tree groves", "Armoured shell takes less damage from the front; hit it from behind.", charge, 1.3f, 0.9f, false, false, ItemGrade.Common);
            L(s, "wood", 0.45f, 1, 2, ItemGrade.Common); L(s, "iron_ore", 0.1f, 1, 1, ItemGrade.Uncommon);
            s = S("b04_wasp", "Thicket Wasp", 4, "Insect", "monster_wasp", "flyer", 1.1f, 5.4f, Water, Air, 0xc9a020, 0x2a2a2a, 0x806010,
                  "Berry thickets", "Darts in and out. Weak to splash attacks.", sting, 0.7f, 1.1f, true, false, ItemGrade.Common);
            L(s, "herb_blue", 0.35f, 1, 1, ItemGrade.Common); L(s, "crystal_shard", 0.05f, 1, 1, ItemGrade.Uncommon);

            s = S("b05_scarboar", "Scarred Boar", 5, "Beast", "monster_boar", "quad", 1.9f, 5.0f, Ice, Fire, 0x3a2a24, 0x7a5a40, 0x601818,
                  "Deep forest", "Tougher charge that leaves it stunned if it hits a tree.", charge, 1.5f, 1.2f, false, false, ItemGrade.Uncommon);
            L(s, "hide", 0.55f, 1, 3, ItemGrade.Common); L(s, "bone", 0.35f, 1, 2, ItemGrade.Uncommon);
            s = S("b05_burrowmole", "Burrow Mole", 5, "Beast", "monster_mole", "quad", 1.3f, 3.2f, Water, Earth, 0x4a3a2a, 0xa08060, 0x2a1a10,
                  "Hill burrows", "Pops out of the ground to ambush, then burrows away.", claw, 1.3f, 1.1f, false, false, ItemGrade.Uncommon);
            L(s, "hide", 0.5f, 1, 2, ItemGrade.Common); L(s, "iron_ore", 0.15f, 1, 1, ItemGrade.Uncommon);

            s = S("b06_packwolf", "Pack Wolf", 6, "Beast", "monster_wolf", "quad", 2.0f, 5.8f, Fire, Ice, 0x6a6f78, 0xb0b5be, 0x282a30,
                  "Ridge trails", "Hunts in groups and flanks the player.", pounce, 1.3f, 1.2f, false, false, ItemGrade.Uncommon);
            L(s, "hide", 0.55f, 1, 3, ItemGrade.Common); L(s, "bone", 0.35f, 1, 2, ItemGrade.Uncommon);
            s = S("b06_nightbat", "Night Bat", 6, "Flying", "monster_bat", "flyer", 1.6f, 5.0f, Light, Void, 0x28202e, 0x50405a, 0x120c16,
                  "Cave interiors", "Screeches to disorient, then dives in groups.", bite, 1.0f, 1.2f, true, false, ItemGrade.Uncommon);
            L(s, "bone", 0.35f, 1, 2, ItemGrade.Uncommon); L(s, "herb_blue", 0.25f, 1, 2, ItemGrade.Common);

            s = S("b07_cavespider", "Cave Spider", 7, "Insect", "monster_spider", "quad", 1.7f, 4.6f, Fire, Earth, 0x40382e, 0x7a6a58, 0x181410,
                  "Dark caves", "Lunges from the ceiling and webs the player's feet.", bc, 1.4f, 1.3f, false, false, ItemGrade.Uncommon);
            L(s, "bone", 0.4f, 1, 2, ItemGrade.Uncommon); L(s, "herb_blue", 0.25f, 1, 1, ItemGrade.Common);
            s = S("b07_frostslime", "Frost Slime", 7, "Slime", "monster_slime", "blob", 1.4f, 2.6f, Fire, Ice, 0x5ab0e0, 0xbfe8ff, 0x2a6a9a,
                  "Cold stream banks", "Leaves slowing puddles of ice as it hops.", blob, 1.5f, 1.2f, false, false, ItemGrade.Uncommon);
            L(s, "crystal_shard", 0.12f, 1, 1, ItemGrade.Uncommon); L(s, "herb_blue", 0.4f, 1, 2, ItemGrade.Common);

            s = S("b08_hornbeetle", "Horned Beetle", 8, "Insect", "monster_beetle", "quad", 1.6f, 3.6f, Thunder, Earth, 0x3a3a2a, 0x7a7a4a, 0x181810,
                  "Quarry slopes", "Horn charge that breaks light cover.", charge, 1.8f, 1.4f, false, false, ItemGrade.Rare);
            L(s, "iron_ore", 0.25f, 1, 2, ItemGrade.Uncommon); L(s, "wood", 0.3f, 1, 2, ItemGrade.Common);
            s = S("b08_hivewasp", "Hive Wasp", 8, "Insect", "monster_wasp", "flyer", 1.3f, 5.4f, Water, Air, 0xd0a010, 0x1a1a1a, 0x907010,
                  "Wild hives", "Swarms: one stung player draws the whole hive.", sting, 1.1f, 1.4f, true, false, ItemGrade.Uncommon);
            L(s, "herb_blue", 0.4f, 1, 2, ItemGrade.Common); L(s, "crystal_shard", 0.1f, 1, 1, ItemGrade.Uncommon);

            s = S("b09_direwolf", "Dire Wolf", 9, "Beast", "monster_wolf", "quad", 2.4f, 5.8f, Thunder, Ice, 0x4a4a52, 0x8a8a96, 0xa02020,
                  "Mountain pass", "Pack leader: howls to buff nearby wolves.", pounce, 2.2f, 1.6f, false, false, ItemGrade.Rare);
            L(s, "hide", 0.6f, 2, 3, ItemGrade.Uncommon); L(s, "bone", 0.4f, 1, 3, ItemGrade.Uncommon); L(s, "crystal_shard", 0.1f, 1, 1, ItemGrade.Rare);
            s = S("b09_venomspider", "Venom Spider", 9, "Insect", "monster_spider", "quad", 1.9f, 4.6f, Fire, Nature, 0x2a2a3a, 0x5a4a7a, 0x7a30a0,
                  "Webbed ruins", "Spits venom from range, then closes in.", bc, 1.6f, 1.6f, false, true, ItemGrade.Rare);
            L(s, "bone", 0.4f, 1, 2, ItemGrade.Uncommon); L(s, "herb_blue", 0.4f, 1, 2, ItemGrade.Uncommon);

            s = S("b10_scorpion", "Dust Scorpion", 10, "Insect", "monster_scorpion", "quad", 1.8f, 4.2f, Water, Earth, 0x9a7a4a, 0xd8b878, 0x3a2a14,
                  "Dry canyon", "Claw pinch and a tail strike; burrows to heal.", sting, 2.0f, 1.7f, false, false, ItemGrade.Rare);
            L(s, "bone", 0.45f, 1, 3, ItemGrade.Uncommon); L(s, "iron_ore", 0.3f, 1, 2, ItemGrade.Uncommon);
            s = S("b10_troll", "Stone Troll", 10, "Beast", "monster_troll", "heavy", 3.2f, 3.0f, Thunder, Earth, 0x6a7a5a, 0x9aaa8a, 0x3a4a30,
                  "Mountain caves", "Slow, heavy slams. Staggers after a missed slam.", slam, 3.0f, 2.0f, false, false, ItemGrade.Rare);
            L(s, "iron_ore", 0.4f, 1, 3, ItemGrade.Uncommon); L(s, "stone_heart", 0.1f, 1, 1, ItemGrade.Rare);
        }

        public static SpeciesRecord Get(string id)
        {
            SpeciesRecord s;
            return map.TryGetValue(id, out s) ? s : null;
        }
    }

    public struct LootRoll { public string ItemId; public int Count; public ItemGrade Grade; }

    /// <summary>Pure loot roll. The SERVER supplies the seed; clients never roll loot.</summary>
    public static class LootRoller
    {
        public static List<LootRoll> Roll(SpeciesRecord species, int seed, float luck)
        {
            List<LootRoll> result = new List<LootRoll>();
            if (species == null) return result;
            Random rng = new Random(seed);
            for (int i = 0; i < species.Loot.Count; i++)
            {
                SpeciesLoot l = species.Loot[i];
                if (l.Grade == ItemGrade.Divine) continue; // divine items are never random beast drops
                if (rng.NextDouble() >= l.Chance * (1f + luck)) continue;
                LootRoll r = new LootRoll();
                r.ItemId = l.ItemId; r.Grade = l.Grade;
                r.Count = l.Min + (l.Max > l.Min ? rng.Next(0, l.Max - l.Min + 1) : 0);
                result.Add(r);
            }
            return result;
        }
    }
}
