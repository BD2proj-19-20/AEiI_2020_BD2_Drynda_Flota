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
                if (reserv.ended == true)
                {

                    OneItem.Background = Brushes.OrangeRed;
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
                else
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
          //  System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
          //  glowneOkno.DataContext = new DodajRezerwacjeModel();
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
            ListViewReservations.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
}
