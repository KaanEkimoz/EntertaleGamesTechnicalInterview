using System;
using UnityEngine;

namespace Tower
{
    [Serializable]
    public struct TowerStats
    {
        public float MaxHP;
        public float Damage;
        public float AttackSpeed;
        public float Regen;

        public static TowerStats Default => new TowerStats
        {
            MaxHP = 100f,
            Damage = 10f,
            AttackSpeed = 1f,
            Regen = 1f,
        };
    }
}
