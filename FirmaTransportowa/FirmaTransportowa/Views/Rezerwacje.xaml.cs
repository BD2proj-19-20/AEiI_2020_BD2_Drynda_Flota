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
using System.ComponentModel;
using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Rezerwacje.xaml
    /// </summary>
    /// 
    public class ReservationList
    {
        public int ReservationId { get; set; }
        public string Person { get; set; }
        public string ReservationStart { get; set; }
        public string ReservationEnd { get; set; }
        public string ReservationDate { get; set; }
        public string Vehicle { get; set; }


    }
    public partial class Rezerwacje : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();

        public void UpdateView()
        {
            ListaRezerwacji();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("ReservationId", ListSortDirection.Descending));
            view.Filter += UserFilter;
        }
        public Rezerwacje()
        {
            InitializeComponent();
            UpdateView();

            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource);
            // view.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
        }

        public void ListaRezerwacji()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            var reservations = db.Reservations;
            var cars = db.Cars;

            foreach (var reserv in reservations)
            {
                ListViewItem OneItem = new ListViewItem();
                var date = "";
                //checkbox zrob
                var opiekun = "";
                var vehicle = "";

                foreach (var person in people)
                {
                    if (reserv.personId == person.id)
                        opiekun = person.lastName + " " + person.firstName;

                }
                foreach (var car in cars)
                {
                    if (car.id == reserv.carId)
                        vehicle = car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";


                }


                if (reserv.ended == true && ZakonczoneBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.OrangeRed;  //zakonczone 
                    string dateTime = reserv.lendDate.ToString();
                    date = dateTime.Substring(0, 10);

                    OneItem.Content = new ReservationList
                    {
                        ReservationId = reserv.id + 1,
                        Person = opiekun,
                        ReservationStart = date,
                        ReservationEnd = reserv.returnDate.ToString().Substring(0, 10),
                        ReservationDate = reserv.reservationDate.ToString().Substring(0, 10),
                        Vehicle = vehicle
                    };
                    items.Add(OneItem);
                }
                else if (reserv.@private == true && PrywatneBox.IsChecked.Value ==true)
                {
                    OneItem.Background = Brushes.BlueViolet;  //prywatne
                    string dateTime = reserv.lendDate.ToString();
                    date = dateTime.Substring(0, 10);

                    OneItem.Content = new ReservationList
                    {
                        ReservationId = reserv.id + 1,
                        Person = opiekun,
                        ReservationStart = date,
                        ReservationEnd = reserv.returnDate.ToString().Substring(0, 10),
                        ReservationDate = reserv.reservationDate.ToString().Substring(0, 10),
                        Vehicle = vehicle
                    };
                    items.Add(OneItem);
                }
                else if(PozostałeBox.IsChecked.Value == true && reserv.ended == false && reserv.@private == false)
                {
                    string dateTime = reserv.lendDate.ToString();
                    date = dateTime.Substring(0, 10);

                    OneItem.Content = new ReservationList
                    {
                        ReservationId = reserv.id + 1,
                        Person = opiekun,
                        ReservationStart = date,
                        ReservationEnd = reserv.returnDate.ToString().Substring(0, 10),
                        ReservationDate = reserv.reservationDate.ToString().Substring(0, 10),
                        Vehicle = vehicle
                    };
                    items.Add(OneItem);
                }

            }
            ListViewReservations.ItemsSource = items;
        }


        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajRezerwacjeModel();
        }
        private void PrywatneBox_Click(object sender, RoutedEventArgs e)
        {

            ListViewReservations.ItemsSource = null;
            items.Clear();
            UpdateView();

        }
        private void ZakonczoneBox_Click(object sender, RoutedEventArgs e)
        {

            ListViewReservations.ItemsSource = null;
            items.Clear();
            UpdateView();



        }
        private void PozostałeBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewReservations.ItemsSource = null;
            items.Clear();
            UpdateView();
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (dataStartFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ReservationList).ReservationStart.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataStartFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ReservationList).ReservationStart.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }
            if (dataEndFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ReservationList).ReservationEnd.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataEndFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ReservationList).ReservationEnd.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }
            if (dataReservationFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ReservationList).ReservationDate.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataReservationFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as ReservationList).ReservationDate.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }
            if (!String.IsNullOrEmpty(personFilter.Text))
                if (!((toFilter.Content as ReservationList).Person.IndexOf(personFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(idFilter.Text))
                if (!((toFilter.Content as ReservationList).ReservationId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(carFilter.Text))
                if (!((toFilter.Content as ReservationList).Vehicle.ToString().IndexOf(carFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;


            if (!String.IsNullOrEmpty(dataStartFilter.Text))
                if (!((toFilter.Content as ReservationList).ReservationStart.IndexOf(dataStartFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dataEndFilter.Text))
                if (!((toFilter.Content as ReservationList).ReservationEnd.IndexOf(dataEndFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dataReservationFilter.Text))
                if (!((toFilter.Content as ReservationList).ReservationDate.IndexOf(dataReservationFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            return true;
        }

        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource).Refresh();
        }

        private void personFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource).Refresh();
        }
        private void dataStartFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource).Refresh();
        }
        private void dataEndFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource).Refresh();
        }

        private void dataReservationFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource).Refresh();
        }
        private void carFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource).Refresh();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                ListViewReservations.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);

            var tempItems = items.ToArray();

            if (sortBy == "ReservationId")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareReservationByIdAscending);
                else
                    Array.Sort(tempItems, CompareReservationByIdDescending);
            }
            else if (sortBy == "Person")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, ComparePeopleByPersonAscending);
                else
                    Array.Sort(tempItems, ComparePeopleByPersonDescending);
            }
            else if (sortBy == "DateStart")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareReservationByDateStartAscending);
                else
                    Array.Sort(tempItems, CompareReservationByDateStartDescending);
            }
            else if (sortBy == "DateEnd")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareReservationByDateEndAscending);
                else
                    Array.Sort(tempItems, CompareReservationByDateEndDescending);
            }
            else if (sortBy == "DateReservation")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, ComparePeopleByDateReservationAscending);
                else
                    Array.Sort(tempItems, ComparePeopleByDateReservationDescending);
            }
            else if (sortBy == "Car")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareCarAscending);
                else
                    Array.Sort(tempItems, CompareCarDescending);
            }


            ListViewReservations.ItemsSource = tempItems;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewReservations.ItemsSource);
            //   view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));
            view.Filter += UserFilter;

        }
        int CompareReservationByIdAscending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            return first.ReservationId.CompareTo(second.ReservationId);
        }
        int CompareReservationByIdDescending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            return second.ReservationId.CompareTo(first.ReservationId);
        }
        int ComparePeopleByPersonAscending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            return String.Compare(first.Person, second.Person);

        }
        int ComparePeopleByPersonDescending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            return String.Compare(second.Person, first.Person);
        }
        int CompareCarAscending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            return String.Compare(first.Vehicle, second.Vehicle);

        }
        int CompareCarDescending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            return String.Compare(second.Vehicle, first.Vehicle);
        }

        int CompareReservationByDateStartAscending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.ReservationStart.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.ReservationStart);
            else
                firstDate = DateTime.MinValue;
            if (second.ReservationStart.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.ReservationStart);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);
        }
        int CompareReservationByDateStartDescending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.ReservationStart.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.ReservationStart);
            else
                firstDate = DateTime.MinValue;
            if (second.ReservationStart.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.ReservationStart);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }
        int CompareReservationByDateEndAscending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.ReservationEnd.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.ReservationEnd);
            else
                firstDate = DateTime.MinValue;
            if (second.ReservationEnd.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.ReservationEnd);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);


        }
        int CompareReservationByDateEndDescending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.ReservationEnd.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.ReservationEnd);
            else
                firstDate = DateTime.MinValue;
            if (second.ReservationEnd.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.ReservationEnd);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }
        int ComparePeopleByDateReservationAscending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.ReservationDate.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.ReservationDate);
            else
                firstDate = DateTime.MinValue;
            if (second.ReservationDate.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.ReservationDate);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);


        }
        int ComparePeopleByDateReservationDescending(ListViewItem a, ListViewItem b)
        {
            ReservationList first = (ReservationList)a.Content;
            ReservationList second = (ReservationList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.ReservationDate.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.ReservationDate);
            else
                firstDate = DateTime.MinValue;
            if (second.ReservationDate.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.ReservationDate);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }
    }
}
