using FirmaTransportowa.Model;
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
using System.Windows.Shapes;

namespace FirmaTransportowa.Views
{
    /// <summary>
    /// Logika interakcji dla klasy StatystykiWypozyczenia.xaml
    /// </summary>
    public partial class StatystykiWypozyczenia : Window
    {
        Lend reservationLend;
        public StatystykiWypozyczenia(Lend lendChange)
        {
            reservationLend = lendChange;
            InitializeComponent();
        }
    }
}
