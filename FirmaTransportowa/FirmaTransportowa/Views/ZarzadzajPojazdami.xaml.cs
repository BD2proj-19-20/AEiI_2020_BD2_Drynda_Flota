using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using Brushes = System.Windows.Media.Brushes;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZarzadzajPojazdami.xaml
    /// </summary>
    /// 
    public class ItemList
    {
        public int carId { get; set; }

        public string registration { get; set; }

        public string carSupervisor { get; set; }

        public string saleDate { get; set; }
    }
    public partial class ZarzadzajPojazdami : UserControl
    {

        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        List<ListViewItem> items = new List<ListViewItem>();

        public ZarzadzajPojazdami()
        {
            InitializeComponent();
            initializeList();
        }

        private void initializeList()
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;
            var carSupervisors = db.CarSupervisors;
            var people = db.People;

            foreach (var car in cars)
            {
                string supervisorString = "Brak";

                foreach (var supervisor in carSupervisors)
                {
                    if (supervisor.carId == car.id && supervisor.endDate == null)
                    {
                        supervisorString = supervisor.Person.firstName + " " + supervisor.Person.lastName;
                        break;
                    }
                }

                ListViewItem OneItem = new ListViewItem();
                string saleDate = "";
                if (car.saleDate != null)
                {
                    if (car.saleDate <= DateTime.Today)
                    {
                        OneItem.Background = Brushes.LightGray;
                    }
                    DateTime temp = (DateTime)car.saleDate;
                    saleDate = temp.ToShortDateString();
                }
                OneItem.Content = new ItemList { carId = car.id, registration = car.Registration, carSupervisor = supervisorString, saleDate = saleDate };
                items.Add(OneItem);
            }
            carList.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("carId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }


        private void Generuj_Raport(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carSupervisors = db.CarSupervisors;
            var cars = db.Cars;
            var people = db.People;

            iTextSharp.text.Font times = FontFactory.GetFont("Arial", 32, new BaseColor(System.Drawing.Color.Black));
            iTextSharp.text.Font times2 = FontFactory.GetFont("Arial", 20, new BaseColor(System.Drawing.Color.Black));
            FileStream fs = new FileStream("Raport na temat pojazdow " + DateTime.Now.ToShortDateString() + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            foreach (var car in cars)
            {
                doc.Add(new iTextSharp.text.Paragraph(car.id + " " + car.Registration + " "  + car.CarModel.make + " " + car.CarModel.model , times));
                doc.Add(new iTextSharp.text.Paragraph("Opiekunowie: ", times2));
                foreach (var carSupervisor in carSupervisors)
                {
                    if (car.id == carSupervisor.carId)
                    {
                        carSupervisor.beginDate.ToShortDateString();
                        string endDate = "";
                        if (carSupervisor.endDate != null)
                        {
                            DateTime temp = (DateTime)carSupervisor.endDate;
                            endDate = temp.ToShortDateString();
                        }
                        doc.Add(new iTextSharp.text.Chunk(carSupervisor.Person.id + " " + carSupervisor.Person.firstName + " " + carSupervisor.Person.lastName + ": " + carSupervisor.beginDate.ToShortDateString() + " - " + endDate + "\n"));
                    }
                }

            }
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
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            ItemList selectedObj = (ItemList)selected.Content;
            int selectedId = selectedObj.carId;
            //Usuwam zaznaczony samochód z listy
            items.Remove((ListViewItem)carList.SelectedItem);
            carList.ItemsSource = items;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Usuwam zaznaczony samochód z bazy
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    db.Cars.Remove(car);
                    break;
                }
            }
            db.SaveChanges();
        }


        private void Sprzedaj_Pojazd(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            ItemList selectedObj = (ItemList)selected.Content;
            int selectedId = selectedObj.carId;
            selectedObj.saleDate = DateTime.Now.ToShortDateString();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Ustawiam datę sprzedaży
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    car.saleDate = DateTime.Today;
                    break;
                }
            }
            db.SaveChanges();
            //Odświeżenie listy
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }

        private void Kup_Pojazd(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            ItemList selectedObj = (ItemList)selected.Content;
            int selectedId = selectedObj.carId;
            selectedObj.saleDate = null;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Usuwam datę sprzedaży
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    car.saleDate = null;
                    break;
                }
            }
            db.SaveChanges();
            //Odświeżenie listy
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new ZarzadzajPojazdami();
        }

        private void Zmiana_Opiekuna(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            ItemList selectedObj = (ItemList)selected.Content;
            int selectedId = selectedObj.carId;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Wysyłam zaznaczony samochód do zmiany opiekuna
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    ZmianaOpiekuna zmianaOpiekunaView = new ZmianaOpiekuna(car, selectedObj);
                    zmianaOpiekunaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
                    zmianaOpiekunaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
                    zmianaOpiekunaView.ShowDialog();
                    while (zmianaOpiekunaView.IsActive)
                    {

                    }
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new ZarzadzajPojazdami();
                    return;
                }
            }
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (!String.IsNullOrEmpty(carSupervisorFilter.Text))
                //jezeli item nie spelnia filtra opiekuna nie wyswietlam go
                if (!((toFilter.Content as ItemList).carSupervisor.IndexOf(carSupervisorFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(registrationFiler.Text))
                //jezeli item nie spelnia filtra rejestracji nie wyswietlam go
                if (!((toFilter.Content as ItemList).registration.IndexOf(registrationFiler.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(idFilter.Text))
                //jezeli item nie spelnia filtra id nie wyswietlam go
                if (!((toFilter.Content as ItemList).carId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(saleDateFilter.Text))
                //jezeli item nie spelnia filtra id nie wyswietlam go
                if (!((toFilter.Content as ItemList).saleDate.IndexOf(saleDateFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            //cała reszte wyswietlam
            return true;
        }


        private void CarStatistics_Click(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            ItemList selectedObj = (ItemList)selected.Content;
            int selectedId = selectedObj.carId;
            selectedObj.saleDate = null;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Usuwam datę sprzedaży
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    StatystykiPojazdu statystykiPojazduView = new StatystykiPojazdu(car);
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = statystykiPojazduView;
                    return;
                }
            }

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

        private void saleDateFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
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

            var tempItems = items.ToArray();
            if (sortBy == "carId")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareCarsByIdAscending);
                else
                    Array.Sort(tempItems, CompareCarsByIdDescending);
            }
            else if (sortBy == "registration")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareCarsByRegistrationAscending);
                else
                    Array.Sort(tempItems, CompareCarsByRegistrationDescending);
            }
            else if (sortBy == "carSupervisor")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareCarsBySupervisorAscending);
                else
                    Array.Sort(tempItems, CompareCarsBySupervisorDescending);
            }
            else if (sortBy == "saleDate")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareCarsBySaleDateAscending);
                else
                    Array.Sort(tempItems, CompareCarsBySaleDateDescending);
            }

            carList.ItemsSource = tempItems;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
        }
        int CompareCarsByIdAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return first.carId.CompareTo(second.carId);
        }

        int CompareCarsByIdDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return second.carId.CompareTo(first.carId);
        }

        int CompareCarsByRegistrationDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(first.registration, second.registration);
        }

        int CompareCarsByRegistrationAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(second.registration, first.registration);
        }

        int CompareCarsBySupervisorDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(first.carSupervisor, second.carSupervisor);
        }

        int CompareCarsBySupervisorAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(second.carSupervisor, first.carSupervisor);
        }

        int CompareCarsBySaleDateDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.saleDate.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.saleDate);
            else
                firstDate = DateTime.MinValue;
            if (second.saleDate.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.saleDate);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);
        }

        int CompareCarsBySaleDateAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.saleDate.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.saleDate);
            else
                firstDate = DateTime.MinValue;
            if (second.saleDate.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.saleDate);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }

    }
}

