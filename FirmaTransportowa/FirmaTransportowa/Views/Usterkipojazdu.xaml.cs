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
    /// Interaction logic for Usterki_pojazdu.xaml
    /// </summary>
    public partial class Usterkipojazdu : UserControl
    {
        int userPermission = 0;
        Car car1;
        public Usterkipojazdu(int permission, Car car)
        {
            InitializeComponent();
            car1 = car;
            userPermission = permission;
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPojazdu(car1, userPermission);
        }
    }
}
