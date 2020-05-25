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
    /// Interaction logic for MojeRezerwacje.xaml
    /// </summary>
    public partial class MojeRezerwacje : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        public MojeRezerwacje()
        {
            InitializeComponent();
            List<MyReservations> items = new List<MyReservations>();
            items.Add(new MyReservations() { Id = 101, Date="14 03 2020", Car = "Skoda Octavia", Keeper = "Jan Nowak" });
            items.Add(new MyReservations() { Id = 102, Date = "14 03 2019", Car = "Ford Focus", Keeper = "Jan Nowak" });
            items.Add(new MyReservations() { Id = 103, Date = "12 05 2019", Car = "Audi A8", Keeper = "Jan Nowak" });
            items.Add(new MyReservations() { Id = 104, Date = "25 03 2019", Car = "Volkswagen Passat", Keeper = "Jan Nowak" });
            items.Add(new MyReservations() { Id = 105, Date = "30 01 2019", Car = "Ford Mondeo", Keeper = "Jan Nowak" });
            items.Add(new MyReservations() { Id = 106, Date = "22 12 2019", Car = "Ford Focus", Keeper = "Jan Nowak" });
            ListViewMyReservations.ItemsSource = items;

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
                ListViewMyReservations.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            ListViewMyReservations.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }
    public class MyReservations
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Car { get; set; }
        public string Keeper { get; set; }
    }
}
