using FirmaTransportowa.Model;
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
            UpdateReservations(); // aktualizacja  rezerwacji
            Menu.Content = new MenuPracownikModel();     //wybrane menu - dla pracownika
            //Menu.Content = new MenuKierownikModel();   //wybrane menu - dla Kierownika
            //Menu.Content = new MenuOpiekunModel();     //wybrane menu - dla opiekuna
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
        private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Rent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new RezerwacjeModel();
        }

        private void CarList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new ListaPojazdowModel();
        }

        private void Manage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new ZarzadzajPojazdamiModel();
        }

        private void MyCars_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new MojePojazdyModel();
        }
        private void Reservations_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new MojeRezerwacjeModel();
        }

        private void Workers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataContext = new PracownicyModel();
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Logout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Minimize_Maximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Minimized;
        }
    }
}
