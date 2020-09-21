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
    /// Interaction logic for HistoriaPojazdu.xaml
    /// </summary>

    public partial class HistoriaPojazdu : UserControl
    {
        public class Activity
        {
            public int IDusterki { get; set; }
            public string Opis { get; set; }
            public string Krytyczna { get; set; }
            public string DataZgloszenia { get; set; }
            public string DataSerwisowania { get; set; }
            public string DataNaprawienia { get; set; }
            public int ID { get; set; }
            public String Serwisowana { get; set; }
        }

        private Car car1;
        private int userPermission;
        public HistoriaPojazdu(int permission, Car car)
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
            Title.Content = "Historia usterek pojazdu:" + car1.CarModel.make + " " + car1.CarModel.model + " " + car1.Registration;
            this.ListViewActivities.Items.Clear();
            for (int i = 0; i < car1.Activities.Count; i++)
            {
                if (car1.Activities.ElementAt(i).closeDate != null)
                {
                    this.ListViewActivities.Items.Add(new Activity
                    {
                        IDusterki = car1.Activities.ElementAt(i).id,
                        Opis = car1.Activities.ElementAt(i).comments,
                        Krytyczna = car1.Activities.ElementAt(i).closeDate.ToString(),
                        DataZgloszenia = car1.Activities.ElementAt(i).reportDate.ToString(),
                        DataSerwisowania = car1.Activities.ElementAt(i).orderDate.ToString(),
                        DataNaprawienia = car1.Activities.ElementAt(i).closeDate.ToString(),
                        ID = car1.Activities.ElementAt(i).reporterId,
                        Serwisowana = car1.Activities.ElementAt(i).service.ToString()
                    });
                }
            }
        }
    }
}
