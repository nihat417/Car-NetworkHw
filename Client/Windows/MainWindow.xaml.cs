using Client.Windows;
using Server.Commands;
using Server.Enums;
using Server.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;


namespace Client
{

    public partial class MainWindow : Window
    {
        public Car? Car { get; set; }

        private TcpClient tcpClient;
        private BinaryWriter? bw;
        private BinaryReader? br;        

        public MainWindow()
        {
            InitializeComponent();
            ClientConnect();
        }

        public void ClientConnect()
        {
            combobox.ItemsSource = Enum.GetValues(typeof(HttpMethods));
            var ip = IPAddress.Parse("127.0.0.1");
            var port = 12345;
            tcpClient = new TcpClient();
            tcpClient.Connect(ip, port);
            var stream = tcpClient.GetStream();
            bw = new BinaryWriter(stream);
            br = new BinaryReader(stream);
        }

        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
                if (comboBox.SelectedItem is HttpMethods method &&
                    (method == HttpMethods.GET ||
                    method == HttpMethods.DELETE))
                    textblock.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (combobox.SelectedItem is null)
            {
                MessageBox.Show("Please select any command.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (combobox.SelectedItem is HttpMethods method)
            {
                ExecuteCommand(method);
                combobox.SelectedItem = null;
            }
        }


        private async void ExecuteCommand(HttpMethods method)
        {
            var stream = tcpClient.GetStream();
            var bw = new BinaryWriter(stream);
            var br = new BinaryReader(stream);

            switch (method)
            {  
                case HttpMethods.POST:
                    {
                        MessageBox.Show("POST");
                        AddCar addCar = new AddCar();
                        addCar.ShowDialog();
                        Command command = new Command()
                        {
                            Method = HttpMethods.POST,
                            Car = addCar.Car
                        };
                        string jsonString = JsonSerializer.Serialize(command);
                        bw.Write(jsonString);
                        MessageBox.Show(br.ReadBoolean() ? "Add process is successfully..."
                            : "Fauled...");
                        break;
                    }
                case HttpMethods.GET:
                    {
                        if (textblock.Text is not null)
                        {
                            
                            var Car = new Car();
                            Car.Id = int.Parse(textblock.Text);

                            Command command = new Command()
                            {
                                Method = HttpMethods.GET,
                                Car = Car
                            };

                            string jsonString = JsonSerializer.Serialize(command);
                            bw.Write(jsonString);
                            var jsonResponse = br.ReadString();
                            var car = JsonSerializer.Deserialize<Car>(jsonResponse);
                            if (car is not null && car.Make is not null && car.Make.Length > 0)
                            {
                                GetCar getCar = new GetCar(car);
                                getCar.ShowDialog();
                            }
                            else
                                MessageBox.Show("Car cannot find...", "", MessageBoxButton.OK, MessageBoxImage.Error);

                            textblock.IsEnabled = false;
                            textblock.Text = string.Empty;
                        }
                        break;
                    }
                case HttpMethods.DELETE:
                    {
                        if (textblock.Text is not null)
                        {
                            var Car = new Car();
                            Car.Id = int.Parse(textblock.Text);
                            Command command = new Command()
                            {
                                Method = HttpMethods.DELETE,
                                Car = Car
                            };


                            string jsonString = JsonSerializer.Serialize(command);
                            bw.Write(jsonString);
                            bool isDeleted = br.ReadBoolean();
                            MessageBox.Show(isDeleted ? "Delete process is successfully..." : "Fauled...");
                            textblock.IsEnabled = false;
                            textblock.Text = string.Empty;
                        }
                        break;
                        
                    }
            }
        }
    }
}
