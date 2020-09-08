using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

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

            var cars = db.Cars;
            foreach (var car in cars)
            {

                if(car.onService==false)  //gdy w sewisie nie wypożyczamy
                PojazdID.Items.Add(car.id);

            }
            PojazdID.SelectedIndex = 0;
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
         private void Function_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Dane_Pojzadu();
        }

        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {

            int id = Logowanie.actualUser.id;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            DateTime temp;
            var reservations = db.Reservations;
            var lends = db.Lends;
            var people = db.People;
            DateTime? datePersonOut = null ;
            DateTime? actualCarLendDate = null;
            DateTime? actualCarReturnDate = null;
            Person personReservation=null;
            foreach (var person in people)
            {
               // string check = Pracownicy.Text;
                if (person.id==id)
                {
                    datePersonOut = person.layoffDate;
                    personReservation = person;
                }

            }
            bool doReservationCar = true;
            bool doReservationPerson = true;

                if (!ReservationStart.Text.Equals("") && DateTime.TryParse(ReservationStart.Text, out temp) &&
                    !ReservationEnd.Text.Equals("") && DateTime.TryParse(ReservationEnd.Text, out temp) &&
                    Convert.ToDateTime(ReservationEnd.Text) > Convert.ToDateTime(ReservationStart.Text) && Convert.ToDateTime(ReservationStart.Text) > DateTime.Now.AddDays(-1)
                    && (datePersonOut > Convert.ToDateTime(ReservationEnd.Text) || datePersonOut == null))  //sprawdzanie poprawności danych

                {

                foreach (var reserv in reservations)  //sprawdzanie rezerwacji wybranego pojazdu
                {
                    if (reserv.carId.ToString() == PojazdID.SelectedItem.ToString())
                    {
                        actualCarLendDate = reserv.lendDate;
                        actualCarReturnDate = reserv.returnDate;

                        if (actualCarReturnDate < Convert.ToDateTime(ReservationStart.Text) || (actualCarLendDate > Convert.ToDateTime(ReservationEnd.Text))
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

                        if (actualCarReturnDate < Convert.ToDateTime(ReservationStart.Text) || (actualCarLendDate > Convert.ToDateTime(ReservationEnd.Text))
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
                        var newReservation = new Reservation();
                    var newLend = new Lend(); //?


                    newReservation.carId = Int16.Parse(PojazdID.SelectedItem.ToString());
                    newReservation.reservationDate = Convert.ToDateTime(ReservationDate.Text);
                    newReservation.lendDate = Convert.ToDateTime(ReservationStart.Text);
                    newReservation.returnDate = Convert.ToDateTime(ReservationEnd.Text);
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
                    
                   db.SaveChanges();

                    MessageBox.Show("Dodano rezerwację.", "Komunikat");
                }
                else
                {
                    if(doReservationCar == false)
                    MessageBox.Show("Samochód w tym czasie \njest już zarezerwowany!", "Komunikat");
                    else if  (doReservationPerson == false)
                        MessageBox.Show("Pracownik w tym czasie \njest zajęty!", "Komunikat");
                    else
                        MessageBox.Show("Pracownik i samochód w tym czasie \nsą zajęci!", "Komunikat");
                }
            }
            else
            {
                if(!ReservationEnd.Text.Equals("") && DateTime.TryParse(ReservationEnd.Text, out temp) && 
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
            glowneOkno.DataContext = new MojeRezerwacjeModel();

        }
    }
}
