using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
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
        private String projectDir;

        public MainWindow()
        {
            InitializeComponent();

            String currentDir = Directory.GetCurrentDirectory();
            int httpPosition = currentDir.IndexOf("HTTP");
            projectDir = currentDir[..httpPosition];
        }

        private void CharServer_Click(object sender, RoutedEventArgs e)
        {
            String serverPath = @"Server\bin\Debug\net6.0-windows\server.exe";
            Process serverProcess = Process.Start(projectDir + serverPath);
        }
        private void ChatClient_Click(object sender, RoutedEventArgs e)
        {
            String serverPath = @"Client\bin\Debug\net6.0-windows\client.exe";
            Process clientProcess = Process.Start(projectDir + serverPath);
        }

        private void HttpRequests_Click(object sender, RoutedEventArgs e)
        {
            new HttpRequests().Show();
        }

        private void Api_Click(object sender, RoutedEventArgs e)
        {
            new ApiWindow().Show();
        }
    }
}
