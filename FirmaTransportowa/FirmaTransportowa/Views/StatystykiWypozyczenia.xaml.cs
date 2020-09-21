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
    /// Logika interakcji dla klasy StatystykiWypozyczenia.xaml
    /// </summary>
    public partial class StatystykiWypozyczenia : UserControl
    {
        Lend lendChange;
        bool bossCheck;
        public StatystykiWypozyczenia(Lend lendChange, bool bossBool)
        {
            InitializeComponent();
            this.bossCheck = bossBool;
            this.lendChange = lendChange;
            carName.Content = this.lendChange.Car.CarModel.make + " " + this.lendChange.Car.CarModel.model;
            lendId.Content = (this.lendChange.id + 1).ToString();
            UpdateView();

        }
        public void UpdateView()
        {
           
            startOdometer.Text = lendChange.startOdometer.ToString();

            if (lendChange.endOdometer != null)
                endOdometer.Text = lendChange.endOdometer.ToString();

            startFuel.Text = lendChange.startFuel.ToString();

            if (lendChange.endFuel != null)
                endFuel.Text = lendChange.endFuel.ToString();


        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            if(this.bossCheck==false)
            glowneOkno.DataContext = new MojeWypozyczenia();
            else
                glowneOkno.DataContext = new Wypozyczenia();
        }
        private void Update_statistic(object sender, RoutedEventArgs e)
        {
            int n;
            if (
                (int.TryParse(startOdometer.Text, out n) && int.TryParse(endOdometer.Text, out n)
                && int.TryParse(startFuel.Text, out n) && int.TryParse(endFuel.Text, out n))
                && Int32.Parse(startOdometer.Text) < Int32.Parse(endOdometer.Text) && Int32.Parse(startFuel.Text) < Int32.Parse(endFuel.Text))

            {
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                //var lends = db.Lends;


                var lend  = (from lendd in db.Lends
                              where lendd.id == lendChange.id
                             select lendd).FirstOrDefault();

                if (lend != null)
                {
                    lend.startOdometer = Int32.Parse(startOdometer.Text);
                    lend.endOdometer = Int32.Parse(endOdometer.Text);
                    lend.startFuel = Int32.Parse(startFuel.Text);
                    lend.endFuel = Int32.Parse(endFuel.Text);
                    this.lendChange = lend;
                }

                db.SaveChanges();
                MessageBox.Show("Wpisanie danych powiodło się", "Komunikat");
                UpdateView();
            }
            else
                MessageBox.Show("Błędne dane.", "Komunikat");
        }
    }
}
