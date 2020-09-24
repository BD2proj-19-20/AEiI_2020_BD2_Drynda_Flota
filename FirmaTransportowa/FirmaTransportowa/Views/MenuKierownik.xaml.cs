using System.Windows.Controls;
using System.Windows.Input;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for MenuKierownik.xaml
    /// </summary>
    public partial class MenuKierownik : UserControl
    {
        public MenuKierownik()
        {
            InitializeComponent();

        }

        private void Workers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Pracownicy();
        }

        private void Rent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Rezerwacje();
        }
        private void Lend_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Wypozyczenia();
        }

        private void Manage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }
		private void Contractors_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
			glowneOkno.DataContext = new Kontraktorzy();
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
