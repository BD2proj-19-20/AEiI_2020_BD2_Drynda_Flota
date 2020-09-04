using FirmaTransportowa.Model;
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
    /// Interaction logic for DodajZlecenie.xaml
    /// </summary>
    public partial class DodajZlecenie : UserControl
    {
        public DodajZlecenie()
        {
            InitializeComponent();
        }

        private void Dodaj_Zlecenie(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var activities = db.Activities;
            var newActivity = new Activity();
            newActivity.comments = Comment.Text;
            activities.Add(newActivity);
            //db.SaveChanges();
        }
    }
}
