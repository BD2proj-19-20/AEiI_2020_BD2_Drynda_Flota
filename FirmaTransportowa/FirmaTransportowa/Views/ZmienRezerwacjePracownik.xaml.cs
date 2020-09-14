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
    /// Logika interakcji dla klasy ZmienRezerwacjePracownik.xaml
    /// </summary>
    public partial class ZmienRezerwacjePracownik : UserControl
    {
        Reservation reservationChange;
        public ZmienRezerwacjePracownik(Reservation reservationChange)
        {
            InitializeComponent();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var reservations = db.Reservations;

            foreach (var reserv in reservations)
            {
                if (reserv.id == reservationChange.id)
                    this.reservationChange = reserv;

            }
            ReservationDate.SelectedDate = reservationChange.reservationDate;
            ReservationStart.BlackoutDates.AddDatesInPast();
            ReservationEnd.BlackoutDates.AddDatesInPast();
            ReservationStart.SelectedDate = reservationChange.lendDate;
            ReservationEnd.SelectedDate = reservationChange.returnDate;

            var people = db.People;

            var cars = db.Cars;
            foreach (var car in cars)
            {
                if (car.onService == false)  //gdy w sewisie nie wypożyczamy
                    PojazdID.Items.Add(car.id.ToString());
            }
            PojazdID.SelectedItem = reservationChange.carId.ToString();
            int index = -1;
            foreach (var human in people)
            {
                if (human.layoffDate > DateTime.Now || human.layoffDate == null)
                    index++;
                if (reservationChange.personId == human.id)
                    break;
            }

            Dane_Pojzadu();

        }
        private void Dane_Pojzadu()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            foreach (var car in cars)
            {
                if ((car.id).ToString() == PojazdID.Text)
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
            PrywatneBox.IsChecked = reservationChange.@private;
            Rejestracja.IsReadOnly = true;
            PojemnoscSilnika.IsReadOnly = true;
            Marka.IsReadOnly = true;
            Model.IsReadOnly = true;
            Zastosowanie.IsReadOnly = true;

        }
        private void Zmien_Dane_Rezerwacji(object sender, RoutedEventArgs e)
        {

            int id = Logowanie.actualUser.id;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            DateTime temp;
            var reservations = db.Reservations;
            var lends = db.Lends;
            var people = db.People;
            DateTime? datePersonOut = null;
            DateTime? actualCarLendDate = null;
            DateTime? actualCarReturnDate = null;
            Person personReservation = null;
            foreach (var person in people)
            {
                if (person.id==id)
                {
                    datePersonOut = person.layoffDate;
                    personReservation = person;
                }
            }
            bool doReservationCar = true;
            bool doReservationPerson = true;

            if (ReservationEnd.SelectedDate != null && datePersonOut < ReservationEnd.SelectedDate)
                MessageBox.Show("Wybrany pracownik zostaje zwolniony\nw czasie nowej rezerwacji.", "Komunikat");
            else if (ReservationStart.SelectedDate != null && ReservationEnd.SelectedDate != null &&
                ReservationEnd.SelectedDate > ReservationStart.SelectedDate) //sprawdzanie poprawności danych
            {

                foreach (var reserv in reservations)
                {
                    if (reserv.carId.ToString() == PojazdID.SelectedItem.ToString() && reserv.id != this.reservationChange.id) //nie obchodzi nas zmieniana rezerwacja
                    {
                        actualCarLendDate = reserv.lendDate;
                        actualCarReturnDate = reserv.returnDate;

                        if (actualCarReturnDate < ReservationStart.SelectedDate || (actualCarLendDate > ReservationEnd.SelectedDate)
                             || (actualCarLendDate == null && actualCarReturnDate == null) || reserv.ended == true)
                        {
                            doReservationCar = true;
                        }
                        else
                        {
                            doReservationCar = false;
                            break;
                        }
                    }

                }
                foreach (var reserv in reservations) //sprawdzanie rezerwacji wybranego pracownika
                {
                    if (reserv.personId.ToString() == personReservation.id.ToString() && reserv.id != this.reservationChange.id) //nie obchodzi nas zmieniana rezerwacja
                    {
                        actualCarLendDate = reserv.lendDate;
                        actualCarReturnDate = reserv.returnDate;

                        if (actualCarReturnDate < ReservationStart.SelectedDate || (actualCarLendDate > ReservationEnd.SelectedDate)
                             || (actualCarLendDate == null && actualCarReturnDate == null) || reserv.ended == true)
                        {
                            doReservationPerson = true;
                        }
                        else
                        {
                            doReservationPerson = false;
                            break;
                        }
                    }

                }

                if (doReservationCar == true && doReservationPerson == true) //sprawdzanie czy samochod jest zareezrwowany w wybranym czasie lub pracownik ma rezerwacje w tym czasie
                {

                    Reservation reservationChange = null;
                    foreach (var reserv in reservations)
                    {
                        if (reserv.id == this.reservationChange.id)
                            reservationChange = reserv;
                    }

                    reservationChange.carId = Int16.Parse(PojazdID.SelectedItem.ToString());
                    reservationChange.reservationDate = ReservationDate.SelectedDate.Value;
                    reservationChange.lendDate = ReservationStart.SelectedDate.Value;
                    reservationChange.returnDate = ReservationEnd.SelectedDate;
                    reservationChange.ended = false;

                    if (PrywatneBox.IsChecked == true)
                        reservationChange.@private = true;
                    else
                        reservationChange.@private = false;

                    reservationChange.personId = id;
               
                    db.SaveChanges();

                    foreach (var lend in lends)
                    {
                        if (lend.reservationId == reservationChange.id)
                        {
                            lend.carId = reservationChange.carId;
                            lend.personId = reservationChange.personId;
                            lend.lendDate = reservationChange.reservationDate;
                            lend.plannedReturnDate = reservationChange.returnDate;
                            lend.@private = (bool)reservationChange.@private;
                            lend.comments += "\nZmiana w dniu " + DateTime.Now.ToShortDateString();
                        }
                    }

                    db.SaveChanges();

                    MessageBox.Show("Zmodyfikowano rezerwację.", "Komunikat");
                }
                else
                {
                    if (doReservationCar == false)
                        MessageBox.Show("Samochód w tym czasie \njest już zarezerwowany!", "Komunikat");
                    else if (doReservationPerson == false)
                        MessageBox.Show("Pracownik w tym czasie \njest zajęty!", "Komunikat");
                    else
                        MessageBox.Show("Pracownik i samochód w tym czasie \nsą zajęci!", "Komunikat");
                }
            }
            else
                MessageBox.Show("Błędne dane.", "Komunikat");

        }

        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojeRezerwacje();

        }
        private void Function_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Dane_Pojzadu();
        }

        private CalendarDateRange reservationEndBlackoutRange = null;
        private void ReservationStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
            if (ReservationEnd.SelectedDate < ReservationStart.SelectedDate)
                ReservationEnd.SelectedDate = null;
            if (reservationEndBlackoutRange == null) {
                reservationEndBlackoutRange = new CalendarDateRange(DateTime.Today.AddDays(-1), ((DateTime)ReservationStart.SelectedDate).AddDays(-1));
                ReservationEnd.BlackoutDates.Insert(1, reservationEndBlackoutRange);
            }
            else {
                reservationEndBlackoutRange.End = ((DateTime)ReservationStart.SelectedDate).AddDays(0);
                ReservationEnd.BlackoutDates[1] = reservationEndBlackoutRange;
            }
        }
    }
}
