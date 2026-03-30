using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeWPF.Models
{
    public static class MonsterFactory
    {
        private static readonly Random _rnd = new Random();

        public static List<Enemy> CreateEnemies(bool isBoss)
        {
            var enemies = new List<Enemy>();

            if (isBoss)
            {
                enemies.Add(new Boss("Гоблин", 2.0, 1.5, 1.2, "+10% крита")); // можно рандомизировать позже
            }
            else
            {
                int count = _rnd.Next(1, 4);
                for (int i = 0; i < count; i++)
                {
                    int type = _rnd.Next(3);
                    if (type == 0) enemies.Add(new Goblin());
                    else if (type == 1) enemies.Add(new Skeleton());
                    else enemies.Add(new Mage());
                }
            }
            return enemies;
        }

        private static Boss CreateBoss()
        {
            // можно расширить позже
            return new Boss("Гоблин", 2.0, 1.5, 1.2, "+10% крита");
        }
    }
}