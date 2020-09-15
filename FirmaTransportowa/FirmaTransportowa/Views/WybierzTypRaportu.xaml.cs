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
    /// Interaction logic for WybierzTypRaportu.xaml
    /// </summary>
    public partial class WybierzTypRaportu : UserControl
    {
        ZarzadzajPojazdami prevWindow;
        Car carToRaport;
        public WybierzTypRaportu(ZarzadzajPojazdami prevWindow, Car car)
        {
            InitializeComponent();

            this.prevWindow = prevWindow;
            this.carToRaport = car;
            if(car != null)
            {
                Marka.Visibility = Visibility.Hidden;
                Model.Visibility = Visibility.Hidden;
                Zastosowanie.Visibility = Visibility.Hidden;
                MarkaLabel.Content = MarkaLabel.Content + car.CarModel.make;
                ModelLabel.Content = ModelLabel.Content + car.CarModel.model;
                ZastosowanieLabel.Content = ZastosowanieLabel.Content + car.CarDestination.name;
            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var carModels = db.CarModels;

            foreach (var carModel in carModels)
            {
                Marka.Items.Add(carModel.make);
                Model.Items.Add(carModel.model);
            }

            var carDests = db.CarDestinations;

            foreach (var carDest in carDests)
            {
                Zastosowanie.Items.Add(carDest.name);
            }
        }

        private IQueryable<Car> GetCars()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var CarMake = Marka.Text;
            var CarModel = Model.Text;
            var CarDestination = Zastosowanie.Text;

            var cars = from car in db.Cars
                       select car;

            if (CarMake.Length > 0)
            {
                cars = cars.Where(x => x.CarModel.make.Equals(CarMake));
            }
            if (CarModel.Length > 0)
            {
                cars = cars.Where(x => x.CarModel.model == CarModel);
            }
            if (CarDestination.Length > 0)
            {
                cars = cars.Where(x => x.CarDestination.name.Contains(CarDestination));
            }

            return cars;
        }

        private void CostsRaport(object sender, RoutedEventArgs e)
        {

            var RaportBegin = RaportOd.SelectedDate;
            var RaportEnd = RaportDo.SelectedDate;

            if (carToRaport != null)
            {
                RaportGenerator.GenerateCostsRaportAboutCar(carToRaport, RaportBegin, RaportEnd);
            }
            else
            {
                var cars = GetCars();
                RaportGenerator.GenerateCostsRaportAboutCars(cars, RaportBegin, RaportEnd);
            }
        }

        private void GeneralRaport(object sender, RoutedEventArgs e)
        {
            if (carToRaport != null)
                RaportGenerator.GenerateGeneralRaportAboutOneCar(carToRaport);
            else
            {
                var cars = GetCars();
                RaportGenerator.GenerateGeneralRaportAboutCars(cars);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = prevWindow;
        }
    }
}
