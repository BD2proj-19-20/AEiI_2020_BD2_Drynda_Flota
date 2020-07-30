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

                    this.reservationChange = reserv;

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
        private void Zmien_Dane_Rezerwacji(object sender, RoutedEventArgs e)
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            DateTime temp;
            var reservations = db.Reservations;
            var lends = db.Lends;
            var people = db.People;
            DateTime? datePersonOut = null;
            DateTime? actualCarLendDate = null;
            DateTime? actualCarReturnDate = null;

            foreach (var person in people)
            {
                string name = person.id.ToString() + ") " + person.firstName + " " + person.lastName;
                // string check = Pracownicy.Text;
                if (name.Equals(Pracownicy.Text))
                {
                    datePersonOut = person.layoffDate;
                }

            }
            bool doReservation = true;


            if (!ReservationStart.Text.Equals("") && DateTime.TryParse(ReservationStart.Text, out temp) &&
                !ReservationEnd.Text.Equals("") && DateTime.TryParse(ReservationEnd.Text, out temp) &&
                Convert.ToDateTime(ReservationEnd.Text) > Convert.ToDateTime(ReservationStart.Text) && Convert.ToDateTime(ReservationStart.Text) > DateTime.Now.AddDays(-1)
                && (datePersonOut > Convert.ToDateTime(ReservationEnd.Text) || datePersonOut == null))  //sprawdzanie poprawności danych

            {

                foreach (var reserv in reservations)
                {
                    if (reserv.carId.ToString() == PojazdID.SelectedItem.ToString() && reserv.id != this.reservationChange.id) //nie obchodzi nas zmieniana rezerwacja
                    {
                        actualCarLendDate = reserv.lendDate;
                        actualCarReturnDate = reserv.returnDate;

                        if (actualCarReturnDate < Convert.ToDateTime(ReservationStart.Text) || (actualCarLendDate > Convert.ToDateTime(ReservationEnd.Text))
                             || (actualCarLendDate == null && actualCarReturnDate == null) || reserv.ended == true)
                        {
                            doReservation = true;
                        }
                        else
                        {
                            doReservation = false;
                            break;
                        }
                    }

                }
                if (doReservation == true) //sprawdzanie czy samochod jest zareezrwowany w wybranym czasie 
                {
                    //var newReservation = new Reservation();
                    //var newLend = new Lend(); //?
                    Reservation reservationChange = null;
                    foreach (var reserv in reservations)
                    {
                        if(reserv.id==this.reservationChange.id)
                        {
                            reservationChange = reserv;
                        }
                    }

                    reservationChange.carId = Int16.Parse(PojazdID.SelectedItem.ToString());
                    reservationChange.reservationDate = Convert.ToDateTime(ReservationDate.Text);
                    reservationChange.lendDate = Convert.ToDateTime(ReservationStart.Text);
                    reservationChange.returnDate = Convert.ToDateTime(ReservationEnd.Text);
                    reservationChange.ended = false;

                    if (PrywatneBox.IsChecked == true)
                        reservationChange.@private = true;
                    else
                        reservationChange.@private = false;



                    foreach (var person in people)
                    {
                        string name = person.id.ToString() + ") " + person.firstName + " " + person.lastName;

                        if (name.Equals(Pracownicy.Text))
                        {
                            reservationChange.personId = person.id;
                        }

                    }
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
                            //lend.reservationId = reservationChange.id;
                            lend.comments += "\nZmiana w dniu " + DateTime.Now.ToShortDateString();
                        }
                    }

                    db.SaveChanges();

                    MessageBox.Show("Zmodyfikowano rezerwację.", "Komunikat");
                }
                else
                {
                    MessageBox.Show("Samochód w tym czasie \njest już zarezerwowany!", "Komunikat");
                }
            }
            else
            {
                if (!ReservationEnd.Text.Equals("") && DateTime.TryParse(ReservationEnd.Text, out temp) &&
                    !ReservationEnd.Text.Equals("") && DateTime.TryParse(ReservationEnd.Text, out temp) &&
                    datePersonOut < Convert.ToDateTime(ReservationEnd.Text))
                    MessageBox.Show("Wybrany pracownik zostaje zwolniony\nw czasie nowej rezerwacji.", "Komunikat");
                else
                    MessageBox.Show("Błędne dane.", "Komunikat");
            }

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
