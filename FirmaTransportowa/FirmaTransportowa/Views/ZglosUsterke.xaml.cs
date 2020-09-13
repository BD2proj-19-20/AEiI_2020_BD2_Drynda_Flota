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
using FirmaTransportowa.ViewModels;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZglosUsterke.xaml
    /// </summary>
    public partial class ZglosUsterke : UserControl
    {
        private Car car1;
        public ZglosUsterke(Car car)
        {
            InitializeComponent();
            car1 = car;
          
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojePojazdyModel();
        }

        private void zglos(object sender, RoutedEventArgs e)
        {
            if (comment.Text.Length == 0)
            {
                MessageBox.Show("Wprowadz opis usterki","Brak opisu usterki");
                return;
            }
            else
            {
                if(Logowanie.actualUser==null)
                {
                    MessageBox.Show("null", "null");
                    return;
                }
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var activities = db.Activities;
                var activity = new Activity();
                activity.critical = (bool)krytyczna.IsChecked;
                activity.comments = comment.Text;
                activity.reportDate = DateTime.Now;
                activity.service = false;
                activity.carId = car1.id;
                activity.reporterId = Logowanie.actualUser.id;
                activities.Add(activity);
                db.SaveChanges();
                System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                glowneOkno.DataContext = new MojePojazdyModel();

            }
        }
    }
}
