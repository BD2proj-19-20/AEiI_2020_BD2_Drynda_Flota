﻿using FirmaTransportowa.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Diagnostics;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ListaPojazdow.xaml
    /// </summary>
    public class CarList
    {
        public int carId { get; set; }

        public string registration { get; set; }

        public string carSupervisor { get; set; }
    }
    public partial class ListaPojazdow : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();
        public ListaPojazdow()
        {
            InitializeComponent();
            initializeList();
        }

        private void initializeList()
        {
            Stopwatch stoper = new Stopwatch();
            stoper.Start();

            items.Clear();
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from car in db.Cars where car.saleDate == null
                        join supervisor in db.CarSupervisors on car.id equals supervisor.carId into final
                        from f in final.DefaultIfEmpty()
                        where f.endDate == null || f.endDate > DateTime.Today
                        select new
                        {
                            SupervisorName = f == null ? "Brak" : f.Person.lastName + " " + f.Person.firstName,
                            CarRegistration = car.Registration,
                            CarId = car.id,
                        };

            foreach (var car in query)
            {
                ListViewItem OneItem = new ListViewItem();
                OneItem.Content = new CarList { carId = car.CarId, registration = car.CarRegistration, carSupervisor = car.SupervisorName};
                items.Add(OneItem);
            }
            Array.Sort(items.ToArray(), CompareCarsByIdAscending);
            carList.ItemsSource = items;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(carList.ItemsSource);
            view.Filter += UserFilter;

            stoper.Stop();
            //Title.Text = stoper.Elapsed.ToString();
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (!String.IsNullOrEmpty(carSupervisorFilter.Text))
                //jezeli item nie spelnia filtra opiekuna nie wyswietlam go
                if (!((toFilter.Content as CarList).carSupervisor.IndexOf(carSupervisorFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
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
                        StatystykiPojazdu statystykiPojazduView = new StatystykiPojazdu(car, 1);
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

        private void carSupervisorFilter_TextChanged(object sender, TextChangedEventArgs e)
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

        int CompareCarsBySupervisorDescending(ListViewItem a, ListViewItem b)
        {
            CarList first = (CarList)a.Content;
            CarList second = (CarList)b.Content;
            return String.Compare(first.carSupervisor, second.carSupervisor);
        }

        int CompareCarsBySupervisorAscending(ListViewItem a, ListViewItem b)
        {
            CarList first = (CarList)a.Content;
            CarList second = (CarList)b.Content;
            return String.Compare(second.carSupervisor, first.carSupervisor);
        }

        private void Reserve_Click(object sender, RoutedEventArgs e)
        {
            //Pobieram zaznaczony samochód
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            ListViewItem selected = (ListViewItem)carList.SelectedItem;
            if (selected != null)
            {
                CarList selectedObj = (CarList)selected.Content;
                int selectedId = selectedObj.carId;

                var car = (from carr in db.Cars
                           where carr.id == selectedId
                           select carr).FirstOrDefault();
                if(car.onService==true)
                {
                    MessageBox.Show("Samochód jest w serwisie!", "Komunikat");
                    return;
                }

                System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                glowneOkno.DataContext = new DodajRezerwacjeDlaPojazdu(selectedId);
            }
            else
                MessageBox.Show("Nie wybrano samochodu!", "Komunikat");
        }

    }
}
