using FirmaTransportowa.Model;
using System;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for DodajPojazd.xaml
    /// </summary>
    public partial class DodajPojazd : UserControl
    {
        private ZarzadzajPojazdami prevWindow = null;
        public DodajPojazd(ZarzadzajPojazdami prevWindow)
        {
            InitializeComponent();
            DataZakupu.SelectedDate = DateTime.Today;
            this.prevWindow = prevWindow;

            DataZakupu.SelectedDate = DateTime.Today;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carModels = db.CarModels;

            foreach (var carModel in carModels)
            {
                Model.Items.Add(carModel.make + " " + carModel.model);
            }

            var carDests = db.CarDestinations;

            foreach (var carDest in carDests)
            {
                Zastosowanie.Items.Add(carDest.name);
            }

            var people = db.People;

            foreach (var human in people)
            {
                if (!(human.layoffDate <= DateTime.Now)) // wyświetlamy tych co nie są zwolnieni
                    Opiekunowie.Items.Add(human.firstName + " " + human.lastName);
            }
        }

        private void BuyCar(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            var newCar = new Car();

            //REJESTRACJA
            if (Rejestracja.Text.Length == 0)
            {
                System.Windows.MessageBox.Show("Nie wprowadzono numeru rejestracyjnego!", "Niepoprawny numer rejestracyjny!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(Rejestracja.Text.Length != 4 && Rejestracja.Text.Length != 7)
            {
                System.Windows.MessageBox.Show("Wprowadzony numer ma niepoprawną ilość znaków!", "Niepoprawny numer rejestracyjny!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            newCar.Registration = Rejestracja.Text;

            foreach (var car in cars)
            {
                if (car.Registration.Contains(newCar.Registration))
                {
                    System.Windows.MessageBox.Show("Pojazd z takim numerem rejestracyjnym już istnieje!", "Niepoprawny numer rejestracyjny!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (newCar.Registration.Length > 8)
                {
                    MessageBox.Show("Nieprawidłowa długość numeru rejestracyjnego!", "Niepoprawny numer rejestracyjny!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            //REJESTRACJA


            //POJEMNOŚĆ SILNIKA
            if (PojemnoscSilnika.Text.Length == 0)
            {
                MessageBox.Show("Wprowadź pojemność silnika!", "Nieprawidłowa pojemność silnika!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                newCar.engineCapacity = Int16.Parse(PojemnoscSilnika.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Nieprawidłowy format pojemności silnika!", "Nieprawidłowa pojemność silnika!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //POJEMNOŚĆ SILNIKA


            //DATA ZAKUPU
            if (DataZakupu.SelectedDate != null)
            {
                newCar.purchaseDate = DataZakupu.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Wprowadź datę zakupu!", "Brak daty zakupu!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //DATA ZAKUPU


            //DATA WAŻNOŚCI PRZEGLĄDU
            if (DataPrzegladu.SelectedDate != null)
            {
                newCar.inspectionValidUntil = DataPrzegladu.SelectedDate.Value;
            }
            else
            {
                MessageBox.Show("Wprowadź datę ważności badania technicznego!", "Brak daty badania technicznego", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //DATA WAŻNOŚCI PRZEGLĄDU


            //MODEL
            if (Model.Text.Length == 0)
            {
                MessageBox.Show("Wprowadź model pojazdu!", "Brak modelu pojazdu!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var carModels = db.CarModels;

            bool found = false;
            foreach (var carModel in carModels)
            {
                string fullName = carModel.make + " " + carModel.model;
                if (fullName.Contains(Model.Text))
                {
                    newCar.modelId = carModel.id;
                    found = true;
                }
            }

            if(found == false)
            {
                MessageBox.Show("Wprowadzony model pojazdu nie istnieje!", "Brak takiego modelu pojazdu!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //MODEL


            //ZASTOSOWANIE
            if (Zastosowanie.Text.Length == 0)
            {
                MessageBox.Show("Wprowadź zastosowanie pojazdu!", "Brak zastosowania pojazdu!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var carDests = db.CarDestinations;

            found = false;
            foreach (var carDest in carDests)
            {
                if (carDest.name.Contains(Zastosowanie.Text))
                {
                    newCar.destinationId = carDest.id;
                    found = true;
                }
            }

            if (found == false)
            {
                MessageBox.Show("Wprowadzone zastosowanie pojazdu nie istnieje!", "Brak takiego zastosowania pojazdu!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //ZASTOSOWANIE


            cars.Add(newCar);

            //OPIEKUN
            if (!Opiekunowie.Text.Equals(""))
            {
                var carSupervisors = db.CarSupervisors;

                var newSupervisor = new CarSupervisor();
                newSupervisor.carId = newCar.id;
                newSupervisor.beginDate = DateTime.Today;
                newSupervisor.endDate = null;

                var People = db.People;

                foreach (var human in People)
                {
                    string fullName = human.firstName + " " + human.lastName;
                    if (fullName.Equals(Opiekunowie.Text))
                    {
                        newSupervisor.personId = human.id;
                        newSupervisor.Person = human;
                    }
                }
                carSupervisors.Add(newSupervisor);
            }
            //OPIEKUN

            db.SaveChanges();

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = prevWindow;
        }
    }
}
