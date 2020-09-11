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
        private Car car1;
        private int userPermission;
        public RezerwacjePojazdu(int permission, Car car)
        {
            InitializeComponent();
            car1 = car;
            userPermission = permission;
            label.Content = "Data rezerwacji: " + car.Reservations.ElementAt(0).reservationDate + "\nOsoba wypożyczająca"+
                 car.Reservations.ElementAt(0).Person.lastName+" "+car.Reservations.ElementAt(0).Person.lastName+"\nData wypożyczenia"+
                 car.Reservations.ElementAt(0).lendDate+"\nData zwrotu"+
                 car.Reservations.ElementAt(0).returnDate;
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPojazdu(car1, userPermission);
        }
    }
}
