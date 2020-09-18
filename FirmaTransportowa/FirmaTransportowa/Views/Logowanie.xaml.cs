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

            int permissionLevel = 0;
            bool kierownikLogin = false;
            bool opiekunLogin = false;



            var query = from person in db.People
                        join peoplePermission in db.PeoplesPermissions on person.id equals peoplePermission.personId into permissionTable

                        from permissionPeople in permissionTable.DefaultIfEmpty()
                        select new
                        {
                            Person = person,
                            Id = person.id,
                            LastName = person.lastName,
                            FirstName = person.firstName,
                            Login = person.systemLogin,
                            PasswordHash= person.passwordHash,
                            LayoffDate = person.layoffDate ==  null ? DateTime.MaxValue : person.layoffDate,
                            PermissionName = permissionPeople.Permission.name,
                            PermissionGrant = permissionPeople.grantDate == null ? DateTime.MaxValue : permissionPeople.grantDate,
                            RevokeDate = permissionPeople.revokeDate == null ? DateTime.MaxValue : permissionPeople.revokeDate,

                        };

                //todo: LINQ
                if (login.Length >= 6 && password.Length >= 6)
                foreach (var person in query)
                {
                    if (person.Login == login)
                    {
                   
                        if (person.PasswordHash.SequenceEqual(getHash(password)) && (person.LayoffDate >= DateTime.Now || person.LayoffDate == null )) //zwolniony nie może się zalogować
                        {

                            actualUser = person.Person;
                            permissionLevel++; //każdy jest pracownikiem


                             if( person.PermissionGrant.Date <= DateTime.Now.Date &&  person.RevokeDate > DateTime.Now.Date 
                                || person.RevokeDate == null)

                                            if (person.PermissionName == "Kierownik")
                                                kierownikLogin = true;


                            var query2 = from supervisor in db.CarSupervisors
                                         where person.Id == supervisor.personId
                                         join car in db.Cars on supervisor.carId equals car.id
                                         select new
                                         {
                                             BeginDate = supervisor.beginDate == null ? DateTime.MinValue : supervisor.beginDate,
                                             EndDate = supervisor.endDate == null ? DateTime.MinValue : supervisor.endDate,
                                         };

                            foreach (var personSup in query2)

                            {
                                 if( personSup.BeginDate <= DateTime.Now.Date && (personSup.EndDate > DateTime.Now.Date || personSup.EndDate == null))
                                    opiekunLogin = true;
                            }

                                MessageBox.Show("Witaj " + person.FirstName + " " + person.LastName + " !", "Komunikat");

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
