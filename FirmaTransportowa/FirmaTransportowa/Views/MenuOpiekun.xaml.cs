using System.Windows.Controls;
using System.Windows.Input;

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
            glowneOkno.DataContext = new MojePojazdy();
        }

        private void Logout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = null;
            glowneOkno.Width = 300;
            glowneOkno.Height = 450;
            ((MainWindow)System.Windows.Application.Current.MainWindow).Menu.Content = null;
            ((MainWindow)System.Windows.Application.Current.MainWindow).LoginScreen.Content = new Logowanie();
            Logowanie.actualUser = null;
        }
    }
}
