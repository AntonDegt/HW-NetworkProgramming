using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint endpoint;

            try
            {
                IPAddress ip = IPAddress.Parse(serverIp.Text);
                int port = Convert.ToInt32(serverPort.Text);
                endpoint = new IPEndPoint(ip, port);
            }
            catch
            {
                MessageBox.Show("Check server network parametrs");
                return;
            }

            Socket clientSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(endpoint);
                clientSocket.Send(Encoding.UTF8.GetBytes(Message.Text));

                clientSocket.Listen(100);
                byte[] buf = new byte[1024];
                StringBuilder sb = new StringBuilder();
                do
                {
                    int n = clientSocket.Receive(buf);
                    sb.Append(Encoding.UTF8.GetString(buf, 0, n));
                } while (clientSocket.Available > 0);

                // Запись в логи
                String str = sb.ToString();

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Dispose();
            }
            catch (Exception ex)
            {
                chatLogs.Text += ex.Message + "\n";
            }
        }
    }
}
