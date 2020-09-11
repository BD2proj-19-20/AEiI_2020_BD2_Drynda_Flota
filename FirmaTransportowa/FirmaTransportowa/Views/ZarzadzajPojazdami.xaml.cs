using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Brushes = System.Windows.Media.Brushes;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZarzadzajPojazdami.xaml
    /// </summary>
    /// 
    public class ItemList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChange(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public int carId { get; set; }

        private string CarSupervisor;

        public string carSupervisor
        {
            get
            {
                return this.CarSupervisor;
            }
            set
            {
                this.CarSupervisor = value;
                RaisePropertyChange("carSupervisor");
            }
        }

        public string registration { get; set; }

        private string SaleDate;

        public string saleDate
        {
            get
            {
                return this.SaleDate;
            }
            set
            {
                this.SaleDate = value;
                RaisePropertyChange("saleDate");
            }
        }
    }
    public partial class ZarzadzajPojazdami : UserControl
    {

        private ObservableCollection<ListViewItem> items = new ObservableCollection<ListViewItem>();
        private SortAdorner listViewSortAdorner = null;
        private GridViewColumnHeader listViewSortCol = null;
        private GridViewColumnHeader sortingColumn = null;
        Stopwatch stoper;
        public ZarzadzajPojazdami()
        {
            InitializeComponent();
            InitializeList();
        }

        private void CarStatisticsClick(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                ItemList selectedObj = (ItemList)selected.Content;
                int selectedId = selectedObj.carId;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var cars = db.Cars;

                //Usuwam datę sprzedaży
                foreach (var car in cars)
                {
                    if (car.id == selectedId)
                    {
                        StatystykiPojazdu statystykiPojazduView = new StatystykiPojazdu(car, 2);
                        System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                        glowneOkno.DataContext = statystykiPojazduView;
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }

        }

        private void CarSupervisorFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
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

        int CompareCarsByRegistrationAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(second.registration, first.registration);
        }

        int CompareCarsByRegistrationDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(first.registration, second.registration);
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

        int CompareCarsBySupervisorAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(second.carSupervisor, first.carSupervisor);
        }

        int CompareCarsBySupervisorDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(first.carSupervisor, second.carSupervisor);
        }

        private void BuyCar(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajPojazd(this);
        }

        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carSupervisors = db.CarSupervisors;
            var cars = db.Cars;
            var people = db.People;

            iTextSharp.text.Font times = FontFactory.GetFont("Arial", 32, new BaseColor(System.Drawing.Color.Black));
            iTextSharp.text.Font times2 = FontFactory.GetFont("Arial", 20, new BaseColor(System.Drawing.Color.Black));
            iTextSharp.text.Font times3 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times3.Size = 14;
            FileStream fs = new FileStream("Raport na temat pojazdow " + DateTime.Now.ToShortDateString() + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            foreach (var car in cars)
            {
                doc.Add(new iTextSharp.text.Paragraph(car.id + " " + car.Registration + " " + car.CarModel.make + " " + car.CarModel.model, times));
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
                        doc.Add(new iTextSharp.text.Chunk(carSupervisor.Person.id + " " + carSupervisor.Person.firstName + " " + carSupervisor.Person.lastName + ": " + carSupervisor.beginDate.ToShortDateString() + " - " + endDate + "\n", times3));
                    }
                }

            }
            doc.Close();
        }

        private void GridViewColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            sortingColumn = column;
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
            carList.Items.Refresh();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);

            view.Filter += UserFilter;
        }

        private void IdFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }

        private void InitializeList()
        {
            stoper = new Stopwatch();
            stoper.Start();

            items.Clear();
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from car in db.Cars
                        join supervisor in db.CarSupervisors on car.id equals supervisor.carId into final
                        from f in final.DefaultIfEmpty() where f.endDate == null || f.endDate > DateTime.Today
                        select new
                        {
                            SupervisorName = f == null ? "Brak" : f.Person.firstName + " " + f.Person.lastName,
                            CarRegistration = car.Registration,
                            CarId = car.id,
                            SaleDate = car.saleDate
                        };

            foreach (var car in query)
            {
                ListViewItem OneItem = new ListViewItem();
                string saleDate = "";
                if (car.SaleDate != null)
                {
                    if (car.SaleDate <= DateTime.Today)
                    {
                        OneItem.Background = Brushes.LightGray;
                    }
                    DateTime temp = (DateTime)car.SaleDate;
                    saleDate = temp.ToShortDateString();
                }
                OneItem.Content = new ItemList { carId = car.CarId, registration = car.CarRegistration, carSupervisor = car.SupervisorName, saleDate = saleDate };
                items.Add(OneItem);
            }
            Array.Sort(items.ToArray(), CompareCarsByIdAscending);
            carList.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.Filter += UserFilter;

            stoper.Stop();
            Title.Text = stoper.Elapsed.ToString();
        }
        private void RepurchaseCar(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
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
                selected.Background = Brushes.White;
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }
        }

        private void RegistrationFilerTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }

        private void SaleDateFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }

        private void SellCar(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
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
                selected.Background = Brushes.LightGray;
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (saleDateFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ItemList).saleDate.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (saleDateFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ItemList).saleDate.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

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

        private void DeleteCar(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
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
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    var reservations = db.Reservations;
                    foreach (var reservation in reservations)
                    {
                        if (reservation.carId == selectedId)
                        {
                            db.Reservations.Remove(reservation);
                        }
                    }

                    var lends = db.Lends;
                    foreach (var lend in lends)
                    {
                        if (lend.carId == selectedId)
                        {
                            db.Lends.Remove(lend);
                        }
                    }

                    MessageBox.Show("Usunięto zarezerwowany samochód, powiązane rezerwacje zostały również usunięte.", "Informacja");
                    db.SaveChanges();
                }

                items.Remove(selected);
                carList.ItemsSource = items;
                carList.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }
        }
        private void ChangeCarSupervisor(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
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
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }
        }
    }
}

