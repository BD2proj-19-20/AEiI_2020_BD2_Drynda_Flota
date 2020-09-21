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

        private void SerwisowaClick(object sender, RoutedEventArgs e)
        {
            if (this.Serwisowa.IsChecked == true)
                this.Eksploatacyjna.IsChecked = false;
            else
                this.Eksploatacyjna.IsChecked = true;
        }

        private void EksploatacyjnaClick(object sender, RoutedEventArgs e)
        {
            if (this.Eksploatacyjna.IsChecked == true)
                this.Serwisowa.IsChecked = false;
            else
                this.Serwisowa.IsChecked = true;
        }

        private void zlec(object sender, RoutedEventArgs e)
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var activities = db.Activities;
            var cars = db.Cars;

            /* foreach (var activity in activities)
             {
                 if (activity.id == activity1.id)
                 {
                     activity.orderDate = DateTime.Now;
                     if (Logowanie.actualUser != null)
                         activity.contractorId = Logowanie.actualUser.id;
                     else
                         activity.contractorId = 1;
                     if (this.Serwisowa.IsChecked == true)
                         activity.service = true;
                     else
                         activity.service = false;

                 }
             }
             db.SaveChanges();*/


            // for (int i = 0; i < car1.Activities.Count; i++)
            //{
            foreach (var activity in activities)
            {
                if (activity.id == activity1.id)
                {
                    activity.orderDate = DateTime.Now;
                }
            }
            //}
            foreach (var car in cars)
            {
                if (car.id == car1.id)
                {
                    car.onService = true;
                }
            }
            db.SaveChanges();
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Usterkipojazdu(userPermission, car1);

        }
    }
}
