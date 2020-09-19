using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using FirmaTransportowa.Model;
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

        private void carList_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            carList.UnselectAll();
        }


        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                ItemList selectedObj = (ItemList)selected.Content;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

                var car = (from cars in db.Cars
                           where cars.id == selectedObj.carId
                           select cars).FirstOrDefault();

                System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                glowneOkno.DataContext = new WybierzTypRaportu(this, car);
                //RaportGenerator.GenerateRaportAboutOneCar(car);
            }
            else
            {
                System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                glowneOkno.DataContext = new WybierzTypRaportu(this, null);
            }
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
                MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć pojazd " + selectedObj.registration + "?"
                                                          ,"Potwierdź usunięcie", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
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
                            result = MessageBox.Show("Istnieją rezerwacje powiązane z " + selectedObj.registration 
                                                                        + ", które również zostaną usunięte, czy chcesz kontynuować?"
                                                                        , "Potwierdź usunięcie", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                            if (result != MessageBoxResult.Yes)
                            {
                                return;
                            }
                            db.Reservations.Remove(reservation);
                        }
                    }

                    var lends = db.Lends;
                    foreach (var lend in lends)
                    {
                        if (lend.carId == selectedId)
                        {
                            result = MessageBox.Show("Istnieją wypożyczenia powiązane z " + selectedObj.registration
                                                                        + ", które również zostaną usunięte, czy chcesz kontynuować?"
                                                                        , "Potwierdź usunięcie", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                            if (result != MessageBoxResult.Yes)
                            {
                                return;
                            }
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
                        ZmianaOpiekuna zmianaOpiekunaView = new ZmianaOpiekuna(car, selectedObj)
                        {
                            Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2,
                            Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2
                        };
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

