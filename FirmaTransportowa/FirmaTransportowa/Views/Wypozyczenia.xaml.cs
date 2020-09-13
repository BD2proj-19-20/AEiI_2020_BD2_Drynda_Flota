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
using System.Diagnostics;
using System.Linq;
using System.Data.Entity.Core.Common.CommandTrees;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Wypozyczenia.xaml
    /// </summary>
    /// 

    public class LendList
    {
        public int LendId { get; set; }
        public string Person { get; set; }
        public string LendStart { get; set; }
        public string LendPlannedEnd { get; set; }
        public string LendEnd { get; set; }
        public string ReservationDate { get; set; }
        public string Vehicle { get; set; }
    }
    public partial class Wypozyczenia : UserControl
    {
        public Wypozyczenia()
        {
            InitializeComponent();
            UpdateView();
        }
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();
        public void UpdateView()
        {
            ListaWypozyczen();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("LendId", ListSortDirection.Descending));
            view.Filter += UserFilter;
        }
        public void ListaWypozyczen()
        {
            //  int id = Logowanie.actualUser.id;
            Stopwatch stoper = new Stopwatch();
            stoper.Start();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from lend in db.Lends
                        select new
                        {
                            LendId = lend.id,
                            Owner = lend.Person.lastName + " " + lend.Person.firstName,
                            Vehicle = lend.Car.CarModel.make + "/" + lend.Car.CarModel.model + "/" + lend.Car.Registration + "\n",
                            Private = lend.@private,
                            LendDate = lend.lendDate,
                            ReturnDate = lend.returnDate,
                            PlannedReturnDate = lend.plannedReturnDate,
                            ReservationDate = lend.Reservation.reservationDate,
                         LendEnded = lend.Reservation.ended
                        };

            foreach (var lend in query)
            {
                ListViewItem OneItem = new ListViewItem();
                string date = "";
                string dateTime = lend.LendDate.ToString();

                if (dateTime.Length > 0)
                    date = dateTime.Substring(0, 10);

                OneItem.Content = new LendList
                {
                    LendId = lend.LendId + 1,
                    Person = lend.Owner,
                    LendStart = date,
                    LendPlannedEnd = lend.PlannedReturnDate.ToString().Substring(0, 10),
                    LendEnd = lend.ReturnDate != null ? lend.ReturnDate.ToString().Substring(0, 10) : "",
                    ReservationDate = lend.ReservationDate.ToShortDateString(),
                    Vehicle = lend.Vehicle
                };

                if ((lend.ReturnDate <= DateTime.Now || lend.LendEnded==true) && ZakonczoneBox.IsChecked.Value == true && lend.Private == true ) //zakończone
                {
                    OneItem.Background = Brushes.Red;  //zakonczone prywatne
                    items.Add(OneItem);
                }
                else if ((lend.ReturnDate <= DateTime.Now || lend.LendEnded == true) && Zakonczone_i_PrywatneBox.IsChecked.Value == true && lend.Private == false) //zakończone
                {
                    OneItem.Background = Brushes.OrangeRed; //zakonczone nie prywatne
                    items.Add(OneItem);
                }
                else if (lend.Private == true && (lend.ReturnDate > DateTime.Now || lend.ReturnDate == null) &&
                    PrywatneBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.BlueViolet;  //prywatne
                    items.Add(OneItem);
                }
                else if(PozostałeBox.IsChecked.Value == true && lend.Private == false
                        && (lend.ReturnDate > DateTime.Now || lend.ReturnDate == null))
                {
                    items.Add(OneItem);
                }
            }
            ListViewLends.ItemsSource = items;

            stoper.Stop();
            Title.Text = stoper.Elapsed.ToString();
        }
        private void PrywatneBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewLends.ItemsSource = null;
            items.Clear();
            UpdateView();
        }
        private void ZakonczoneBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewLends.ItemsSource = null;
            items.Clear();
            UpdateView();
        }
        private void PozostałeBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewLends.ItemsSource = null;
            items.Clear();
            UpdateView();
        }
        private void Zakonczone_i_PrywatneBox_Click(object sender, RoutedEventArgs e)
        {
            ListViewLends.ItemsSource = null;
            items.Clear();
            UpdateView();
        }
        private void Statystyki_Wypozyczenia(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewLends.SelectedItem;
            if (selected != null)
            {
                LendList selectedObj = (LendList)selected.Content;
                int selectedId = selectedObj.LendId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var lends = db.Lends;
                Lend lendChange = null;

                foreach (var lend in lends)
                {
                    if (lend.id == selectedId)
                        lendChange = lend;
                }
                if (lendChange.lendDate > DateTime.Now.Date || lendChange.returnDate < lendChange.lendDate)
                    MessageBox.Show("Wypożyczenie nie zaczeło się!", "Komunikat");
                else
                {
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new StatystykiWypozyczenia(lendChange,true);
                }
            }
            else

                MessageBox.Show("Niczego nie wybrano !", "Komunikat");
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (dateStartFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).LendStart.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dateStartFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).LendStart.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (datePlannedEndFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).LendPlannedEnd.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (datePlannedEndFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).LendPlannedEnd.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (dateEndFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).LendEnd.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dateEndFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).LendEnd.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (dateReservationFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).ReservationDate.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dateReservationFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as LendList).ReservationDate.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (!String.IsNullOrEmpty(personFilter.Text))
                if (!((toFilter.Content as LendList).Person.IndexOf(personFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(idFilter.Text))
                if (!((toFilter.Content as LendList).LendId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(carFilter.Text))
                if (!((toFilter.Content as LendList).Vehicle.IndexOf(carFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;


            if (!String.IsNullOrEmpty(dateStartFilter.Text))
                if (!((toFilter.Content as LendList).LendStart.IndexOf(dateStartFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(datePlannedEndFilter.Text))
                if (!((toFilter.Content as LendList).LendPlannedEnd.IndexOf(datePlannedEndFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dateEndFilter.Text))
                if (!((toFilter.Content as LendList).LendEnd.IndexOf(dateEndFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dateReservationFilter.Text))
                if (!((toFilter.Content as LendList).ReservationDate.IndexOf(dateReservationFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            return true;


        }
        private void personFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }
        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }
        private void dateStartFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }

        private void datePlannedEndFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }
        private void dataEndFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }
        private void dateReservationFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }
        private void carFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource).Refresh();
        }
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                ListViewLends.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);

            var tempItems = items.ToArray();

            if (sortBy == "LendId")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareLendByIdAscending);
                else
                    Array.Sort(tempItems, CompareLendByIdDescending);
            }
            else if (sortBy == "Person")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareLendByPersonAscending);
                else
                    Array.Sort(tempItems, CompareLendByPersonDescending);
            }
            else if (sortBy == "LendStart")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareLendByDateStartAscending);
                else
                    Array.Sort(tempItems, CompareLendByDateStartDescending);
            }
            else if (sortBy == "PlannedLendEnd")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareLendByDatePlannedEndAscending);
                else
                    Array.Sort(tempItems, CompareLendByDatePlannedEndDescending);
            }
            else if (sortBy == "LendEnd")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareLendByDateEndAscending);
                else
                    Array.Sort(tempItems, CompareLendByDateEndDescending);
            }
            else if (sortBy == "DateReservation")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareLendByDateReservationAscending);
                else
                    Array.Sort(tempItems, CompareLendByDateReservationDescending);
            }
            else if (sortBy == "Car")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, CompareCarAscending);
                else
                    Array.Sort(tempItems, CompareCarDescending);
            }
            ListViewLends.ItemsSource = tempItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewLends.ItemsSource);
            view.Filter += UserFilter;

          
        }
        int CompareLendByIdAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            return first.LendId.CompareTo(second.LendId);
        }
        int CompareLendByIdDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            return second.LendId.CompareTo(first.LendId);
        }

        int CompareLendByPersonAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            return String.Compare(first.Person, second.Person);

        }
        int CompareLendByPersonDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            return String.Compare(second.Person, first.Person);
        }

        int CompareCarAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            return String.Compare(first.Vehicle, second.Vehicle);

        }
        int CompareCarDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            return String.Compare(second.Vehicle, first.Vehicle);
        }
        int CompareLendByDateStartAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.LendStart.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.LendStart);
            else
                firstDate = DateTime.MinValue;
            if (second.LendStart.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.LendStart);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);
        }
        int CompareLendByDateStartDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.LendStart.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.LendStart);
            else
                firstDate = DateTime.MinValue;
            if (second.LendStart.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.LendStart);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }
        int CompareLendByDatePlannedEndAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.LendPlannedEnd.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.LendPlannedEnd);
            else
                firstDate = DateTime.MinValue;
            if (second.LendPlannedEnd.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.LendPlannedEnd);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);


        }
        int CompareLendByDatePlannedEndDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.LendPlannedEnd.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.LendPlannedEnd);
            else
                firstDate = DateTime.MinValue;
            if (second.LendPlannedEnd.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.LendPlannedEnd);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }


        int CompareLendByDateEndAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.LendEnd.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.LendEnd);
            else
                firstDate = DateTime.MinValue;
            if (second.LendEnd.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.LendEnd);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);


        }
        int CompareLendByDateEndDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.LendEnd.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.LendEnd);
            else
                firstDate = DateTime.MinValue;
            if (second.LendEnd.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.LendEnd);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }

        int CompareLendByDateReservationAscending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
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
        int CompareLendByDateReservationDescending(ListViewItem a, ListViewItem b)
        {
            LendList first = (LendList)a.Content;
            LendList second = (LendList)b.Content;
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
        private void Zakoncz_Wypozyczenie(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewLends.SelectedItem;

            if (selected != null)
            {

                LendList selectedObj = (LendList)selected.Content;

                int selectedId = selectedObj.LendId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                //    var reservations = db.Reservations;
                //  Reservation reservationChange = null;

                Lend lendChange = null;


                var lends = db.Lends;
                foreach (var lend in lends)
                {
                    if (lend.id == selectedId)
                        lendChange = lend;
                }

                if (lendChange.lendDate > DateTime.Now)
                    MessageBox.Show("Wypożyczenie się jeszcze\nnie rozpoczeło!", "Komunikat");
                else if (lendChange.returnDate >= DateTime.Now)
                    MessageBox.Show("Wypożyczenie się zakończyło!", "Komunikat");
                else
                {

                    DialogResult result = MessageBox.Show("Czy chcesz zakonczyc wypożyczenie ?"
                        , "Komunikat", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        lendChange.returnDate = Convert.ToDateTime(DateTime.Now);

                        lendChange.comments += "Zakończono przez zakończenie\nwypożyczenia przez pracownika " + Logowanie.actualUser.id + ") " +
                            Logowanie.actualUser.firstName + " " + Logowanie.actualUser.lastName + " - " + DateTime.Now.ToString() + "\n";
                        //   reservationChange.ended = true;
                        //   var lends = db.Lends;
                        var reservations = db.Reservations;

                        foreach (var reserv in reservations)
                        {
                            if (lendChange.id == reserv.lendId)
                                reserv.ended = true;

                        }
                        db.SaveChanges();

                        ListViewLends.ItemsSource = null;
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

    }

}
