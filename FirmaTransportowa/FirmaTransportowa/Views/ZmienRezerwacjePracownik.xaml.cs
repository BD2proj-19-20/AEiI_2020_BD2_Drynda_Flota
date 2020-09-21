﻿using FirmaTransportowa.Model;
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

            var reservation = (from reserv in db.Reservations
                               where reserv.id == reservationChange.id
                               select reserv).FirstOrDefault();

            if(reservation!=null)
                    this.reservationChange = reservation;

            PrywatneBox.IsChecked = reservationChange.@private;


            ReservationDate.SelectedDate = reservationChange.reservationDate;
            ReservationStart.BlackoutDates.AddDatesInPast();
            ReservationEnd.BlackoutDates.AddDatesInPast();
            ReservationStart.SelectedDate = reservationChange.lendDate;
            ReservationEnd.SelectedDate = reservationChange.returnDate;

            var people = db.People;

            var query2 = from car in db.Cars
                         where car.onService == false //gdy w serwisie nie wypożyczamy
                         select new
                         {
                             Id = car.id,
                         };

            foreach (var car in query2)
                PojazdID.Items.Add(car.Id.ToString());

            PojazdID.SelectedItem = reservationChange.carId.ToString();
            Dane_Pojzadu();

        }
        private void Dane_Pojzadu()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var car = (from cars in db.Cars
                       where cars.id.ToString() == PojazdID.Text
                       select cars).FirstOrDefault();

            if (car != null)
            {
                Rejestracja.Text = car.Registration;
                PojemnoscSilnika.Text = car.engineCapacity.ToString();
                Marka.Text = car.CarModel.make;
                Model.Text = car.CarModel.model;
                Zastosowanie.Text = car.CarDestination.name;
            }
            else
            {
                Rejestracja.Text = "";
                PojemnoscSilnika.Text = "";
                Marka.Text = "";
                Model.Text = "";
                Zastosowanie.Text = "";
            }

        }
        private void Zmien_Dane_Rezerwacji(object sender, RoutedEventArgs e)
        {

            int id = Logowanie.actualUser.id;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            DateTime? datePersonOut = null;
            DateTime? actualCarLendDate = null;
            DateTime? actualCarReturnDate = null;
            Person personReservation = null;
            var worker = (from persons in db.People
                          where persons.id == id
                          select persons).FirstOrDefault();

            if (worker != null)
            {
                datePersonOut = worker.layoffDate;
                personReservation = worker;
            }
            bool doReservationCar = true;
            bool doReservationPerson = true;

            if (ReservationEnd.SelectedDate != null && datePersonOut < ReservationEnd.SelectedDate)
                MessageBox.Show("Wybrany pracownik zostaje zwolniony\nw czasie nowej rezerwacji.", "Komunikat");
            else if (ReservationStart.SelectedDate != null && ReservationEnd.SelectedDate != null &&
                ReservationEnd.SelectedDate > ReservationStart.SelectedDate) //sprawdzanie poprawności danych
            {

                var query = from reserv in db.Reservations
                            where reserv.carId.ToString() == PojazdID.SelectedItem.ToString() && reserv.id != this.reservationChange.id
                            select new
                            {
                                Id = reserv,
                                CarId = reserv.carId,
                                LendDate = reserv.lendDate,
                                ReturnDate = reserv.returnDate,
                                Ended = reserv.ended,
                            };


                foreach (var reserv in query)
                {
                    actualCarLendDate = reserv.LendDate;
                    actualCarReturnDate = reserv.ReturnDate;

                    if (actualCarReturnDate <= ReservationStart.SelectedDate || (actualCarLendDate >= ReservationEnd.SelectedDate)
                         || (actualCarLendDate == null && actualCarReturnDate == null) || reserv.Ended == true)
                        doReservationCar = true;
                    else
                    {
                        doReservationCar = false;
                        break;
                    }

                }
                var query2 = from reserv in db.Reservations
                             where reserv.personId.ToString() == personReservation.id.ToString() && reserv.id != this.reservationChange.id
                             select new
                             {
                                 Id = reserv,
                                 CarId = reserv.carId,
                                 LendDate = reserv.lendDate,
                                 ReturnDate = reserv.returnDate,
                                 Ended = reserv.ended,
                             };

                foreach (var reserv in query2)
                {

                    actualCarLendDate = reserv.LendDate;
                    actualCarReturnDate = reserv.ReturnDate;

                    if (actualCarReturnDate <= ReservationStart.SelectedDate || (actualCarLendDate >= ReservationEnd.SelectedDate)
                         || (actualCarLendDate == null && actualCarReturnDate == null) || reserv.Ended == true)
                        doReservationPerson = true;
                    else
                    {
                        doReservationPerson = false;
                        break;
                    }

                }

                if (doReservationCar == true && doReservationPerson == true) //sprawdzanie czy samochod jest zareezrwowany w wybranym czasie lub pracownik ma rezerwacje w tym czasie
                {

                    Reservation reservationChange = null;

                    var reservation = (from reserv in db.Reservations
                               where reserv.id == this.reservationChange.id
                                  select reserv).FirstOrDefault();

                        if (reservation!=null)
                            reservationChange = reservation;

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

                    var lend = (from lendd in db.Lends
                                where lendd.reservationId == reservationChange.id
                                select lendd).FirstOrDefault();

                    if (lend != null)
                    {
                        lend.carId = reservationChange.carId;
                        lend.personId = reservationChange.personId;
                        lend.lendDate = reservationChange.lendDate;
                        lend.plannedReturnDate = reservationChange.returnDate;
                        lend.@private = (bool)reservationChange.@private;
                        lend.comments += "\nZmiana w dniu " + DateTime.Now.ToShortDateString();
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
