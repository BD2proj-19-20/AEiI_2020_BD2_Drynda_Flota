﻿using System;
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
using FirmaTransportowa.Model;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for RezerwacjePojazdu.xaml
    /// </summary>
    public partial class RezerwacjePojazdu : UserControl
    {
        public class Reservation
        {
            public string Person { get; set; }
            public string ReservationStart { get; set; }
            public string ReservationEnd { get; set; }
            public string ReservationDate { get; set; }
        }

        private Car car1;
        private int userPermission;
        public RezerwacjePojazdu(int permission, Car car)
        {
            InitializeComponent();
            car1 = car;
            userPermission = permission;
            Title.Content = "Rezerwacje pojazdu: " + car.CarModel.make + " " + car.CarModel.model +" "+car.Registration;
            loadTable();
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPojazdu(car1, userPermission);
        }

        private void changeAktualne(object sender, RoutedEventArgs e)
        {
            if (AktualneBox.IsChecked == true)
                Title.Content = "Aktualne rezerwacje pojazdu: " + car1.CarModel.make + " " + car1.CarModel.model + " " + car1.Registration;
            else
                Title.Content = "Zakończone rezerwacje pojazdu: " + car1.CarModel.make + " " + car1.CarModel.model + " " + car1.Registration;

            loadTable();
        }
        void loadTable()
        {
            this.ListViewReservations.Items.Clear();
            for (int i = 0; i < car1.Reservations.Count; i++)
            {
               if ((AktualneBox.IsChecked == true && DateTime.Compare((DateTime)car1.Reservations.ElementAt(i).returnDate, DateTime.Now) <0)
                    || (AktualneBox.IsChecked == false))
                {
                    this.ListViewReservations.Items.Add(new Reservation
                    {
                        Person = car1.Reservations.ElementAt(i).Person.firstName + " " + car1.Reservations.ElementAt(i).Person.lastName,
                        ReservationStart = car1.Reservations.ElementAt(i).lendDate.ToString(),
                        ReservationEnd = car1.Reservations.ElementAt(i).reservationDate.ToString(),
                        ReservationDate = car1.Reservations.ElementAt(i).returnDate.ToString()
                    }) ;
                }
            }
        }
    }
}
