using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;

namespace Client
{
    public partial class Dashboard : Window
    {
        private Grid currentSelectedMenu;

        public Dashboard()
        {
            InitializeComponent();
            Username.Text = ApiClient.CurrentPhone;

            // Воспроизводим звук приветствия Windows
            try { SystemSounds.Asterisk.Play(); }
            catch { }

            // Устанавливаем стартовую кнопку активной
            currentSelectedMenu = MenuHome;
            Home_BG.Visibility = Visibility.Visible;

            // Загружаем домашнюю страницу
            PageFrame.Navigate(new HomePage());
        }

        // ==========================================
        // 🎨 ЛОГИКА ПОДСВЕТКИ БОКОВОГО МЕНЮ (HOVER)
        // ==========================================
        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid hoveredGrid = sender as Grid;
            if (hoveredGrid != currentSelectedMenu)
            {
                Rectangle bg = (Rectangle)hoveredGrid.Children[0];
                bg.Visibility = Visibility.Visible;
                bg.Opacity = 0.5; // Легкая подсветка при наведении
            }
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid hoveredGrid = sender as Grid;
            if (hoveredGrid != currentSelectedMenu)
            {
                Rectangle bg = (Rectangle)hoveredGrid.Children[0];
                bg.Visibility = Visibility.Collapsed; // Убираем подсветку
            }
        }

        private void Menu_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid clickedGrid = sender as Grid;

            // Сбрасываем старую кнопку
            if (currentSelectedMenu != null)
            {
                ((Rectangle)currentSelectedMenu.Children[0]).Visibility = Visibility.Collapsed;
                ((Label)currentSelectedMenu.Children[2]).FontFamily = new FontFamily("Segoe UI"); // <--- ИЗМЕНЕНО НА 2
            }

            // Активируем новую кнопку
            currentSelectedMenu = clickedGrid;
            ((Rectangle)currentSelectedMenu.Children[0]).Visibility = Visibility.Visible;
            ((Rectangle)currentSelectedMenu.Children[0]).Opacity = 1; // Полная яркость
            ((Label)currentSelectedMenu.Children[2]).FontFamily = new FontFamily("Segoe UI Semibold"); // <--- ИЗМЕНЕНО НА 2

            // Переключаем страницу во Frame
            if (clickedGrid.Name == "MenuHome")
            {
                PageFrame.Navigate(new HomePage());
            }
            else if (clickedGrid.Name == "MenuTransfer")
            {
                PageFrame.Navigate(new TransferPage());
            }
            else if (clickedGrid.Name == "MenuServices")
            {
                PageFrame.Navigate(new ServicesPage());
            }
        }
             
            

        // ==========================================
        // 🚪 КНОПКА ВЫХОДА
        // ==========================================
        private void MenuLogout_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ApiClient.CurrentSid = null;
            ApiClient.CurrentPhone = null;

            // Открываем окно логина и закрываем текущее
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}