using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    public partial class ServicesPage : Page
    {
        public ServicesPage()
        {
            InitializeComponent();
            LoadProviders();
        }

        private void LoadProviders()
        {
            cbServices.DisplayMemberPath = "Name";
            cbServices.SelectedValuePath = "GoodId";
            cbServices.Items.Add(new { Name = "МегаФон (Моб. связь)", GoodId = "313203" });
            cbServices.Items.Add(new { Name = "МТС", GoodId = "318116" });
            cbServices.Items.Add(new { Name = "Билайн", GoodId = "313065" });
            cbServices.Items.Add(new { Name = "Мосэнергосбыт", GoodId = "350054" });
            cbServices.Items.Add(new { Name = "ВКонтакте (Голоса)", GoodId = "320750" });
            cbServices.SelectedIndex = 0;
        }

        private void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).TextDecorations.Add(TextDecorations.Underline);
        }

        private void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).TextDecorations.Clear();
        }

        private void BtnPay_Click(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccount.Text) || string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("Заполните лицевой счет и сумму!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var payload = new Dictionary<string, object> { 
                    { "request", "transfer_add" }, 
                    { "good_id", cbServices.SelectedValue.ToString() } 
                };

                // ВАЖНО: Пакуем поля в список словарей (в точности как ждет наш сервер)
                var fieldsList = new List<object> {
                    new Dictionary<string, string> { { "name", "account" }, { "value", txtAccount.Text } },
                    new Dictionary<string, string> { { "name", "sum" }, { "value", txtAmount.Text } }
                };
                payload["fields"] = fieldsList;

                var response = ApiClient.Execute(payload);
                if (response != null && response["result"].ToString() == "ok")
                {
                    MessageBox.Show("Услуга успешно оплачена!", "Квитанция", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Эмуляция запроса чека на Email
                    ApiClient.Execute(new Dictionary<string, object> { { "request", "transfer_receipt" }, { "transfer_id", response["transfer_id"].ToString() }, { "email", "client@desktop.wpf" } });

                    txtAccount.Text = "";
                    txtAmount.Text = "";
                }
                else
                {
                    string err = response != null && response.ContainsKey("text") ? response["text"].ToString() : "Ошибка оплаты";
                    MessageBox.Show(err, "Отказ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex) { MessageBox.Show("Сбой сети: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}