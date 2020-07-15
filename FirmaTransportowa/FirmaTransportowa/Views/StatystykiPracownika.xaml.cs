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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                Haslo.Text = people.passwordHash.ToString();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var activities = db.Activities;
            var carSupervisior = db.CarSupervisors;

            int aktywnosci = 0;
            var lends = db.Lends;
            var cars = db.Cars;
            string textOpiekun = "";

            foreach (var carS in carSupervisior)
            {
                if (carS.personId == people.id)
                {

                    foreach (var car in cars)
                        if (car.id == carS.carId)
                        {
                            textOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";

                        }
                }

            }
            Opiekun.Text = textOpiekun;


            foreach (var aktyw in activities)
            {
                if (aktyw.reporterId == people.id && aktyw.closeDate > DateTime.Today && aktyw.orderDate < DateTime.Today)
                    aktywnosci++;
            }

            Aktywnosci.Text = aktywnosci.ToString();


            int przejechaneKm = 0;
            foreach (var lend in lends)
            {
                
                if (lend.personId == people.id && lend.returnDate < DateTime.Today && lend.plannedReturnDate < DateTime.Today)
                {
                    Zlecenia.Text = (people.Lends.Count - 1).ToString();
                    przejechaneKm += lend.endOdometer.Value - lend.startOdometer; 
                }
                foreach (var car in cars)
                {
                    if (lend.carId == car.id)
                    {
                        Pojazd.Text = car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration;
                    }

                }
            }
            Przejechane.Text = przejechaneKm.ToString()+" km";



        }
    }
}
