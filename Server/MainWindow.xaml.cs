using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket? listenSocket;
        bool server = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Check start network parametrs");
                return;
            }
            listenSocket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);


            // Постоянно актывный слушающий сокет
            // заюлокирует UI если его не отделитьв отдельный поток

            if (server)
            {
                serverLogs.Text += "Server is already Started\n";
                return;
            }
            new Thread(StartServer).Start(endpoint);
            server = true;
        }

        private void StartServer(object? param)
        {
            if (listenSocket == null) return;
            IPEndPoint? endpoint = param as IPEndPoint;
            if (endpoint == null) return;

            try
            {
                listenSocket.Bind(endpoint);
                listenSocket.Listen(100);
                byte[] buf = new byte[1024];

                Dispatcher.Invoke(() => serverLogs.Text += $"Server started\n");
                Dispatcher.Invoke(() => { serverStatus.Content = "ON"; serverStatus.Foreground = new SolidColorBrush(Colors.LimeGreen);} );

                while (true) 
                {
                    // Ждём подключения 
                    Socket socket =
                        listenSocket.Accept();


                    // Получаем строку
                    StringBuilder sb = new StringBuilder();
                    do
                    {
                        int n = socket.Receive(buf);
                        sb.Append(Encoding.UTF8.GetString(buf, 0, n));
                    } while (socket.Available > 0);

                    // Запись в логи
                    String str = sb.ToString();
                    Dispatcher.Invoke(() => serverLogs.Text += $"{str}\n" );

                    // Отправляем ответ
                    str = $"Received at {DateTime.Now}";
                    socket.Send(Encoding.UTF8.GetBytes(str));


                    // Закрываем
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Dispose();
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke( () => serverLogs.Text += $"Server stoped {ex.Message}\n");
                server = false;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (listenSocket == null)
            {
                serverLogs.Text += "Server is already Stoped\n";
                return;
            }
            if (!server)
            {
                serverLogs.Text += "Server is already Stoped\n";
                return;
            }

            listenSocket?.Close();
            server = false;

            serverStatus.Content = "OFF";
            serverStatus.Foreground = new SolidColorBrush(Colors.Red);
            serverLogs.Text += "Server Stoped\n";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            StopServer();
        }
    }
}
