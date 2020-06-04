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
        public ZmianaOpiekuna(Car toChange)
        {
            InitializeComponent();
            nrRej.Content = toChange.Registration;
            this.toChange = toChange;

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
                DateTime today = DateTime.Today;

                var carSupervisors = db.CarSupervisors;
                var newSupervisor = new CarSupervisor();

                foreach (var carSupervisor in carSupervisors)
                {
                    if(carSupervisor.carId == toChange.id)
                    {
                        carSupervisor.endDate = today;
                    }
                }

                newSupervisor.carId = toChange.id;
                newSupervisor.beginDate = today;
                newSupervisor.endDate = today;

                var People = db.People;

                foreach (var human in People)
                {
                    string fullName = human.firstName + " " + human.lastName;
                    if (fullName.Equals(temp))
                    {
                        newSupervisor.personId = human.id;
                        newSupervisor.Person = human;
                    }
                }
                carSupervisors.Add(newSupervisor);
            }
            db.SaveChanges();
            this.Close();
        }

        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
