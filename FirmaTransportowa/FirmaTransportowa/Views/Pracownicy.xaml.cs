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
using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Pracownicy.xaml
    /// </summary>
    /// 
    public class WorkersList
    {
        public string Person { get; set; }
    }


    public partial class Pracownicy : UserControl
    {

        List<WorkersList> items = new List<WorkersList>();

        public Pracownicy()
        {
            InitializeComponent();
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            foreach (var person in people)
            {
                items.Add(new WorkersList { Person = person.firstName + " " + person.lastName });
            }
            
            workersList.ItemsSource = items;

            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            //  view.Filter += UserFilter;


        }
        private void Dodaj_Pracownika(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajPracownikModel();
        }


        private void WorkerStatistics_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow.DataContext = new StatystykiPracownikaModel();
        }

    }
}
