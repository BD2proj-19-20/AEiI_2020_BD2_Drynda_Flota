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
using System.Windows.Shapes;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for DodajZastosowaniePojazdu.xaml
    /// </summary>
    public partial class DodajZastosowaniePojazdu : Window
    {
        DodajPojazd dodajPojazd;
        public DodajZastosowaniePojazdu(DodajPojazd dodajPojazd)
        {
            this.dodajPojazd = dodajPojazd;
            InitializeComponent();
            Zastosowanie.Focus();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddDestination(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var CarDestinations = db.CarDestinations;
           
            if (Zastosowanie.Text.Length > 0)
            {
                CarDestination newDestination = new CarDestination();
                newDestination.name = Zastosowanie.Text;
                CarDestinations.Add(newDestination);
                db.SaveChanges();
                dodajPojazd.Zastosowanie.Items.Add(newDestination.name);
                this.Close();
                MessageBox.Show("Pomyślnie dodano nowe zastosowanie pojazdu.");
            }
            else
            {
                MessageBox.Show("Pole nie może być puste!", "Błąd przy dodawaniu!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
