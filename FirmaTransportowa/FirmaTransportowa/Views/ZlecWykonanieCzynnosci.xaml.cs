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
    /// Interaction logic for ZlecWykonanieCzynnosci.xaml
    /// </summary>
    public partial class ZlecWykonanieCzynnosci : UserControl
    {
        private int userPermission;
        private Car car1;
        private Activity activity1;
        public ZlecWykonanieCzynnosci(int permission, Car car, Activity activity)
        {
            InitializeComponent();
            userPermission = permission;
            car1 = car;
            activity1 = activity;
            this.Title.Content = "Zleć wykonanie czynności dla samochodu: "+ car1.CarModel.make+ " "+car1.CarModel.model + " " + car1.Registration + "\n"
                + "Usterka: " + activity1.comments;
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Usterkipojazdu(userPermission,car1);
        }
    }
}
