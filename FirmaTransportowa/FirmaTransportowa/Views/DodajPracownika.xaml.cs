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
                return hashGenerator.GetBytes(256);
            }
        }
        private void Dodaj_Pracownika(object sender, RoutedEventArgs e)
        {

            DateTime temp;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var workers = db.People;
            var newWorker = new Person();
            bool loginCheck = true;

            if(Hasło1.Password != Hasło2.Password )
            {
                MessageBox.Show("Hasła są różne!", "Komunikat");

            }
            else 
            if (Imie.Text.Length >= 3 && Nazwisko.Text.Length >= 3 && Login.Text.Length >= 6 && Hasło1.Password.Length >= 6)
            {
                foreach (var person in workers)
                {
                    if (person.systemLogin == Login.Text)
                        loginCheck = false;

                }
                if(loginCheck == false)
                    MessageBox.Show("Login jest zajęty!", "Komunikat");
                else if (!DzienZatrudnienia.Text.Equals("") && (DateTime.TryParse(DzienZatrudnienia.Text, out temp)))
                {
                    newWorker.firstName = Imie.Text;
                    newWorker.lastName = Nazwisko.Text;
                    newWorker.systemLogin = Login.Text;


                    newWorker.employmentData = Convert.ToDateTime(DzienZatrudnienia.Text);


                    newWorker.passwordHash = getHash(Hasło1.Password);

                    workers.Add(newWorker);
                    db.SaveChanges();
                    MessageBox.Show("Dodano Pracownika: " + Imie.Text + " " + Nazwisko.Text, "Komunikat");
                }
                else
                    MessageBox.Show("Zła data zatrudnienia!", "Komunikat");
            }
            else
                MessageBox.Show("Błędne dane rejestracji!", "Komunikat");
        }

        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new PracownicyModel();

        }
    }
}
