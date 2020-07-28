using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
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
    /// Logika interakcji dla klasy DodajRezerwacje.xaml
    /// </summary>
    public partial class DodajRezerwacje : UserControl
    {
        public DodajRezerwacje()
        {
            InitializeComponent();
            ReservationDate.Text = DateTime.Today.ToString("dd.MM.yyyy");
            ReservationDate.IsReadOnly = true;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var people = db.People;

            foreach (var human in people)
            {
                Pracownicy.Items.Add(human.firstName + " " + human.lastName);
            }
          

            var cars = db.Cars;
            foreach (var car in cars)
            {

                PojazdID.Items.Add(car.id);

            }
            // var aaa = Pojazdy.Text;
            PojazdID.SelectedIndex = 0;
            Dane_Pojzadu();

        }


        private void Dane_Pojzadu()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            foreach (var car in cars)
            {
                if ((car.id).ToString() == PojazdID.Text)
                {
                    Rejestracja.Text = car.Registration;
                    PojemnoscSilnika.Text = car.engineCapacity.ToString();

                    var carmodel = db.CarModels;
                    foreach (var carModel in carmodel)
                    {
                        if (car.modelId == carModel.id)
                        {
                            Marka.Text = car.CarModel.make;
                            Model.Text = car.CarModel.model;
                        }
                    }
                    var carDes = db.CarDestinations;
                    foreach(var cardes in carDes)
                    {
                        if (car.destinationId == cardes.id)
                            Zastosowanie.Text = cardes.name;      
                    }
                }
            }
            Rejestracja.IsReadOnly = true;
            PojemnoscSilnika.IsReadOnly = true;
            Marka.IsReadOnly = true;
            Model.IsReadOnly = true;
            Zastosowanie.IsReadOnly = true;

        }
         private void Pojazd_Change(object sender, RoutedEventArgs e)
        {
            Dane_Pojzadu();
        }

        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {

        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajRezerwacjeModel();

        }
    }
}
