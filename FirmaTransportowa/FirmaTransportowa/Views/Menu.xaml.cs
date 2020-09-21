using FirmaTransportowa.ViewModels;
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

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu(int permissionLevel)
        {
            InitializeComponent();

            switch (permissionLevel)
            {
                case 1: //pracownik
                           for (int i = 0; i < 4; i++)
                    Kierownik.Items.RemoveAt(0);
                               Kierownik.Items.RemoveAt(3);
                    break;

                 case 2: //pracownik + opiekun
                          for (int i = 0; i < 4; i++)
                    Kierownik.Items.RemoveAt(0);
                    break;
                case 3: //pracownik + kierownik
                        Kierownik.Items.RemoveAt(7);
                    break;
                default:
                    break;
            }
        }

        private void Workers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Pracownicy();
        }

        private void Rent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Rezerwacje();
        }
        private void Lend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Wypozyczenia();
        }

        private void Manage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdamiModel();
        }
        private void Reservations_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojeRezerwacje();
        }
        private void Lends_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojeWypozyczenia();
        }
        private void CarList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ListaPojazdowModel();
        }
        private void MyCars_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojePojazdyModel();
        }
        private void Logout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = null;
            glowneOkno.Width = 300;
            glowneOkno.Height = 450;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = new Logowanie();
            Logowanie.actualUser = null;
        }
    }
}
