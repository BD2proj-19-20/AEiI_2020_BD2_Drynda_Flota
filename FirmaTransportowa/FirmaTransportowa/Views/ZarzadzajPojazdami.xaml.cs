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

        public DateTime saleDate { get; set; }
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
                        foreach (var human in people)
                        {
                            if (human.id == supervisor.personId)
                            {
                                supervisorString = human.firstName + " " + human.lastName;
                            }
                        }
                    }
                }

                ListViewItem OneItem = new ListViewItem();
                DateTime today = DateTime.Today;
                if (car.saleDate <= today )
                {
                    OneItem.Background = Brushes.Red;
                }
                OneItem.Content = new ItemList { carId = car.id, registration = car.Registration, carSupervisor = supervisorString, saleDate = DateTime.MaxValue };
                items.Add(OneItem);
            }
            carList.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.Filter += UserFilter;
        }

        private void Generuj_Raport(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carSupervisors = db.CarSupervisors;
            var cars = db.Cars;
            var people = db.People;

            iTextSharp.text.Font times = FontFactory.GetFont("Arial", 28, new BaseColor(System.Drawing.Color.Black));
            FileStream fs = new FileStream("Raport na temat pojazdu.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            bool carSupervisorWritten = false;

            foreach(var human in people)
            {
                carSupervisorWritten = false;

                foreach (var carSupervisor in carSupervisors)
                {
                    if (human.id == carSupervisor.personId)
                    {
                        if (carSupervisorWritten == false)
                        {
                            doc.Add(new iTextSharp.text.Paragraph(human.id + " " + human.firstName + " " + human.lastName + " ma pod opieka:", times));
                            carSupervisorWritten = true;
                        }
                        
                        foreach (var car in cars)
                        {
                            if (car.id == carSupervisor.carId)
                            {
                                doc.Add(new iTextSharp.text.Chunk("    "+car.id + " " + car.CarModel.make + " " + car.CarModel.model + "\n"));
                            }
                        }
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
            selectedObj.saleDate = DateTime.Today;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            //Ustawiam datę sprzedaży
            foreach (var car in cars)
            {
                if (car.id == selectedId)
                {
                    car.saleDate = DateTime.Today;
                }
            }
            db.SaveChanges();
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
                    while(zmianaOpiekunaView.IsActive)
                    {

                    }
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new ZarzadzajPojazdami();
                    /*carList.ItemsSource = items;
                    CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();*/
                    return;
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
                if (!((item as ItemList).carSupervisor.IndexOf(carSupervisorFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(registrationFiler.Text))
                //jezeli item nie spelnia filtra rejestracji nie wyswietlam go
                if (!((item as ItemList).registration.IndexOf(registrationFiler.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(idFilter.Text))
                //jezeli item nie spelnia filtra id nie wyswietlam go
                if (!((item as ItemList).carId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
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

