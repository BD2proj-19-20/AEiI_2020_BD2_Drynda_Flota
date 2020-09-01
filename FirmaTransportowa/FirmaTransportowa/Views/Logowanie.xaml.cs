﻿using FirmaTransportowa.Model;
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
    public partial class Logowanie : UserControl
    {
        public Logowanie()
        {
            InitializeComponent();
            CenterWindowOnScreen();
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
            foreach (var person in people)
            {
                if (person.systemLogin == login)
                {
                    var k = Convert.ToBase64String(person.passwordHash).Substring(0, 8);
                    byte[] check2 = getHash(password);
                    
                    bool ki = person.passwordHash.SequenceEqual(check2);
                    if (person.passwordHash.SequenceEqual(getHash(password)))
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
            return 3;
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
            var permissionId = getPermission(login, passwordBox.Password); //zwracana wartość to id dostępu
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
