using Server.Models;
using System.Windows;

namespace Client.Windows
{
   
    public partial class AddCar : Window
    {
        public Car Car { get; set; }

        public AddCar()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Car = new Car
            {
                Model = modelTxt.Text,
                Make = makeTxt.Text,
                Year = ushort.Parse(yearTxt.Text),
                VIN = VinTxt.Text,
                Color = ColorTxt.Text
            };

            DialogResult = true;
        }
    }
}
