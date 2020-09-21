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
    /// Logika interakcji dla klasy ZmienLogin.xaml
    /// </summary>
    public partial class ZmienLogin : Window
    {
        Person toChange;
        public ZmienLogin(Person changePerson)
        {
            InitializeComponent();
            newLogin.Text = changePerson.systemLogin;
            toChange = changePerson;

        }

        private void Zmien(object sender, RoutedEventArgs e)
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            // var people = db.People;

            Person person = db.People.Where(p => toChange.id == p.id).SingleOrDefault();

            if (newLogin.Text.Length >= 6)
            {

                int personSameLogin = db.People.Where(pp => pp.systemLogin == newLogin.Text && pp.id != toChange.id).Count();
                if (personSameLogin > 0)
                { 
                        MessageBox.Show("Login jest już zajęty!", "Komunikat");
                        return;
                }
                    else 
                        person.systemLogin = newLogin.Text;

                

                MessageBox.Show("Udało sie zmienić\nlogin!", "Komunikat");
                db.SaveChanges();

            }
            else
                MessageBox.Show("Zła długość loginu!", "Komunikat");
        }


        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
