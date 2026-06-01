using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client
{
    public partial class Login : Window
    {
        private string defaultPhonePlaceholder = "Номер телефона (например, 7926...)";

        public Login()
        {
            InitializeComponent();
        }

        // ==========================================
        // 🎨 ВИЗУАЛЬНЫЕ ЭФФЕКТЫ (Плейсхолдеры)
        // ==========================================
        private void UserBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (UserBox.Text == defaultPhonePlaceholder)
            {
                UserBox.Text = string.Empty;
                UserBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void UserBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserBox.Text))
            {
                UserBox.Text = defaultPhonePlaceholder;
                UserBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void PassBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PasswordPreviewText.Visibility = Visibility.Collapsed;
        }

        private void PassBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PassBox.Password))
            {
                PasswordPreviewText.Visibility = Visibility.Visible;
            }
        }

        private void HostProv_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HostProv.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void HostProv_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HostProv.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ServWrapPanel.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ServWrapPanel.Visibility = Visibility.Collapsed;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).TextDecorations.Add(TextDecorations.Underline);
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).TextDecorations.Clear();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BG.Focus();
        }

        // ==========================================
        // 🔘 КНОПКА АВТОРИЗАЦИИ (Стрелочка)
        // ==========================================
        private void PassBoxButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PassBoxButton.Source = new BitmapImage(new Uri("pack://application:,,,/res/signinbutton-3.png"));
        }

        private void PassBoxButton_MouseEnter(object sender, MouseEventArgs e)
        {
            PassBoxButton.Source = new BitmapImage(new Uri("pack://application:,,,/res/signinbutton-2.png"));
        }

        private void PassBoxButton_MouseLeave(object sender, MouseEventArgs e)
        {
            PassBoxButton.Source = new BitmapImage(new Uri("pack://application:,,,/res/signinbutton-1.png"));
        }

        private async void PassBoxButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PassBoxButton.Source = new BitmapImage(new Uri("pack://application:,,,/res/signinbutton-1.png"));

            string phone = UserBox.Text == defaultPhonePlaceholder ? "" : UserBox.Text;
            string pass = PassBox.Password;

            if (HostCheckBox.IsChecked == true) ApiClient.ServerUrl = HostProv.Text;

            ShowLoadingScreen("Выполняется вход в систему...");

            try
            {
                var response = await Task.Run(() => ApiClient.Execute(new Dictionary<string, object> { 
                    { "request", "auth" }, 
                    { "username", phone }, 
                    { "password", pass } 
                }));

                if (response != null && response.ContainsKey("result") && response["result"].ToString() == "ok")
                {
                    ApiClient.CurrentSid = response["sid"].ToString();
                    ApiClient.CurrentPhone = phone;

                    // 🔥 ИСПРАВЛЕНИЕ: Открываем Дашборд как Окно!
                    Dashboard dashboard = new Dashboard();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    ShowErrorScreen(response != null && response.ContainsKey("text") ? response["text"].ToString() : "Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                ShowErrorScreen("Ошибка сети: " + ex.Message);
            }
        }

        // ==========================================
        // 📩 ЗАПРОС СМС КОДА
        // ==========================================
        private async void GetSmsCode_Click(object sender, MouseButtonEventArgs e)
        {
            string phone = UserBox.Text == defaultPhonePlaceholder ? "" : UserBox.Text;
            if (HostCheckBox.IsChecked == true) ApiClient.ServerUrl = HostProv.Text;

            ShowLoadingScreen("Отправка запроса...");

            try
            {
                var response = await Task.Run(() => ApiClient.Execute(new Dictionary<string, object> { 
                    { "request", "password_get" }, 
                    { "msisdn", phone } 
                }));

                ShowErrorScreen(""); // Сбрасываем загрузку
                Error.Visibility = Visibility.Collapsed;

                if (response != null && response.ContainsKey("result") && response["result"].ToString() == "ok")
                {
                    MessageBox.Show("Код успешно отправлен в Telegram!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    PassBox.Focus();
                }
                else
                {
                    ShowErrorScreen(response != null && response.ContainsKey("text") ? response["text"].ToString() : "Сбой отправки кода");
                }
            }
            catch (Exception ex)
            {
                ShowErrorScreen("Ошибка сети: " + ex.Message);
            }
        }

        private void Support_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("В этом эмуляторе поддержка осуществляется через чат Telegram канала. ", "Поддержка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==========================================
        // 🔄 УПРАВЛЕНИЕ СОСТОЯНИЯМИ ЭКРАНА
        // ==========================================
        private void ShowLoadingScreen(string text)
        {
            Error.Visibility = Visibility.Collapsed;

           
            LoginPanel.Visibility = Visibility.Collapsed;
            WelcomeText.Content = text;
            WelcomePanel.Visibility = Visibility.Visible;
        }

        private void ShowErrorScreen(string errorMsg)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(errorMsg))
            {
                Error.Content = errorMsg;
                Error.Visibility = Visibility.Visible;
            }
        }
    }
}