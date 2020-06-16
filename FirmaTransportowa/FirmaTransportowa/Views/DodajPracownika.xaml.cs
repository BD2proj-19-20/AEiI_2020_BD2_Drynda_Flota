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
    /// Logika interakcji dla klasy DodajPracownika.xaml
    /// </summary>
    public partial class DodajPracownika : UserControl
    {
        public DodajPracownika()
        {
            InitializeComponent();

        }

       
        private void Dodaj_Pracownika(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var workers = db.People;
            var newWorker = new Person();

            newWorker.firstName = Imie.Text;
            newWorker.lastName = Nazwisko.Text;
            newWorker.systemLogin = Login.Text;
            newWorker.employmentData = Convert.ToDateTime(DzienZatrudnienia.Text);
            //newWorker.passwordHash = Hasło.GetHashCode(); 


            workers.Add(newWorker);
            db.SaveChanges();

        }

    }
}
