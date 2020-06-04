using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using MaterialDesignThemes.Wpf;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZarzadzajPojazdami.xaml
    /// </summary>
    /// 
    public class ItemList
    {
        public int CarId { get; set; }

        public string Registration { get; set; }

        public string CarSupervisor { get; set; }
    }
    public partial class ZarzadzajPojazdami : UserControl
    {

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        public ZarzadzajPojazdami()
        {
            InitializeComponent();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            var carSupervisors = db.CarSupervisors;
            var people = db.People;

            List<ItemList> items = new List<ItemList>();

            foreach (var car in cars)
            {
                string supervisorString = "Brak";

                foreach (var supervisor in carSupervisors)
                {
                    if (supervisor.carId == car.id)
                    {
                        foreach (var human in people)
                        {
                            if (human.id == supervisor.personId)
                            {
                                supervisorString = human.firstName + " " + human.lastName;
                            }
                        }
                    }
                }

                items.Add(new ItemList { CarId = car.id, Registration = car.Registration, CarSupervisor = supervisorString });
            }
            carList.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.Filter += UserFilter;
        }
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void Generuj_Raport(object sender, RoutedEventArgs e)
        {
            int selectedCarId = 1;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            foreach (var c in cars)
            {
                int id = c.id;
                int eC = c.engineCapacity;
            }

            FileStream fs = new FileStream("Raport na temat pojazdu.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Zawartosc raportu"));
            doc.Close();
        }

        private void Dodaj_Pojazd(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajPojazdModel();
        }

        private void Usun_Pojazd(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ItemList selected = (ItemList)carList.SelectedItem;
            int selectedId = selected.CarId;
            //Usuwam zaznaczony samochód z listy
            carList.Items.RemoveAt(carList.SelectedIndex);

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Usuwam zaznaczony samochód z bazy
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    db.Cars.Remove(car);
                }
            }
            db.SaveChanges();
        }

        private void Zmiana_Opiekuna(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ItemList selected = (ItemList)carList.SelectedItem;
            int selectedId = selected.CarId;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Wysyłam zaznaczony samochód do zmiany opiekuna
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    ZmianaOpiekuna zmianaOpiekunaView = new ZmianaOpiekuna(car);
                    zmianaOpiekunaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
                    zmianaOpiekunaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
                    zmianaOpiekunaView.ShowDialog();
                }
            }
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                carList.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            carList.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private bool UserFilter(object item)
        {
            if (!String.IsNullOrEmpty(carSupervisorFilter.Text))
                //jezeli item nie spelnia filtra opiekuna nie wyswietlam go
                if (!((item as ItemList).CarSupervisor.IndexOf(carSupervisorFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(registrationFiler.Text))
                //jezeli item nie spelnia filtra rejestracji nie wyswietlam go
                if (!((item as ItemList).Registration.IndexOf(registrationFiler.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(idFilter.Text))
                //jezeli item nie spelnia filtra id nie wyswietlam go
                if (!((item as ItemList).CarId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            //cała reszte wyswietlam
            return true;
        }


        private void CarStatistics_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow.DataContext = new StatystykiPojazduModel();
        }

        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }

        private void registrationFiler_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }

        private void carSupervisorFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }
    }
}

