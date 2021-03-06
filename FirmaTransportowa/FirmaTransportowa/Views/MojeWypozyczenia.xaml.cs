﻿using System.Collections.Generic;
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
using System.Diagnostics;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Logika interakcji dla klasy MojeWypozyczenia.xaml
    /// </summary>
    /// 
    public class MyLendList
    {
        public int LendId { get; set; }
        public string LendStart { get; set; }
        public string LendPlannedEnd { get; set; }
        public string LendEnd { get; set; }
        public string ReservationDate { get; set; }
        public string Vehicle { get; set; }
    }
    public partial class MojeWypozyczenia : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();
        public void UpdateView()
        {
            MojaListaWypozyczen();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("LendId", ListSortDirection.Descending));
            view.Filter += UserFilter;
        }
        public void MojaListaWypozyczen()
        {
            Stopwatch stoper = new Stopwatch();
            stoper.Start();

            int id = Logowanie.actualUser.id;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from lend in db.Lends
                        where lend.personId == id
                        select new
                        {
                            LendId = lend.id,
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
                string dateTime = lend.LendDate.ToString();
                string date = "";

                if (dateTime.Length > 0)
                    date = dateTime.Substring(0, 10);

                OneItem.Content = new MyLendList
                {
                    LendId = lend.LendId + 1,
                    LendStart = date,
                    LendPlannedEnd = lend.PlannedReturnDate.ToString().Substring(0, 10),
                    LendEnd = lend.ReturnDate != null ? lend.ReturnDate.ToString().Substring(0, 10) : "",
                    ReservationDate = lend.ReservationDate.ToShortDateString(),
                    Vehicle = lend.Vehicle
                };

                bool addItem = false;
                if ((lend.ReturnDate <= DateTime.Now || lend.LendEnded == true) && ZakonczoneBox.IsChecked.Value == true && lend.Private == true) //zakończone
                {
                    OneItem.Background = Brushes.Red;  //zakonczone prywatne
                    addItem = true;
                }
                else if ((lend.ReturnDate <= DateTime.Now || lend.LendEnded == true) && Zakonczone_i_PrywatneBox.IsChecked.Value == true && lend.Private == false) //zakończone
                {
                    OneItem.Background = Brushes.OrangeRed; //zakonczone nie prywatne
                    addItem = true;
                }
                else if (lend.Private == true && (lend.ReturnDate > DateTime.Now || lend.ReturnDate == null) && lend.LendEnded == false &&
                    PrywatneBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.BlueViolet;  //prywatne
                    addItem = true;
                }
                else if (PozostałeBox.IsChecked.Value == true && lend.Private == false && lend.LendEnded == false
                        && (lend.ReturnDate > DateTime.Now || lend.ReturnDate == null))
                    addItem = true;

                if (RozpoczeteBox.IsChecked.Value == true &&
             (lend.LendDate > DateTime.Now.Date || lend.ReturnDate <= lend.LendDate.Date))
                    continue;
                if (addItem == true)
                    items.Add(OneItem);
            }
            ListViewMyLends.ItemsSource = items;

            stoper.Stop();
            //Title.Text = stoper.Elapsed.ToString();
        }
        public MojeWypozyczenia()
        {
            InitializeComponent();
            UpdateView();
        }

        private void Statystyki_Wypozyczenia(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewMyLends.SelectedItem;
            if (selected != null)
            {
                MyLendList selectedObj = (MyLendList)selected.Content;
                int selectedId = selectedObj.LendId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
              
                Lend lendChange = null;

                var lend = (from lendd in db.Lends
                          where lendd.id == selectedId
                                   select lendd).FirstOrDefault();

                if(lend != null)
                        lendChange = lend;
                
                if (lendChange.lendDate > DateTime.Now.Date || lendChange.returnDate <= lendChange.lendDate.Date)
                    MessageBox.Show("Wypożyczenie nie zaczeło się!", "Komunikat");
                else
                {
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new StatystykiWypozyczenia(lendChange,false);
                }
            }
            else

                MessageBox.Show("Niczego nie wybrano !", "Komunikat");
        }
        private void Zakoncz_Wypozyczenie(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewMyLends.SelectedItem;

            if (selected != null)
            {

                MyLendList selectedObj = (MyLendList)selected.Content;

                int selectedId = selectedObj.LendId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                Lend lendChange = null;

                var lend = (from lendd in db.Lends
                            where lendd.id == selectedId
                            select lendd).FirstOrDefault();

                if (lend != null)
                    lendChange = lend;


                if (lendChange.returnDate <= DateTime.Now || lendChange.Reservation.ended == true)
                    MessageBox.Show("Wypożyczenie się zakończyło!", "Komunikat");
                else if (lendChange.lendDate > DateTime.Now)
                    MessageBox.Show("Wypożyczenie się jeszcze\nnie rozpoczeło!", "Komunikat");
                else
                {

                    DialogResult result = MessageBox.Show("Czy chcesz zakonczyc wypożyczenie ?"
                        , "Komunikat", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        lendChange.returnDate = DateTime.Now.Date;

                        lendChange.comments += "Zakończono przez zakończenie\nwypożyczenia przez pracownika " + Logowanie.actualUser.id + ") " +
                            Logowanie.actualUser.firstName + " " + Logowanie.actualUser.lastName + " - " + DateTime.Now.ToString() + "\n";
                    //    var reservations = db.Reservations;

                        var reservation = (from reserv in db.Reservations
                                           where lendChange.id == reserv.lendId
                                           select reserv).FirstOrDefault();

                    
                        reservation.ended = true;
                        db.SaveChanges();

                        ListViewMyLends.ItemsSource = null;
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
        private void Zglos_usterke(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewMyLends.SelectedItem;
            if (selected != null)
            {
                MyLendList selectedObj = (MyLendList)selected.Content;

                int selectedId = selectedObj.LendId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();


                Lend lendChange = null;
                var lend = (from lendd in db.Lends
                            where lendd.id == selectedId
                            select lendd).FirstOrDefault();

                if (lend != null)
                    lendChange = lend;


                if (lendChange.lendDate > DateTime.Now.Date || lendChange.returnDate < lendChange.lendDate.Date)
                //usterkę można zgłosic w tym oknie dla rozpoczętych wypozyczeń
                {
                    MessageBox.Show("Wypożyczenie nie zaczeło się!", "Komunikat");
                    return;
                }

                var car = (from carr in db.Cars
                            where carr.id == lendChange.carId
                           select carr).FirstOrDefault();

                if(car!=null)
                { 
                        ZglosUsterke zglosUsterke = new ZglosUsterke(car, 2);
                        System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                        glowneOkno.DataContext = zglosUsterke;
                        return;
                    
                }
            }
            else
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
        }


        private void Box_Click(object sender, RoutedEventArgs e)
        {
            ListViewMyLends.ItemsSource = null;
            items.Clear();
            UpdateView();
        }
      
        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (dateStartFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).LendStart.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dateStartFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).LendStart.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (datePlannedEndFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).LendPlannedEnd.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (datePlannedEndFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).LendPlannedEnd.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }



            if (dateEndFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).LendEnd.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dateEndFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).LendEnd.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (dateReservationFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).ReservationDate.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dateReservationFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as MyLendList).ReservationDate.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (!String.IsNullOrEmpty(idFilter.Text))
                if (!((toFilter.Content as MyLendList).LendId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(carFilter.Text))
                if (!((toFilter.Content as MyLendList).Vehicle.IndexOf(carFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;


            if (!String.IsNullOrEmpty(dateStartFilter.Text))
                if (!((toFilter.Content as MyLendList).LendStart.IndexOf(dateStartFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(datePlannedEndFilter.Text))
                if (!((toFilter.Content as MyLendList).LendPlannedEnd.IndexOf(datePlannedEndFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dateEndFilter.Text))
                if (!((toFilter.Content as MyLendList).LendEnd.IndexOf(dateEndFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;

            if (!String.IsNullOrEmpty(dateReservationFilter.Text))
                if (!((toFilter.Content as MyLendList).ReservationDate.IndexOf(dateReservationFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            return true;


        }

        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource).Refresh();
        }
        private void dateStartFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource).Refresh();
        }

        private void datePlannedEndFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource).Refresh();
        }
        private void dataEndFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource).Refresh();
        }
        private void dateReservationFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource).Refresh();
        }
        private void carFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource).Refresh();
        }
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {

            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                ListViewMyLends.Items.SortDescriptions.Clear();
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
            ListViewMyLends.ItemsSource = tempItems;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMyLends.ItemsSource);
            view.Filter += UserFilter;


        }

        int CompareLendByIdAscending(ListViewItem a, ListViewItem b)
        {
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
            return first.LendId.CompareTo(second.LendId);
        }
        int CompareLendByIdDescending(ListViewItem a, ListViewItem b)
        {
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
            return second.LendId.CompareTo(first.LendId);
        }



        int CompareCarAscending(ListViewItem a, ListViewItem b)
        {
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
            return String.Compare(first.Vehicle, second.Vehicle);

        }
        int CompareCarDescending(ListViewItem a, ListViewItem b)
        {
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
            return String.Compare(second.Vehicle, first.Vehicle);
        }
        int CompareLendByDateStartAscending(ListViewItem a, ListViewItem b)
        {
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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
            MyLendList first = (MyLendList)a.Content;
            MyLendList second = (MyLendList)b.Content;
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

