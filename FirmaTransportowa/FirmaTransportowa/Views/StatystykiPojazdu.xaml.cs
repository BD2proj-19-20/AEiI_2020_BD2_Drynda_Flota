using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Statystyki.xaml
    /// </summary>
    public partial class StatystykiPojazdu : UserControl
    {
        int permission = 0;
        Car car1;
        public StatystykiPojazdu(Car car, int userPermission)
        {
            InitializeComponent();
            permission = userPermission;
            car1 = car;
            Rejestracja.Text = car.Registration;
            Pojemnosc_silnika.Text = car.engineCapacity.ToString();

            string saleDate = "";
            string purchaseDate = "";
            if (car.saleDate != null)
            {
                DateTime temp = (DateTime)car.saleDate;
                saleDate = temp.ToShortDateString();
            }
            if (car.purchaseDate != null)
            {
                DateTime temp2 = (DateTime)car.purchaseDate;
                purchaseDate = temp2.ToShortDateString();
            }
            Data_zakupu.Text = purchaseDate;
            Data_sprzedaży.Text = saleDate;
            Przeglad.Text = car.inspectionValidUntil.ToShortDateString();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var models = db.CarModels;
            foreach (var model in models)
            {
                if (car.modelId == model.id)
                {
                    ModelPojazdu.Text = model.make + " " + model.model;
                    Marka.Text = model.make;
                    Model.Text = model.model;
                }
            }

            Zastosowanie.Text = car.CarDestination.name;

            var lends = car.Lends;
            foreach (var lend in lends)
            {
                if (lend.returnDate == null)
                {
                    Wypozyczony_od.Text = lend.lendDate.ToShortDateString();
                    string planowanyZwrot = "";
                    if (lend.plannedReturnDate != null)
                    {
                        DateTime temp = (DateTime)lend.plannedReturnDate;
                        planowanyZwrot = temp.ToShortDateString();
                    }
                    Planowany_zwrot.Text = planowanyZwrot;
                    Wypozyczony_przez.Text = lend.Person.firstName + " " + lend.Person.lastName;
                }
            }

        }
        private void Cofnij(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            switch (permission)
            {
                case 1:
                    glowneOkno.DataContext = new ListaPojazdowModel();
                    break;
                case 2:
                    glowneOkno.DataContext = new ZarzadzajPojazdamiModel();
                    break;
                case 3:
                    glowneOkno.DataContext = new MojePojazdyModel();
                    break;
                default:
                    break;
            }
        }

        private void GenerateRaport(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "Dokument pdf|*.pdf";
            saveFileDialog.Title = "Zapisz raport na temat pojazdów";
            saveFileDialog.ShowDialog();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var car = (from cars in db.Cars
                       where cars.id == car1.id select cars).FirstOrDefault();

            iTextSharp.text.Font times = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times.Size = 32;
            iTextSharp.text.Font times2 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times2.Size = 20;
            iTextSharp.text.Font times3 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true)); //polskie znaki
            times3.Size = 14;
            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            //SAMOCHÓD
            doc.Add(new iTextSharp.text.Paragraph(car.id + "   " + car.Registration + "\n", times));
            //SAMOCHÓD

            //DATA ZAKUPU I SPRZEDAŻY
            doc.Add(new iTextSharp.text.Paragraph("Data zakupu: " + car.purchaseDate.ToShortDateString(), times2));
            if (car.saleDate != null)
                doc.Add(new iTextSharp.text.Paragraph("Data sprzedaży: " + car.saleDate, times2));
            //DATA ZAKUPU I SPRZEDAŻY

            //CZY SPRAWNY
            string onService = car.onService == false ? "Tak" : "Nie";
            doc.Add(new iTextSharp.text.Paragraph("Sprawny? " + onService, times2));
            //CZY SPRAWNY

            //DANE POJAZDU
            doc.Add(new iTextSharp.text.Chunk("Marka: " + car.CarModel.make + "\n", times3));
            doc.Add(new iTextSharp.text.Chunk("Model: " + car.CarModel.model + "\n", times3));
            doc.Add(new iTextSharp.text.Chunk("Pojemność silnika: " + car.engineCapacity + "\n", times3));
            doc.Add(new iTextSharp.text.Chunk("Zastosowanie: " + car.CarDestination.name + "\n", times3));
            doc.Add(new iTextSharp.text.Chunk("Przegląd ważny do: " + car.inspectionValidUntil.ToShortDateString() + "\n\n", times3));
            //DANE POJAZDU

            //HISTORIA OPIEKUNÓW
            var carSupervisors = from carSupervisor in db.CarSupervisors
                                 where carSupervisor.carId == car.id
                                 orderby carSupervisor.beginDate
                                 select new
                                 {
                                     carSupervisor.Person,
                                     BeginDate = carSupervisor.beginDate,
                                     EndDate = carSupervisor.endDate
                                 };

            if (carSupervisors.Count() != 0)
                doc.Add(new iTextSharp.text.Paragraph("Historia opiekunów: ", times2));
            foreach (var carSupervisor in carSupervisors)
            {
                carSupervisor.BeginDate.ToShortDateString();
                string endDate = "";
                if (carSupervisor.EndDate != null)
                {
                    endDate = carSupervisor.EndDate.ToString();
                }
                doc.Add(new iTextSharp.text.Chunk(carSupervisor.Person.id + " " + carSupervisor.Person.firstName + " "
                    + carSupervisor.Person.lastName + ": " + carSupervisor.BeginDate.ToString() + " - "
                    + endDate + "\n", times3));
            }
            //HISTORIA OPIEKUNÓW

            //HISTORIA WYPOŻYCZEŃ
            var carLends = from carLend in db.Lends
                           where carLend.carId == car.id
                           orderby carLend.lendDate
                           select new
                           {
                               carLend.Person,
                               BeginDate = carLend.lendDate,
                               EndDate = carLend.returnDate
                           };

            if (carLends.Count() != 0)
                doc.Add(new iTextSharp.text.Paragraph("\nHistoria wypożyczeń: ", times2));
            foreach (var carLend in carLends)
            {
                carLend.BeginDate.ToShortDateString();
                string endDate = "";
                if (carLend.EndDate != null)
                {
                    endDate = carLend.EndDate.ToString();
                }
                doc.Add(new iTextSharp.text.Chunk(carLend.Person.id + " " + carLend.Person.firstName + " "
                    + carLend.Person.lastName + ": " + carLend.BeginDate.ToString() + " - "
                    + endDate + "\n", times3));
            }
            //HISTORIA WYPOŻYCZEŃ

            //HISTORIA AKTYWNOŚCI
            var carActivities = from carActivity in db.Activities
                                where carActivity.carId == car.id
                                orderby carActivity.reportDate
                                select new
                                {
                                    Name = carActivity.comments,
                                    BeginDate = carActivity.reportDate,
                                    EndDate = carActivity.closeDate
                                };

            if (carActivities.Count() != 0)
                doc.Add(new iTextSharp.text.Paragraph("\nHistoria aktywności: ", times2));
            foreach (var carActivity in carActivities)
            {
                carActivity.BeginDate.ToShortDateString();
                string endDate = "";
                if (carActivity.EndDate != null)
                {
                    endDate = carActivity.EndDate.ToString();
                }
                doc.Add(new iTextSharp.text.Chunk(carActivity.Name + " " + carActivity.BeginDate.ToString() + " - "
                    + endDate + "\n", times3));
            }
            //HISTORIA AKTYWNOŚCI

            doc.NewPage();

            doc.Close();
            stopwatch.Stop();
            MessageBox.Show("Raport wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        private void rezerwacjeClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new RezerwacjePojazdu(permission, car1);
        }

        private void historiaClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new HistoriaPojazdu(permission, car1);
        }

        private void UsterkiClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Usterkipojazdu(permission, car1);
        }
    }
}
