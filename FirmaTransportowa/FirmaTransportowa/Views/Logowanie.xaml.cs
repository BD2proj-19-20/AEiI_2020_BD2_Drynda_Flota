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
