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
        public class Activity
        {
            public int IDusterki { get; set; }
            public string Opis { get; set; }
            public string Krytyczna { get; set; }
            public string DataZgloszenia { get; set; }
            public string DataSerwisowania { get; set; }
            public int ID { get; set; }
            public String Serwisowana { get; set; }

        }
        int userPermission = 0;
        Car car1;
        public Usterkipojazdu(int permission, Car car)
        {
            InitializeComponent();
            car1 = car;
            userPermission = permission;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            foreach (var car3 in cars)
            {
                if (car3.id == car1.id)
                {
                    car1 = car3;
                    break;
                }
            }
            loadTable();
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojePojazdy();
        }
        private void loadTable()
        {

            Title.Content = "Usterki pojazdu: " + car1.CarModel.make + " " + car1.CarModel.model + " " + car1.Registration;
            this.ListViewActivities.Items.Clear();
            String czyKrytyczna, czySerwisowana;
            for (int i = 0; i < car1.Activities.Count; i++)
            {
                if (car1.Activities.ElementAt(i).closeDate == null)
                {
                    if (car1.Activities.ElementAt(i).critical)
                        czyKrytyczna = "Tak";
                    else
                        czyKrytyczna = "Nie";
                    if (car1.Activities.ElementAt(i).orderDate == null)
                        czySerwisowana = "Nie";
                    else
                        czySerwisowana = "Tak";
                    this.ListViewActivities.Items.Add(new Activity
                    {
                        IDusterki = car1.Activities.ElementAt(i).id,
                        Opis = car1.Activities.ElementAt(i).comments,
                        Krytyczna = czyKrytyczna,
                        DataZgloszenia = car1.Activities.ElementAt(i).reportDate.ToString(),
                        DataSerwisowania = car1.Activities.ElementAt(i).orderDate.ToString(),
                        ID = car1.Activities.ElementAt(i).reporterId,
                        Serwisowana = czySerwisowana

                    });
                }
            }
        }

        private void zlec(object sender, RoutedEventArgs e)
        {
            Activity selected = (Activity)ListViewActivities.SelectedItem;
            if (selected != null)
            {

                int selectedId = selected.IDusterki;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var activities = db.Activities;

                foreach (var activity in activities)
                {
                    if (activity.id == selectedId)
                    {
                        System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                        glowneOkno.DataContext = new ZlecWykonanieCzynnosci(userPermission, car1, activity);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano usterki!", "Komunikat");
            }
        }
        private void fixed_button(object sender, RoutedEventArgs e)
        {
            Activity selected = (Activity)ListViewActivities.SelectedItem;
            if (selected != null)
            {
                int selectedId = selected.IDusterki;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var activities = db.Activities;
                var cars = db.Cars;
                foreach (var activity in activities)
                {
                    if (activity.id == selectedId)
                    {
                        if (activity.orderDate != null)
                        {
                            activity.closeDate = DateTime.Now;
                            /*foreach (var car in cars)
                            {
                                if(car.id == car1.id)
                                {
                                    car.Activities.Count = 0;
                                }
                            }*/
                        }
                        else
                        {
                            MessageBox.Show("Jeszcze nie serwisowana!", "Komunikat");
                        }
                    }
                }
                db.SaveChanges();
                loadTable();
            }
            else
            {
                MessageBox.Show("Nie wybrano usterki!", "Komunikat");
            }
        }
    }
}
