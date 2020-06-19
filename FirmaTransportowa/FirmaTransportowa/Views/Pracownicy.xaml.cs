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
    }


    public partial class Pracownicy : UserControl
    {

        List<ListViewItem> items = new List<ListViewItem>();

        public Pracownicy()
        {
            InitializeComponent();
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;

            foreach (var person in people)
            {
                ListViewItem OneItem = new ListViewItem();

                if (person.layoffDate < DateTime.Today)
                {
                    OneItem.Background = Brushes.Red;
                }
                OneItem.Content = new WorkersList { PersonId = person.id, Person = person.firstName + " " + person.lastName+person.layoffDate.ToString() };
                items.Add(OneItem);
            }
            
            workersList.ItemsSource = items;


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


                int selectedId = selectedObj.PersonId;
                
                // items.Remove((ListViewItem)workersList.SelectedItem);
                // workersList.ItemsSource = items;


                CarSupervisor carSupervisiorChange=null;

                Person personChange=null;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var people = db.People;
                var carSupervisor = db.CarSupervisors;
                foreach (var person in people)
                {
                    if (person.id == selectedId)
                    {
                        //nazwisko = person.lastName;
                        //imie = person.firstName;
                        personChange = person;
                        foreach (var carS in carSupervisor)
                        {
                            if (person.id == carS.personId)
                            {
                                // carS.endDate = DateTime.Today; 

                                carSupervisiorChange = carS;

                            }
                           
                            //person.layoffDate = DateTime.Today;
                        }   
                        

                    }
                }
                //db.SaveChanges();
                DataZwolnienia dataZwolnieniaView = new DataZwolnienia(carSupervisiorChange, personChange);
                dataZwolnieniaView.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
                dataZwolnieniaView.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2;
                dataZwolnieniaView.ShowDialog();
                while (dataZwolnieniaView.IsActive)
                {

                }
                System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                glowneOkno.DataContext = new Pracownicy();


              //  komunikat = "Zwolniono Pracownika: " + personChange.firstName + " " + personChange.lastName;
               //aktualizacja widoku pracowników 
                workersList.ItemsSource = null;
                items.Clear();

                foreach (var person in people)
                {
                    ListViewItem OneItem = new ListViewItem();

                    if (person.layoffDate < DateTime.Today )
                    {
                        OneItem.Background = Brushes.Red;
                    }
                    OneItem.Content = new WorkersList { PersonId = person.id, Person = person.firstName + " " + person.lastName };
                    items.Add(OneItem);
                }

                workersList.ItemsSource = items;

            }
            else
            {
                MessageBox.Show ("Nikogo nie wybrano !","Komunikat");
            }
            
            
           

        }
        private void WorkerStatistics_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow.DataContext = new StatystykiPracownikaModel();
        }
     
    }
}
