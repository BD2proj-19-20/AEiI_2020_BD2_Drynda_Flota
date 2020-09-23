using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using FirmaTransportowa.Model;
using Brushes = System.Windows.Media.Brushes;

namespace FirmaTransportowa.Views {
	public class ListItem {
		public int contractorId { get; set; }
		public string name { get; set; }
		public DateTime startDate { get; set; }
		public DateTime? endDate { get; set; }
	}
	public partial class Kontraktorzy : UserControl {

		private ObservableCollection<ListViewItem> items = new ObservableCollection<ListViewItem>();
		private SortAdorner listViewSortAdorner = null;
		private GridViewColumnHeader listViewSortCol = null;
		private GridViewColumnHeader sortingColumn = null;
		public Kontraktorzy() {
			InitializeComponent();
			InitializeList();
		}



		private void InitializeList() {
			Stopwatch stoper = new Stopwatch(); //todo: usunąć stoper
			stoper.Start();

			items.Clear();
			var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
			var contractors = db.Contractors;

			foreach (var contractor in contractors) {
				ListViewItem list = new ListViewItem();
				list.Content = new ListItem { contractorId = contractor.id, name = contractor.name, startDate = contractor.startSate, endDate = contractor.endDate?.Date };
				items.Add(list);
			}
			Array.Sort(items.ToArray(), (ListViewItem a, ListViewItem b) => ((ListItem)a.Content).contractorId.CompareTo(((ListItem)b.Content).contractorId));
			contractorList.ItemsSource = items;

			stoper.Stop();
			Title.Text = stoper.Elapsed.ToString();
		}

		private void contractorList_MouseDown(object sender, MouseButtonEventArgs e) {
			contractorList.UnselectAll();
		}

		private void GridViewColumnHeaderClick(object sender, RoutedEventArgs e) {
			GridViewColumnHeader column = (sender as GridViewColumnHeader);
			sortingColumn = column;
			string sortBy = column.Tag.ToString();
			if (listViewSortCol != null) {
				AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
				contractorList.Items.SortDescriptions.Clear();
			}

			ListSortDirection newDir = ListSortDirection.Ascending;
			if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
				newDir = ListSortDirection.Descending;

			listViewSortCol = column;
			listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
			AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);

			var tempItems = items.ToArray();
			if (sortBy == "contractorId") {
				if (newDir.ToString() == "Ascending")
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => ((ListItem)a.Content).contractorId.CompareTo(((ListItem)b.Content).contractorId));
				else
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => ((ListItem)b.Content).contractorId.CompareTo(((ListItem)a.Content).contractorId));
			}
			else if (sortBy == "name") {
				if (newDir.ToString() == "Ascending")
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => ((ListItem)a.Content).name.CompareTo(((ListItem)b.Content).name));
				else
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => ((ListItem)b.Content).name.CompareTo(((ListItem)a.Content).name));
			}
			else if (sortBy == "startDate") {
				if (newDir.ToString() == "Ascending")
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => ((ListItem)a.Content).startDate.CompareTo(((ListItem)b.Content).startDate));
				else
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => ((ListItem)b.Content).startDate.CompareTo(((ListItem)a.Content).startDate));
			}
			else if (sortBy == "endDate") {
				if (newDir.ToString() == "Ascending")
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => {
						var aa = (ListItem)a.Content;
						var bb = (ListItem)b.Content;
						if (aa.endDate == null)
							return 1;
						if (bb.endDate == null)
							return -1;
						return aa.endDate.Value.CompareTo(bb.endDate.Value);
					});
				else
					Array.Sort(tempItems, (ListViewItem a, ListViewItem b) => {
						var aa = (ListItem)a.Content;
						var bb = (ListItem)b.Content;
						if (bb.endDate == null)
							return 1;
						if (aa.endDate == null)
							return -1;
						return bb.endDate.Value.CompareTo(aa.endDate.Value);
					});
			}

			contractorList.ItemsSource = tempItems;
			contractorList.Items.Refresh();
		}
		private void AddClick(object sender, RoutedEventArgs e) {
			System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
			glowneOkno.DataContext = new DodajKontraktora();
			InitializeList();
		}
		private void EndClick(object sender, RoutedEventArgs e) {
			ListViewItem selected = (ListViewItem)contractorList.SelectedItem;
			if (selected != null) {
				int selectedId = ((ListItem)selected.Content).contractorId;
				var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
				var selectedContractor = db.Contractors.Where(c => c.id == selectedId).Single();
				selectedContractor.endDate = DateTime.Now;
				db.SaveChanges();
			}
			contractorList.UnselectAll();
		}
	}
}

