﻿using FirmaTransportowa.Model;
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

            this.prevWindow = prevWindow;

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
            if(Rejestracja.Text.Length == 0)
            {
                System.Windows.MessageBox.Show("Nie wprowadzono numeru rejestracyjnego!", "Niepoprawny numer rejestracyjny!",MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Wprowadź datę ważności badania technicznego!","Brak daty badania technicznego", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //DATA WAŻNOŚCI PRZEGLĄDU


            //Dla poprawy juzer ekspiriens (nie pytam 2 razy jak nie wprowadzil modelu i zastosowania)
            if (Model.Text.Length == 0 && Zastosowanie.Text.Length == 0)
            {
                var answer = MessageBox.Show("Nie wprowadzono modelu pojazdu i zastosowania, czy chcesz kontynuować?", "Brak modelu pojazdu i zastosowania!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.No)
                    return;
            }
            else
            {
                //MODEL
                if (Model.Text.Length == 0)
                {
                    var answer = MessageBox.Show("Nie wprowadzono modelu pojazdu, czy chcesz kontynuować?", "Brak modelu pojazdu!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (answer == MessageBoxResult.No)
                        return;
                }

                var carModels = db.CarModels;

                foreach (var carModel in carModels)
                {
                    string fullName = carModel.make + " " + carModel.model;
                    if (fullName.Equals(Model.Text))
                    {
                        newCar.modelId = carModel.id;
                    }
                }
                //MODEL


                //ZASTOSOWANIE
                if (Zastosowanie.Text.Length == 0)
                {
                    var answer = MessageBox.Show("Nie wprowadzono zastosowania, czy chcesz kontynuować?", "Brak zastosowania!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (answer == MessageBoxResult.No)
                        return;
                }

                var carDests = db.CarDestinations;

                foreach (var carDest in carDests)
                {
                    if (carDest.name.Equals(Zastosowanie.Text))
                    {
                        newCar.destinationId = carDest.id;
                    }
                }
                //ZASTOSOWANIE
            }

            cars.Add(newCar);

            //OPIEKUN
            if (!Opiekun.Text.Equals(""))
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
                    if (fullName.Equals(Opiekun.Text))
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
