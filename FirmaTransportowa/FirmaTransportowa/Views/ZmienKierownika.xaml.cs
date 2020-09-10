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
        bool changeDate = false;
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
                    newKierownikStart.SelectedDate = permissionWorker.grantDate;
                    newKierownikEnd.SelectedDate = permissionWorker.revokeDate;

                    newKierownikEnd.BlackoutDates.AddDatesInPast();
                    newKierownikStart.IsEnabled = false; //jeśli jest w trakcie nie możemy zmienić początku
                 //   newKierownikStart.BlackoutDates.AddDatesInPast();
                    changeDate = true;
                }
                else if (permissionWorker.personId == toChange.id && permissionWorker.Permission.name == "Kierownik" &&
                   permissionWorker.grantDate > DateTime.Now)
                {
                    newKierownikStart.SelectedDate = permissionWorker.grantDate;
                    newKierownikEnd.SelectedDate = permissionWorker.revokeDate;

                    newKierownikEnd.BlackoutDates.AddDatesInPast();
                    newKierownikStart.BlackoutDates.AddDatesInPast();
                    changeDate = true;
                }
            }
         
            if(changeDate==false)
            {

                newKierownikEnd.SelectedDate = DateTime.Today;
                newKierownikStart.SelectedDate = DateTime.Today;
                newKierownikEnd.BlackoutDates.AddDatesInPast();
                newKierownikStart.BlackoutDates.AddDatesInPast();
                changeDate = true;
            }

        }
        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private CalendarDateRange dzienKierownictwaEndBlackoutRange = null;
        private void DzienKierownictwaStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (changeDate == true)
            {
                if (newKierownikEnd.SelectedDate < newKierownikStart.SelectedDate)
                    newKierownikEnd.SelectedDate = newKierownikStart.SelectedDate;
                if (dzienKierownictwaEndBlackoutRange == null)
                {
                    dzienKierownictwaEndBlackoutRange = new CalendarDateRange(DateTime.Today.AddDays(-1), ((DateTime)newKierownikStart.SelectedDate).AddDays(-1));
                    newKierownikEnd.BlackoutDates.Insert(1, dzienKierownictwaEndBlackoutRange);
                }
                else
                {
                    dzienKierownictwaEndBlackoutRange.End = ((DateTime)newKierownikStart.SelectedDate).AddDays(-1);
                    newKierownikEnd.BlackoutDates[1] = dzienKierownictwaEndBlackoutRange;
                }
            }
        }


        private void Zmien(object sender, RoutedEventArgs e)
        {
          //  DateTime temp;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            var peoplePermission = db.PeoplesPermissions;

            //if (!(DateTime.TryParse(newKierownikStart.Text, out temp) &&
            //    (Convert.ToDateTime(newKierownikStart.Text) >= toChange.employmentData)))
            //{
            //    MessageBox.Show("Błedna data początku", "Komunikat");
            //    return;
            //}
            //if (!((DateTime.TryParse(newKierownikEnd.Text, out temp) &&
            //     Convert.ToDateTime(newKierownikEnd.Text) > Convert.ToDateTime(newKierownikStart.Text))
            //               || newKierownikEnd.Text == ""))
            //{
            //    MessageBox.Show("Błedna data zakonczenia", "Komunikat");
            //    return;
            //}

            bool newPermission = true;
            foreach (var person in people)
            {
                if (person.id == toChange.id)
                {
                    foreach (var permissionWorker in peoplePermission)
                    {
                        if (permissionWorker.personId == person.id  && permissionWorker.Permission.name=="Kierownik" ) //jeśli jest kierownikiem edytujemy uprawnienie 
                        {
                            permissionWorker.grantDate = (System.DateTime)newKierownikStart.SelectedDate;
                            if (newKierownikEnd.Text != "")
                                permissionWorker.revokeDate = (System.DateTime)newKierownikEnd.SelectedDate;
                            else
                                permissionWorker.revokeDate = null;

                            newPermission = false;
                        }
                        
                    }

                    if (newPermission == true)  //jeśli nie jest kierownikiem dodajemy mu uprawnienie 
                    {
                        PeoplesPermission workerPermission = new PeoplesPermission();

                        workerPermission.grantDate = (System.DateTime)newKierownikStart.SelectedDate;
                        if (newKierownikEnd.Text != "")
                            workerPermission.revokeDate = (System.DateTime)newKierownikEnd.SelectedDate;
                        else
                            workerPermission.revokeDate = null;

                        workerPermission.personId = person.id;

                        peoplePermission.Add(workerPermission);
                        // db.SaveChanges();
                    }
                }
            }
            MessageBox.Show("Zmienieono!", "Komunikat");
            db.SaveChanges();
        }
    }
}
