using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;
using FirmaTransportowa.Model;
using System.Windows.Forms;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
        }
        public void ListaRezerwacji()
        {
            Stopwatch stoper = new Stopwatch();
            stoper.Start();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var query = from reserv in db.Reservations
                        select new
                        {
                            ReservationId = reserv.id,
                            Owner = reserv.Person.lastName + " " + reserv.Person.firstName,
                            Vehicle = reserv.Car.CarModel.make + "/" + reserv.Car.CarModel.model + "/" + reserv.Car.Registration + "\n",
                            Private = reserv.@private,
                            Ended = reserv.ended,
                            LendDate = reserv.lendDate,
                            ReturnDate = reserv.returnDate,
                            ReservationDate = reserv.reservationDate
                        };


            foreach (var reserv in query)
            {
                ListViewItem OneItem = new ListViewItem();

                string dateTime = reserv.LendDate.ToString();
                string date = dateTime.Substring(0, 10);

                OneItem.Content = new ReservationList
                {
                    ReservationId = reserv.ReservationId + 1,
                    Person = reserv.Owner,
                    ReservationStart = date,
                    ReservationEnd = reserv.ReturnDate.ToString().Substring(0, 10),
                    ReservationDate = reserv.ReservationDate.ToString().Substring(0, 10),
                    Vehicle = reserv.Vehicle
                };

                if (reserv.Private == false && reserv.Ended == true && ZakonczoneBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.OrangeRed; //zakonczone nie prywatne
                    items.Add(OneItem);
                }
                else if (reserv.Private == true && reserv.Ended == true && Zakonczone_i_PrywatneBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.Red; // zakonczone prywante
                    items.Add(OneItem);
                }
                else if (reserv.Private == true && PrywatneBox.IsChecked.Value == true && reserv.Ended == false)
                {
                    OneItem.Background = Brushes.BlueViolet;  //prywatne
                    items.Add(OneItem);
                }
                else if (PozostałeBox.IsChecked.Value == true &&
                    reserv.Ended == false && reserv.Private == false)
                    items.Add(OneItem);
            }
            ListViewReservations.ItemsSource = items;

            stoper.Stop();
            //Title.Text = stoper.Elapsed.ToString();
        }


        private void Modyfikuj_Rezerwacje(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)ListViewReservations.SelectedItem;
            if (selected != null)
            {
                ReservationList selectedObj = (ReservationList)selected.Content;
                int selectedId = selectedObj.ReservationId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

                Reservation reservationChange = null;

                var reservation = (from reserv in db.Reservations
                                   where reserv.id == selectedId
                                   select reserv).FirstOrDefault();

                if (reservation != null)
                    reservationChange = reservation;
                if (reservationChange.ended == false)
                {
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new ZmienRezerwacje(reservationChange);

                }
                else
                    MessageBox.Show("Rezerwacja się zakończyła!", "Komunikat");
            }
            else

                MessageBox.Show("Niczego nie wybrano !", "Komunikat");

        }

        private void Zakoncz_Rezerwacje(object sender, RoutedEventArgs e)
        {

            ListViewItem selected = (ListViewItem)ListViewReservations.SelectedItem;

            if (selected != null)
            {
                ReservationList selectedObj = (ReservationList)selected.Content;

                int selectedId = selectedObj.ReservationId - 1;
                var reservationPerson = selectedObj.Person;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
             
                Reservation reservationChange = null;

                var reservation = (from reserv in db.Reservations
                                   where reserv.id == selectedId
                                   select reserv).FirstOrDefault();


                if (reservation != null)
                    reservationChange = reservation;

                if (reservationChange.ended == true)
                    MessageBox.Show("Rezerwacja się zakończyła!", "Komunikat");

                else
                {

                    DialogResult result = MessageBox.Show("Czy chcesz zakonczyc rezerwację " + reservationPerson + "?"
                        , "Komunikat", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {

                        reservationChange.ended = true;
                        var lend = (from lends in db.Lends
                                    where lends.reservationId == reservationChange.id
                                    select lends).FirstOrDefault();


                        if (lend != null)
                        {
                            lend.returnDate = DateTime.Now.Date;
                                if (Logowanie.actualUser != null)
                                    lend.comments += "Zakończono przez zakończenie\nrezerwacji przez Kierownika " + Logowanie.actualUser.id + ") " +
                              Logowanie.actualUser.firstName + " " + Logowanie.actualUser.lastName + " - " + DateTime.Now.ToString() + "\n";

                            

                        }
                        db.SaveChanges();

                        ListViewReservations.ItemsSource = null;
                        items.Clear();
                        UpdateView();

                    }
                    else if (result == DialogResult.No)
                    {
                    }

                }
            }
            else
                MessageBox.Show("Niczego nie wybrano !", "Komunikat");
        }
        private string GetPath()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "Dokument pdf|*.pdf";
            saveFileDialog.Title = "Zapisz raport na temat rezerwacji";
            saveFileDialog.FileName = "Raport na temat rezerwacji - " + DateTime.Now.ToShortDateString();
            saveFileDialog.ShowDialog();

            return saveFileDialog.FileName;
        }
        private void Generuj_Raport_Rezerwacje(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            string path = GetPath();
            if (path == "")
                return;

            Font times = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times.Size = 32;
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            string namePerson = "";
            var vehicle = "";

            var query = from reserv in db.Reservations
                        select new
                        {
                            ReservationId = reserv.id,
                            Owner = reserv.Person.lastName + " " + reserv.Person.firstName,
                            Vehicle = reserv.Car.CarModel.make + "/" + reserv.Car.CarModel.model + "/" + reserv.Car.Registration + "\n",
                            Private = reserv.@private,
                            Ended = reserv.ended,
                            LendDate = reserv.lendDate,
                            ReturnDate = reserv.returnDate,
                            ReservationDate = reserv.reservationDate
                        };


            foreach (var reserv in query)
            {

                        namePerson = reserv.Owner;
                Chunk c = new Chunk((reserv.ReservationId + 1) + ")\n" + namePerson, times);

                vehicle = reserv.Vehicle;

                if ((Regex.IsMatch(namePerson, personFilter.Text, RegexOptions.IgnoreCase))
                    && (Regex.IsMatch(reserv.LendDate.ToShortDateString(), dataStartFilter.Text, RegexOptions.IgnoreCase))
                     && (Regex.IsMatch(reserv.ReturnDate.ToString().Substring(0, 10), dataEndFilter.Text, RegexOptions.IgnoreCase))
                       && (Regex.IsMatch(reserv.ReservationDate.ToString().Substring(0, 10), dataReservationFilter.Text, RegexOptions.IgnoreCase))
                   && Regex.IsMatch(vehicle, carFilter.Text, RegexOptions.IgnoreCase) && Regex.IsMatch((reserv.ReservationId + 1).ToString(), idFilter.Text))
                {
                    if (reserv.Private == false && reserv.Ended == true && ZakonczoneBox.IsChecked.Value == true)
                    {
                        c.SetBackground(BaseColor.ORANGE);
                        times.Size = 26;
                        doc.Add(new iTextSharp.text.Paragraph(c));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rozpoczęcia: " + reserv.LendDate).Substring(0,29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Zakończenia: " + reserv.ReturnDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rezerwacji: " + reserv.ReservationDate).Substring(0, 29), times));


                        doc.Add(new iTextSharp.text.Paragraph("Pojazd: " + vehicle + "\n", times));
                        times.Size = 32;
                    }
                    else if (reserv.Private == true && reserv.Ended == true && Zakonczone_i_PrywatneBox.IsChecked.Value == true)
                    {
                        c.SetBackground(BaseColor.RED);
                        times.Size = 26;
                        doc.Add(new iTextSharp.text.Paragraph(c));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rozpoczęcia: " + reserv.LendDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Zakończenia: " + reserv.ReturnDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rezerwacji: " + reserv.ReservationDate).Substring(0, 29), times));

                        doc.Add(new iTextSharp.text.Paragraph("Pojazd: " + vehicle + "\n", times));
                        times.Size = 32;
                    }
                    else if (reserv.Private == true && PrywatneBox.IsChecked.Value == true && reserv.Ended == false)
                    {
                        c.SetBackground(BaseColor.BLUE);
                        times.Size = 26;
                        doc.Add(new iTextSharp.text.Paragraph(c));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rozpoczęcia: " + reserv.LendDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Zakończenia: " + reserv.ReturnDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rezerwacji: " + reserv.ReservationDate).Substring(0, 29), times));

                        doc.Add(new iTextSharp.text.Paragraph("Pojazd: " + vehicle + "\n", times));
                        times.Size = 32;
                    }
                    else if (PozostałeBox.IsChecked.Value == true && reserv.Ended == false && reserv.Private == false)
                    {
                        times.Size = 26;
                        doc.Add(new iTextSharp.text.Paragraph(c));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rozpoczęcia: " + reserv.LendDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Zakończenia: " + reserv.ReturnDate).Substring(0, 29), times));
                        doc.Add(new iTextSharp.text.Paragraph(("Dzień Rezerwacji: " + reserv.ReservationDate).Substring(0, 29), times));


                        doc.Add(new iTextSharp.text.Paragraph("Pojazd: " + vehicle, times));
                        times.Size = 32;

                    }
                }

            }
            Chunk c1 = new Chunk("");
            doc.Add(c1); //doc nie może być pusty 

            doc.Close();
        }

        private void Box_Click(object sender, RoutedEventArgs e)
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
