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
using FirmaTransportowa.ViewModels;
using System.Security.Cryptography;

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
            DzienZatrudnienia.Text = DateTime.Today.ToString("dd.MM.yyyy");
        }

        public byte[] getHash(string password)
        {
            byte[] passwordSalt = { 67,128,62,208,147,77,143,197 };

            using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, passwordSalt))
            {
                hashGenerator.IterationCount = 1001;
                return hashGenerator.GetBytes(8);
            }
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
            newWorker.passwordHash = getHash(Hasło.Text);

            workers.Add(newWorker);
            db.SaveChanges();
            MessageBox.Show("Dodano Pracownika: " + Imie.Text + " " + Nazwisko.Text,"Komunikat");

        }

        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new PracownicyModel();

        }
    }
}
