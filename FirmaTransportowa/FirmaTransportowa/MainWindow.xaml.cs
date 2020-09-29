using FirmaTransportowa.Model;
using FirmaTransportowa.Views;
using System;
using System.Windows;
using System.Windows.Input;

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
                    if (res.returnDate <= DateTime.Now.Date && res.ended == false)
                        res.ended = true; //zakańczamy rezerwację 
            }
            db.SaveChanges();
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
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
