using FirmaTransportowa.ViewModels;
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
using FirmaTransportowa.Model;
using System.ComponentModel;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Pracownicy.xaml
    /// </summary>
    /// 
    public class WorkersList
    {
        public int PersonId { get; set; }
        public string Person { get; set; }
        public string PersonDateOut { get; set; }
    }

    public partial class Pracownicy : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        List<ListViewItem> items = new List<ListViewItem>();

        public Pracownicy()
        {
            InitializeComponent();
            workersList.ItemsSource = ListaPracownikow();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(workersList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }
    
        public List<ListViewItem> ListaPracownikow()
        {
            
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;
   
            foreach (var person in people)
            {
                ListViewItem OneItem = new ListViewItem();
                var date = "";

                if (person.layoffDate <= DateTime.Today && ZwolnieniBox.IsChecked.Value==true )
                {
                    OneItem.Background = Brushes.Red;
                    string dateTime = person.layoffDate.ToString();
                    date = dateTime.Substring(0, 10);
                    OneItem.Content = new WorkersList { PersonId = person.id + 1, Person = person.firstName + " " + person.lastName, PersonDateOut = date };
                    items.Add(OneItem);
                }
                else if (person.layoffDate > DateTime.Today && DataZwolnieniaBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.Orange;
                    string dateTime = person.layoffDate.ToString();
                    date = dateTime.Substring(0, 10);
                    OneItem.Content = new WorkersList { PersonId = person.id + 1, Person = person.firstName + " " + person.lastName, PersonDateOut = date };
                    items.Add(OneItem);
                }
                else if (BezZwolnieniaBox.IsChecked.Value == true && (person.layoffDate is null))
                {
                    OneItem.Content = new WorkersList { PersonId = person.id + 1, Person = person.firstName + " " + person.lastName, PersonDateOut = date };
                    items.Add(OneItem);
                }
 
            }
            return items;
        }

        private void Dodaj_Pracownika(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajPracownikModel();
        }

        private void Zwolnij_Pracownika(object sender, RoutedEventArgs e)
        {

            ListViewItem selected = (ListViewItem)workersList.SelectedItem;

            if (selected != null)
            {
                WorkersList selectedObj = (WorkersList)selected.Content;


                int selectedId = selectedObj.PersonId - 1;


                CarSupervisor carSupervisiorChange = null;
                Person personChange = null;


                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var people = db.People;
                var carSupervisor = db.CarSupervisors;
                foreach (var person in people)
                {
                    if (person.id == selectedId)
                    {
                           personChange = person;

                        if ((personChange.layoffDate is null) || personChange.layoffDate > DateTime.Today)

                            foreach (var carS in carSupervisor)
                            {
                                if (person.id == carS.personId)
                                    carSupervisiorChange = carS;
                            }
                    }
                }

                if ((personChange.layoffDate is null) || personChange.layoffDate > DateTime.Today)
                {
                    DataZwolnienia dataZwolnieniaView = new DataZwolnienia(carSupervisiorChange, personChange);
                    dataZwolnieniaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
                    dataZwolnieniaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
                    dataZwolnieniaView.ShowDialog();
                    while (dataZwolnieniaView.IsActive)
                    {

                    }
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new Pracownicy();
                    //aktualizacja widoku pracowników 
                    workersList.ItemsSource = null;
                    items.Clear();
                    workersList.ItemsSource = ListaPracownikow();
                }
                else
                    MessageBox.Show("Ta osoba została zwolniona", "Komunikat");

            }
            else
            {
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
            }
        }
        private void Usun_zwolnienie(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)workersList.SelectedItem;

            if (selected != null)
            {
                WorkersList selectedObj = (WorkersList)selected.Content;

                int selectedId = selectedObj.PersonId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var people = db.People;
                var carSupervisor = db.CarSupervisors;

                foreach (var person in people)
                {
                    if (person.id == selectedId)
                    {

                        if (person.layoffDate > DateTime.Today)
                        {
                            person.layoffDate = null;
                            MessageBox.Show("Usunieto zwolnienie", "Komunikat");
                        }
                        else
                        {
                            MessageBox.Show("Ta osoba została zwolniona!", "Komunikat");
                        }
                    }
                }
                db.SaveChanges();
                
                //aktualizacja widoku pracowników 
                workersList.ItemsSource = null;
                items.Clear();
                workersList.ItemsSource = ListaPracownikow();

            }
         
            else
            {
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
            }
        }
        
        private void WorkerStatistics_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)workersList.SelectedItem;

            if (selected != null)
            {
                WorkersList selectedObj = (WorkersList)selected.Content;

                int selectedId = selectedObj.PersonId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var people = db.People;
               // var carSupervisor = db.CarSupervisors;

                foreach (var person in people)
                {
                    if (person.id == selectedId)
                    {
                        System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
                        mainWindow.DataContext = new StatystykiPracownika(person);
                        return;

                    }
                }
            }
            else
            {
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
            }   
        }



        private void ZwolnieniBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            workersList.ItemsSource = ListaPracownikow();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(workersList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }
        private void DataZwolnieniaBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            workersList.ItemsSource = ListaPracownikow();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(workersList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }
        private void BezZwolnieniaBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            workersList.ItemsSource = ListaPracownikow();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(workersList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }

        private bool UserFilter(object item)
        {
            ListViewItem toFilter = (ListViewItem)item;

            if (dataOutFilter.Text.Equals("nie", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as WorkersList).PersonDateOut.CompareTo("") != 0)
                    return false;
                else
                    return true;
            }
            else if (dataOutFilter.Text.Equals("tak", StringComparison.OrdinalIgnoreCase) == true)
            {
                if ((toFilter.Content as WorkersList).PersonDateOut.CompareTo("") == 0)
                    return false;
                else
                    return true;
            }

            if (!String.IsNullOrEmpty(personFilter.Text))
                //jezeli item nie spelnia filtra opiekuna nie wyswietlam go
                if (!((toFilter.Content as WorkersList).Person.IndexOf(personFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(idFilter.Text))
                //jezeli item nie spelnia filtra id nie wyswietlam go
                if (!((toFilter.Content as WorkersList).PersonId.ToString().IndexOf(idFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            if (!String.IsNullOrEmpty(dataOutFilter.Text))
                //jezeli item nie spelnia filtra rejestracji nie wyswietlam go
                if (!((toFilter.Content as WorkersList).PersonDateOut.IndexOf(dataOutFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0))
                    return false;
            //cała reszte wyswietlam
            return true;
        }

        private void idFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(workersList.ItemsSource).Refresh();
        }

        private void personFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(workersList.ItemsSource).Refresh();
        }
        private void dataOutFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(workersList.ItemsSource).Refresh();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                workersList.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);

            var tempItems = items.ToArray();

            if (sortBy == "PersonId")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, ComparePeopleByPersonIdAscending);
                else
                    Array.Sort(tempItems, ComparePeopleByPersonIdDescending);
            }
            else if (sortBy == "Person")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, ComparePeopleByPersonAscending);
                else
                    Array.Sort(tempItems, ComparePeopleByPersonDescending);
            }
            else if (sortBy == "PersonDateOut")
            {
                if (newDir.ToString() == "Ascending")
                    Array.Sort(tempItems, ComparePeopleByDateAscending);
                else
                    Array.Sort(tempItems, ComparePeopleByDateDescending);
            }
            
            workersList.ItemsSource = tempItems;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(workersList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));

            view.Filter += UserFilter;
        }
        int ComparePeopleByPersonIdAscending(ListViewItem a, ListViewItem b)
        {
           WorkersList first = (WorkersList)a.Content;
            WorkersList second = (WorkersList)b.Content;
            return first.PersonId.CompareTo(second.PersonId);
        }
        int ComparePeopleByPersonIdDescending(ListViewItem a, ListViewItem b)
        {
            WorkersList first = (WorkersList)a.Content;
            WorkersList second = (WorkersList)b.Content;
            return second.PersonId.CompareTo(first.PersonId);
        }
        int ComparePeopleByPersonAscending(ListViewItem a, ListViewItem b)
        {
            WorkersList first = (WorkersList)a.Content;
            WorkersList second = (WorkersList)b.Content;
            return String.Compare(first.Person,second.Person);
          
        }
        int ComparePeopleByPersonDescending(ListViewItem a, ListViewItem b)
        {
            WorkersList first = (WorkersList)a.Content;
            WorkersList second = (WorkersList)b.Content;
            return String.Compare(second.Person, first.Person);
        }
        int ComparePeopleByDateAscending(ListViewItem a, ListViewItem b)
        {
            WorkersList first = (WorkersList)a.Content;
            WorkersList second = (WorkersList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.PersonDateOut.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.PersonDateOut);
            else
                firstDate = DateTime.MinValue;
            if (second.PersonDateOut.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.PersonDateOut);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);

          
        }
        int ComparePeopleByDateDescending(ListViewItem a, ListViewItem b)
        {
            WorkersList first = (WorkersList)a.Content;
            WorkersList second = (WorkersList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.PersonDateOut.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.PersonDateOut);
            else
                firstDate = DateTime.MinValue;
            if (second.PersonDateOut.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.PersonDateOut);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }

    }
}
