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
            // if(!carSupervisiorChange.Equals(null))
            DataZwolnieniaPracownika.Text = DateTime.Today.ToString("dd.MM.yyyy");
            
            this.personChange = personChange;

        }

        private void Dodaj_Zwolnienie(object sender, RoutedEventArgs e)
        {
            DateTime temp;
            if (!DataZwolnieniaPracownika.Text.Equals("") && (DateTime.TryParse(DataZwolnieniaPracownika.Text, out temp)))
            { 
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

                var carSupervisors = db.CarSupervisors;
                // var newSupervisor = new CarSupervisor();


                foreach (var carS in carSupervisors)
                {
                    if (personChange.id == carS.personId)
                    {
                        
                        carS.endDate = Convert.ToDateTime(DataZwolnieniaPracownika.Text);
                    }
                }
                var people = db.People;

                foreach (var person in people)
                {
                    if (person.id == personChange.id)
                    {
                        person.layoffDate = Convert.ToDateTime(DataZwolnieniaPracownika.Text);

                    }
                }
                var reservation = db.Reservations;
                foreach( var res in reservation)
                {
                    if(res.personId == personChange.id && res.ended == false)
                    {  

                        if(res.returnDate > Convert.ToDateTime(DataZwolnieniaPracownika.Text))
                         res.ended = true; //zakańczamy rezerwację przy zwolnienu pracownika zostawiamy daty startu i końca

                    }

                }
                var lends = db.Lends;
                foreach (var lend in lends)
                {
                    if(lend.personId==personChange.id )
                    {
                        lend.returnDate = Convert.ToDateTime(DataZwolnieniaPracownika.Text);
                        lend.plannedReturnDate = Convert.ToDateTime(DataZwolnieniaPracownika.Text);
                        lend.comments = "Zakończono przez zwolnienie pracownika - "+ DateTime.Now.ToString();
                    }

                }

                db.SaveChanges();
            }
            else
            {
                MessageBox.Show("Zła data zwolnienia!", "Komunikat");
            }
            this.Close();
        }
        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
