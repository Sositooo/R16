using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RoguelikeWPF.Services;

namespace RoguelikeWPF.Pages
{
    public partial class GamePage : Page
    {
        private readonly GameManager _game;


        public GamePage(GameManager game)
        {
            InitializeComponent();
            _game = game;
            UpdateUI();

        }

        private void UpdateInventory()
        {
            spInventory.Children.Clear();

            // Оружие
            var weaponPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
            weaponPanel.Children.Add(new Image { Width = 40, Height = 40, Source = new BitmapImage(new Uri("/Assets/weapon.png", UriKind.Relative)), Margin = new Thickness(0, 0, 10, 0) });
            weaponPanel.Children.Add(new TextBlock
            {
                Text = $"Оружие: {_game.Player.CurrentWeapon.Name} (+{_game.Player.CurrentWeapon.Attack})",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Yellow)
            });
            spInventory.Children.Add(weaponPanel);

            // Броня
            var armorPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
            armorPanel.Children.Add(new Image { Width = 40, Height = 40, Source = new BitmapImage(new Uri("/Assets/armor.png", UriKind.Relative)), Margin = new Thickness(0, 0, 10, 0) });
            armorPanel.Children.Add(new TextBlock
            {
                Text = $"Броня: {_game.Player.CurrentArmor.Name} (+{_game.Player.CurrentArmor.Defense})",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Cyan)
            });
            spInventory.Children.Add(armorPanel);
        }
        private void UpdateUI()
        {
            // === Обновление верхней панели ===
            tbFloor.Text = _game.Player.Floor.ToString();
            pbHealth.Value = (double)_game.Player.HP / _game.Player.MaxHP * 100;
            tbHP.Text = $"{_game.Player.HP}/{_game.Player.MaxHP}";
            tbWeapon.Text = _game.Player.CurrentWeapon.Name;
            tbArmor.Text = _game.Player.CurrentArmor.Name;

            // === Загрузка картинки комнаты ===
            if (!string.IsNullOrEmpty(_game.CurrentRoomImage))
            {
                try
                {
                    imgRoom.Source = new BitmapImage(new Uri(_game.CurrentRoomImage, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    imgRoom.Source = null;
                }
            }

            // === Обновление лога (последние 15 строк) ===
            lbLog.Items.Clear();
            int startIndex = Math.Max(0, _game.Log.Count - 15);
            for (int i = startIndex; i < _game.Log.Count; i++)
            {
                lbLog.Items.Add(_game.Log[i]);
            }

            // === Очистка кнопок и инвентаря ===
            spActions.Children.Clear();
            spInventory.Children.Clear();   // если используешь StackPanel

            // === Логика кнопок ===
            if (_game.IsFight)
            {
                // Бой — кнопки Атаковать / Защищаться
                var btnAttack = new Button { Content = "Атаковать", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                var btnDefend = new Button { Content = "Защищаться", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                btnAttack.Click += (s, e) => PlayerAct(true);
                btnDefend.Click += (s, e) => PlayerAct(false);

                spActions.Children.Add(btnAttack);
                spActions.Children.Add(btnDefend);
            }
            else if (_game.PendingItem != null)
            {
                // Выпал предмет из сундука
                var btnTake = new Button { Content = "Взять", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                var btnDiscard = new Button { Content = "Выбросить", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };

                btnTake.Click += (s, e) => { _game.TakePendingItem(true); UpdateUI(); };
                btnDiscard.Click += (s, e) => { _game.TakePendingItem(false); UpdateUI(); };

                spActions.Children.Add(btnTake);
                spActions.Children.Add(btnDiscard);
            }
            else
            {
                // Обычный переход на следующий этаж
                var nextBtn = new Button { Content = "Далее →", Width = 250, Height = 65, FontSize = 20 };
                nextBtn.Click += (s, e) => { _game.NewRoom(); UpdateUI(); };
                spActions.Children.Add(nextBtn);
            }

            // === Обновление инвентаря ===
            UpdateInventory();
        }




        private void PlayerAct(bool isAttack)
        {
            _game.PlayerAction(isAttack);
            UpdateUI();

            if (_game.Player.IsDead)
                NavigationService.Navigate(new GameOverPage());
        }

    }


    }