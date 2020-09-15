using FirmaTransportowa.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirmaTransportowa
{
    public static class RaportGenerator
    {
        private static iTextSharp.text.Font Font32 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true))
        {
            Size = 32
        };

        private static iTextSharp.text.Font Font20 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true))
        {
            Size = 20
        };

        private static iTextSharp.text.Font Font14 = new iTextSharp.text.Font(BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.CP1250, true))
        {
            Size = 14
        };

        private static string GetPath()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "Dokument pdf|*.pdf";
            saveFileDialog.Title = "Zapisz raport na temat pojazdów";
            saveFileDialog.ShowDialog();

            return saveFileDialog.FileName;
        }

        private static void GeneralInfoAboutCar(Car car, Document doc)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            //SAMOCHÓD
            doc.Add(new iTextSharp.text.Paragraph(car.id + "   " + car.Registration + "\n", Font32));
            //SAMOCHÓD

            //DATA ZAKUPU I SPRZEDAŻY
            doc.Add(new iTextSharp.text.Paragraph("Data zakupu: " + car.purchaseDate.ToShortDateString(), Font20));
            if (car.saleDate != null)
                doc.Add(new iTextSharp.text.Paragraph("Data sprzedaży: " + car.saleDate, Font20));
            //DATA ZAKUPU I SPRZEDAŻY

            //CZY SPRAWNY
            string onService = car.onService == false ? "Tak" : "Nie";
            doc.Add(new iTextSharp.text.Paragraph("Sprawny? " + onService, Font20));
            //CZY SPRAWNY

            //DANE POJAZDU
            doc.Add(new iTextSharp.text.Chunk("Marka: " + car.CarModel.make + "\n", Font14));
            doc.Add(new iTextSharp.text.Chunk("Model: " + car.CarModel.model + "\n", Font14));
            doc.Add(new iTextSharp.text.Chunk("Pojemność silnika: " + car.engineCapacity + "\n", Font14));
            doc.Add(new iTextSharp.text.Chunk("Zastosowanie: " + car.CarDestination.name + "\n", Font14));
            doc.Add(new iTextSharp.text.Chunk("Przegląd ważny do: " + car.inspectionValidUntil.ToShortDateString() + "\n\n", Font14));
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
                doc.Add(new iTextSharp.text.Paragraph("Historia opiekunów: ", Font20));
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
                    + endDate + "\n", Font14));
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
                doc.Add(new iTextSharp.text.Paragraph("\nHistoria wypożyczeń: ", Font20));
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
                    + endDate + "\n", Font14));
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
                doc.Add(new iTextSharp.text.Paragraph("\nHistoria aktywności: ", Font20));
            foreach (var carActivity in carActivities)
            {
                carActivity.BeginDate.ToShortDateString();
                string endDate = "";
                if (carActivity.EndDate != null)
                {
                    endDate = carActivity.EndDate.ToString();
                }
                doc.Add(new iTextSharp.text.Chunk(carActivity.Name + " " + carActivity.BeginDate.ToString() + " - "
                    + endDate + "\n", Font14));
            }
            //HISTORIA AKTYWNOŚCI

            doc.NewPage();

        }

        public static void GenerateGeneralRaportAboutOneCar(Car car)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();
            if (path == "")
                return;

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            GeneralInfoAboutCar(car, doc);

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        public static void GenerateGeneralRaportAboutCars(IQueryable<Car> cars)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();
            if (path == "")
                return;

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            foreach (var car in cars)
            {
                GeneralInfoAboutCar(car, doc);
            }

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        private static void CostInfoAboutCar(Car car, Document doc, DateTime? raportBegin, DateTime? raportEnd)
        {
            var distance = 0;
            var carCosts = 0.0;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var lends = from Lends in db.Lends
                        where car.id == Lends.carId
                        select Lends;

            if (raportBegin != null)
            {
                lends = lends.Where(x => x.lendDate >= raportBegin);
            }
            if (raportEnd != null)
            {
                lends = lends.Where(x => x.returnDate <= raportEnd);
            }



            foreach (var lend in lends)
            {
                if (lend.endOdometer != null)
                    distance += lend.endOdometer.Value - lend.startOdometer;

                carCosts += (distance * 4.75) + (0.05 * lend.Car.engineCapacity);
            }

            carCosts = Math.Round(carCosts, 2, MidpointRounding.AwayFromZero);

            var costPerKm = 0.0;

            if (distance != 0)
                costPerKm = carCosts / distance;

            costPerKm = Math.Round(costPerKm, 2, MidpointRounding.AwayFromZero);

            //SAMOCHÓD
            doc.Add(new iTextSharp.text.Paragraph(car.id + "   " + car.Registration + "\n", Font32));
            //SAMOCHÓD

            //O KOSZTACH
            doc.Add(new iTextSharp.text.Paragraph("Przejechany dystans: " + distance + "\n", Font14));
            doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa: " + carCosts + "\n", Font14));
            doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / 1km: " + costPerKm + "\n", Font14));
            //O KOSZTACH

            doc.NewPage();
        }

        public static void GenerateCostsRaportAboutCars(IQueryable<Car> cars, DateTime? raportBegin, DateTime? raportEnd)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            foreach (var car in cars)
            {
                CostInfoAboutCar(car, doc, raportBegin, raportEnd);
            }

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport kosztów wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        public static void GenerateCostsRaportAboutCar(Car car, DateTime? raportBegin, DateTime? raportEnd)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            CostInfoAboutCar(car, doc, raportBegin, raportEnd);

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport kosztów wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }
    }
}

