using System;
using System.Windows;
using FirmaTransportowa.Model;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Logika interakcji dla klasy DataZwolnienia.xaml
    /// </summary>
    public partial class DataZwolnienia : Window
    {
        Person personChange;

        public DataZwolnienia(Person personChange)
        {
            InitializeComponent();
            Kalendarz.BlackoutDates.AddDatesInPast();
            this.personChange = personChange;

        }

        private void Dodaj_Zwolnienie(object sender, RoutedEventArgs e)
        {
          
            if (Kalendarz.SelectedDate != null)
            { 
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var carSupervisors = db.CarSupervisors;
                // var newSupervisor = new CarSupervisor();


                foreach (var carS in carSupervisors)
                {
                    if (personChange.id == carS.personId)
                        carS.endDate = Kalendarz.SelectedDate;
                }
                var people = db.People;

                foreach (var person in people)
                {
                    if (person.id == personChange.id)
                        person.layoffDate = Kalendarz.SelectedDate;
                }
                var reservation = db.Reservations;
                foreach( var res in reservation)
                {
                    if(res.personId == personChange.id && res.ended == false)
                        if(res.returnDate > Kalendarz.SelectedDate)
                         res.ended = true; //zakańczamy rezerwację przy zwolnienu pracownika zostawiamy daty startu i końca

                }
                var lends = db.Lends;
                foreach (var lend in lends)
                {
                    if(lend.personId==personChange.id )
                    {
                        lend.returnDate = Kalendarz.SelectedDate;
                        lend.plannedReturnDate = Kalendarz.SelectedDate;
                        lend.comments = "Zakończono przez zwolnienie pracownika - "+ DateTime.Now.ToString(); //na pewno Now, a nie data zwolnienia?
                    }

                }
                var permissions = db.PeoplesPermissions;
                foreach (var permission in permissions)
                {
                    if (permission.personId == personChange.id && Kalendarz.SelectedDate > permission.grantDate)
                        permission.revokeDate = Kalendarz.SelectedDate; //zamykamy wszyskie uprawnienia pracownika
                }
                MessageBox.Show("Ustawiono zwolnienie!", "Komunikat");
                db.SaveChanges();
            }
            else
                MessageBox.Show("Zła data zwolnienia!", "Komunikat");
            this.Close();
        }
        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
