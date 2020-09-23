using FirmaTransportowa.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FirmaTransportowa.Views {
    public partial class DodajKontraktora : UserControl {
        public DodajKontraktora() {
            InitializeComponent();
			StartDate.SelectedDate = DateTime.Now;
		}
		private void Dodaj(object sender, RoutedEventArgs e) {
			if (Nazwa.Text == null)
				MessageBox.Show("Podaj nazwę kontrahenta");
			var newContractor = new Contractor() {
				name = Nazwa.Text,
				startSate = StartDate.SelectedDate.Value
			};
			var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
			db.Contractors.Add(newContractor);
			db.SaveChanges();
			MessageBox.Show($"Dodano kontrahenta \"{Nazwa.Text}\"");
			Cofnij(sender, e);
		}
		private void Cofnij(object sender, RoutedEventArgs e) {
            System.Windows.Window glowneOkno = System.Windows.Application.Current.MainWindow;
            glowneOkno.DataContext = new Kontraktorzy();
        }

    }
}
