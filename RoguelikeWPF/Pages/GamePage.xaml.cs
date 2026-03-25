using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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


        private void UpdateUI()
        {
            // Обновление верхней панели
            tbFloor.Text = _game.Player.Floor.ToString();
            pbHealth.Value = (double)_game.Player.HP / _game.Player.MaxHP * 100;
            tbHP.Text = $"{_game.Player.HP}/{_game.Player.MaxHP}";
            tbWeapon.Text = _game.Player.CurrentWeapon.Name;
            tbArmor.Text = _game.Player.CurrentArmor.Name;


            // Загрузка картинки комнаты
            if (!string.IsNullOrEmpty(_game.CurrentRoomImage))
            {
                try
                {
                    var uri = new Uri(_game.CurrentRoomImage, UriKind.RelativeOrAbsolute);
                    imgRoom.Source = new BitmapImage(uri);
                }
                catch (Exception ex)
                {
                    imgRoom.Source = null;
                
                }
            }

            // Лог — показываем только последние строки
            lbLog.Items.Clear();
            int start = Math.Max(0, _game.Log.Count - 15);
            for (int i = start; i < _game.Log.Count; i++)
            {
                lbLog.Items.Add(_game.Log[i]);
            }

            spActions.Children.Clear();
            spInventory.Children.Clear();

            if (_game.IsFight)
            {
                // бой — кнопки атака/защита
                var btnAttack = new Button { Content = "Атаковать", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                var btnDefend = new Button { Content = "Защищаться", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                btnAttack.Click += (s, e) => PlayerAct(true);
                btnDefend.Click += (s, e) => PlayerAct(false);
                spActions.Children.Add(btnAttack);
                spActions.Children.Add(btnDefend);
            }
            else if (_game.PendingItem != null)
            {
                // есть предмет на выбор
                var btnTake = new Button { Content = "Взять", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                var btnDiscard = new Button { Content = "Выбросить", Width = 180, Height = 60, FontSize = 18, Margin = new Thickness(12) };
                btnTake.Click += (s, e) => { _game.TakePendingItem(true); UpdateUI(); };
                btnDiscard.Click += (s, e) => { _game.TakePendingItem(false); UpdateUI(); };
                spActions.Children.Add(btnTake);
                spActions.Children.Add(btnDiscard);
            }
            else
            {
                // обычный переход дальше
                var nextBtn = new Button { Content = "Далее →", Width = 250, Height = 65, FontSize = 20 };
                nextBtn.Click += (s, e) => { _game.NewRoom(); UpdateUI(); };
                spActions.Children.Add(nextBtn);
            }
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