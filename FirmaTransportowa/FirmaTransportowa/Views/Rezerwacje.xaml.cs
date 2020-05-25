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

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Rezerwacje.xaml
    /// </summary>
    public partial class Rezerwacje : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        public Rezerwacje()
        {
            InitializeComponent();
            List<Reservations> items = new List<Reservations>();
            items.Add(new Reservations() { Id = 101, Name="Jan Kowalski", Date = "14 03 2020", Car = "Skoda Octavia", Keeper="Jan Nowak" });
            items.Add(new Reservations() { Id = 102, Name = "Jan Kowalski", Date = "14 03 2019", Car = "Ford Focus", Keeper = "Jan Nowak" });
            items.Add(new Reservations() { Id = 103, Name = "Jan Kowalski", Date = "12 05 2019", Car = "Audi A8", Keeper = "Jan Nowak" });
            items.Add(new Reservations() { Id = 104, Name = "Jan Kowalski", Date = "25 03 2019", Car = "Volkswagen Passat", Keeper = "Jan Nowak" });
            items.Add(new Reservations() { Id = 105, Name = "Jan Kowalski", Date = "30 01 2019", Car = "Ford Mondeo", Keeper = "Jan Nowak" });
            items.Add(new Reservations() { Id = 106, Name = "Jan Kowalski", Date = "22 12 2019", Car = "Ford Focus", Keeper = "Jan Nowak" });
            ListViewReservations.ItemsSource = items;

            //CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewMyReservations.ItemsSource);
            // view.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
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
    public class Reservations
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Car { get; set; }
        public string Keeper { get; set; }
    }
}
