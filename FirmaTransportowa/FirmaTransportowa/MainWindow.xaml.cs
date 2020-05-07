using FirmaTransportowa.ViewModels;
using FirmaTransportowa.Views;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace FirmaTransportowa
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
        private void Refuel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Rent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new RezerwacjeModel();
        }
    }
}
