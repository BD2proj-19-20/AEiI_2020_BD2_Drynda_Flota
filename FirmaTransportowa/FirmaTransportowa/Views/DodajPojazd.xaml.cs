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
    /// Interaction logic for DodajPojazd.xaml
    /// </summary>
    public partial class DodajPojazd : UserControl
    {
        public DodajPojazd()
        {
            InitializeComponent();
        }

        private void Dodaj_Pojazd(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            var newCar = new Car();
            newCar.Registration = Rejestracja.Text;
            newCar.purchaseDate = Convert.ToDateTime(DataZakupu.Text);
            newCar.engineCapacity = Int16.Parse(PojemnoscSilnika.Text);
            cars.Add(newCar);

        }
    }
}
