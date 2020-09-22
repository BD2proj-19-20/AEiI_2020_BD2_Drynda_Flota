using FirmaTransportowa.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FirmaTransportowa
{
    public class CarCost
    {
        public double fuelCost { get; set; }
        public double serviceCost { get; set; }
        public double fuelCostPerKm { get; set; }
        public double serviceCostPerKm { get; set; }
    }
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

        private static CarCost CostInfoAboutCar(Car car, Document doc, DateTime? raportBegin, DateTime? raportEnd, bool write)
        {
            var distance = 0;
            var fuelCost = 0.0;
            var serviceCost = 0.0;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            //KOSZTA PALIWA
            var lends = from Lends in db.Lends
                        where car.id == Lends.carId
                        select Lends;

            if (raportBegin != null)
                lends = lends.Where(x => x.lendDate >= raportBegin);
            if (raportEnd != null)
                lends = lends.Where(x => x.returnDate <= raportEnd);

            foreach (var lend in lends)
            {
                if (lend.endOdometer != null)
                    distance += lend.endOdometer.Value - lend.startOdometer;

                fuelCost += (distance * 4.75) + (0.05 * lend.Car.engineCapacity);
            }

            fuelCost = Math.Round(fuelCost, 2, MidpointRounding.AwayFromZero);

            var fuelCostPerKm = 0.0;

            if (distance != 0)
                fuelCostPerKm = fuelCost / distance;

            fuelCostPerKm = Math.Round(fuelCostPerKm, 2, MidpointRounding.AwayFromZero);
            //KOSZTA PALIWA

            //KOSZTA SERWISU
            var services = from Services in db.Activities
                           where car.id == Services.carId
                           select Services;

            if (raportBegin != null)
                services = services.Where(x => x.reportDate >= raportBegin);
            if (raportEnd != null)
                services = services.Where(x => x.reportDate >= raportBegin);

            foreach (var service in services)
            {
                serviceCost += service.price == null ? 0 : (double)service.price;
            }

            serviceCost = Math.Round(serviceCost, 2, MidpointRounding.AwayFromZero);

            var serviceCostPerKm = 0.0;

            if (distance != 0)
                serviceCostPerKm = serviceCost / distance;

            serviceCostPerKm = Math.Round(serviceCostPerKm, 2, MidpointRounding.AwayFromZero);
            //KOSZTA SERWISU

            if (write)
            {
                //SAMOCHÓD
                doc.Add(new iTextSharp.text.Paragraph(car.id + "   " + car.Registration + "\n\n", Font32));
                //SAMOCHÓD

                //O KOSZTACH
                doc.Add(new iTextSharp.text.Paragraph("Przejechany dystans: " + distance + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa: " + fuelCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / 1km: " + fuelCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu: " + serviceCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / 1km: " + serviceCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta: " + (fuelCost + serviceCost) + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta / 1km: " + (fuelCostPerKm + serviceCostPerKm) + "\n", Font14));
                //O KOSZTACH

                doc.NewPage();
            }

            CarCost carCost = new CarCost
            {
                fuelCost = fuelCost,
                serviceCost = serviceCost,
                fuelCostPerKm = fuelCostPerKm,
                serviceCostPerKm = serviceCostPerKm
            };
            return carCost;
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
                CostInfoAboutCar(car, doc, raportBegin, raportEnd, true);
            }

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport kosztów wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        public static void GenerateCostsRaportAboutCarsDestinations(DateTime? raportBegin, DateTime? raportEnd)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var destinations = from Destinations in db.CarDestinations
                               select Destinations;

            foreach (var destination in destinations)
            {
                var cars = from Cars in db.Cars
                           where Cars.CarDestination.id == destination.id
                           select Cars;

                double destinationFuelCost = 0.0;
                double destinationServiceCost = 0.0;
                double destinationFuelCostPerKm = 0.0;
                double destinationServiceCostPerKm = 0.0;

                foreach (var car in cars)
                {
                    CarCost carCost = CostInfoAboutCar(car, doc, raportBegin, raportEnd, false);
                    destinationFuelCost += carCost.fuelCost;
                    destinationServiceCost += carCost.serviceCost;
                    destinationFuelCostPerKm += carCost.fuelCostPerKm;
                    destinationServiceCostPerKm += carCost.serviceCostPerKm;
                }

                //ZASTOSOWANIE
                doc.Add(new iTextSharp.text.Paragraph(destination.name + "\n\n", Font32));
                //ZASTOSOWANIE

                //O KOSZTACH
                var destinationFuelCostPerCar = destinationFuelCost / cars.Count();
                destinationFuelCostPerCar = Math.Round(destinationFuelCostPerCar, 2, MidpointRounding.AwayFromZero);
                var destinationServiceCostPerCar = destinationServiceCost / cars.Count();
                destinationServiceCostPerCar = Math.Round(destinationServiceCostPerCar, 2, MidpointRounding.AwayFromZero);

                doc.Add(new iTextSharp.text.Paragraph("Samochodów: " + cars.Count() + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa: " + destinationFuelCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / samochoód: " + destinationFuelCostPerCar + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / 1km: " + destinationFuelCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu: " + destinationServiceCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / samochód: " + destinationServiceCostPerCar + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / 1km: " + destinationServiceCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta: " + (destinationFuelCost + destinationServiceCost) + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta/ samochod: " + (destinationFuelCostPerCar + destinationServiceCostPerCar) + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta / 1km: " + (destinationFuelCostPerKm + destinationServiceCostPerKm) + "\n", Font14));
                //O KOSZTACH

                doc.NewPage();

            }

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport kosztów wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        public static void GenerateCostsRaportAboutCarsMake(DateTime? raportBegin, DateTime? raportEnd)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var carModels = (from Makes in db.CarModels
                             select new
                             {
                                 Makes.make
                             }).Distinct();

            //Raport na temat np. Seatów, nie pojedynczych modeli
            foreach (var carModel in carModels)
            {
                var cars = from Cars in db.Cars
                           where Cars.CarModel.make == carModel.make
                           select Cars;

                double makeFuelCost = 0.0;
                double makeServiceCost = 0.0;
                double makeFuelCostPerKm = 0.0;
                double makeServiceCostPerKm = 0.0;

                foreach (var car in cars)
                {
                    CarCost carCost = CostInfoAboutCar(car, doc, raportBegin, raportEnd, false);
                    makeFuelCost += carCost.fuelCost;
                    makeServiceCost += carCost.serviceCost;
                    makeFuelCostPerKm += carCost.fuelCostPerKm;
                    makeServiceCostPerKm += carCost.serviceCostPerKm;
                }

                //ZASTOSOWANIE
                doc.Add(new iTextSharp.text.Paragraph(carModel.make + "\n\n", Font32));
                //ZASTOSOWANIE

                //O KOSZTACH
                var makeFuelCostPerCar = makeFuelCost / cars.Count();
                makeFuelCostPerCar = Math.Round(makeFuelCostPerCar, 2, MidpointRounding.AwayFromZero);
                var makeServiceCostPerCar = makeServiceCost / cars.Count();
                makeServiceCostPerCar = Math.Round(makeServiceCostPerCar, 2, MidpointRounding.AwayFromZero);

                doc.Add(new iTextSharp.text.Paragraph("Samochodów: " + cars.Count() + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa: " + makeFuelCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / samochoód: " + makeFuelCostPerCar + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / 1km: " + makeFuelCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu: " + makeServiceCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / samochód: " + makeServiceCostPerCar + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / 1km: " + makeServiceCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta: " + (makeFuelCost + makeServiceCost) + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta/ samochod: " + (makeFuelCostPerCar + makeServiceCostPerCar)+ "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta / 1km: " + (makeFuelCostPerKm + makeServiceCostPerKm) + "\n", Font14));
                //O KOSZTACH

                doc.NewPage();
            }

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport kosztów wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }

        public static void GenerateCostsRaportAboutCarsModel(DateTime? raportBegin, DateTime? raportEnd)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = GetPath();

            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.Open();

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();

            var carModels = from Models in db.CarModels
                            select Models;

            //Raport na temat np. Toledo, czyli pojedynczych modeli
            foreach (var carModel in carModels)
            {
                var cars = from Cars in db.Cars
                           where Cars.CarModel.model == carModel.model
                           select Cars;

                double modelFuelCost = 0.0;
                double modelServiceCost = 0.0;
                double modelFuelCostPerKm = 0.0;
                double modelServiceCostPerKm = 0.0;

                foreach (var car in cars)
                {
                    CarCost carCost = CostInfoAboutCar(car, doc, raportBegin, raportEnd, false);
                    modelFuelCost += carCost.fuelCost;
                    modelServiceCost += carCost.serviceCost;
                    modelFuelCostPerKm += carCost.fuelCostPerKm;
                    modelServiceCostPerKm += carCost.serviceCostPerKm;
                }

                //ZASTOSOWANIE
                doc.Add(new iTextSharp.text.Paragraph(carModel.model + "\n\n", Font32));
                //ZASTOSOWANIE

                //O KOSZTACH
                var modelFuelCostPerCar = modelFuelCost / cars.Count();
                modelFuelCostPerCar = Math.Round(modelFuelCostPerCar, 2, MidpointRounding.AwayFromZero);
                var modelServiceCostPerCar = modelServiceCost / cars.Count();
                modelServiceCostPerCar = Math.Round(modelServiceCostPerCar, 2, MidpointRounding.AwayFromZero);

                doc.Add(new iTextSharp.text.Paragraph("Samochodów: " + cars.Count() + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa: " + modelFuelCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / samochoód: " + modelFuelCostPerCar + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta paliwa / 1km: " + modelFuelCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu: " + modelServiceCost + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / samochód: " + modelServiceCostPerCar + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Koszta serwisu / 1km: " + modelServiceCostPerKm + "\n\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta: " + (modelFuelCost + modelServiceCost) + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta/ samochod: " + (modelServiceCostPerCar + modelFuelCostPerCar) + "\n", Font14));
                doc.Add(new iTextSharp.text.Paragraph("Sumaryczne koszta / 1km: " + (modelFuelCostPerKm + modelServiceCostPerKm) + "\n", Font14));
                //O KOSZTACH

                doc.NewPage();
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

            CostInfoAboutCar(car, doc, raportBegin, raportEnd, true);

            doc.Close();

            stopwatch.Stop();
            MessageBox.Show("Raport kosztów wygenerowany w czasie " + stopwatch.Elapsed + "!");
        }
    }
}

