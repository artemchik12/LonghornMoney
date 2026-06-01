using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            lblPhone.Text = ApiClient.CurrentPhone; // Показываем текущий номер кошелька
            LoadBalance();
        }

        private void LoadBalance()
        {
            try
            {
                lblBalance.Text = "Загрузка...";

                var payload = new Dictionary<string, object> { { "request", "balance" } };
                var response = ApiClient.Execute(payload);

                if (response != null && response["result"].ToString() == "ok")
                {
                    double bal = Convert.ToDouble(response["balance"]);
                    lblBalance.Text = bal.ToString("N2") + " ₽";
                }
                else
                {
                    lblBalance.Text = "Ошибка";
                }
            }
            catch
            {
                lblBalance.Text = "Сбой сети";
            }
        }

        // ==========================================
        // 🎨 ВИЗУАЛЬНЫЕ ЭФФЕКТЫ (Из Post.xaml)
        // ==========================================
        private void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            // Подчеркиваем текст при наведении
            ((TextBlock)sender).TextDecorations.Add(TextDecorations.Underline);
        }

        private void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            // Убираем подчеркивание
            ((TextBlock)sender).TextDecorations.Clear();
        }

        private void BtnRefresh_Click(object sender, MouseButtonEventArgs e)
        {
            LoadBalance();
        }
    }
}