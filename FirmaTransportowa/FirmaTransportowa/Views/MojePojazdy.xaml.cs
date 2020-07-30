using FirmaTransportowa.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for MojePojazdy.xaml
    /// </summary>
    public partial class MojePojazdy : UserControl
    {
        public MojePojazdy()
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
