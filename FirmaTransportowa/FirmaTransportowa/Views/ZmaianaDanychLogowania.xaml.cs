using FirmaTransportowa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Logika interakcji dla klasy ZmaianaDanychLogowania.xaml
    /// </summary>
    public partial class ZmaianaDanychLogowania : Window
    {
        Person toChange;
        public ZmaianaDanychLogowania(Person changePerson)
        {
            InitializeComponent();

            newLogin.Text = changePerson.systemLogin;
            toChange = changePerson;

        

        }
        public byte[] getHash(string password)
        {
            byte[] passwordSalt = { 67, 128, 62, 208, 147, 77, 143, 197 };

            using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, passwordSalt))
            {
                hashGenerator.IterationCount = 1001;
                return hashGenerator.GetBytes(256);
            }
        }

        private void Zmien(object sender, RoutedEventArgs e)
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;


            // toChange.systemLogin = newLogin.Text;

            if (newHaslo1.Password == newHaslo2.Password)
            if (newLogin.Text.Length >= 6 && newHaslo1.Password.Length>=6)
            {
                foreach (var person in people)
                {
                    if (person.systemLogin == newLogin.Text && person.id != toChange.id)
                    {
                        MessageBox.Show("Login jest już zajęty!", "Komunikat");
                        return;
                    }
                    else if (person.id == toChange.id)
                    {
                        person.systemLogin = newLogin.Text;
                        person.passwordHash= getHash(newHaslo1.Password);
                    }

                }

                MessageBox.Show("Zmienieono!", "Komunikat");
                db.SaveChanges();

            }
            else
            {
                MessageBox.Show("Zła długość nowych danych logowania!", "Komunikat");
            }
            else
            {
                MessageBox.Show("Hasła są różne", "Komunikat");
            }
        }


        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
