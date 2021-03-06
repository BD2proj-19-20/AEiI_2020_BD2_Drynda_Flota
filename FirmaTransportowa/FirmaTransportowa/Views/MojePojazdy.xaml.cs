﻿using FirmaTransportowa.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for MojePojazdy.xaml
    /// </summary>
    public partial class MojePojazdy : UserControl
    {
        public class CarList
        {
            public int carId { get; set; }

            public string registration { get; set; }
            public int fault { get; set; }

            public String isUsed { get; set; }
        }
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();
        public MojePojazdy()
        {
            InitializeComponent();
            initializeList();
        }
        private void initializeList()
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            int id = Logowanie.actualUser.id;


            var query = from supervisor in db.CarSupervisors
                         where id == supervisor.personId
                         join car in db.Cars on supervisor.carId equals car.id
                         select new
                         {
                             carId = car.id,
                             EndDate = supervisor.endDate == null ? DateTime.MinValue : supervisor.endDate,
                             registration = car.Registration,
                             onService = car.onService
                         };


            String isCarUsed;
            int carFaults = 0;
            var activities = db.Activities;

            foreach (var car in query)
            {
                if (car.EndDate == DateTime.MinValue)
                {
                    carFaults = 0;
                    foreach (var activity in activities)
                    {

                        if (activity.carId == car.carId)
                        {
                            if (activity.closeDate == null)
                                carFaults++;
                        }
                    }
                    ListViewItem OneItem = new ListViewItem();
                    if (car.onService)
                        isCarUsed = "Nie";
                    else
                        isCarUsed = "Tak";
                    OneItem.Content = new CarList { carId = car.carId, registration = car.registration, fault = carFaults, isUsed = isCarUsed };

                    items.Add(OneItem);
                }
            }
            carList.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("carId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }
        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (!String.IsNullOrEmpty(registrationFiler.Text))
                //jezeli item nie spelnia filtra rejestracji nie wyswietlam go
                if (!((toFilter.Content as CarList).registration.IndexOf(registrationFiler.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(idFilter.Text))
                //jezeli item nie spelnia filtra id nie wyswietlam go
                if (!((toFilter.Content as CarList).carId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            //cała reszte wyswietlam
            return true;
        }
        private void CarStatistics_Click(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                CarList selectedObj = (CarList)selected.Content;
                int selectedId = selectedObj.carId;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var cars = db.Cars;

                //Usuwam datę sprzedaży
                foreach (var car in cars)
                {
                    if (car.id == selectedId)
                    {
                        StatystykiPojazdu statystykiPojazduView = new StatystykiPojazdu(car, 3);
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
        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(carList.ItemsSource).Refresh();
        }

        private void registrationFiler_TextChanged(object sender, TextChangedEventArgs e)
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

            carList.ItemsSource = tempItems;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("carId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }

        int CompareCarsByIdAscending(ListViewItem a, ListViewItem b)
        {
            CarList first = (CarList)a.Content;
            CarList second = (CarList)b.Content;
            return first.carId.CompareTo(second.carId);
        }

        int CompareCarsByIdDescending(ListViewItem a, ListViewItem b)
        {
            CarList first = (CarList)a.Content;
            CarList second = (CarList)b.Content;
            return second.carId.CompareTo(first.carId);
        }

        int CompareCarsByRegistrationDescending(ListViewItem a, ListViewItem b)
        {
            CarList first = (CarList)a.Content;
            CarList second = (CarList)b.Content;
            return String.Compare(first.registration, second.registration);
        }

        int CompareCarsByRegistrationAscending(ListViewItem a, ListViewItem b)
        {
            CarList first = (CarList)a.Content;
            CarList second = (CarList)b.Content;
            return String.Compare(second.registration, first.registration);
        }



        private void Activate_Disactivate_Click(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                CarList selectedObj = (CarList)selected.Content;
                int selectedId = selectedObj.carId;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var cars = db.Cars;
                foreach (var car in cars)
                {
                    if (car.id == selectedId)
                    {
                        if (car.onService == true)
                            car.onService = false;
                        else if (car.onService == false)
                            car.onService = true;
                    }
                }
                db.SaveChanges();
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }
        }

        private void zglos(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                CarList selectedObj = (CarList)selected.Content;
                int selectedId = selectedObj.carId;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var cars = db.Cars;

                //Usuwam datę sprzedaży
                foreach (var car in cars)
                {
                    if (car.id == selectedId)
                    {
                        ZglosUsterke zglosUsterke = new ZglosUsterke(car, 1);
                        System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                        glowneOkno.DataContext = zglosUsterke;
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
            }
        }
        private void UsterkiClick(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                CarList selectedObj = (CarList)selected.Content;
                int selectedId = selectedObj.carId;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var cars = db.Cars;

                //Usuwam datę sprzedaży
                foreach (var car in cars)
                {
                    if (car.id == selectedId)
                    {
                        System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                        glowneOkno.DataContext = new Usterkipojazdu(3, car);
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
