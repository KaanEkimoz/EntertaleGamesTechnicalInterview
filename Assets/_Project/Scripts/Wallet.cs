using System;
using UnityEngine;

namespace Tower
{
    public static class Wallet
    {
        private const string KeyGold = "Tower.Gold";

        private static int? _cached;
        public static event Action<int> OnGoldChanged;

        public static int Gold
        {
            get
            {
                if (!_cached.HasValue) _cached = PlayerPrefs.GetInt(KeyGold, 0);
                return _cached.Value;
            }
        }

        public static void Add(int amount)
        {
            if (amount <= 0) return;
            int n = Gold + amount;
            Set(n);
        }

        public static bool TrySpend(int amount)
        {
            if (amount < 0) return false;
            if (Gold < amount) return false;
            Set(Gold - amount);
            return true;
        }

        private static void Set(int value)
        {
            _cached = value;
            PlayerPrefs.SetInt(KeyGold, value);
            PlayerPrefs.Save();
            OnGoldChanged?.Invoke(value);
        }
    }
}
