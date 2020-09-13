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
            public string Opis { get; set; }
            public string Krytyczna { get; set; }
            public string DataZgloszenia { get; set; }
            public int ID { get; set; }
        }
        int userPermission = 0;
        Car car1;
        public Usterkipojazdu(int permission, Car car)
        {
            InitializeComponent();
            car1 = car;
            userPermission = permission;
            loadTable();
        }

        private void cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPojazdu(car1, userPermission);
        }
        private void loadTable()
        {

           Title.Content = "Usterki pojazdu: " + car1.CarModel.make + " " + car1.CarModel.model + " " + car1.Registration;
            this.ListViewActivities.Items.Clear();
            String czyKrytyczna;
            for (int i = 0; i < car1.Activities.Count; i++)
            {
                if (car1.Activities.ElementAt(i).critical)
                    czyKrytyczna = "Tak";
                else
                    czyKrytyczna = "Nie";
                this.ListViewActivities.Items.Add(new Activity
                {
                    Opis = car1.Activities.ElementAt(i).comments,
                    Krytyczna = czyKrytyczna,
                    DataZgloszenia = car1.Activities.ElementAt(i).reportDate.ToString(),
                    ID = car1.Activities.ElementAt(i).reporterId,
                }) ; 
                
            }
        }
    }
}
