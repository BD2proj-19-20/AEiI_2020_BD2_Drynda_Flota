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
    /// Logika interakcji dla klasy ZmienKierownika.xaml
    /// </summary>
    public partial class ZmienKierownika : Window
    {
        Person toChange;
        public ZmienKierownika(Person changePerson)
        {
            InitializeComponent();

            toChange = changePerson;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            //    var permissionCompany = db.Permissions;
            var peoplePermission = db.PeoplesPermissions;

            foreach (var permissionWorker in peoplePermission)
            {
                if (permissionWorker.personId == toChange.id && permissionWorker.Permission.name == "Kierownik" &&
                   permissionWorker.grantDate < DateTime.Now && (permissionWorker.revokeDate > DateTime.Now || permissionWorker.revokeDate == null))
                {
                    newKierownikStart.Text = permissionWorker.grantDate.ToString().Substring(0, 10);
                    newKierownikEnd.Text = permissionWorker.revokeDate.ToString().Substring(0, 10);
                }
                else if (permissionWorker.personId == toChange.id && permissionWorker.Permission.name == "Kierownik" &&
                   permissionWorker.grantDate > DateTime.Now)
                {
                    newKierownikStart.Text = permissionWorker.grantDate.ToString().Substring(0, 10);
                    newKierownikEnd.Text = permissionWorker.revokeDate.ToString().Substring(0, 10);
                }
            }
        }
        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Zmien(object sender, RoutedEventArgs e)
        {
            DateTime temp;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            var peoplePermission = db.PeoplesPermissions;

            if (!(DateTime.TryParse(newKierownikStart.Text, out temp) &&
                (Convert.ToDateTime(newKierownikStart.Text) >= toChange.employmentData)))
            {

                MessageBox.Show("Błedna data początku", "Komunikat");
                return;
            }
            if (!((DateTime.TryParse(newKierownikEnd.Text, out temp) &&
                 Convert.ToDateTime(newKierownikEnd.Text) > Convert.ToDateTime(newKierownikStart.Text))
                           || newKierownikEnd.Text == ""))
            {

                MessageBox.Show("Błedna data zakonczenia", "Komunikat");
                return;
            }
            foreach (var person in people)
            {
                if (person.id == toChange.id)
                {
                    foreach (var permissionWorker in peoplePermission)
                    {
                        if (permissionWorker.personId == person.id && permissionWorker.Permission.name == "Kierownik")
                        {
                            permissionWorker.grantDate = Convert.ToDateTime(newKierownikStart.Text);
                            if (newKierownikEnd.Text != "")
                                permissionWorker.revokeDate = Convert.ToDateTime(newKierownikEnd.Text);
                        }
                    }
                
                }
            }
            MessageBox.Show("Zmienieono!", "Komunikat");
            db.SaveChanges();
        }
    }
}
