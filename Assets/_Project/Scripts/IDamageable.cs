using UnityEngine;

namespace Tower
{
    public interface IDamageable
    {
        Vector3 Position { get; }
        bool IsAlive { get; }
        void TakeDamage(float amount);
    }
}
