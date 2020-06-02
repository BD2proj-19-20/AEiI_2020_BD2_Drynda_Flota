using System;
using System.Collections.Generic;
using System.IO;
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
using FirmaTransportowa.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for ZarzadzajPojazdami.xaml
    /// </summary>
    public partial class ZarzadzajPojazdami : UserControl
    {
        public ZarzadzajPojazdami()
        {
            InitializeComponent();
        }
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void Generuj_Raport(object sender, RoutedEventArgs e)
        {
            int selectedCarId = 1;
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var cars = db.Cars;

            foreach (var c in cars)
            {
                int id = c.id;
                int eC = c.engineCapacity;
            }

            FileStream fs = new FileStream("Raport na temat pojazdu.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Zawartosc raportu"));
            doc.Close();
        }

        private void Dodaj_Pojazd(object sender, RoutedEventArgs e)
        {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            //var glowneOkno = Application.Current.Windows[0];
            glowneOkno.DataContext = new DodajPojazdModel();
            //MainWindow
            //DataContext = new DodajPojazd();
        }
    }
}
