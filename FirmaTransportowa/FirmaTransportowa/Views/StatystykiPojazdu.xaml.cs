using FirmaTransportowa.Model;
using System;
using System.Windows.Controls;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Statystyki.xaml
    /// </summary>
    public partial class StatystykiPojazdu : UserControl
    {
        public StatystykiPojazdu(Car car)
        {
            InitializeComponent();
            //Model.Text = car.
            Rejestracja.Text = car.Registration;
            Pojemnosc_silnika.Text = car.engineCapacity.ToString();

            string saleDate = "";
            string purchaseDate = "";
            if (car.saleDate != null)
            {
                DateTime temp = (DateTime)car.saleDate;
                saleDate = temp.ToShortDateString();
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
    }
}
