using System;

namespace Tower
{
    public sealed class Health
    {
        public event Action<float> OnHealthChanged;
        public event Action OnDied;

        public float Max { get; private set; }
        public float Current { get; private set; }
        public float Normalized => Max > 0f ? Current / Max : 0f;
        public bool IsDead => Current <= 0f;

        public Health(float max)
        {
            if (max <= 0f) throw new ArgumentOutOfRangeException(nameof(max));
            Max = max;
            Current = max;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || IsDead) return;
            Current = Math.Max(0f, Current - amount);
            OnHealthChanged?.Invoke(Normalized);
            if (IsDead) OnDied?.Invoke();
        }

        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead) return;
            Current = Math.Min(Max, Current + amount);
            OnHealthChanged?.Invoke(Normalized);
        }

        public void Tick(float regenPerSecond, float dt)
        {
            if (IsDead || regenPerSecond <= 0f || Current >= Max) return;
            Heal(regenPerSecond * dt);
        }

        public void SetMax(float newMax, bool addDelta)
        {
            if (newMax <= 0f) return;
            float delta = newMax - Max;
            Max = newMax;
            if (addDelta && delta > 0f) Current = Math.Min(Max, Current + delta);
            else Current = Math.Min(Current, Max);
            OnHealthChanged?.Invoke(Normalized);
        }

        public void HealToFull()
        {
            Current = Max;
            OnHealthChanged?.Invoke(Normalized);
        }
    }
}
