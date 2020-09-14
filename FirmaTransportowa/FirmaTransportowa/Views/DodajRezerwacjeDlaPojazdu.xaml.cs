﻿using FirmaTransportowa.Model;
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
    /// Logika interakcji dla klasy DodajRezerwacjeDlaPojazdu.xaml
    /// </summary>
    public partial class DodajRezerwacjeDlaPojazdu : UserControl
    {
        public DodajRezerwacjeDlaPojazdu(int carId)
        {
            InitializeComponent();
            ReservationStart.BlackoutDates.AddDatesInPast(); //uniemożliwia wybór dat z przeszłości
            ReservationEnd.BlackoutDates.AddDatesInPast(); //uniemożliwia wybór dat z przeszłości
            ReservationDate.SelectedDate = DateTime.Today;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var people = db.People;

            var cars = db.Cars;
            PojazdID.Text = carId.ToString();
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
            PojazdID.IsReadOnly = true;
            Rejestracja.IsReadOnly = true;
            PojemnoscSilnika.IsReadOnly = true;
            Marka.IsReadOnly = true;
            Model.IsReadOnly = true;
            Zastosowanie.IsReadOnly = true;

        }
        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {
            int id = Logowanie.actualUser.id;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var reservations = db.Reservations;
            var lends = db.Lends;
            var people = db.People;
            DateTime? datePersonOut = null;
            DateTime? actualCarLendDate = null;
            DateTime? actualCarReturnDate = null;
            Person personReservation = null;
            foreach (var person in people)
            {
                if (person.id == id)
                {
                    datePersonOut = person.layoffDate;
                    personReservation = person;
                }

            }
            bool doReservationCar = true;
            bool doReservationPerson = true;

            if (ReservationStart != null && ReservationEnd != null && ReservationEnd.SelectedDate > ReservationStart.SelectedDate
                && (datePersonOut > ReservationEnd.SelectedDate || datePersonOut == null))  //sprawdzanie poprawności danych
            {

                foreach (var reserv in reservations)  //sprawdzanie rezerwacji wybranego pojazdu
                {
                    if (reserv.carId.ToString() == PojazdID.Text)
                    {
                        actualCarLendDate = reserv.lendDate;
                        actualCarReturnDate = reserv.returnDate;

                        if (actualCarReturnDate <=  ReservationStart.SelectedDate || (actualCarLendDate >= ReservationEnd.SelectedDate)
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
                    if (reserv.personId.ToString() == personReservation.id.ToString())
                    {
                        actualCarLendDate = reserv.lendDate;
                        actualCarReturnDate = reserv.returnDate;

                        if (actualCarReturnDate <= ReservationStart.SelectedDate || (actualCarLendDate >= ReservationEnd.SelectedDate)
                             || (actualCarLendDate == null && actualCarReturnDate == null) || reserv.ended == true)
                            doReservationPerson = true;
                        else
                        {
                            doReservationPerson = false;
                            break;
                        }
                    }

                }


                if (doReservationCar == true && doReservationPerson == true) //sprawdzanie czy samochod jest zareezrwowany w wybranym czasie lub pracownik ma rezerwacje w tym czasie
                {
                    var newReservation = new Reservation();
                    var newLend = new Lend(); //?


                    newReservation.carId = Int16.Parse(PojazdID.Text);
                    newReservation.reservationDate = ReservationDate.SelectedDate.Value;
                    newReservation.lendDate = ReservationStart.SelectedDate.Value;
                    newReservation.returnDate = ReservationEnd.SelectedDate;
                    newReservation.ended = false;


                    if (PrywatneBox.IsChecked == true)
                        newReservation.@private = true;
                    else
                        newReservation.@private = false;


                    newReservation.personId = id;


                    reservations.Add(newReservation);
                    db.SaveChanges();

                    newLend.carId = newReservation.carId;
                    newLend.personId = newReservation.personId;
                    newLend.lendDate = newReservation.lendDate;
                    newLend.plannedReturnDate = newReservation.returnDate;
                    newLend.@private = (bool)newReservation.@private;
                    newLend.reservationId = newReservation.id;
                    newLend.comments = "Zainicjowane przez kierownika";
                    lends.Add(newLend);

                    newReservation.lendId = newLend.id;

                    db.SaveChanges();

                    MessageBox.Show("Dodano rezerwację.", "Komunikat");
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
            {
                if (ReservationEnd == null && datePersonOut < ReservationEnd.SelectedDate)
                    MessageBox.Show("Wybrany pracownik zostaje zwolniony\nw czasie nowej rezerwacji.", "Komunikat");
                else
                    MessageBox.Show("Błędne dane.", "Komunikat");
            }

        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ListaPojazdow();

        }

        private CalendarDateRange reservationEndBlackoutRange = null;

        private CalendarDateRange dzienKierownictwaEndBlackoutRange2 = null;

        private void ReservationStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReservationEnd.SelectedDate < ReservationStart.SelectedDate)
                ReservationEnd.SelectedDate = null;
            if (reservationEndBlackoutRange == null)
            {
                reservationEndBlackoutRange = new CalendarDateRange(DateTime.Today.AddDays(-1), ((DateTime)ReservationStart.SelectedDate).AddDays(-1));
                ReservationEnd.BlackoutDates.Insert(1, reservationEndBlackoutRange);

                if (Logowanie.actualUser.layoffDate != null) //uwzględnienie daty zwolnienia dla pracownika posiadającą ją
                {
                    dzienKierownictwaEndBlackoutRange2 = new CalendarDateRange(((DateTime)Logowanie.actualUser.layoffDate).AddDays(1),
                       DateTime.MaxValue);
                    ReservationEnd.BlackoutDates.Insert(2, dzienKierownictwaEndBlackoutRange2);

                }
            }
            else
            {
                reservationEndBlackoutRange.End = ((DateTime)ReservationStart.SelectedDate).AddDays(-1);
                ReservationEnd.BlackoutDates[1] = reservationEndBlackoutRange;

                if (Logowanie.actualUser.layoffDate != null) //uwzględnienie daty zwolnienia dla pracownika posiadającą ją
                {
                    dzienKierownictwaEndBlackoutRange2 = new CalendarDateRange(((DateTime)Logowanie.actualUser.layoffDate).AddDays(1),
                       DateTime.MaxValue);
                    ReservationEnd.BlackoutDates.Insert(2, dzienKierownictwaEndBlackoutRange2);

                }
            }
        }
    }
}