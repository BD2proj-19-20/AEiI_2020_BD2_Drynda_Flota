using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using FirmaTransportowa.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;

namespace FirmaTransportowa.Views
{
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


        private void BuyCar(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajPojazd(this);
        }

        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "Dokument pdf|*.pdf";
            saveFileDialog.Title = "Zapisz raport na temat pojazdów";
            saveFileDialog.ShowDialog();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            iTextSharp.text.Font times = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times.Size = 32;
            iTextSharp.text.Font times2 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times2.Size = 20;
            iTextSharp.text.Font times3 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times3.Size = 14;
            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            foreach (var car in cars)
            {
                //SAMOCHÓD
                doc.Add(new iTextSharp.text.Paragraph(car.id + "   " + car.Registration + "\n", times));
                //SAMOCHÓD

                //DATA ZAKUPU I SPRZEDAŻY
                doc.Add(new iTextSharp.text.Paragraph("Data zakupu: " + car.purchaseDate.ToShortDateString(), times2));
                if (car.saleDate != null)
                    doc.Add(new iTextSharp.text.Paragraph("Data sprzedaży: " + car.saleDate, times2));
                //DATA ZAKUPU I SPRZEDAŻY

                //CZY SPRAWNY
                string onService = car.onService == false ? "Tak" : "Nie";
                doc.Add(new iTextSharp.text.Paragraph("Sprawny? " + onService, times2));
                //CZY SPRAWNY

                //DANE POJAZDU
                doc.Add(new iTextSharp.text.Chunk("Marka: " + car.CarModel.make + "\n", times3));
                doc.Add(new iTextSharp.text.Chunk("Model: " + car.CarModel.model + "\n", times3));
                doc.Add(new iTextSharp.text.Chunk("Pojemność silnika: " + car.engineCapacity + "\n", times3));
                doc.Add(new iTextSharp.text.Chunk("Zastosowanie: " + car.CarDestination.name + "\n", times3));
                doc.Add(new iTextSharp.text.Chunk("Przegląd ważny do: " + car.inspectionValidUntil.ToShortDateString() + "\n\n", times3));
                //DANE POJAZDU

                //HISTORIA OPIEKUNÓW
                var carSupervisors = from carSupervisor in db.CarSupervisors
                                     where carSupervisor.carId == car.id
                                     orderby carSupervisor.beginDate
                                     select new
                                     {
                                         carSupervisor.Person,
                                         BeginDate = carSupervisor.beginDate,
                                         EndDate = carSupervisor.endDate
                                     };

                if (carSupervisors.Count() != 0)
                    doc.Add(new iTextSharp.text.Paragraph("Historia opiekunów: ", times2));
                foreach (var carSupervisor in carSupervisors)
                {
                    carSupervisor.BeginDate.ToShortDateString();
                    string endDate = "";
                    if (carSupervisor.EndDate != null)
                    {
                        endDate = carSupervisor.EndDate.ToString();
                    }
                    doc.Add(new iTextSharp.text.Chunk(carSupervisor.Person.id + " " + carSupervisor.Person.firstName + " "
                        + carSupervisor.Person.lastName + ": " + carSupervisor.BeginDate.ToString() + " - "
                        + endDate + "\n", times3));
                }
                //HISTORIA OPIEKUNÓW

                //HISTORIA WYPOŻYCZEŃ
                var carLends = from carLend in db.Lends
                               where carLend.carId == car.id
                               orderby carLend.lendDate
                               select new
                               {
                                   carLend.Person,
                                   BeginDate = carLend.lendDate,
                                   EndDate = carLend.returnDate
                               };

                if (carLends.Count() != 0)
                    doc.Add(new iTextSharp.text.Paragraph("\nHistoria wypożyczeń: ", times2));
                foreach (var carLend in carLends)
                {
                    carLend.BeginDate.ToShortDateString();
                    string endDate = "";
                    if (carLend.EndDate != null)
                    {
                        endDate = carLend.EndDate.ToString();
                    }
                    doc.Add(new iTextSharp.text.Chunk(carLend.Person.id + " " + carLend.Person.firstName + " "
                        + carLend.Person.lastName + ": " + carLend.BeginDate.ToString() + " - "
                        + endDate + "\n", times3));
                }
                //HISTORIA WYPOŻYCZEŃ

                //HISTORIA AKTYWNOŚCI
                var carActivities = from carActivity in db.Activities
                               where carActivity.carId == car.id
                               orderby carActivity.reportDate
                               select new
                               {
                                   Name = carActivity.comments,
                                   BeginDate = carActivity.reportDate,
                                   EndDate = carActivity.closeDate
                               };

                if (carActivities.Count() != 0)
                    doc.Add(new iTextSharp.text.Paragraph("\nHistoria aktywności: ", times2));
                foreach (var carActivity in carActivities)
                {
                    carActivity.BeginDate.ToShortDateString();
                    string endDate = "";
                    if (carActivity.EndDate != null)
                    {
                        endDate = carActivity.EndDate.ToString();
                    }
                    doc.Add(new iTextSharp.text.Chunk(carActivity.Name + " " + carActivity.BeginDate.ToString() + " - "
                        + endDate + "\n", times3));
                }
                //HISTORIA AKTYWNOŚCI

                doc.NewPage();
            }
            doc.Close();
            stopwatch.Stop();
            MessageBox.Show("Raport wygenerowany w czasie " + stopwatch.Elapsed + "!");
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

            CarComparator carComparator = new CarComparator();
            var tempItems = items.ToArray();
            if (sortBy == "carId")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, carComparator.CompareCarsByIdAscending);
                else
                    Array.Sort(tempItems, carComparator.CompareCarsByIdDescending);
            }
            else if (sortBy == "registration")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, carComparator.CompareCarsByRegistrationAscending);
                else
                    Array.Sort(tempItems, carComparator.CompareCarsByRegistrationDescending);
            }
            else if (sortBy == "carSupervisor")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, carComparator.CompareCarsBySupervisorAscending);
                else
                    Array.Sort(tempItems, carComparator.CompareCarsBySupervisorDescending);
            }
            else if (sortBy == "saleDate")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, carComparator.CompareCarsBySaleDateAscending);
                else
                    Array.Sort(tempItems, carComparator.CompareCarsBySaleDateDescending);
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
            Stopwatch stoper = new Stopwatch();
            stoper.Start();

            items.Clear();
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from car in db.Cars
                        join supervisor in db.CarSupervisors on car.id equals supervisor.carId into final
                        from f in final.DefaultIfEmpty()
                        where f.endDate == null
                        select new
                        {
                            SupervisorName = f == null ? "Brak" : f.Person.lastName + " " + f.Person.firstName,
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
            Array.Sort(items.ToArray(), new CarComparator().CompareCarsByIdAscending);
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
                        if (car.saleDate == null)
                        {
                            MessageBox.Show("Nie można kupić niesprzedanego samochodu!", "Błąd przy zakupie!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        car.saleDate = null;
                        car.purchaseDate = DateTime.Today;
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
                        if (car.saleDate != null)
                        {
                            MessageBox.Show("Nie można sprzedać sprzedanego samochodu!", "Błąd przy sprzedaży!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        car.saleDate = DateTime.Now;
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

