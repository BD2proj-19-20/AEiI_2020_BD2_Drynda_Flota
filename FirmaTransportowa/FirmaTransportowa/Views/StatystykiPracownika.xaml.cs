using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using FirmaTransportowa.ViewModels;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for StatystykiPracownika.xaml
    /// </summary>
    public partial class StatystykiPracownika : UserControl
    {
        Person changePerson;

        public StatystykiPracownika(Person people)
        {
            InitializeComponent();

            changePerson = people;

            if (changePerson.layoffDate <= DateTime.Now) //jeśli pracownik jest zwolniony nie można zmienić danych logownaia
            {
                KierownikPanel.Visibility = Visibility.Hidden;

                KierownikStartPanel.Visibility = Visibility.Hidden;
                KierownikEndPanel.Visibility = Visibility.Hidden;
               zmienLoginButton.Visibility = Visibility.Hidden;
                zmienpassowrdButton.Visibility = Visibility.Hidden;
                zmienKierownikaButton.Visibility = Visibility.Hidden;
                OpiekunPanel.Visibility = Visibility.Hidden; //nie może byc opiekunem zwolniony pracownik
                Thickness margin = BylyOpiekunPanel.Margin;
                margin.Top = margin.Top - 40;
                BylyOpiekunPanel.Margin = margin; //przesuwamy w górę panel z byłymi opiekunami
            }
            else
            {
                ScrolllBylyOpiekun.Height = 40;
                BylyOpiekunText.Height = 40;
                BylyOpiekunPanel.Height = 40;
            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from person in db.People where person.id == changePerson.id
                        join peoplePermission in db.PeoplesPermissions on person.id equals peoplePermission.personId into permissionTable

                        from permissionPeople in permissionTable.DefaultIfEmpty()
                        select new
                        {
                            Id = person.id,
                            LastName = person.lastName,
                            FirstName = person.firstName,
                            Login = person.systemLogin,
                            EmploymentData = person.employmentData,
                            LayoffDate = person.layoffDate,
                            PermissionName = permissionPeople.Permission.name,
                            PermissionGrant = permissionPeople.grantDate == null ? DateTime.MinValue : permissionPeople.grantDate,
                            RevokeDate = permissionPeople.revokeDate == null ? DateTime.MinValue : permissionPeople.revokeDate,

                        };

            foreach (var person in query)
            {
                //Tutaj beda wszystkie pojazdy, jakich pracownik byl opiekunem
                var query2 = from supervisor in db.CarSupervisors
                             where person.Id == supervisor.personId
                             join car in db.Cars on supervisor.carId equals car.id
                             select new
                             {
                                 BeginDate = supervisor.beginDate == null ? DateTime.MinValue : supervisor.beginDate,
                                 EndDate = supervisor.endDate == null ? DateTime.MinValue : supervisor.endDate,
                                 SaleDate = car.saleDate == null ? DateTime.MinValue : car.saleDate,
                                 CarMake = car.CarModel.make,
                                 CarModel = car.CarModel.model,
                                 CarRegistration = car.Registration,
                             };


                var query3 = from lends2 in db.Lends
                             where lends2.personId == person.Id
                             select new
                             {
                                 LendDate = lends2.lendDate == null ? DateTime.MinValue : lends2.lendDate,
                                 EngineCar = lends2.Car.engineCapacity, // == null ? 0 : lends2.Car.engineCapacity,
                                 ReservationEnd = lends2.Reservation.ended,
                                 ReturnDate = lends2.returnDate == null ? DateTime.MinValue : lends2.returnDate,
                                 Private = lends2.@private,
                                 StartOdometer = lends2.startOdometer,
                                 StartFuel = lends2.startFuel,
                                 EndOdometer = lends2.endOdometer,
                                 EndFuel = lends2.endFuel,
                                 PlannedReturnDate = lends2.plannedReturnDate,
                                 LendedCar = lends2.Car
                             };

                ImieNazwisko.Text = person.FirstName + " " + person.LastName;
                DataZatrudnienia.Text = person.EmploymentData.ToString().Substring(0, 10);
                if (people.layoffDate != null)
                    DataZwolnienia.Text = person.LayoffDate.ToString().Substring(0, 10);
                Login.Text = person.Login;


                var carSupervisior = db.CarSupervisors;

                var lends = db.Lends;
                var cars = db.Cars;
                string textOpiekun = "";
                string bylyOpiekun = "";

                foreach (var personSup in query2)

                {
                    if ((personSup.EndDate > DateTime.Today || personSup.EndDate == null) &&
                            (personSup.SaleDate > DateTime.Today || personSup.SaleDate == null))
                        textOpiekun += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                    else
                        bylyOpiekun += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";


                }


                if (!(changePerson.layoffDate <= DateTime.Now))
                    Opiekun.Text = textOpiekun;
                BylyOpiekun.Text = bylyOpiekun;




                Kierownik.Text = "Nie";

                if (person.PermissionName == "Kierownik" &&
               person.PermissionGrant <= DateTime.Now.Date && (person.RevokeDate > DateTime.Now || person.RevokeDate == null))
                {

                        KierownikEndPanel.Visibility = Visibility.Visible;
                        KierownikStartPanel.Visibility = Visibility.Visible;
                        Kierownik.Text = "Tak";
                        KierownikDateStart.Text = person.PermissionGrant.ToString().Substring(0, 10);
                    if (person.RevokeDate != null)
                        KierownikDateEnd.Text = person.RevokeDate.ToString().Substring(0, 10);


                    }
                else if (person.PermissionName == "Kierownik" && person.PermissionGrant > DateTime.Now.Date)
                {
                        KierownikEndPanel.Visibility = Visibility.Visible;
                        KierownikStartPanel.Visibility = Visibility.Visible;

                    if (((person.PermissionGrant - DateTime.Now).Days + 1) == 1)
                        Kierownik.Text = "Za " + ((person.PermissionGrant - DateTime.Now.Date).Days + 1) + " dzień";
                    else
                            Kierownik.Text = "Za " + ((person.PermissionGrant - DateTime.Now.Date).Days + 1) + " dni";
                    KierownikDateStart.Text = person.PermissionGrant.ToString().Substring(0, 10);
                    if (person.RevokeDate != null)
                        KierownikDateEnd.Text = person.RevokeDate.ToString().Substring(0, 10);
                }
                    else
                    {
                        KierownikEndPanel.Visibility = Visibility.Hidden;
                        KierownikStartPanel.Visibility = Visibility.Hidden;

                    }
                int zleceniaPrywatne = 0;
                int przejechaneKm = 0;
                int zleceniaSluzbowe = 0;
                int przejechaneKmSluzbowe = 0;


                int dni = 0;
                int dniSluzbowe = 0;

                var pojazd = "";
                var pojazdSluzbowy = "";

                double koszty = 0;
                double kosztySluzbowe = 0;

                foreach (var personLend in query3)
                {

                    if (personLend.ReservationEnd == true && personLend.ReturnDate != null) //lend ktore były
                    {
                        if (personLend.Private == true)
                        {
                            zleceniaPrywatne++;
                            if (personLend.EndOdometer != null)
                                przejechaneKm += personLend.EndOdometer.Value - personLend.StartOdometer;
                            if (personLend.ReturnDate > personLend.LendDate)
                            {
                                TimeSpan t = (DateTime)personLend.ReturnDate - personLend.LendDate;
                                dni += (int)t.TotalDays;
                            }

                            //jaki to samochód;

                            koszty = (przejechaneKm * 4.75) + (0.05 * personLend.EngineCar);


                        }
                        else
                        {
                            zleceniaSluzbowe++;
                            if (personLend.EndOdometer != null)
                                przejechaneKmSluzbowe += personLend.EndOdometer.Value - personLend.StartOdometer;
                            if (personLend.ReturnDate > personLend.LendDate)
                            {
                                TimeSpan t = (DateTime)personLend.ReturnDate - personLend.LendDate;
                                dniSluzbowe += (int)t.TotalDays;
                            }

                            kosztySluzbowe = (przejechaneKmSluzbowe * 4.75) + (0.05 * personLend.EngineCar);
                        }
                    }

                    if (personLend.LendDate < DateTime.Today && personLend.PlannedReturnDate < DateTime.Today &&
                        personLend.ReturnDate > DateTime.Today) //aktualny pojazd
                    {

                        foreach (var personSup in query2)
                        {
                            if (personLend.Private == true)
                                pojazd += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                            else
                                pojazdSluzbowy += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                        }
                    }

                }

                //Koszty???
                Pojazd.Text = pojazd;
                PojazdSluzbowe.Text = pojazdSluzbowy;


                Dni.Text = dni.ToString() + " dni";
                DniSluzbowe.Text = dniSluzbowe.ToString() + " dni";

                Zlecenia.Text = zleceniaPrywatne.ToString();
                Przejechane.Text = przejechaneKm.ToString() + " km";


                PrzejechaneSluzobowe.Text = przejechaneKmSluzbowe.ToString() + " km";
                ZleceniaSluzbowe.Text = zleceniaSluzbowe.ToString();

                Koszty.Text = koszty.ToString() + " PLN";
                KosztySluzbowe.Text = kosztySluzbowe.ToString() + " PLN";
            }
        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Pracownicy();

        }
        private void Zmien_haslo(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            int id = changePerson.id;
            ZmienHaslo zmianaView = new ZmienHaslo(changePerson);
            zmianaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            zmianaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            zmianaView.ShowDialog();
            while (zmianaView.IsActive)
            {

            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;


            // toChange.systemLogin = newLogin.Text;

            foreach (var person in people)
            {
                if (person.id == changePerson.id)
                {

                    newPerson = person;
                    break;

                }

            }

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPracownika(newPerson);
            return;
        }
        private void Zmien_login(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            int id = changePerson.id;
            ZmienLogin zmianaView = new ZmienLogin(changePerson);
            zmianaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            zmianaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            zmianaView.ShowDialog();
            while (zmianaView.IsActive)
            {

            }

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;


            // toChange.systemLogin = newLogin.Text;

            foreach (var person in people)
            {
                if (person.id == changePerson.id)
                {

                    newPerson = person;
                    break;

                }

            }

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPracownika(newPerson);
            return;
        }
        private void Zmien_Kierownika(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            int id = changePerson.id;
            ZmienKierownika zmianaOpiekunaView = new ZmienKierownika(changePerson);
            zmianaOpiekunaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            zmianaOpiekunaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
            zmianaOpiekunaView.ShowDialog();
            while (zmianaOpiekunaView.IsActive)
            {

            }
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;

            foreach (var person in people)
            {
                if (person.id == changePerson.id)
                {

                    newPerson = person;
                    break;

                }

            }

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new StatystykiPracownika(newPerson);
            return;
        }
    }
}
