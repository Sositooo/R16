using System;
using System.Collections.Generic;
using System.Linq;
using RoguelikeWPF.Models;

namespace RoguelikeWPF.Services
{
    public class GameManager
    {
        public Player Player { get; private set; }
        public List<Enemy> CurrentEnemies { get; private set; } = new List<Enemy>();
        public string CurrentRoomImage { get; private set; } // путь к картинке
        public List<string> Log { get; private set; } = new List<string>();

        private readonly Random _rnd = new Random();
        public object PendingItem { get; private set; } = null;
        public bool IsPendingItemWeapon { get; private set; }

        private bool _isFight = false;
        private bool _playerFrozen = false;

        public GameManager()
        {
            Player = new Player();
            Log.Add("Добро пожаловать в Roguelike!");
            NewRoom();
        }

        public void NewRoom()
        {
            Player.NextFloor();
            Log.Add($"=== Этап {Player.Floor} ===");

            bool isBoss = Player.Floor % 10 == 0;
            bool isChest = _rnd.Next(2) == 0 && !isBoss;     // 50% шанс

            if (isChest)
            {
                CurrentRoomImage = "pack://application:,,,/Assets/chest.png";
                Log.Add("★★★ Вы нашли сундук! ★★★");
                HandleChest();
                _isFight = false;
            }
            else
            {
                CurrentEnemies = EnemyFactory.Create(isBoss);
                CurrentRoomImage = isBoss
                    ? "pack://application:,,,/Assets/boss.png"
                    : GetEnemyImage(CurrentEnemies[0]);

                Log.Add(isBoss ? "БОСС!" : $"Враги: {string.Join(", ", CurrentEnemies.Select(e => e.Name))}");
                _isFight = true;
            }
        }

        private string GetEnemyImage(Enemy e)
        {
            if (e is Goblin) return "pack://application:,,,/Assets/goblin.png";
            if (e is Skeleton) return "pack://application:,,,/Assets/skeleton.png";
            if (e is Mage) return "pack://application:,,,/Assets/mage.png";
            return "pack://application:,,,/Assets/boss.png";
        }

        private void HandleChest()
        {
            int type = _rnd.Next(3);

            if (type == 0) // зелье
            {
                Player.HealFull();
                Log.Add("Зелье восстановления HP! Вы полностью здоровы.");
                PendingItem = null;
            }
            else if (type == 1) // оружие
            {
                var newWeapon = new Weapon($"Новый меч", _rnd.Next(12, 28));
                PendingItem = newWeapon;
                IsPendingItemWeapon = true;
                Log.Add($"Оружие: {newWeapon.Name} (атака {newWeapon.Attack}). Взять?");
            }
            else // броня
            {
                var newArmor = new Armor($"Новая броня", _rnd.Next(6, 20));
                PendingItem = newArmor;
                IsPendingItemWeapon = false;
                Log.Add($"Броня: {newArmor.Name} (защита {newArmor.Defense}). Взять?");
            }
        }

        // === Бой ===
        public bool IsFight => _isFight && CurrentEnemies.Any(e => !e.IsDead);

        public string PlayerAction(bool attack) // true = атака, false = защита
        {
            if (!_isFight || _playerFrozen)
            {
                _playerFrozen = false;
                return "Вы заморожены!";
            }

            string log = attack ? "Вы атакуете!" : "Вы защищаетесь!";
            int playerDef = attack ? 0 : Player.CurrentArmor.Defense;

            if (!attack && _rnd.Next(100) < 40)
            {
                log += " Уклонение!";
                return log;
            }

            // Атака игрока по всем врагам
            int playerDmg = Player.CurrentWeapon.Attack;
            foreach (var enemy in CurrentEnemies.Where(e => !e.IsDead))
            {
                enemy.TakeDamage(playerDmg);
                log += $" {enemy.Name} получил {playerDmg} урона.";
            }

            // Враги атакуют
            foreach (var enemy in CurrentEnemies.Where(e => !e.IsDead))
            {
                int dmg = enemy.PerformAttack(playerDef, out string special);
                Player.TakeDamage(dmg);
                log += $" {enemy.Name} атакует ({dmg}) {special}";
            }

            // Удаляем мёртвых
            CurrentEnemies.RemoveAll(e => e.IsDead);

            if (!CurrentEnemies.Any())
            {
                _isFight = false;
                log += " Враги повержены!";
            }

            if (Player.IsDead)
                log += " Вы погибли...";

            Log.Add(log);
            return log;
        }


        public void TakePendingItem(bool take)
        {
            if (PendingItem == null) return;

            if (take)
            {
                if (IsPendingItemWeapon && PendingItem is Weapon weapon)
                    Player.EquipWeapon(weapon);
                else if (!IsPendingItemWeapon && PendingItem is Armor armor)
                    Player.EquipArmor(armor);

                Log.Add($"Вы взяли: {(IsPendingItemWeapon ? ((Weapon)PendingItem).Name : ((Armor)PendingItem).Name)}");
            }
            else
            {
                Log.Add("Предмет выброшен.");
            }

            PendingItem = null;
        }


    }
}