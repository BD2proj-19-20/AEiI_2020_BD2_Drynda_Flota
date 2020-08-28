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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Logowanie.xaml
    /// </summary>
    public partial class Logowanie : UserControl
    {
        public Logowanie()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }

        private int getPermission(string login, byte[] password)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            var permissions = db.PeoplesPermissions;
            foreach (var person in people)
            {
                if (person.systemLogin == login)
                {
                    if (person.passwordHash == password)
                    {
                        foreach (var permission in permissions)
                        {
                            if (permission.personId == person.id)
                            {
                                return permission.permissionId;
                            }
                        }
                    }
                }

            }
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
            byte[] password = Encoding.ASCII.GetBytes(passwordBox.Password);
            var permissionId = getPermission(login, password);
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            switch (permissionId)
            {
                case 0:
                    glowneOkno.Width = 1000;
                    glowneOkno.Height = 600;
                    CenterWindowOnScreen();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new MenuPracownik();
                    break;
                case 1:
                    glowneOkno.Width = 1000;
                    glowneOkno.Height = 600;
                    CenterWindowOnScreen();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new MenuKierownik();
                    break;
                case 2:
                    glowneOkno.Width = 1000;
                    glowneOkno.Height = 600;
                    CenterWindowOnScreen();
                    ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = null;
                    ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = new MenuOpiekun();
                    break;
                default:
                    break;
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
