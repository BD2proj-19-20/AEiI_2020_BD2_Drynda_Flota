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
            workersList.ItemsSource = listaPracownikow();

        }

        public List<ListViewItem> listaPracownikow()
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
                else if(BezZwolnieniaBox.IsChecked.Value == true && (person.layoffDate is null))
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
                    workersList.ItemsSource = listaPracownikow();
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
                workersList.ItemsSource = listaPracownikow();

            }
         
            else
            {
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
            }
        }
        
        private void WorkerStatistics_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow.DataContext = new StatystykiPracownikaModel();
        }
        private void ZwolnieniBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            workersList.ItemsSource = listaPracownikow();
        }
        private void DataZwolnieniaBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            workersList.ItemsSource = listaPracownikow();
        }
        private void BezZwolnieniaBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            workersList.ItemsSource = listaPracownikow();
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
            workersList.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
            
        }
    }
}
