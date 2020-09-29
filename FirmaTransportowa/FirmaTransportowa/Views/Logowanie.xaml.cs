using FirmaTransportowa.Model;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            /*loginBox.Text= "kamBach";
			passwordBox.Password = "kamBach";*/

            //loginBox.Text= "rancisek";
            //passwordBox.Password = "rancisek";
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

            int permissionLevel = 1; //każdy jest pracownikiem, jak nim nie jest to i tak rzuci zero
			bool kierownikLogin = false;
            bool opiekunLogin = false;

			if (login.Length >= 6 && password.Length >= 6) {
				Person person = db.People.Where(p => p.systemLogin == login && (p.layoffDate == null || p.layoffDate > DateTime.Now)).SingleOrDefault();
				if (person != null) {
					if (person.passwordHash.SequenceEqual(getHash(password))) { //sprawdzenie hasła

						actualUser = person;

						var dateNow = DateTime.Now.Date;

						int kierownikPermissionsCount = db.PeoplesPermissions.Where(pp => pp.personId == person.id && pp.Permission.name == "Kierownik" && pp.grantDate <= dateNow && (pp.revokeDate == null || pp.revokeDate > dateNow)).Count();

						//liczba uprawnień kierowniczych, powinno być zawsze jedno, bądź zero

						if (kierownikPermissionsCount > 0)
							kierownikLogin = true;

						int supervisedCarsCount = db.CarSupervisors.Where(cs => cs.personId == person.id && cs.beginDate <= dateNow && (cs.endDate == null || cs.endDate > dateNow)).Count();
						if (supervisedCarsCount > 0)
							opiekunLogin = true;

						MessageBox.Show("Witaj " + person.firstName + " " + person.lastName + " !", "Komunikat");

						if (opiekunLogin == true && kierownikLogin == false) //tylko opiekun
							permissionLevel = 2;
						else if (kierownikLogin == true && opiekunLogin == false) //tylko kierownik
							permissionLevel = 3;
						else if (kierownikLogin == true && opiekunLogin == true) // oby dwa 
							permissionLevel = 4;

						return permissionLevel; //ten permissionLevel mógłby być jakąś elegancką flagą bitową, trzy bity, jeden czy sukces, jeden czy kierownik, jeden czy opiekun
												//ale piszemy w C#, a nie assemblerze, więc tutaj nikt się w coś takiego nie bawi, int jest ok

					}
					else {
						MessageBox.Show("Błędne dane logowania", "Komunikat"); //złe hasło
						return 0;
					}
				}
			}
			MessageBox.Show("Błędne dane logowania.", "Komunikat");
            return 0;
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

		private void TextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == System.Windows.Input.Key.Enter)
				Login_Click(sender, e);
		}
	}
}
