﻿using System.Windows.Controls;
using System.Windows.Input;

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
                           for (int i = 0; i < 5; i++)
                    Kierownik.Items.RemoveAt(0);
                               Kierownik.Items.RemoveAt(3);
                    break;

                 case 2: //pracownik + opiekun
                          for (int i = 0; i < 5; i++)
                    Kierownik.Items.RemoveAt(0);
                    break;
                case 3: //pracownik + kierownik
                        Kierownik.Items.RemoveAt(8);
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
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }
		private void Contractors_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
			glowneOkno.DataContext = new Kontraktorzy();
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
            glowneOkno.DataContext = new ListaPojazdow();
        }
        private void MyCars_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojePojazdy();
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
		private void Help_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			string dir = System.IO.Directory.GetCurrentDirectory();
			System.Windows.Forms.Help.ShowHelp(null, @"..\..\help\index.htm");
			HelpButton.IsSelected = false;
		}
	}
}
