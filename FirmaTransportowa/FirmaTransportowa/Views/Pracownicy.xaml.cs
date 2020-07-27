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
using iTextSharp.text;
using iTextSharp.text.pdf;
using MaterialDesignThemes.Wpf;
using System.IO;

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
        public void UpdateView()
        {
            workersList.ItemsSource = ListaPracownikow();


            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(workersList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));
            view.Filter += UserFilter;


        }

        public Pracownicy()
        {
            InitializeComponent();
            UpdateView();
        }

        public List<ListViewItem> ListaPracownikow()
        {

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var people = db.People;

            foreach (var person in people)
            {
                ListViewItem OneItem = new ListViewItem();
                var date = "";

                if (person.layoffDate <= DateTime.Today && ZwolnieniBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.Red;
                    string dateTime = person.layoffDate.ToString();
                    date = dateTime.Substring(0, 10);
                    OneItem.Content = new WorkersList { PersonId = person.id + 1, Person = person.lastName + " " + person.firstName, PersonDateOut = date };
                    items.Add(OneItem);
                }
                else if (person.layoffDate > DateTime.Today && DataZwolnieniaBox.IsChecked.Value == true)
                {
                    OneItem.Background = Brushes.Orange;
                    string dateTime = person.layoffDate.ToString();
                    date = dateTime.Substring(0, 10);
                    OneItem.Content = new WorkersList { PersonId = person.id + 1, Person = person.lastName + " " + person.firstName, PersonDateOut = date };
                    items.Add(OneItem);
                }
                else if (BezZwolnieniaBox.IsChecked.Value == true && (person.layoffDate is null))
                {
                    OneItem.Content = new WorkersList { PersonId = person.id + 1, Person = person.lastName + " " + person.firstName, PersonDateOut = date };
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

                Person personChange = null;

                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
                var people = db.People;
                var carSupervisor = db.CarSupervisors;
                foreach (var person in people)
                {
                    if (person.id == selectedId)
                    {
                        personChange = person;

                    }
                }

                if ((personChange.layoffDate is null) || personChange.layoffDate > DateTime.Today)
                {
                    DataZwolnienia dataZwolnieniaView = new DataZwolnienia(personChange);
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
                    //  workersList.ItemsSource = ListaPracownikow();
                    UpdateView();
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
                UpdateView();
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
            // workersList.ItemsSource = ListaPracownikow();

            UpdateView();
        }
        private void DataZwolnieniaBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            // workersList.ItemsSource = ListaPracownikow();
            UpdateView();
        }
        private void BezZwolnieniaBox_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;
            items.Clear();
            //  workersList.ItemsSource = ListaPracownikow();
            UpdateView();
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
        private void Generuj_Raport_Pracownicy(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carSupervisors = db.CarSupervisors;
            //  var cars = db.Cars.ToList().OrderBy(t => t.Registration);
            var people = db.People.ToList().OrderBy(t => t.lastName);
            var cars = db.Cars;


            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";  //pobranie lokalizacji pulpitu

            // FontFactory.Register("C:\\Windows\\Fonts\\ARIALUNI.TTF", "arial unicode ms");
            Font times = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times.Size = 32;
            iTextSharp.text.Font times2 = FontFactory.GetFont("Arial", 20, new BaseColor(System.Drawing.Color.Black));
            iTextSharp.text.Font times3 = FontFactory.GetFont("Arial", 26, new BaseColor(System.Drawing.Color.Black));
            FileStream fs = new FileStream(path + "Raport na temat pracowników" + DateTime.Now.ToShortDateString() + ".pdf", FileMode.Create, FileAccess.Write, FileShare.None);

            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();


            //z checkboxami zrob
            foreach (var person in people)
            {
                Chunk c = new Chunk(person.lastName + " " + person.firstName, times);
                var dateO = "";
                var dateE = "";

                if (person.layoffDate <= DateTime.Today && ZwolnieniBox.IsChecked.Value == true)
                {
                    string dateTime = person.layoffDate.ToString();
                    dateO = dateTime.Substring(0, 10);
                    dateTime = person.employmentData.ToString();
                    dateE = dateTime.Substring(0, 10);


                    c.SetBackground(BaseColor.RED);
                    doc.Add(new iTextSharp.text.Paragraph(c));
                    doc.Add(new iTextSharp.text.Paragraph("Zatrudniony:" + dateE, times2));
                    doc.Add(new iTextSharp.text.Paragraph("Zwolniony:" + dateO, times2));

                }
                else if (person.layoffDate > DateTime.Today && DataZwolnieniaBox.IsChecked.Value == true)
                {

                    string dateTime = person.layoffDate.ToString();
                    dateO = dateTime.Substring(0, 10);
                    dateTime = person.employmentData.ToString();
                    dateE = dateTime.Substring(0, 10);


                    c.SetBackground(BaseColor.ORANGE);
                    doc.Add(new iTextSharp.text.Paragraph(c));
                    doc.Add(new iTextSharp.text.Paragraph("Zatrudniony:" + dateE, times2));
                    doc.Add(new iTextSharp.text.Paragraph("Zwolnionienie:" + dateO, times2));
                }
                else if (BezZwolnieniaBox.IsChecked.Value == true && (person.layoffDate is null))
                {

                    string dateTime = person.employmentData.ToString();
                    dateE = dateTime.Substring(0, 10);

                    doc.Add(new iTextSharp.text.Paragraph(c));
                    doc.Add(new iTextSharp.text.Paragraph("Zatrudniony:" + dateE, times2));
                }
                else
                {
                    continue;
                }


                string textOpiekun = "";
                string bylyOpiekun = "";

                foreach (var carS in carSupervisors)
                {
                    if (carS.personId == person.id)
                    {

                        foreach (var car in cars)
                            if (car.id == carS.carId && (carS.endDate > DateTime.Today || carS.endDate == null) && (car.saleDate > DateTime.Today || car.saleDate == null))
                            {
                                textOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                            }
                            else if (car.id == carS.carId)
                            {
                                bylyOpiekun += car.CarModel.make + "/" + car.CarModel.model + "/" + car.Registration + "\n";
                            }
                    }

                }
                if (textOpiekun != "")
                {
                    doc.Add(new iTextSharp.text.Paragraph("Opiekun: ", times3));
                    doc.Add(new iTextSharp.text.Paragraph(textOpiekun, times2));
                }
                if (bylyOpiekun != "")
                {
                    doc.Add(new iTextSharp.text.Paragraph("Byly Opiekun: ", times3));
                    doc.Add(new iTextSharp.text.Paragraph(bylyOpiekun, times2));
                }
                //foreach (var carSupervisor in carSupervisors)
                //{
                //    if (person.id == carSupervisor.carId)
                //    {
                //        carSupervisor.beginDate.ToShortDateString();
                //        string endDate = "";
                //        if (carSupervisor.endDate != null)
                //        {
                //            DateTime temp = (DateTime)carSupervisor.endDate;
                //            endDate = temp.ToShortDateString();
                //        }
                //        doc.Add(new iTextSharp.text.Chunk(carSupervisor.Person.id + " " + carSupervisor.Person.firstName + " " + carSupervisor.Person.lastName + ": " + carSupervisor.beginDate.ToShortDateString() + " - " + endDate + "\n"));
                //    }
                //}

            }
            doc.Close();
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
            //   view.SortDescriptions.Add(new SortDescription("PersonId", ListSortDirection.Descending));
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
            return String.Compare(first.Person, second.Person);

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