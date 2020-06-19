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
using FirmaTransportowa.Model;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Logika interakcji dla klasy DataZwolnienia.xaml
    /// </summary>
    public partial class DataZwolnienia : Window
    {

        CarSupervisor carSupervisiorChange;

        Person personChange;

        //ItemList itemToChange;
        public DataZwolnienia(CarSupervisor carSupervisiorChange,Person personChange)
        {
            InitializeComponent();
           // if(!carSupervisiorChange.Equals(null))
            this.carSupervisiorChange = carSupervisiorChange;
            this.personChange = personChange;

        }

        private void Dodaj_Zwolnienie(object sender, RoutedEventArgs e)
        { 
            if(!DataZwolnieniaPracownika.Text.Equals(""))
            {
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

                var carSupervisors = db.CarSupervisors;
                // var newSupervisor = new CarSupervisor();

                if (!(carSupervisiorChange is null))
                {
                    foreach (var carSupervisor in carSupervisors)
                    {
                        if (carSupervisor.carId == carSupervisiorChange.carId)
                        {
                            carSupervisor.endDate = Convert.ToDateTime(DataZwolnieniaPracownika.Text);

                        }
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
