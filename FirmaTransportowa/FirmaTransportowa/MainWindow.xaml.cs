using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
using FirmaTransportowa.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
            UpdateReservations(); // aktualizacja  rezerwacji
            LogIn();
        }
        private void LogIn()
        {
            Width = 300;
            Height = 450;
            LoginScreen.Content = new Logowanie();
            
            //Window logowanieView = new Logowanie();
            //logowanieView.ShowDialog();
        }
        private void UpdateReservations()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var reservation = db.Reservations;
            foreach (var res in reservation)
            {
                    if (res.returnDate < DateTime.Now )
                        res.ended = true; //zakańczamy rezerwację 
            }
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Minimize_Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Minimized;
        }
    }
}
