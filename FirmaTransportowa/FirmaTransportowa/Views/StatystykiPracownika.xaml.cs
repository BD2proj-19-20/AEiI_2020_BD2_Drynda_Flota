using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using FirmaTransportowa.ViewModels;

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
                zmienDaneButton.Visibility = Visibility.Hidden;
                zmienKierownikaButton.Visibility = Visibility.Hidden;
                OpiekunPanel.Visibility = Visibility.Hidden; //nie może byc opiekunem zwolniony pracownik
                AktywnosciPanel.Visibility = Visibility.Hidden; //chpowiemy ilość obecnych aktywności
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


                ImieNazwisko.Text = people.firstName + " " + people.lastName;
            DataZatrudnienia.Text = people.employmentData.ToString().Substring(0, 10);
            if (people.layoffDate != null)
                DataZwolnienia.Text = people.layoffDate.ToString().Substring(0, 10);
            Login.Text = people.systemLogin;


            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var activities = db.Activities;
            var carSupervisior = db.CarSupervisors;

            int aktywnosci = 0;
            var lends = db.Lends;
            var cars = db.Cars;
            string textOpiekun = "";
            string bylyOpiekun = "";
            foreach (var carS in carSupervisior)
            {
                if (carS.personId == people.id)
                {
                    foreach (var car in cars)
                        if (!(changePerson.layoffDate <= DateTime.Now) && car.id == carS.carId && (carS.endDate > DateTime.Today || carS.endDate == null) 
                            && (car.saleDate > DateTime.Today || car.saleDate == null) )
                            textOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                        else if (car.id == carS.carId)
                            bylyOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                }
            }
            if (!(changePerson.layoffDate <= DateTime.Now) )
                Opiekun.Text = textOpiekun ;
            BylyOpiekun.Text = bylyOpiekun ;

            var peoplePermission = db.PeoplesPermissions;


            Kierownik.Text = "Nie";
            foreach( var permissionWorker in peoplePermission)
            {
              if(permissionWorker.personId==changePerson.id && permissionWorker.Permission.name=="Kierownik" &&
                    permissionWorker.grantDate<DateTime.Now && (permissionWorker.revokeDate > DateTime.Now || permissionWorker.revokeDate ==null))
                {

                    Kierownik.Text = "Tak";
                    KierownikDateStart.Text = permissionWorker.grantDate.ToString().Substring(0, 10);
                    if(permissionWorker.revokeDate!=null)
                    KierownikDateEnd.Text = permissionWorker.revokeDate.ToString().Substring(0, 10);

                }
              else if(permissionWorker.personId == changePerson.id && permissionWorker.Permission.name == "Kierownik" &&
                    permissionWorker.grantDate > DateTime.Now )
                {
                    if (((permissionWorker.grantDate - DateTime.Now).Days + 1)==1)
                        Kierownik.Text = "Za "+((permissionWorker.grantDate- DateTime.Now).Days+1) + " dzień";
                    else
                        Kierownik.Text = "Za " + ((permissionWorker.grantDate - DateTime.Now).Days + 1) + " dni";
                    // Kierownik.Text ="Zostanie za "+ (DateTime.Now - permissionWorker.grantDate).TotalDays.ToString()+"dni";
                    KierownikDateStart.Text = permissionWorker.grantDate.ToString().Substring(0, 10);
                    if (permissionWorker.revokeDate != null)
                        KierownikDateEnd.Text = permissionWorker.revokeDate.ToString().Substring(0, 10);
                }

            }

            foreach (var aktyw in activities)
            {
                if (aktyw.reporterId == people.id && aktyw.closeDate > DateTime.Today && aktyw.orderDate < DateTime.Today)
                    aktywnosci++;
            }

            Aktywnosci.Text = aktywnosci.ToString();

            int zleceniaPrywatne = 0;
            int przejechaneKm = 0;
            int zleceniaSluzbowe = 0;
            int przejechaneKmSluzbowe = 0;


            int dni = 0;
            int dniSluzbowe = 0;

            var pojazd = "";
            var pojazdSluzbowy = "";

            //double koszty=0;
           // double kosztySluzbowe = 0;

            foreach (var lend in lends)
            {

                if (lend.personId == people.id && lend.returnDate < DateTime.Today && lend.plannedReturnDate < DateTime.Today) //lend ktore były
                {
                    if (lend.@private == true)
                    {
                        zleceniaPrywatne++;
                        if (lend.endOdometer != null)
                            przejechaneKm += lend.endOdometer.Value - lend.startOdometer;
                        TimeSpan t = (DateTime)lend.returnDate - (DateTime)lend.lendDate;
                        dni += (int)t.TotalDays;
                        
                    }
                    else
                    {
                        zleceniaSluzbowe++;
                        if (lend.endOdometer != null)
                            przejechaneKmSluzbowe += lend.endOdometer.Value - lend.startOdometer;
                        TimeSpan t = (DateTime)lend.returnDate - (DateTime)lend.lendDate;
                        dniSluzbowe += (int)t.TotalDays;
                    }

                }


                foreach (var car in cars)
                {
                    if (lend.carId == car.id && lend.returnDate < DateTime.Today && lend.plannedReturnDate < DateTime.Today && lend.lendDate > DateTime.Today) //aktualny pojazd
                    { //jedno wypozyczenie?
                        if(lend.@private==true)
                            pojazd += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration+"\n";
                        else
                            pojazdSluzbowy += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration+"\n";
                    }
                    //dodanie kosztów zależności różnych kategorii - model typ ...
                }
            }
            Pojazd.Text = pojazd;
            PojazdSluzbowe.Text = pojazdSluzbowy;


            Dni.Text = dni.ToString() +" dni";
            DniSluzbowe.Text = dniSluzbowe.ToString() + " dni";

            Zlecenia.Text = zleceniaPrywatne.ToString();
            Przejechane.Text = przejechaneKm.ToString() + " km";


            PrzejechaneSluzobowe.Text = przejechaneKmSluzbowe.ToString() + " km";
            ZleceniaSluzbowe.Text = zleceniaSluzbowe.ToString();

            Koszty.Text= ((double)(przejechaneKm*4.75)).ToString() + " PLN";
            KosztySluzbowe.Text = ((double)(przejechaneKmSluzbowe * 4.75)).ToString() +" PLN";

        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {

            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new PracownicyModel();

        }

        private void Zmien_Dane(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            int id = changePerson.id;
            ZmaianaDanychLogowania zmianaView = new ZmaianaDanychLogowania(changePerson);
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
