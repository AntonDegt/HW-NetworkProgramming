using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
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
    /// Логика взаимодействия для ApiWindow.xaml
    /// </summary>
    public partial class ApiWindow : Window
    {
        public ObservableCollection<NbuRate> NbuRates { get; set; }
        private List<NbuRate>? _nbuRates = null;

        private string sorted = "";

        private HttpClient httpClient;
        public ApiWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            httpClient = new HttpClient();
            NbuRates = new();
        }

        private async void NbuToday_Click(object sender, RoutedEventArgs e)
        {
            string url = $"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={RateCalendar.SelectedDate.Value.Date.ToString("yyyyMMdd")}&json";
            _nbuRates = await httpClient.GetFromJsonAsync<List<NbuRate>>(url);



            if (_nbuRates is null)
                MessageBox.Show("JSON parse error");
            else
            {
                NbuRates.Clear();
                foreach (NbuRate rate in _nbuRates)
                    NbuRates.Add(rate);
            }
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader header
                && header.Content is not null
                && _nbuRates is not null)
            {
                string name = header.Content.ToString();
                switch (name)
                {
                    case "cc":
                        _nbuRates = _nbuRates.OrderBy(r => r.cc).ToList();
                        break;
                    case "txt":
                        _nbuRates = _nbuRates.OrderBy(r => r.txt).ToList();
                        break;
                    case "rate":
                        _nbuRates = _nbuRates.OrderBy(r => r.rate).ToList();
                        break;
                    case "r030":
                        _nbuRates = _nbuRates.OrderBy(r => r.r030).ToList();
                        break;
                };
                if (sorted == name)
                    _nbuRates.Reverse();
                else sorted = name;

                NbuRates.Clear();
                foreach (NbuRate rate in _nbuRates)
                {
                    NbuRates.Add(rate);
                }
            }
        }

        public class NbuRate
        { 
            public int r030 { get; set; }
            public String txt { get; set; }
            public Double rate { get; set; }    
            public String cc { get; set; }
            public String exchangedate { get; set; }
        }
    }
}
