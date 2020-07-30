using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;
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

            DateTime temp;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var workers = db.People;
            var newWorker = new Person();


            if (Imie.Text.Length >= 3 && Nazwisko.Text.Length >= 3 && Login.Text.Length >= 6 && Hasło.Text.Length >= 6)
            {
                if (!DzienZatrudnienia.Text.Equals("") && (DateTime.TryParse(DzienZatrudnienia.Text, out temp)))
                {
                    newWorker.firstName = Imie.Text;
                    newWorker.lastName = Nazwisko.Text;
                    newWorker.systemLogin = Login.Text;


                    newWorker.employmentData = Convert.ToDateTime(DzienZatrudnienia.Text);


                    newWorker.passwordHash = getHash(Hasło.Text);

                    workers.Add(newWorker);
                    db.SaveChanges();
                    MessageBox.Show("Dodano Pracownika: " + Imie.Text + " " + Nazwisko.Text, "Komunikat");
                }
                else
                {
                    MessageBox.Show("Zła data zatrudnienia!", "Komunikat");

                }
            }
            else
            {
                MessageBox.Show("Błędne dane rejestracji!", "Komunikat");
            }
        }

        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new PracownicyModel();

        }
    }
}
