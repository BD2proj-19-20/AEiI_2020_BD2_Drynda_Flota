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
            for (int i = 0; i < car.Reservations.Count; i++)
            {
                this.ListViewReservations.Items.Add(new Reservation
                {
                    Person = car.Reservations.ElementAt(i).Person.firstName + " " + car.Reservations.ElementAt(i).Person.lastName,
                    ReservationStart = car.Reservations.ElementAt(i).lendDate.ToString(),
                    ReservationEnd = car.Reservations.ElementAt(i).reservationDate.ToString(),
                    ReservationDate = car.Reservations.ElementAt(i).returnDate.ToString()
                }) ;
            }


        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPojazdu(car1, userPermission);
        }
    }
}
