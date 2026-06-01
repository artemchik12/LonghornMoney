using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    public partial class TransferPage : Page
    {
        public TransferPage()
        {
            InitializeComponent();
        }

        private void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).TextDecorations.Add(TextDecorations.Underline);
        }

        private void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).TextDecorations.Clear();
        }

        private void BtnSend_Click(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text) || string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                MessageBox.Show("Заполните номер и сумму!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var payload = new Dictionary<string, object> { 
                    { "request", "send_transfer_msisdn" }, 
                    { "receiver_phone", txtPhone.Text }, 
                    { "amount", txtAmount.Text } 
                };

                var response = ApiClient.Execute(payload);
                if (response != null && response["result"].ToString() == "ok")
                {
                    MessageBox.Show("Перевод успешно завершен!", "Чек", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtPhone.Text = "";
                    txtAmount.Text = "";
                }
                else
                {
                    string err = response != null && response.ContainsKey("text") ? response["text"].ToString() : "Ошибка перевода";
                    MessageBox.Show(err, "Отказ", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex) { MessageBox.Show("Сбой сети: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}