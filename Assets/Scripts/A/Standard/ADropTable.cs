using UnityEngine;

namespace A
{
    public abstract class ADropTable<T> : ScriptableObject where T : ScriptableObject
    {
        public virtual Drop[] Drops { get; }

        [System.Serializable]
        public class Drop
        {
            public T drop;
            public float dropChance;
        }

        public T MakeDrop()
        {
            var prob = 0f;
            var currentProb = Random.Range(1f, 100f);
            for (int i = 0; i < Drops.Length; i++)
            {
                prob += Drops[i].dropChance;
                if (currentProb <= prob)
                    return Drops[i].drop;
            }
            return default;
        }

        public virtual T[] MakeDrops(int amount)
        {
            var drops = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                drops[i] = MakeDrop();
            }
            return drops;
        }
    }
}
