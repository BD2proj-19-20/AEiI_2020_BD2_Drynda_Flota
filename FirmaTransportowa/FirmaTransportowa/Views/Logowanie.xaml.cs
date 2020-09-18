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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Logowanie.xaml
    /// </summary>
    /// 

    public partial class Logowanie : UserControl
    {
        public static Person actualUser;
        public Logowanie()
        {

            InitializeComponent();
            CenterWindowOnScreen();
            //  loginBox.Text= "kamBach";
            //   passwordBox.Password = "kamBach";

              loginBox.Text= "rancisek";
               passwordBox.Password = "rancisek";
        }

        static public byte[] getHash(string password)
        {
            byte[] passwordSalt = { 67, 128, 62, 208, 147, 77, 143, 197 };

            using (Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, passwordSalt))
            {
                hashGenerator.IterationCount = 1001;
                return hashGenerator.GetBytes(256);
            }
        }

        private int getPermission(string login, string password)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            var permissions = db.PeoplesPermissions;
            var carSupervisors = db.CarSupervisors;
            var permissionComapny = db.Permissions;

            var permissionFlota = db.Permissions;

            int permissionLevel = 0;
            bool kierownikLogin = false;
            bool opiekunLogin = false;

			//todo: LINQ
            if (login.Length >= 6 && password.Length >= 6)
                foreach (var person in people)
                {
                    if (person.systemLogin == login)
                    {
                   
                        if (person.passwordHash.SequenceEqual(getHash(password)) && (person.layoffDate >= DateTime.Now || person.layoffDate == null )) //zwolniony nie może się zalogować
                        {

                            actualUser = person;
                            permissionLevel++; //każdy jest pracownikiem


                                foreach (var permission in permissions)
                                {
                                    if (permission.personId == person.id && permission.grantDate.Date <= DateTime.Now.Date && 
                                    permission.revokeDate > DateTime.Now.Date)
                                    {
                                        foreach (var permissionWorkers in permissionComapny)
                                        {

                                            if (permissionWorkers.Id == permission.permissionId && permissionWorkers.name == "Kierownik")
                                            {
                                                actualUser = person;
                                                kierownikLogin = true;
                                            }

                                        }

                                    }
                                }
                               
                                //opiekuna sprawdzamy po CarSupervisior
                                foreach(var carSupervisor in carSupervisors)
                                {
                                if (carSupervisor.personId == person.id)
                                    if (carSupervisor.personId==person.id && carSupervisor.beginDate <= DateTime.Now.Date
                                    && (carSupervisor.endDate >DateTime.Now.Date || carSupervisor.endDate ==null))
                                    opiekunLogin = true;
                                }


                                MessageBox.Show("Witaj " + person.firstName + " " + person.lastName + " !", "Komunikat");


                            if (opiekunLogin == true && kierownikLogin == false) //tylko opiekun
                                permissionLevel = 2;
                           else if (kierownikLogin == true && opiekunLogin == false) //tylko kierownik
                                permissionLevel = 3;
                          else  if (kierownikLogin == true && opiekunLogin == true) // oby dwa 
                                permissionLevel = 4;

                            return permissionLevel;

                        }
                        else
                        {
                            MessageBox.Show("Logowanie nie udało się :-(", "Komunikat");
                            return 0;
                        }
                    }
				}
			MessageBox.Show("Błędne dane logowania.", "Komunikat");
            return 0;
        }

        private void Worker_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.Width = 1000;
            glowneOkno.Height = 600;
            CenterWindowOnScreen();
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new MenuPracownik();
        }

        private void Leader_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.Width = 1000;
            glowneOkno.Height = 600;
            CenterWindowOnScreen();
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new MenuKierownik();
        }

        private void Supervisor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.Width = 1000;
            glowneOkno.Height = 600;
            CenterWindowOnScreen();
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new MenuOpiekun();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = loginBox.Text;
            var permissionId = getPermission(login, passwordBox.Password);
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;

            if (permissionId != 0)
            {
                glowneOkno.Width = 1000;
                glowneOkno.Height = 600;
                CenterWindowOnScreen();
                ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
                ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new Menu(permissionId);
            }

        }
        private void CenterWindowOnScreen()
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = glowneOkno.Width;
            double windowHeight = glowneOkno.Height;
            glowneOkno.Left = (screenWidth / 2) - (windowWidth / 2);
            glowneOkno.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
