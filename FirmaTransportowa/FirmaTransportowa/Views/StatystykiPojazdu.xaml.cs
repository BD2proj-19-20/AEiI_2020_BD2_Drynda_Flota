using FirmaTransportowa.Model;
using FirmaTransportowa.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

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
