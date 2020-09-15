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
    /// Interaction logic for DodanieModeluPojazdu.xaml
    /// </summary>
    public partial class DodanieModeluPojazdu : Window
    {
        DodajPojazd dodajPojazd;
        public DodanieModeluPojazdu(DodajPojazd dodajPojazd)
        {
            InitializeComponent();
            this.dodajPojazd = dodajPojazd;
            Marka.Focus();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddModel(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var CarModels = db.CarModels;

            if (Marka.Text.Length > 0 && Model.Text.Length > 0)
            {
                CarModel newModel = new CarModel();
                newModel.make = Marka.Text;
                newModel.model = Model.Text;
                CarModels.Add(newModel);
                db.SaveChanges();
                dodajPojazd.Model.Items.Add(newModel.make + " " + newModel.model);
                this.Close();
                MessageBox.Show("Pomyślnie dodano nowy model pojazdu.");
            }
            else
            {
                MessageBox.Show("Pola nie mogą być puste!", "Błąd przy dodawaniu!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
