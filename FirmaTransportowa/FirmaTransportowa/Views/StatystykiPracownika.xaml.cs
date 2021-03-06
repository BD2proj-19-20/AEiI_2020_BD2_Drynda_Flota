﻿using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for StatystykiPracownika.xaml
    /// </summary>
    public partial class StatystykiPracownika : UserControl
    {
        Person changePerson;

        public StatystykiPracownika(Person people)
        {
            InitializeComponent();

            changePerson = people;

            if (changePerson.layoffDate <= DateTime.Now) //jeśli pracownik jest zwolniony nie można zmienić danych logownaia
            {
                KierownikPanel.Visibility = Visibility.Hidden;

                KierownikStartPanel.Visibility = Visibility.Hidden;
                KierownikEndPanel.Visibility = Visibility.Hidden;
                zmienLoginButton.Visibility = Visibility.Hidden;
                zmienpassowrdButton.Visibility = Visibility.Hidden;
                zmienKierownikaButton.Visibility = Visibility.Hidden;
                OpiekunPanel.Visibility = Visibility.Hidden; //nie może byc opiekunem zwolniony pracownik
                Thickness margin = BylyOpiekunPanel.Margin;
                margin.Top = margin.Top - 20;
                BylyOpiekunPanel.Margin = margin; //przesuwamy w górę panel z byłymi opiekunami
            }
            else
            {
                ScrolllBylyOpiekun.Height = 40;
                BylyOpiekunText.Height = 40;
                BylyOpiekunPanel.Height = 40;
            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var worker = (from peoplee in db.People
                          where peoplee.id == changePerson.id
                          select peoplee).FirstOrDefault();

            var workerPermission = (from personPermission in db.PeoplesPermissions
                                    where personPermission.personId == changePerson.id
                                    select personPermission).FirstOrDefault();



            var query2 = from supervisor in db.CarSupervisors
                         where worker.id == supervisor.personId
                         join car in db.Cars on supervisor.carId equals car.id
                         select new
                         {
                             BeginDate = supervisor.beginDate == null ? DateTime.MinValue : supervisor.beginDate,
                             EndDate = supervisor.endDate == null ? DateTime.MinValue : supervisor.endDate,
                             SaleDate = car.saleDate == null ? DateTime.MinValue : car.saleDate,
                             CarMake = car.CarModel.make,
                             CarModel = car.CarModel.model,
                             CarRegistration = car.Registration,
                         };


            var query3 = from lends2 in db.Lends
                         where lends2.personId == worker.id
                         select new
                         {
                             LendDate = lends2.lendDate == null ? DateTime.MinValue : lends2.lendDate,
                             EngineCar = lends2.Car.engineCapacity, // == null ? 0 : lends2.Car.engineCapacity,
                             ReservationEnd = lends2.Reservation.ended,
                             ReturnDate = lends2.returnDate == null ? DateTime.MinValue : lends2.returnDate,
                             Private = lends2.@private,
                             StartOdometer = lends2.startOdometer,
                             EndOdometer = lends2.endOdometer,
                             PlannedReturnDate = lends2.plannedReturnDate,
                             LendedCar = lends2.Car
                         };

            ImieNazwisko.Text = worker.firstName + " " + worker.lastName;
            DataZatrudnienia.Text = worker.employmentData.ToString().Substring(0, 10);
            if (worker.layoffDate != null)
                DataZwolnienia.Text = worker.layoffDate.ToString().Substring(0, 10);
            Login.Text = worker.systemLogin;


          //  var carSupervisior = db.CarSupervisors;

            var lends = db.Lends;
            var cars = db.Cars;
            string textOpiekun = "";
            string bylyOpiekun = "";

            foreach (var personSup in query2)

            {
                if ((personSup.EndDate > DateTime.Today || personSup.EndDate == null || personSup.EndDate == DateTime.MinValue) &&
                        (personSup.SaleDate > DateTime.Today || personSup.SaleDate == null || personSup.SaleDate == DateTime.MinValue))
                    textOpiekun += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                else
                    bylyOpiekun += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";

            }

            if (!(changePerson.layoffDate <= DateTime.Now))
                Opiekun.Text = textOpiekun;
            BylyOpiekun.Text = bylyOpiekun;

            Kierownik.Text = "Nie";

            if (workerPermission!=null && workerPermission.Permission.name == "Kierownik" &&
          workerPermission.grantDate <= DateTime.Now.Date && (workerPermission.revokeDate > DateTime.Now || workerPermission.revokeDate == null))
            {
            //    KierownikEndPanel.Visibility = Visibility.Visible;
             //   KierownikStartPanel.Visibility = Visibility.Visible;
                Kierownik.Text = "Tak";
                KierownikDateStart.Text = workerPermission.grantDate.ToString().Substring(0, 10);
                if (workerPermission.revokeDate != null)
                    KierownikDateEnd.Text = workerPermission.revokeDate.ToString().Substring(0, 10);

            }
            else if (workerPermission != null && workerPermission.Permission.name == "Kierownik" && workerPermission.grantDate > DateTime.Now.Date 
                && workerPermission.grantDate < worker.layoffDate)
            {
               // KierownikEndPanel.Visibility = Visibility.Visible;
            //    KierownikStartPanel.Visibility = Visibility.Visible;

                if (((workerPermission.grantDate - DateTime.Now).Days + 1) == 1)
                    Kierownik.Text = "Za " + ((workerPermission.grantDate - DateTime.Now.Date).Days + 1) + " dzień";
                else
                    Kierownik.Text = "Za " + ((workerPermission.grantDate - DateTime.Now.Date).Days + 1) + " dni";
                KierownikDateStart.Text = workerPermission.grantDate.ToString().Substring(0, 10);
                if (workerPermission.revokeDate != null)
                    KierownikDateEnd.Text = workerPermission.revokeDate.ToString().Substring(0, 10);
            }
            else
            {
                KierownikEndPanel.Visibility = Visibility.Hidden;
                KierownikStartPanel.Visibility = Visibility.Hidden;
            }
            int zleceniaPrywatne = 0;
            int przejechaneKm = 0;
            int zleceniaSluzbowe = 0;
            int przejechaneKmSluzbowe = 0;


            int dni = 0;
            int dniSluzbowe = 0;

            var pojazd = "";
            var pojazdSluzbowy = "";

            double koszty = 0;
            double kosztySluzbowe = 0;

            foreach (var personLend in query3)
            {

                if (personLend.ReservationEnd == true && personLend.ReturnDate != null) //lend ktore były
                {
                    if (personLend.Private == true)
                    {
                        zleceniaPrywatne++;
                        if (personLend.EndOdometer != null)
                            przejechaneKm += personLend.EndOdometer.Value - personLend.StartOdometer;
                        if (personLend.ReturnDate > personLend.LendDate)
                        {
                            TimeSpan t = (DateTime)personLend.ReturnDate - personLend.LendDate;
                            dni += (int)t.TotalDays;
                        }
                        if(przejechaneKm>0)
                        koszty = (przejechaneKm * 4.75) + (0.05 * personLend.EngineCar);

                    }
                    else
                    {
                        zleceniaSluzbowe++;
                        if (personLend.EndOdometer != null)
                            przejechaneKmSluzbowe += personLend.EndOdometer.Value - personLend.StartOdometer;
                        if (personLend.ReturnDate > personLend.LendDate)
                        {
                            TimeSpan t = (DateTime)personLend.ReturnDate - personLend.LendDate;
                            dniSluzbowe += (int)t.TotalDays;
                        }
                        if (przejechaneKmSluzbowe > 0)
                            kosztySluzbowe = (przejechaneKmSluzbowe * 4.75) + (0.05 * personLend.EngineCar);
                    }
                }

                if (personLend.LendDate <= DateTime.Today && personLend.PlannedReturnDate > DateTime.Today &&
                    ( personLend.ReturnDate==null || personLend.ReturnDate == DateTime.MinValue) && personLend.ReservationEnd == false) //aktualny pojazd
                {
                    {
                        if (personLend.Private == true)
                            pojazd += personLend.LendedCar.CarModel.make+ "/" + personLend.LendedCar.CarModel.model + "/" + personLend.LendedCar.Registration + "\n";
                        else
                            pojazdSluzbowy += personLend.LendedCar.CarModel.make + "/" + personLend.LendedCar.CarModel.model + "/" + personLend.LendedCar.Registration + "\n";
                    }
                }

            }

            Pojazd.Text = pojazd;
            PojazdSluzbowe.Text = pojazdSluzbowy;


            Dni.Text = dni.ToString() + " dni";
            DniSluzbowe.Text = dniSluzbowe.ToString() + " dni";

            Zlecenia.Text = zleceniaPrywatne.ToString();
            Przejechane.Text = przejechaneKm.ToString() + " km";


            PrzejechaneSluzobowe.Text = przejechaneKmSluzbowe.ToString() + " km";
            ZleceniaSluzbowe.Text = zleceniaSluzbowe.ToString();

            Koszty.Text = koszty.ToString() + " PLN";
            KosztySluzbowe.Text = kosztySluzbowe.ToString() + " PLN";

    }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Pracownicy();

        }
        private void Zmien_haslo(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
         //   int id = changePerson.id;
            ZmienHaslo zmianaView = new ZmienHaslo(changePerson);
            zmianaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            zmianaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            zmianaView.ShowDialog();
            while (zmianaView.IsActive)
            {

            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var person = (from people in db.People
                          where people.id == changePerson.id
                          select people).FirstOrDefault();


            if(person!=null)
                    newPerson = person;


            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPracownika(newPerson);
            return;
        }
        private void Zmien_login(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            ZmienLogin zmianaView = new ZmienLogin(changePerson);
            zmianaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            zmianaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            zmianaView.ShowDialog();
            while (zmianaView.IsActive)
            {

            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var person = (from people in db.People
                          where people.id == changePerson.id
                          select people).FirstOrDefault();


            if (person != null)
                newPerson = person;

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPracownika(newPerson);
            return;
        }
        private void Zmien_Kierownika(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            ZmienKierownika zmianaOpiekunaView = new ZmienKierownika(changePerson);
            zmianaOpiekunaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            zmianaOpiekunaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            zmianaOpiekunaView.ShowDialog();
            while (zmianaOpiekunaView.IsActive)
            {

            }
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var person = (from people in db.People
                          where people.id == changePerson.id
                          select people).FirstOrDefault();


            if (person != null)
                newPerson = person;

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPracownika(newPerson);
            return;
        }
    }
}
