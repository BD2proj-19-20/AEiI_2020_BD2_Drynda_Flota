using FirmaTransportowa.Model;
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
    /// Logika interakcji dla klasy ZmienRezerwacje.xaml
    /// </summary>
    public partial class ZmienRezerwacje : UserControl
    {
         Reservation reservationChange;
        public ZmienRezerwacje(Reservation reservationChange)
        {
            InitializeComponent();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var reservations = db.Reservations;

            foreach (var reserv in reservations)
            {
                if (reserv.id == reservationChange.id)
                {

                    reservationChange = reserv;

                }
            }
            ReservationDate.Text = reservationChange.reservationDate.ToString().Substring(0, 10);
            ReservationDate.IsReadOnly = true;

            ReservationStart.Text = reservationChange.lendDate.ToString().Substring(0, 10);
            ReservationEnd.Text = reservationChange.returnDate.ToString().Substring(0, 10);

            var people = db.People;

            foreach (var human in people)
            {
                if (human.layoffDate > DateTime.Now || human.layoffDate == null) //wyswietlamy tych co jeszcze pracują
                    Pracownicy.Items.Add(human.id.ToString() + ") " + human.firstName + " " + human.lastName);
            }
            var cars = db.Cars;
            foreach (var car in cars)
            {
                if (car.onService == false)  //gdy w sewisie nie wypożyczamy
                    PojazdID.Items.Add(car.id);      
            }
            PojazdID.SelectedItem = reservationChange.carId;
            int index = -1;
            foreach (var human in people)
            {
                if (human.layoffDate > DateTime.Now || human.layoffDate == null)
                    index++;
                if (reservationChange.personId == human.id)
                    break;
            }
            Pracownicy.SelectedIndex = index;
            Dane_Pojzadu();

        }
        private void Dane_Pojzadu()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            foreach (var car in cars)
            {
                if ((car.id).ToString() == PojazdID.SelectedItem.ToString())
                {
                    Rejestracja.Text = car.Registration;
                    PojemnoscSilnika.Text = car.engineCapacity.ToString();

                    var carmodel = db.CarModels;
                    foreach (var carModel in carmodel)
                    {
                        if (car.modelId == carModel.id)
                        {
                            Marka.Text = car.CarModel.make;
                            Model.Text = car.CarModel.model;
                        }
                    }
                    var carDes = db.CarDestinations;
                    foreach (var cardes in carDes)
                    {
                        if (car.destinationId == cardes.id)
                            Zastosowanie.Text = cardes.name;
                    }
                }
            }
            Rejestracja.IsReadOnly = true;
            PojemnoscSilnika.IsReadOnly = true;
            Marka.IsReadOnly = true;
            Model.IsReadOnly = true;
            Zastosowanie.IsReadOnly = true;

        }
        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {

        }

            private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new RezerwacjeModel();

        }
        private void Function_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Dane_Pojzadu();
        }
    }
}
