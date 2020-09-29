using System;
using System.Windows;
using System.Windows.Controls;
using FirmaTransportowa.Model;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZglosUsterke.xaml
    /// </summary>
    public partial class ZglosUsterke : UserControl
    {
        int whereGo;
        private Car car1;
        public ZglosUsterke(Car car,int window)
        {
            InitializeComponent();
            car1 = car;
            whereGo = window;
         //   1 dal moje pojazdy , 2 dla mojeWypozyczenia
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            if(whereGo==1)
            glowneOkno.DataContext = new MojePojazdy();
            else if(whereGo==2)
                glowneOkno.DataContext = new MojeWypozyczenia();
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
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var activities = db.Activities;
                var activity = new Activity();
                activity.critical = (bool)krytyczna.IsChecked;
                activity.comments = comment.Text;
                activity.reportDate = DateTime.Now;
                activity.service = false;
                activity.carId = car1.id;
                if (Logowanie.actualUser != null)
                    activity.reporterId = Logowanie.actualUser.id;
                else
                    activity.reporterId = 47;
                activities.Add(activity);
                db.SaveChanges();
                System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                if (whereGo == 1)
                    glowneOkno.DataContext = new MojePojazdy();
                else if (whereGo == 2)
                    glowneOkno.DataContext = new MojeWypozyczenia();

            }
        }
    }
}
