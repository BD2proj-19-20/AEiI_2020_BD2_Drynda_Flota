using FirmaTransportowa.Model;
using System;
using System.Windows;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZmianaOpiekuna.xaml
    /// </summary>
    public partial class ZmianaOpiekuna : Window
    {
        Car toChange;
        ItemList itemToChange;
        public ZmianaOpiekuna(Car toChange, ItemList itemToChange)
        {
            InitializeComponent();
            nrRej.Content = toChange.Registration;
            this.toChange = toChange;
            this.itemToChange = itemToChange;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;

            foreach (var human in people)
            {
                if(!(human.layoffDate < DateTime.Now.Date)) // wyświetlamy tych co nie są zwolnieni
                Opiekunowie.Items.Add(human.firstName + " " + human.lastName);
            }
        }

        private void Zmiana_Opiekuna(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            string temp = Opiekunowie.Text;

            if (!temp.Equals(""))
            {
                var carSupervisors = db.CarSupervisors;
                var newSupervisor = new CarSupervisor();

                //Szukam dotychczasowego opiekuna i ustawiam mu date konca
                foreach (var carSupervisor in carSupervisors)
                {
                    if (carSupervisor.carId == toChange.id && carSupervisor.endDate == null)
                    {
                        carSupervisor.endDate = DateTime.Now;
                        break;
                    }
                }

                newSupervisor.carId = toChange.id;
                newSupervisor.beginDate = DateTime.Now;
                newSupervisor.endDate = null;

                var People = db.People;

                foreach (var human in People)
                {
                    string fullName = human.firstName + " " + human.lastName;
                    if (fullName.Equals(temp))
                    {
                        newSupervisor.personId = human.id;
                        newSupervisor.Person = human;
                        itemToChange.carSupervisor = fullName;
                    }
                }

                bool againSupervisor = false;
                foreach (var carSupervisor in carSupervisors)
                {
                    //Sprawdzam czy taki opiekun już istnieje, jeżeli tak, zmieniam jego endDate?
                    if (carSupervisor.personId == newSupervisor.personId && carSupervisor.carId == newSupervisor.carId)
                    {
                        carSupervisor.endDate = null;
                        carSupervisor.beginDate = DateTime.Today;
                        againSupervisor = true;
                        break;
                    }
                }
                if (!againSupervisor)
                    carSupervisors.Add(newSupervisor);
                db.SaveChanges();
            }
            this.Close();
        }

        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
