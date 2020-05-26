using iTextSharp.text;
using iTextSharp.text.pdf;
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

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Interaction logic for Statystyki.xaml
    /// </summary>
    public partial class StatystykiPojazdu : UserControl
    {
        public StatystykiPojazdu()
        {
            InitializeComponent();
        }

        private void Generuj_Raport(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream("Raport na temat pojazdu.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.Add(new iTextSharp.text.Paragraph("Zawartosc raportu"));
            doc.Close();
        }
    }
}
