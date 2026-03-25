using System;
using System.Collections.Generic;

namespace RoguelikeWPF.Models
{
    public abstract class Enemy
    {
        public string Name { get; }
        public int HP { get; private set; }
        public int Attack { get; }
        public int Defense { get; }
        public string SpecialDescription { get; }

        protected Enemy(string name, int hp, int attack, int defense, string special)
        {
            Name = name;
            HP = hp;
            Attack = attack;
            Defense = defense;
            SpecialDescription = special;
        }

        public void TakeDamage(int dmg)
        {
            HP -= Math.Max(0, dmg);
            if (HP < 0) HP = 0;
        }

        public bool IsDead => HP <= 0;

        // Специальные способности реализуются в наследниках
        public abstract int PerformAttack(int playerDefense, out string specialLog);
    }

    // === Конкретные враги ===
    public class Goblin : Enemy
    {
        public Goblin() : base("Гоблин", 30, 12, 3, "20% шанс крита (x2)") { }
        public override int PerformAttack(int playerDefense, out string specialLog)
        {
            specialLog = "";
            int dmg = Attack;
            if (new Random().Next(100) < 20)
            {
                dmg *= 2;
                specialLog = "КРИТИЧЕСКИЙ УДАР!";
            }
            return Math.Max(0, dmg - playerDefense);
        }
    }

    public class Skeleton : Enemy
    {
        public Skeleton() : base("Скелет", 40, 10, 5, "Игнорирует броню") { }
        public override int PerformAttack(int playerDefense, out string specialLog)
        {
            specialLog = "Игнор брони!";
            return Attack; // полностью игнорирует защиту
        }
    }

    public class Mage : Enemy
    {
        public Mage() : base("Маг", 25, 15, 2, "15% шанс заморозки") { }
        public override int PerformAttack(int playerDefense, out string specialLog)
        {
            specialLog = "";
            int dmg = Math.Max(0, Attack - playerDefense);
            if (new Random().Next(100) < 15)
                specialLog = "ЗАМОРОЗКА! Игрок пропускает следующий ход.";
            return dmg;
        }
    }

    // === Боссы ===
    public class Boss : Enemy
    {
        private readonly double _critBonus;
        private readonly bool _ignoreArmor;
        private readonly double _freezeBonus;

        public Boss(string race, double hpX, double atkX, double defX, string extra)
            : base($"Босс-{race}", (int)(GetBaseHp(race) * hpX), (int)(GetBaseAtk(race) * atkX), (int)(GetBaseDef(race) * defX), extra)
        {
            _critBonus = extra.Contains("крита") ? 0.1 : 0;
            _ignoreArmor = race == "Скелет";
            _freezeBonus = extra.Contains("заморозки") ? 0.15 : 0;
        }

        private static int GetBaseHp(string race) => race == "Гоблин" ? 30 : race == "Скелет" ? 40 : 25;
        private static int GetBaseAtk(string race) => race == "Гоблин" ? 12 : race == "Скелет" ? 10 : 15;
        private static int GetBaseDef(string race) => race == "Гоблин" ? 3 : race == "Скелет" ? 5 : 2;

        public override int PerformAttack(int playerDefense, out string specialLog)
        {
            specialLog = "";
            int dmg = Attack;
            var rnd = new Random();

            if (_ignoreArmor)
            {
                specialLog = "ИГНОР БРОНИ!";
            }
            else
            {
                dmg = Math.Max(0, dmg - playerDefense);
            }

            if (rnd.NextDouble() < 0.2 + _critBonus)
            {
                dmg *= 2;
                specialLog += " КРИТ!";
            }
            if (rnd.NextDouble() < 0.15 + _freezeBonus)
            {
                specialLog += " ЗАМОРОЗКА!";
            }

            return dmg;
        }
    }

    // Фабрика (Single Responsibility)
    public static class EnemyFactory
    {
        private static readonly Random _rnd = new Random();
        public static List<Enemy> Create(bool isBoss)
        {
            var list = new List<Enemy>();
            int count = isBoss ? 1 : _rnd.Next(1, 4);

            if (isBoss)
            {
                int type = _rnd.Next(4);
                string race = type == 0 ? "Гоблин" : type == 1 ? "Скелет" : type == 2 ? "Маг" : "Скелет";
                list.Add(new Boss(race, type == 0 ? 2.0 : type == 1 ? 2.5 : type == 2 ? 1.8 : 1.3,
                                  type == 0 ? 1.5 : type == 1 ? 1.3 : type == 2 ? 1.6 : 1.8,
                                  type == 0 ? 1.2 : type == 1 ? 1.4 : type == 2 ? 1.1 : 0.6,
                                  type == 0 ? "+10% крита" : type == 2 ? "+10% заморозки" : type == 3 ? "+15% заморозки" : "—"));
            }
            else
            {
                int type = _rnd.Next(3);
                for (int i = 0; i < count; i++)
                {
                    if (type == 0) list.Add(new Goblin());
                    else if (type == 1) list.Add(new Skeleton());
                    else list.Add(new Mage());
                }
            }
            return list;
        }
    }
}