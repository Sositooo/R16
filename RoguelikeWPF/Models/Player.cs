using System;

namespace RoguelikeWPF.Models
{
    public class Player
    {
        public int MaxHP { get; } = 100;
        public int HP { get; private set; }
        public Weapon CurrentWeapon { get; private set; }
        public Armor CurrentArmor { get; private set; }
        public int Floor { get; private set; } = 1;

        public Player()
        {
            HP = MaxHP;
            CurrentWeapon = new Weapon("Начальный меч", 10);
            CurrentArmor = new Armor("Начальная броня", 5);
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0) damage = 0;
            HP -= damage;
            if (HP < 0) HP = 0;
        }

        public void HealFull() => HP = MaxHP;

        public void EquipWeapon(Weapon weapon) => CurrentWeapon = weapon;
        public void EquipArmor(Armor armor) => CurrentArmor = armor;
        public void NextFloor() => Floor++;

        public bool IsDead => HP <= 0;
    }
}