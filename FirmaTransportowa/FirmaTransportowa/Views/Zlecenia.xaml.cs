using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
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

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Zlecenia.xaml
    /// </summary>
    public partial class Zlecenia : UserControl
    {
        public class ActivitiesList
        {
            public int activityId { get; set; }
            public string activityComments { get; set; }
        }

        List<ListViewItem> items = new List<ListViewItem>();
        public Zlecenia()
        {
            InitializeComponent(); 
            initializeList();
        }
        private void initializeList()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var activities = db.Activities;
            foreach(var activity in activities)
            {
                ListViewItem OneItem = new ListViewItem();
                OneItem.Content = new ActivitiesList { activityComments = activity.comments };
                items.Add(OneItem);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajZlecenie();
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojePojazdyModel();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
