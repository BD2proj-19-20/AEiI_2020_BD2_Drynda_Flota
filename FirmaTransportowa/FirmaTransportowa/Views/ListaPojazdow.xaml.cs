using FirmaTransportowa.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ListaPojazdow.xaml
    /// </summary>
    public partial class ListaPojazdow : UserControl
    {
        public ListaPojazdow()
        {
            InitializeComponent();
        }

        private void CarStatistics_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow.DataContext = new StatystykiPojazduModel();
        }
    }
}
