using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Statystyki.xaml
    /// </summary>
    public partial class StatystykiPojazdu : UserControl
    {
        int permission = 0;
        Car car1;
        public StatystykiPojazdu(Car car, int userPermission)
        {
            InitializeComponent();

            if (userPermission != 2)
                DataSprzedazy.Visibility = Visibility.Hidden;

            permission = userPermission;
            car1 = car;
            Rejestracja.Text = car.Registration;
            Pojemnosc_silnika.Text = car.engineCapacity.ToString();

            string saleDate = "";
            string purchaseDate = "";
            if (car.saleDate != null)
            {
                DateTime temp = (DateTime)car.saleDate;
                saleDate = temp.ToShortDateString();
            }
            if (car.purchaseDate != null)
            {
                DateTime temp2 = (DateTime)car.purchaseDate;
                purchaseDate = temp2.ToShortDateString();
            }
            Data_zakupu.Text = purchaseDate;
            Data_sprzedaży.Text = saleDate;
            Przeglad.Text = car.inspectionValidUntil.ToShortDateString();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();


            var carModel = (from carModell in db.CarModels
                            where carModell.id == car.modelId
                            select carModell).FirstOrDefault();


            if (carModel != null)
            {
                ModelPojazdu.Text = carModel.make + " " + carModel.model;
                Marka.Text = carModel.make;
                Model.Text = carModel.model;
            }

            Zastosowanie.Text = car.CarDestination.name;


            CalendarDateRange reservationBlackoutRange = null;

            var reservations = car.Reservations;
            int i = 0;

            var query = from reserv in car.Reservations
                        select new
                        {
                            PersonId = reserv.personId,
                            Ended = reserv.ended,
                            LendDate = reserv.lendDate,
                            ReturnDate = reserv.returnDate,
                        };

            foreach (var reservation in query)
            {
                if (reservation.Ended == false)
                {
                    if (reservation.PersonId == Logowanie.actualUser.id && permission != 2)
                        continue;
                    reservationBlackoutRange = new CalendarDateRange(((DateTime)reservation.LendDate).AddDays(-1), ((DateTime)reservation.ReturnDate));
                    Calendar.BlackoutDates.Insert(i, reservationBlackoutRange);
                    i++;
                }
            }

            var query2 = from lend in car.Lends
                         where lend.endOdometer!=null
                         select new
                         {
                             StartOdometer = lend.startOdometer,
                             EndOdometer = lend.endOdometer,
                         };
            int odometerCounter = 0;
            foreach (var lend in query2)
            {
                   odometerCounter = (int)lend.EndOdometer - lend.StartOdometer;
            }
            Przebieg.Text = odometerCounter + " km";
        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            switch (permission)
            {
                case 1:
                    glowneOkno.DataContext = new ListaPojazdow();
                    break;
                case 2:
                    glowneOkno.DataContext = new ZarzadzajPojazdamiModel();
                    break;
                case 3:
                    glowneOkno.DataContext = new MojePojazdyModel();
                    break;
                default:
                    break;
            }
        }

        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            RaportGenerator.GenerateGeneralRaportAboutOneCar(car1);
        }

        private void rezerwacjeClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new RezerwacjePojazdu(permission, car1);
        }

        private void historiaClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new HistoriaPojazdu(permission, car1);
        }

    }
}
