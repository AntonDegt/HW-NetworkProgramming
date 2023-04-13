using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Printing;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HTTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        private async void GetButton_Click(object sender, RoutedEventArgs e)
        {
            String result = await httpClient.GetStringAsync(urlTextBox.Text);
            resultTextBlock.Text = result;
        }

        private async void GetFullButton_Click(object sender, RoutedEventArgs e)
        {
            HttpRequestMessage request = new(   // Более полная форма запроса
                HttpMethod.Get,                 // Метод
                urlTextBox.Text);              // URL w

            HttpResponseMessage response =            // Более полная форма
                await httpClient.SendAsync(request);  // ответа

            resultTextBlock.Text = $"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}\n";

        }

        private async void HeadButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Head,
                    urlTextBox.Text));

            printResponse(response);
        }

        private async void printResponse(HttpResponseMessage response)
        {
            resultTextBlock.Text = $"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}\n";

            foreach (var header in response.Headers)
            {   // var - KeyValuePair<string,IEnumerable<string>>
                String headerString = header.Key + ": ";
                foreach (string value in header.Value)
                {
                    headerString += value + " ";
                }
                resultTextBlock.Text += $"{headerString}\n";
            }
            resultTextBlock.Text += "------------------------------------\n";
            resultTextBlock.Text += await response.Content.ReadAsStringAsync();
            resultTextBlock.Text += "\n------------------------------------\n";
        }

        private async void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Options,
                    urlTextBox.Text));

            printResponse(response);
        }
        private async void Get2Button_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Get,
                    urlTextBox.Text));

            printResponse(response);
        }

        private const string RandomPasswordsUrl = "https://www.random.org/passwords/?num=5&len=8&format=html&rnd=new";
        private async void RandPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Get,
                    RandomPasswordsUrl));

            printPasswords(response);
        }

        private async void printPasswords(HttpResponseMessage response)
        {
            string text = await response.Content.ReadAsStringAsync();

            text = new Regex(@"(<ul class=\" + "\"data\"" + @">)(\n\<li\>[A-Za-z0-9]{8}\<\/li\>){5}").Match(text).Value;
            MatchCollection matches = new Regex("[A-z0-9]{8}").Matches(text);

            resultTextBlock.Text = "";
            foreach (Match match in matches)
                resultTextBlock.Text += match.Value + "\n";
        }
    }
}
