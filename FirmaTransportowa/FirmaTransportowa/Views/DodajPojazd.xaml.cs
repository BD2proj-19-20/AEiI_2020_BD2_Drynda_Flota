using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;

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
            string temp = Zastosowanie.Text;

            foreach(var carDest in carDests)
            {
                if(carDest.name.Equals(temp))
                {
                    newCar.destinationId = carDest.id;
                }
            }

            var carModels = db.CarModels;
            temp = Model.Text;

            foreach (var carModel in carModels)
            {
                string fullName = carModel.make + " " + carModel.model;
                if (fullName.Equals(temp))
                {
                    newCar.modelId = carModel.id;
                }
            }

            cars.Add(newCar);
            db.SaveChanges();

            temp = Opiekunowie.Text;
            if (!temp.Equals(""))
            {
                var carSupervisors = db.CarSupervisors;

                var newSupervisor = new CarSupervisor();
                newSupervisor.carId = newCar.id;
                newSupervisor.beginDate = Convert.ToDateTime(DataZakupu.Text);
                newSupervisor.endDate = Convert.ToDateTime(DataZakupu.Text);

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

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }

        private void Anuluj(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }
    }
}
