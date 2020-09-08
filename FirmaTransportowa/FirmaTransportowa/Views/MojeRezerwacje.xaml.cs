using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.ComponentModel;
using FirmaTransportowa.Model;
using System.Windows.Media;
using System.Windows.Data;
using System;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows.Forms;
using FirmaTransportowa.ViewModels;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for MojeRezerwacje.xaml
    /// </summary>
    /// 
    public class MyReservationList
    {
        public int ReservationId { get; set; }
        public string ReservationStart { get; set; }
        public string ReservationEnd { get; set; }
        public string ReservationDate { get; set; }
        public string Vehicle { get; set; }
    }
    public partial class MojeRezerwacje : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();

        public void UpdateView()
        {
            MojaListaRezerwacji();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("ReservationId", ListSortDirection.Descending));
            view.Filter += UserFilter;
        }
        public MojeRezerwacje()
        {

            InitializeComponent();
            UpdateView();
        }

        public void MojaListaRezerwacji()
        {
            int id = Logowanie.actualUser.id;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
            var reservations = db.Reservations;
            var cars = db.Cars;
            foreach (var reserv in reservations)
            {
                ListViewItem OneItem = new ListViewItem();
                var date = "";

                var vehicle = "";
                if (reserv.personId == id)
                {

                    foreach (var car in cars)
                    {
                        if (car.id == reserv.carId)
                        {
                            vehicle = car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                            break;
                        }
                    }
                    if (reserv.ended == true && ZakonczoneBox.IsChecked.Value == true)
                    {
                        if (reserv.@private == true)
                            OneItem.Background = Brushes.Red;  //zakonczone prywatne
                        else
                            OneItem.Background = Brushes.OrangeRed; //zakonczone nie prywatne
                        string dateTime = reserv.lendDate.ToString();
                        date = dateTime.Substring(0, 10);

                        OneItem.Content = new MyReservationList
                        {
                            ReservationId = reserv.id + 1,

                            ReservationStart = date,
                            ReservationEnd = reserv.returnDate.ToString().Substring(0, 10),
                            ReservationDate = reserv.reservationDate.ToString().Substring(0, 10),
                            Vehicle = vehicle
                        };
                        items.Add(OneItem);
                    }
                    else if (reserv.@private == true && PrywatneBox.IsChecked.Value == true && reserv.ended == false)
                    {
                        OneItem.Background = Brushes.BlueViolet;  //prywatne
                        string dateTime = reserv.lendDate.ToString();
                        date = dateTime.Substring(0, 10);

                        OneItem.Content = new MyReservationList
                        {
                            ReservationId = reserv.id + 1,
                            ReservationStart = date,
                            ReservationEnd = reserv.returnDate.ToString().Substring(0, 10),
                            ReservationDate = reserv.reservationDate.ToString().Substring(0, 10),
                            Vehicle = vehicle
                        };
                        items.Add(OneItem);
                    }

                    else if (PozostałeBox.IsChecked.Value == true && reserv.ended == false && reserv.@private == false)
                    {
                        string dateTime = reserv.lendDate.ToString();
                        date = dateTime.Substring(0, 10);

                        OneItem.Content = new MyReservationList
                        {
                            ReservationId = reserv.id + 1,
                            ReservationStart = date,
                            ReservationEnd = reserv.returnDate.ToString().Substring(0, 10),
                            ReservationDate = reserv.reservationDate.ToString().Substring(0, 10),
                            Vehicle = vehicle
                        };
                        items.Add(OneItem);
                    }
                }
            }

            ListViewMyReservations.ItemsSource = items;


        }
        private void Dodaj_Rezerwacje(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajRezerwacjeModel();
        }
        private void Modyfikuj_Rezerwacje(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewMyReservations.SelectedItem;
            if (selected != null)
            {
                MyReservationList selectedObj = (MyReservationList)selected.Content;
                int selectedId = selectedObj.ReservationId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                //var people = db.People;
                var reservations = db.Reservations;
                Reservation reservationChange = null;

                foreach (var reserv in reservations)
                {
                    if (reserv.id == selectedId)
                        reservationChange = reserv;
                }
                if (reservationChange.ended == false)
                {
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new ZmienRezerwacjePracownik(reservationChange);

                }
                else
                    MessageBox.Show("Rezerwacja się zakończyła!", "Komunikat");
            }
            else
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
        }
        private void Zakoncz_Rezerwacje(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewMyReservations.SelectedItem;

            if (selected != null)
            {
                MyReservationList selectedObj = (MyReservationList)selected.Content;

                int selectedId = selectedObj.ReservationId - 1;
              //  var reservationPerson = selectedObj.Person;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                //var people = db.People;
                var reservations = db.Reservations;
                //  var cars = db.Cars;
                Reservation reservationChange = null;

                foreach (var reserv in reservations)
                {
                    if (reserv.id == selectedId)
                        reservationChange = reserv;
                }

                if (reservationChange.ended == true)
                    MessageBox.Show("Rezerwacja się zakończyła!", "Komunikat");
                else
                {

                    DialogResult result = MessageBox.Show("Czy chcesz zakonczyc rezerwację ?"
                        , "Komunikat", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {

                        reservationChange.ended = true;
                        var lends = db.Lends;
                        foreach (var lend in lends)
                        {
                            if (lend.reservationId == reservationChange.id)
                            {
                                lend.returnDate = Convert.ToDateTime(DateTime.Now);
                                lend.plannedReturnDate = Convert.ToDateTime(DateTime.Now);

                                lend.comments = "Zakończono przez zakończenie\nrezerwacji przez pracownika - " + DateTime.Now.ToString();
                            }

                        }
                        db.SaveChanges();

                        ListViewMyReservations.ItemsSource = null;
                        items.Clear();
                        UpdateView();

                    }
                    else if (result == DialogResult.No)
                    {
                    }

                }
            }
            else
                MessageBox.Show("Nic nie wybrano !", "Komunikat");
        }
        private void PrywatneBox_Click(object sender, RoutedEventArgs e)
        {

            ListViewMyReservations.ItemsSource = null;
            items.Clear();
            UpdateView();

        }
        private void ZakonczoneBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewMyReservations.ItemsSource = null;
            items.Clear();
            UpdateView();
        }
        private void PozostałeBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewMyReservations.ItemsSource = null;
            items.Clear();
            UpdateView();
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (dataStartFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyReservationList).ReservationStart.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataStartFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyReservationList).ReservationStart.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }
            if (dataEndFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyReservationList).ReservationEnd.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataEndFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyReservationList).ReservationEnd.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }
            if (dataReservationFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyReservationList).ReservationDate.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataReservationFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyReservationList).ReservationDate.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (!String.IsNullOrEmpty(idFilter.Text))
                if (!((toFilter.Content as MyReservationList).ReservationId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(carFilter.Text))
                if (!((toFilter.Content as MyReservationList).Vehicle.ToString().IndexOf(carFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;


            if (!String.IsNullOrEmpty(dataStartFilter.Text))
                if (!((toFilter.Content as MyReservationList).ReservationStart.IndexOf(dataStartFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dataEndFilter.Text))
                if (!((toFilter.Content as MyReservationList).ReservationEnd.IndexOf(dataEndFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dataReservationFilter.Text))
                if (!((toFilter.Content as MyReservationList).ReservationDate.IndexOf(dataReservationFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            return true;
        }
        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource).Refresh();
        }
        private void dataStartFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource).Refresh();
        }
        private void dataEndFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource).Refresh();
        }
        private void dataReservationFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource).Refresh();
        }
        private void carFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource).Refresh();
        }
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            {

                GridViewColumnHeader column = (sender as GridViewColumnHeader);
                string sortBy = column.Tag.ToString();
                if (listViewSortCol != null)
                {
                    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                    ListViewMyReservations.Items.SortDescriptions.Clear();
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
                ListViewMyReservations.ItemsSource = tempItems;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource);
                view.Filter += UserFilter;
            }
            int CompareReservationByIdAscending(ListViewItem a, ListViewItem b)
            {
               MyReservationList first = (MyReservationList)a.Content;
               MyReservationList second = (MyReservationList)b.Content;
                return first.ReservationId.CompareTo(second.ReservationId);
            }
            int CompareReservationByIdDescending(ListViewItem a, ListViewItem b)
            {
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
                return second.ReservationId.CompareTo(first.ReservationId);
            }

            int CompareCarAscending(ListViewItem a, ListViewItem b)
            {
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
                return String.Compare(first.Vehicle, second.Vehicle);

            }
            int CompareCarDescending(ListViewItem a, ListViewItem b)
            {
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
                return String.Compare(second.Vehicle, first.Vehicle);
            }
            int CompareReservationByDateStartAscending(ListViewItem a, ListViewItem b)
            {
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
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
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
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
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
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
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
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
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
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
                MyReservationList first = (MyReservationList)a.Content;
                MyReservationList second = (MyReservationList)b.Content;
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
}
