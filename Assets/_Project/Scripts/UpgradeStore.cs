using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tower
{
    public static class UpgradeStore
    {
        public enum Stat { HP, Damage, AttackSpeed, Regen }

        private const float BaseCost = 10f;
        private const float CostMultiplier = 1.1f;

        private static readonly Dictionary<Stat, int> _levels = new Dictionary<Stat, int>();
        private static bool _loaded;

        public static event Action<Stat, int> OnLevelChanged;

        private static string KeyOf(Stat s) => "Tower.Lv." + s;

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            foreach (Stat s in Enum.GetValues(typeof(Stat)))
                _levels[s] = PlayerPrefs.GetInt(KeyOf(s), 0);
            _loaded = true;
        }

        public static int GetLevel(Stat s)
        {
            EnsureLoaded();
            return _levels.TryGetValue(s, out var v) ? v : 0;
        }

        public static int GetCost(Stat s)
        {
            EnsureLoaded();
            return Mathf.RoundToInt(BaseCost * Mathf.Pow(CostMultiplier, GetLevel(s)));
        }

        public static bool TryPurchase(Stat s)
        {
            EnsureLoaded();
            int cost = GetCost(s);
            if (!Wallet.TrySpend(cost)) return false;
            int newLv = GetLevel(s) + 1;
            _levels[s] = newLv;
            PlayerPrefs.SetInt(KeyOf(s), newLv);
            PlayerPrefs.Save();
            OnLevelChanged?.Invoke(s, newLv);
            return true;
        }

        public static TowerStats ApplyTo(TowerStats baseStats)
        {
            EnsureLoaded();
            baseStats.MaxHP        += 20f  * GetLevel(Stat.HP);
            baseStats.Damage       +=  5f  * GetLevel(Stat.Damage);
            baseStats.AttackSpeed  +=  0.2f * GetLevel(Stat.AttackSpeed);
            baseStats.Regen        +=  0.5f * GetLevel(Stat.Regen);
            return baseStats;
        }

        public static string IncrementLabel(Stat s)
        {
            switch (s)
            {
                case Stat.HP:           return "+20 HP";
                case Stat.Damage:       return "+5 Dmg";
                case Stat.AttackSpeed:  return "+0.2 atk/s";
                case Stat.Regen:        return "+0.5 hp/s";
                default: return "";
            }
        }

        public static string ShortName(Stat s)
        {
            switch (s)
            {
                case Stat.HP: return "HP";
                case Stat.Damage: return "Dmg";
                case Stat.AttackSpeed: return "AtkSpd";
                case Stat.Regen: return "Regen";
                default: return "";
            }
        }
    }
}
