using System.Collections.Generic;

using UnityEngine;

namespace IGI.Enemy
{
    public static class EnemyAwarenessSystem
    {
        private static List<EnemyBrain> registeredEnemies = new();

        public static void Register(EnemyBrain enemy)
        {
            if (!registeredEnemies.Contains(enemy))
                registeredEnemies.Add(enemy);
        }

        public static void Unregister(EnemyBrain enemy)
        {
            if (registeredEnemies.Contains(enemy))
                registeredEnemies.Remove(enemy);
        }

        public static void AlertNearbyEnemies(EnemyBrain source, Vector3 position, float radius)
        {
            foreach (var enemy in registeredEnemies)
            {
                //if (enemy.HasBeenAlerted) continue;
                if (enemy == source) continue; // jangan menginfluence diri sendiri

                float sqrDist = (enemy.transform.position - source.transform.position).sqrMagnitude;
                if (sqrDist <= radius * radius)
                {
                    enemy.OnAlertedByAlly(position);
                }
            }
        }
    }
}