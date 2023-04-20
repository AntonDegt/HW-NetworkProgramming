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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static HTTP.ApiWindow;

namespace HTTP
{
    /// <summary>
    /// Логика взаимодействия для CoincapWindow.xaml
    /// </summary>
    public partial class CoincapWindow : Window
    {
        public ObservableCollection<Asset> Assets { get; set; }
        private HttpClient httpClient = new();

        private HistoryResponce? currentHistory = null;
        private Asset? currentAsset = null;

        public CoincapWindow()
        {
            InitializeComponent();

            Assets = new();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssets();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (currentHistory is not null)
                SelectAsset(currentHistory, RandomColor(currentAsset.id.GetHashCode()));
        }

        private async void LoadAssets()
        {
            string url = "https://api.coincap.io/v2/assets";
            var assetsResponse = await httpClient.GetFromJsonAsync<AssetsResponce>(url);
            if (assetsResponse is null)
            {
                MessageBox.Show("JSON error", "JSON error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Assets.Clear();
            foreach (Asset asset in assetsResponse.data)
                Assets.Add(asset);
        }

        private Color RandomColor(int seed)
        {
            Random random = new Random(seed);

            int sum = 500;
            List<int> arr = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                int x = random.Next(255);

                if (sum - x < 0)
                    x = sum;
                sum -= x;

                arr.Add(x);
            }

            int count = arr.Count;
            for (int i = 0; i < count; i++)
            {
                int element = arr[random.Next(count)];
                arr.Remove(element);
                arr.Insert(random.Next(count), element);
            }

            return Color.FromRgb((byte)arr[0], (byte)arr[1], (byte)arr[2]);
        }
        private void ItemClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                SelectAsset((Asset)item.DataContext);
            }
        }

        private async void SelectAsset(Asset asset)
        {
            var historyResponce = await httpClient.GetFromJsonAsync<HistoryResponce>($"https://api.coincap.io/v2/assets/{asset.id}/history?interval=d1");

            currentAsset = asset;
            rateName.Content = asset.name;
            SelectAsset(historyResponce, RandomColor(asset.id.GetHashCode()));
        }
        private void SelectAsset(HistoryResponce historyResponce, Color color)
        {
            currentHistory = historyResponce;

            canvas.Children.Clear();
            long minTime = historyResponce.data.Min(r => r.time);
            long maxTime = historyResponce.data.Max(r => r.time);
            double minRate = historyResponce.data.Min(r => r.priceUsd);
            double maxRate = historyResponce.data.Max(r => r.priceUsd);
            double graphHeight = canvas.ActualHeight;
            double graphWidth = canvas.ActualWidth;

            double deltaHeight = graphHeight / (maxRate - minRate);
            double deltaWidth = graphWidth / (maxTime - minTime);

            double lastX = (historyResponce.data.First().time - minTime) * deltaWidth; ;
            double lastY = graphHeight - (historyResponce.data.First().priceUsd - minRate) * deltaHeight;

            maxPrice.Content = maxRate.ToString();
            minPrice.Content = minRate.ToString();

            foreach (Rate rate in historyResponce.data)
            {
                double x = (rate.time - minTime) * deltaWidth;
                double y = graphHeight - (rate.priceUsd - minRate) * deltaHeight;

                DrawLine(lastX, lastY, x, y, color);

                lastX = x;
                lastY = y;
            }
        }

        private void DrawLine(double fromX, double fromY, double toX, double toY, Color color)
        {
            Line line = new()
            {
                X1 = fromX,
                Y1 = fromY,
                X2 = toX,
                Y2 = toY,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 1
            };
            canvas.Children.Add(line);
        }
    }

    public class Asset

    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
        public string explorer { get; set; }

    }

    public class AssetsResponce
    {
        public List<Asset> data { get; set; }
        public long timestamp { get; set; }
    }

    public class Rate
    {
        public double priceUsd { get; set; }
        public long time { get; set; }
        public DateTime date { get; set; }
    }

    public class HistoryResponce
    {
        public List<Rate> data { get; set; }
    }
}
