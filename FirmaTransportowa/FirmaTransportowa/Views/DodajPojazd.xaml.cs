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
    /// Interaction logic for DodajPojazd.xaml
    /// </summary>
    public partial class DodajPojazd : UserControl
    {
        public DodajPojazd()
        {
            InitializeComponent();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carModels = db.CarModels;

            foreach(var carModel in carModels)
            {
                Model.Items.Add(carModel.make+" "+carModel.model);
            }

            var carDests = db.CarDestinations;

            foreach(var carDest in carDests)
            {
                Zastosowanie.Items.Add(carDest.name);
            }

            var people = db.People;

            foreach(var human in people)
            {
                Opiekunowie.Items.Add(human.firstName + " " + human.lastName);
            }
        }

        private void Dodaj_Pojazd(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            var newCar = new Car();

            newCar.Registration = Rejestracja.Text;
            newCar.purchaseDate = Convert.ToDateTime(DataZakupu.Text);
            newCar.inspectionValidUntil = Convert.ToDateTime(DataZakupu.Text);
            newCar.engineCapacity = Int16.Parse(PojemnoscSilnika.Text);


            var carDests = db.CarDestinations;

            newCar.destinationId = 0;

            string temp = Zastosowanie.Text;

            foreach(var carDest in carDests)
            {
                if(carDest.name.Equals(temp))
                {
                    newCar.destinationId = carDest.id;
                }
            }

            var carModels = db.CarModels;

            newCar.modelId = 51;

            cars.Add(newCar);
            db.SaveChanges();

        }
    }
}
