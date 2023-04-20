using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HTTP
{
    /// <summary>
    /// Логика взаимодействия для SMTPWindow.xaml
    /// </summary>
    public partial class SMTPWindow : Window
    {
        public SMTPWindow()
        {
            InitializeComponent();
        }

        private static readonly char[] CHARS = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y' };

        private static readonly Random random = new Random();

        public static string GenerateCode(int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(CHARS.Length);
                sb.Append(CHARS[index]);
            }
            return sb.ToString();
        }

        public dynamic? emainConfig;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String configFilename = "emailconfig.json";
            try
            {
                emainConfig = JsonSerializer.Deserialize<dynamic>(
                    System.IO.File.ReadAllText(configFilename)
                );
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show($"Не найден файл конфигурации '{configFilename}'");
                this.Close();
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Ошибка JSON файла: {ex.Message}");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                this.Close();
            }
            if (emainConfig is null)
            {
                MessageBox.Show($"Ошибка получения конфигурации");
                this.Close();
            }


        }

        private SmtpClient GetSmtpClient()
        {
            //emainConfig.GetProperty("smtp").GetProperty("gmail").GetProperty("host").GetString();
            JsonElement gmail = emainConfig.GetProperty("smtp").GetProperty("gmail");

            String host = gmail.GetProperty("host").GetString();
            int port = gmail.GetProperty("port").GetInt32();
            String mailbox = gmail.GetProperty("email").GetString();
            String password = gmail.GetProperty("password").GetString();
            bool ssl = gmail.GetProperty("ssl").GetBoolean();

            return new SmtpClient(host)
            {
                Port = port,
                EnableSsl = ssl,
                Credentials = new NetworkCredential(mailbox, password)
            };
        }

        private void SendTestButton_Click(object sender, RoutedEventArgs e)
        {
            using SmtpClient smtpClient = GetSmtpClient();
            JsonElement gmail = emainConfig.GetProperty("smtp").GetProperty("gmail");
            String mailbox = gmail.GetProperty("email").GetString();

            try
            {
                smtpClient.Send(
                    from: mailbox,
                    recipients: "anton1109deg@gmail.com",
                    subject: "Test Message",
                    body: "Test Message from SmtpWindow");

                MessageBox.Show("Sent OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sent error '{ex.Message}'");
            }
        }

        private void SendHtmlButton_Click(object sender, RoutedEventArgs e)
        {
            using SmtpClient smtpClient = GetSmtpClient();
            JsonElement gmail = emainConfig.GetProperty("smtp").GetProperty("gmail");
            String mailbox = gmail.GetProperty("email").GetString();

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(mailbox),
                Body = "<b>Test</b> <i>message</i> <b style='color: green;'>SmtpGreen</b>",
                IsBodyHtml = true,
                Subject = "Test Message"
            };
            mailMessage.To.Add(new MailAddress(mailbox));

            try
            {
                smtpClient.Send(mailMessage);
                MessageBox.Show("Sent HTML OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sent error '{ex.Message}'");
            }
        }

        private void HWButton_Click(object sender, RoutedEventArgs e)
        {
            using SmtpClient smtpClient = GetSmtpClient();
            JsonElement gmail = emainConfig.GetProperty("smtp").GetProperty("gmail");
            String mailbox = gmail.GetProperty("email").GetString();

            string code = GenerateCode(Convert.ToInt32(nbox.Text));

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(mailbox),
                Body = code,
                IsBodyHtml = true,
                Subject = "Code"
            };
            mailMessage.To.Add(new MailAddress(emailbox.Text));

            try
            {
                smtpClient.Send(mailMessage);
                MessageBox.Show("Sent Code OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sent error '{ex.Message}'");
            }
        }
    }
}
