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
    /// Logika interakcji dla klasy DodajRezerwacje.xaml
    /// </summary>
    public partial class DodajRezerwacje : UserControl
    {

        
        public DodajRezerwacje()
        {
            InitializeComponent();
            ReservationDate.Text = DateTime.Today.ToString("dd.MM.yyyy");
            ReservationDate.IsReadOnly = true;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var people = db.People;

            foreach (var human in people)
            {
                if(human.layoffDate>DateTime.Now || human.layoffDate==null) //wyswietlamy tych co jeszcze pracuja
                Pracownicy.Items.Add(human.id.ToString()+") "+ human.firstName + " " + human.lastName);
            }
          

            var cars = db.Cars;
            foreach (var car in cars)
            {
                if(car.onService==false)  //gdy w sewisie nie wypożyczamy
                PojazdID.Items.Add(car.id);

            }
            // var aaa = Pojazdy.Text;
            PojazdID.SelectedIndex = 0;
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
                    foreach(var cardes in carDes)
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
         private void ComboBox_TextChanged(object sender, RoutedEventArgs e)
        {
           //PojazdID.Text.
            Dane_Pojzadu();
        }

        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            DateTime temp;
            var reservations = db.Reservations;
            var lends = db.Lends;
            var people = db.People;
            DateTime? datePersonOut = null ;

            foreach (var person in people)
            {
                string name = person.id.ToString()+") "+person.firstName + " " + person.lastName;
               // string check = Pracownicy.Text;
                if (name.Equals(Pracownicy.Text))
                {
                    
                    datePersonOut = person.layoffDate;
                }

            }


            if (!ReservationStart.Text.Equals("") && DateTime.TryParse(ReservationStart.Text, out temp) &&
                !ReservationEnd.Text.Equals("") && DateTime.TryParse(ReservationEnd.Text, out temp) &&
                Convert.ToDateTime(ReservationEnd.Text) > Convert.ToDateTime(ReservationStart.Text) && Convert.ToDateTime(ReservationStart.Text) > DateTime.Now
                && (datePersonOut > Convert.ToDateTime(ReservationEnd.Text) || datePersonOut==null))
                

            {
                var newReservation = new Reservation();
                var newLend = new Lend(); //?


                newReservation.carId = Int16.Parse(PojazdID.Text);
                newReservation.reservationDate = Convert.ToDateTime(ReservationDate.Text);
                newReservation.lendDate = Convert.ToDateTime(ReservationStart.Text);
                newReservation.returnDate = Convert.ToDateTime(ReservationEnd.Text);
               
                    newReservation.ended = true ;
                if (PrywatneBox.IsChecked == true)
                    newReservation.@private = true; 
                else
                    newReservation.@private = false;


               
                foreach (var person in people)
                {
                    string name = person.id.ToString() + ") " + person.firstName + " " + person.lastName;
                    //string check = Pracownicy.Text;
                    if (name.Equals(Pracownicy.Text))
                    {
                        newReservation.personId = person.id; 
                    }

                }

                reservations.Add(newReservation);
                db.SaveChanges();

                newLend.carId = newReservation.carId;
                newLend.personId = newReservation.personId;
                newLend.lendDate = newReservation.reservationDate;
                newLend.plannedReturnDate = newReservation.returnDate;
                newLend.@private = (bool)newReservation.@private;
                newLend.reservationId = newReservation.id;
                newLend.comments = "Zainicjowane przez kierownika";
                lends.Add(newLend);
                db.SaveChanges();

            }
            else
            {
                MessageBox.Show("Złe daty", "Komunikat");
            }

        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new RezerwacjeModel();

        }
    }
}
