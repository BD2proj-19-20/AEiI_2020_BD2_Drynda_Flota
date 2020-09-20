using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using FirmaTransportowa.Model;
using System.ComponentModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Win32;

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
            Stopwatch stoper = new Stopwatch();
            stoper.Start();

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

            stoper.Stop();
            Title.Text = stoper.Elapsed.ToString();

            return items;
        }

        private void Dodaj_Pracownika(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new DodajPracownika();
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
                var person = (from people in db.People
                                   where people.id == selectedId
                              select people).FirstOrDefault();
               
                    if (person !=null)
                        personChange = person;

                if ((personChange.layoffDate is null) || personChange.layoffDate > DateTime.Today)
                {
                    DataZwolnienia dataZwolnieniaView = new DataZwolnienia(personChange);

                    dataZwolnieniaView.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen; //środek ekranu
                    dataZwolnieniaView.ShowDialog();
                    while (dataZwolnieniaView.IsActive)
                    {

                    }
                    System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
                    glowneOkno.DataContext = new Pracownicy();
                    //aktualizacja widoku pracowników 
                    workersList.ItemsSource = null;
                    items.Clear();
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

                var person = (from people in db.People
                              where people.id == selectedId
                              select people).FirstOrDefault();

                if (person.layoffDate > DateTime.Today)
                {
                    person.layoffDate = null;
                    MessageBox.Show("Usunieto zwolnienie", "Komunikat");
                }
                else
                    MessageBox.Show("Ta osoba została zwolniona!", "Komunikat");

                db.SaveChanges();

                //aktualizacja widoku pracowników 
                workersList.ItemsSource = null;
                items.Clear();
                UpdateView();
            }
            else
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
        }

        private void WorkerStatistics_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)workersList.SelectedItem;

            if (selected != null)
            {
                WorkersList selectedObj = (WorkersList)selected.Content;

                int selectedId = selectedObj.PersonId - 1;
                var db = new AEiI_2020_BD2_Drynda_FlotaEntities();


                var person = (from people in db.People
                              where people.id == selectedId
                              select people).FirstOrDefault();
                if (person != null)
                {
                    System.Windows.Window mainWindow = System.Windows.Application.Current.MainWindow;
                    mainWindow.DataContext = new StatystykiPracownika(person);
                    return;
                }
            }
            else
                MessageBox.Show("Nikogo nie wybrano !", "Komunikat");
        }

        private void Box_Click(object sender, RoutedEventArgs e)
        {
            workersList.ItemsSource = null;

            items.Clear();
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

        private string GetPath()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "Dokument pdf|*.pdf";
            saveFileDialog.Title = "Zapisz raport na temat pracowników";
            saveFileDialog.FileName = "Raport na temat pracowników - " + DateTime.Now.ToShortDateString();
            saveFileDialog.ShowDialog();

            return saveFileDialog.FileName;
        }

        private void Generuj_Raport_Pracownicy(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var carSupervisors = db.CarSupervisors;

           // var people = db.People.ToList().OrderBy(t => t.lastName);
            //var cars = db.Cars;
            //var activities = db.Activities;
            //var lends = db.Lends;

            string path = GetPath();

            Font times = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times.Size = 32;
            iTextSharp.text.Font times2 = FontFactory.GetFont("Arial", 20, new BaseColor(System.Drawing.Color.Black));
            iTextSharp.text.Font times3 = FontFactory.GetFont("Arial", 26, new BaseColor(System.Drawing.Color.Black));
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            //Tutaj beda wszyscy pracownicy z ich uprawnieniami
            var query = from person in db.People 
                        select new
                        {
                            Id = person.id,
                            LastName = person.lastName,
                            FirstName = person.firstName,
                            EmploymentData = person.employmentData,
                            LayoffDate = person.layoffDate,
                        };

            foreach (var person in query)
            {
                //Tutaj beda wszystkie pojazdy, jakich pracownik byl opiekunem
                var query2 = from supervisor in db.CarSupervisors
                             where person.Id == supervisor.personId
                             join car in db.Cars on supervisor.carId equals car.id
                             select new
                             {
                                 BeginDate = supervisor.beginDate == null ? DateTime.MinValue : supervisor.beginDate,
                                 EndDate = supervisor.endDate == null ? DateTime.MinValue : supervisor.endDate,
                                 SaleDate = car.saleDate == null ? DateTime.MinValue : car.saleDate,
                                 CarMake = car.CarModel.make,
                                 CarModel = car.CarModel.model,
                                 CarRegistration = car.Registration,
                             };


                var query3 = from lends2 in db.Lends
                             where lends2.personId == person.Id
                             select new
                             {
                                 LendDate = lends2.lendDate == null ? DateTime.MinValue : lends2.lendDate,
                                 EngineCar = lends2.Car.engineCapacity, // == null ? 0 : lends2.Car.engineCapacity,
                                 ReservationEnd = lends2.Reservation.ended,
                                 ReturnDate = lends2.returnDate == null ? DateTime.MinValue : lends2.returnDate,
                                 Private = lends2.@private,
                                 StartOdometer = lends2.startOdometer,
                                 StartFuel = lends2.startFuel,
                                 EndOdometer = lends2.endOdometer,
                                 EndFuel = lends2.endFuel,
                                 PlannedReturnDate = lends2.plannedReturnDate,
                                 LendedCar = lends2.Car
                             };

                var workerPermission = (from personPermission in db.PeoplesPermissions
                                        where personPermission.personId == person.Id
                                        select personPermission).FirstOrDefault();


                Chunk c = new Chunk((person.Id + 1) + ") " + person.LastName + " " + person.FirstName, times);
                var dateO = "";
                var dateE = "";
                var date = "";
                string namePerson = person.LastName + " " + person.FirstName;

                if (person.LayoffDate != null)
                {

                    string dateTime = person.LayoffDate.ToString();
                    date = dateTime.Substring(0, 10);
                }

                if (person.LayoffDate <= DateTime.Today && ZwolnieniBox.IsChecked.Value == true && Regex.IsMatch(namePerson, personFilter.Text, RegexOptions.IgnoreCase)
                   && Regex.IsMatch(date, dataOutFilter.Text, RegexOptions.IgnoreCase)
                    && Regex.IsMatch((person.Id + 1).ToString(), idFilter.Text))

                {
                    string dateTime = person.LayoffDate.ToString();
                    dateO = dateTime.Substring(0, 10);
                    dateTime = person.EmploymentData.ToString();
                    dateE = dateTime.Substring(0, 10);


                    c.SetBackground(BaseColor.RED);
                    doc.Add(new iTextSharp.text.Paragraph(c));
                    doc.Add(new iTextSharp.text.Paragraph("Zatrudniony: " + dateE, times2));
                    doc.Add(new iTextSharp.text.Paragraph("Zwolniony: " + dateO, times2));

                }
                else if (person.LayoffDate > DateTime.Today && DataZwolnieniaBox.IsChecked.Value == true &&
                    Regex.IsMatch(namePerson, personFilter.Text, RegexOptions.IgnoreCase) && Regex.IsMatch(date, dataOutFilter.Text, RegexOptions.IgnoreCase)
                    && Regex.IsMatch((person.Id + 1).ToString(), idFilter.Text))
                {

                    string dateTime = person.LayoffDate.ToString();
                    dateO = dateTime.Substring(0, 10);
                    dateTime = person.EmploymentData.ToString();
                    dateE = dateTime.Substring(0, 10);


                    c.SetBackground(BaseColor.ORANGE);
                    doc.Add(new iTextSharp.text.Paragraph(c));
                    doc.Add(new iTextSharp.text.Paragraph("Zatrudniony: " + dateE, times2));
                    doc.Add(new iTextSharp.text.Paragraph("Zwolnionienie: " + dateO, times2));
                }
                else if (BezZwolnieniaBox.IsChecked.Value == true && (person.LayoffDate is null) &&
                    Regex.IsMatch(namePerson, personFilter.Text, RegexOptions.IgnoreCase) && Regex.IsMatch(date, dataOutFilter.Text, RegexOptions.IgnoreCase)
                         && Regex.IsMatch((person.Id + 1).ToString(), idFilter.Text))
                {

                    string dateTime = person.EmploymentData.ToString();
                    dateE = dateTime.Substring(0, 10);

                    doc.Add(new iTextSharp.text.Paragraph(c));
                    doc.Add(new iTextSharp.text.Paragraph("Zatrudniony: " + dateE, times2));
                }
                else
                {
                    continue;
                }


                string kierownik = "";
                string kierownikStart = "";
                string kierownikEnd = "";

                if (workerPermission != null && workerPermission.Permission.name == "Kierownik" &&
       workerPermission.grantDate <= DateTime.Now.Date && (workerPermission.revokeDate > DateTime.Now || workerPermission.revokeDate == null))
                {
                    kierownik = "Tak";
                    kierownikStart = workerPermission.grantDate.ToString().Substring(0, 10);
                    if (workerPermission.revokeDate != null)
                        kierownikEnd = workerPermission.revokeDate.ToString().Substring(0, 10);

                }
                else if (workerPermission != null && workerPermission.Permission.name == "Kierownik" && workerPermission.grantDate > DateTime.Now.Date)
                {
                    if (((workerPermission.grantDate - DateTime.Now).Days + 1) == 1)
                        kierownik = "Za " + ((workerPermission.grantDate - DateTime.Now.Date).Days + 1) + " dzień";
                    else
                        kierownik = "Za " + ((workerPermission.grantDate - DateTime.Now.Date).Days + 1) + " dni";
                    kierownikStart = workerPermission.grantDate.ToString().Substring(0, 10);
                    if (workerPermission.revokeDate != null)
                        kierownikEnd = workerPermission.revokeDate.ToString().Substring(0, 10);
                }

                if (kierownik != "")
                    doc.Add(new iTextSharp.text.Paragraph("Kierownik: " + kierownik, times2));
                if (kierownikStart != "")
                    doc.Add(new iTextSharp.text.Paragraph("Data rozopoczęcia: " + kierownikStart, times2));
                if (kierownikEnd != "")
                    doc.Add(new iTextSharp.text.Paragraph("Data zakończenia: " + kierownikEnd, times2));



                string textOpiekun = "";
                string bylyOpiekun = "";


                foreach (var personSup in query2)

                {
                    if ((personSup.EndDate > DateTime.Today || personSup.EndDate == null || personSup.EndDate == DateTime.MinValue) &&
                        (personSup.SaleDate > DateTime.Today || personSup.SaleDate == null || personSup.SaleDate == DateTime.MinValue))
                        textOpiekun += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                else
                    bylyOpiekun += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";

                 
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
                int przejechaneKm = 0;
                int zleceniaPrywatne = 0;
                int zleceniaSluzbowe = 0;
                int przejechaneKmSluzbowe = 0;


                int dni = 0;
                int dniSluzbowe = 0;

                var pojazd = "";
                var pojazdSluzbowy = "";

                double koszty = 0;
                double kosztySluzbowe = 0;


                foreach (var personLend in query3)
                {
                    if (personLend.ReservationEnd == true && personLend.ReturnDate != null) //lend ktore były
                    {
                        if (personLend.Private == true)
                        {
                            zleceniaPrywatne++;
                            if (personLend.EndOdometer != null)
                                przejechaneKm += personLend.EndOdometer.Value - personLend.StartOdometer;
                            if (personLend.ReturnDate > personLend.LendDate)
                            {
                                TimeSpan t = (DateTime)personLend.ReturnDate - personLend.LendDate;
                                dni += (int)t.TotalDays;
                            }

                            //jaki to samochód;

                            koszty = (przejechaneKm * 4.75) + (0.05 * personLend.EngineCar);


                        }
                        else
                        {
                            zleceniaSluzbowe++;
                            if (personLend.EndOdometer != null)
                                przejechaneKmSluzbowe += personLend.EndOdometer.Value - personLend.StartOdometer;
                            if (personLend.ReturnDate > personLend.LendDate)
                            {
                                TimeSpan t = (DateTime)personLend.ReturnDate - personLend.LendDate;
                                dniSluzbowe += (int)t.TotalDays;
                            }

                            kosztySluzbowe = (przejechaneKmSluzbowe * 4.75) + (0.05 * personLend.EngineCar);
                        }
                    }


                    if (personLend.LendDate < DateTime.Today && personLend.PlannedReturnDate < DateTime.Today && 
                        personLend.ReturnDate > DateTime.Today) //aktualny pojazd
                    {

                        foreach (var personSup in query2)
                        {
                            if (personLend.Private == true)
                                pojazd += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                            else
                                pojazdSluzbowy += personSup.CarMake + "/" + personSup.CarModel + "/" + personSup.CarRegistration + "\n";
                        }
                    }

                }

                //Koszty??? jeszcze do dodania

                doc.Add(new iTextSharp.text.Paragraph("Cele prywatne:", times));
                times3.Size = 20;
                doc.Add(new iTextSharp.text.Paragraph("Przejechane: " + przejechaneKm.ToString() + " km", times3));
                doc.Add(new iTextSharp.text.Paragraph("Aktualny pojazd: " + pojazd, times3));
                doc.Add(new iTextSharp.text.Paragraph("Wykonane zlecenia: " + zleceniaPrywatne.ToString(), times3));
                doc.Add(new iTextSharp.text.Paragraph("Czas jazdy " + dni.ToString() + " dni", times3));
                doc.Add(new iTextSharp.text.Paragraph("Koszty: " + koszty.ToString() + " PLN", times3));

                doc.Add(new iTextSharp.text.Paragraph("Cele służbowe:", times));
                doc.Add(new iTextSharp.text.Paragraph("Przejechane: " + przejechaneKmSluzbowe.ToString() + " km", times3));
                doc.Add(new iTextSharp.text.Paragraph("Aktualny pojazd: " + pojazdSluzbowy, times3));
                doc.Add(new iTextSharp.text.Paragraph("Wykonane zlecenia: " + zleceniaSluzbowe.ToString(), times3));
                doc.Add(new iTextSharp.text.Paragraph("Czas jazdy " + dniSluzbowe.ToString() + " dni", times3));
                doc.Add(new iTextSharp.text.Paragraph("Koszty: " + kosztySluzbowe.ToString() + " PLN", times3));
                times3.Size = 26;


                times.Size = 32;

            }
            

            Chunk c1 = new Chunk("");
            doc.Add(c1); //doc nie może być pusty 

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


