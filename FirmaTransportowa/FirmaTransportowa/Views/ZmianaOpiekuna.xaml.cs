using FirmaTransportowa.Model;
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
using System.Windows.Shapes;

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
                    if (carSupervisor.carId == toChange.id)
                    {
                        carSupervisor.endDate = DateTime.Today;
                    }
                }

                newSupervisor.carId = toChange.id;
                newSupervisor.beginDate = DateTime.Today;
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
                //The conversion of a datetime2 data type to a smalldatetime data type resulted in an out-of-range value.
            }
            this.Close();
        }

        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
