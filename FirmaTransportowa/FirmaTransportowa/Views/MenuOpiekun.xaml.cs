using FirmaTransportowa.ViewModels;
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
    /// Interaction logic for MenuOpiekun.xaml
    /// </summary>
    public partial class MenuOpiekun : UserControl
    {
        public MenuOpiekun()
        {
            InitializeComponent();
        }
        private void MyCars_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new MojePojazdyModel();
        }

        private void Logout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = null;
            glowneOkno.Width = 300;
            glowneOkno.Height = 450;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = new Logowanie();
        }
    }
}
