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
            DzienKierownictwaEnd.BlackoutDates.AddDatesInPast();
            DzienKierownictwaStart.BlackoutDates.AddDatesInPast();
            DzienZatrudnienia.BlackoutDates.AddDatesInPast();
         //   DzienKierownictwaEnd.SelectedDate = DateTime.Today;
            DzienKierownictwaStart.SelectedDate = DateTime.Today;
            DzienZatrudnienia.SelectedDate = DateTime.Today;
            
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
                else if (DzienZatrudnienia.SelectedDate != null)
                {
                    newWorker.firstName = Imie.Text;
                    newWorker.lastName = Nazwisko.Text;
                    newWorker.systemLogin = Login.Text;
                    newWorker.employmentData = DzienZatrudnienia.SelectedDate.Value;
                    newWorker.passwordHash = getHash(Hasło1.Password);
                    workers.Add(newWorker);
                    db.SaveChanges();

                    if(KierownikBox.IsChecked==true)
                    {
                        var permissionCompany = db.Permissions;
                        var peoplePermission = db.PeoplesPermissions;
                        var workerPermission = new PeoplesPermission();
                        if(DzienZatrudnienia.SelectedDate == null || DzienKierownictwaStart.SelectedDate < newWorker.employmentData) //jeżeli nie podano od kiedy kierownikiem, lub jest kierownikiem dłużej niż pracuje: błąd
                        {
                            workers.Remove(newWorker);
                            db.SaveChanges();
                            MessageBox.Show("Błedna data początku", "Komunikat");
                            return;
                        }
                        if (DzienKierownictwaEnd.SelectedDate > DzienKierownictwaStart.SelectedDate)
                        {
                            workers.Remove(newWorker);
                            db.SaveChanges();
                            MessageBox.Show("Błedna data zakonczenia", "Komunikat");
                            return;
                        }
                        else if (DzienKierownictwaEnd.Text != "")
                            workerPermission.revokeDate = (System.DateTime)DzienKierownictwaEnd.SelectedDate;
                        else
                            workerPermission.revokeDate = null;
                       // workerPermission.revokeDate = DzienKierownictwaEnd.SelectedDate;
                        
                        foreach (var permissionComp in permissionCompany)
                        {
                            if (permissionComp.name == "Kierownik")
                                workerPermission.permissionId = permissionComp.Id;
                        }
                        workerPermission.grantDate= DzienKierownictwaStart.SelectedDate.Value; //od razu staje się kierownikiem 
                        workerPermission.personId = newWorker.id;
                        peoplePermission.Add(workerPermission);
                        db.SaveChanges();
                    }

                    MessageBox.Show("Dodano Pracownika: " + (newWorker.id+1).ToString() + " " + newWorker.firstName + "\n" + newWorker.lastName, "Komunikat");
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
            glowneOkno.DataContext = new Pracownicy();

        }

        private CalendarDateRange dzienKierownictwaEndBlackoutRange = null;
        private void DzienKierownictwaStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
            if (DzienKierownictwaEnd.SelectedDate <= DzienKierownictwaStart.SelectedDate)
                DzienKierownictwaEnd.SelectedDate = null;
            if (dzienKierownictwaEndBlackoutRange == null) {
                dzienKierownictwaEndBlackoutRange = new CalendarDateRange(DateTime.Today, ((DateTime)DzienKierownictwaStart.SelectedDate));
                DzienKierownictwaEnd.BlackoutDates.Insert(1, dzienKierownictwaEndBlackoutRange);
            }
            else {
                dzienKierownictwaEndBlackoutRange.End = ((DateTime)DzienKierownictwaStart.SelectedDate);
                DzienKierownictwaEnd.BlackoutDates[1] = dzienKierownictwaEndBlackoutRange;
            }
        }

		private void KierownikBox_Checked(object sender, RoutedEventArgs e) {
            DzienKierownictwaStart.IsEnabled = true;
            DzienKierownictwaEnd.IsEnabled = true;
        }

		private void KierownikBox_Unchecked(object sender, RoutedEventArgs e) {
            DzienKierownictwaStart.IsEnabled = false;
            DzienKierownictwaEnd.IsEnabled = false;
        }
	}
}
