using Server.Models;
using System.Windows;


namespace Client.Windows
{
    public partial class GetCar : Window
    {
        public Car Car { get; set; }
        public GetCar(Car car)
        {
            InitializeComponent();
            DataContext = this;
            Car = car;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
            => DialogResult = true;
    }
}
