using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using FirmaTransportowa.ViewModels;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for StatystykiPracownika.xaml
    /// </summary>
    public partial class StatystykiPracownika : UserControl
    {


        public StatystykiPracownika(Person people)
        {
            InitializeComponent();

            ImieNazwisko.Text = people.firstName + " " + people.lastName;
            DataZatrudnienia.Text = people.employmentData.ToString().Substring(0, 10);
            if (people.layoffDate != null)
                DataZwolnienia.Text = people.layoffDate.ToString().Substring(0, 10);
            Login.Text = people.systemLogin;
            if (people.passwordHash != null)
                Haslo.Text = Convert.ToBase64String(people.passwordHash).Substring(0, 8);


            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var activities = db.Activities;
            var carSupervisior = db.CarSupervisors;

            int aktywnosci = 0;
            var lends = db.Lends;
            var cars = db.Cars;
            string textOpiekun = "";
            string bylyOpiekun = "";
            foreach (var carS in carSupervisior)
            {
                if (carS.personId == people.id)
                {
                    foreach (var car in cars)
                        if (car.id == carS.carId && (carS.endDate > DateTime.Today || carS.endDate == null) && (car.saleDate > DateTime.Today || car.saleDate == null))
                        {
                            textOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                        }
                        else if (car.id == carS.carId)
                        {
                            bylyOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                        
                        }
                }
            }
        
            Opiekun.Text = textOpiekun;
            BylyOpiekun.Text = bylyOpiekun;
            

            foreach (var aktyw in activities)
            {
                if (aktyw.reporterId == people.id && aktyw.closeDate > DateTime.Today && aktyw.orderDate < DateTime.Today)
                    aktywnosci++;
            }

            Aktywnosci.Text = aktywnosci.ToString();

            int zleceniaPrywatne = 0;
            int przejechaneKm = 0;
            int zleceniaSluzbowe = 0;
            int przejechaneKmSluzbowe = 0;


            int dni = 0;
            int dniSluzbowe = 0;

            var pojazd = "";
            var pojazdSluzbowy = "";

            //double koszty=0;
           // double kosztySluzbowe = 0;

            foreach (var lend in lends)
            {

                if (lend.personId == people.id && lend.returnDate < DateTime.Today && lend.plannedReturnDate < DateTime.Today) //lend ktore były
                {
                    if (lend.@private == true)
                    {
                        zleceniaPrywatne++;
                        if (lend.endOdometer != null)
                            przejechaneKm += lend.endOdometer.Value - lend.startOdometer;
                        TimeSpan t = (DateTime)lend.returnDate - (DateTime)lend.lendDate;
                        dni += (int)t.TotalDays;
                        
                    }
                    else
                    {
                        zleceniaSluzbowe++;
                        if (lend.endOdometer != null)
                            przejechaneKmSluzbowe += lend.endOdometer.Value - lend.startOdometer;
                        TimeSpan t = (DateTime)lend.returnDate - (DateTime)lend.lendDate;
                        dniSluzbowe += (int)t.TotalDays;
                    }

                }


                foreach (var car in cars)
                {
                    if (lend.carId == car.id && lend.returnDate < DateTime.Today && lend.plannedReturnDate < DateTime.Today && lend.lendDate > DateTime.Today) //aktualny pojazd
                    { //jedno wypozyczenie?
                        if(lend.@private==true)
                            pojazd += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration+"\n";
                        else
                            pojazdSluzbowy += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration+"\n";
                    }
                    //dodanie kosztów zależności różnych kategorii - model typ ...
                }
            }
            Pojazd.Text = pojazd;
            PojazdSluzbowe.Text = pojazdSluzbowy;


            Dni.Text = dni.ToString() +" dni";
            DniSluzbowe.Text = dniSluzbowe.ToString() + " dni";

            Zlecenia.Text = zleceniaPrywatne.ToString();
            Przejechane.Text = przejechaneKm.ToString() + " km";


            PrzejechaneSluzobowe.Text = przejechaneKmSluzbowe.ToString() + " km";
            ZleceniaSluzbowe.Text = zleceniaSluzbowe.ToString();

            Koszty.Text= ((double)(przejechaneKm*4.75)).ToString() + " PLN";
            KosztySluzbowe.Text = ((double)(przejechaneKmSluzbowe * 4.75)).ToString() +" PLN";

        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new PracownicyModel();

        }
    }
}
