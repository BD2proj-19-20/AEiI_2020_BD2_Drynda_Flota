﻿using FirmaTransportowa.Model;
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
    /// Logika interakcji dla klasy ZmienKierownika.xaml
    /// </summary>
    public partial class ZmienKierownika : Window
    {
        Person toChange;

        private CalendarDateRange dzienKierownictwaStartBlackoutRange = null;
        public ZmienKierownika(Person changePerson)
        {
            InitializeComponent();

            toChange = changePerson;

            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var peoplePermission = db.PeoplesPermissions;

            newKierownikEnd.BlackoutDates.AddDatesInPast();
            newKierownikStart.BlackoutDates.AddDatesInPast();

            if (dzienKierownictwaStartBlackoutRange == null) //uwzględnienie daty zwolnienia dla pracownika posiadającą ją
            {
                if (toChange.layoffDate != null)
                {
                    dzienKierownictwaStartBlackoutRange = new CalendarDateRange(((DateTime)toChange.layoffDate).AddDays(1),   DateTime.MaxValue);
                    newKierownikStart.BlackoutDates.Insert(1, dzienKierownictwaStartBlackoutRange);
                }
            }
            else
            {
                if (toChange.layoffDate != null)
                {
                    dzienKierownictwaStartBlackoutRange = new CalendarDateRange(((DateTime)toChange.layoffDate).AddDays(1),  DateTime.MaxValue);
                    newKierownikStart.BlackoutDates[1] = dzienKierownictwaStartBlackoutRange;
                }
            }


            var personPermission = (from workerPermission in db.PeoplesPermissions
                                  where workerPermission.personId == toChange.id && workerPermission.Permission.name == "Kierownik" 

                                  select workerPermission).FirstOrDefault();

           if(personPermission!=null)
            {
                if (personPermission.grantDate <= DateTime.Now && (personPermission.revokeDate > DateTime.Now || personPermission.revokeDate == null))
                {
                    newKierownikEnd.SelectedDate = personPermission.revokeDate;
                    newKierownikStart.IsEnabled = false;
                    newKierownikStart.BlackoutDates.Clear();
                    newKierownikStart.SelectedDate = personPermission.grantDate;  //jeśli jest przed datą dzisiejszą pozwalamy na jej ustawienie ale nie zmienianie przez użytk.
                }
                else if ( personPermission.grantDate > DateTime.Now)
                {
                    newKierownikEnd.SelectedDate = personPermission.revokeDate;
                    newKierownikStart.SelectedDate = personPermission.grantDate;
                }
            }
        }
        private void Anuluj(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private CalendarDateRange dzienKierownictwaEndBlackoutRange1 = null;

        private CalendarDateRange dzienKierownictwaEndBlackoutRange2 = null;

        private void DzienKierownictwaStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            if (newKierownikEnd.SelectedDate < newKierownikStart.SelectedDate)
                newKierownikEnd.SelectedDate = newKierownikStart.SelectedDate;
                if (dzienKierownictwaEndBlackoutRange1 == null)
                {
                    dzienKierownictwaEndBlackoutRange1 = new CalendarDateRange(DateTime.Today.AddDays(-1), ((DateTime)newKierownikStart.SelectedDate).AddDays(-1));
                    newKierownikEnd.BlackoutDates.Insert(1, dzienKierownictwaEndBlackoutRange1);

                    if(toChange.layoffDate != null) //uwzględnienie daty zwolnienia dla pracownika posiadającą ją
                {
                    dzienKierownictwaEndBlackoutRange2 = new CalendarDateRange(((DateTime)toChange.layoffDate).AddDays(1), 
                       DateTime.MaxValue);
                    newKierownikEnd.BlackoutDates.Insert(2, dzienKierownictwaEndBlackoutRange2);

                }
                }
                else
                {
                    dzienKierownictwaEndBlackoutRange1.End = ((DateTime)newKierownikStart.SelectedDate).AddDays(-1);
                    newKierownikEnd.BlackoutDates[1] = dzienKierownictwaEndBlackoutRange1;

                if (toChange.layoffDate != null)
                {
                    dzienKierownictwaEndBlackoutRange2 = new CalendarDateRange(((DateTime)toChange.layoffDate).AddDays(1),
                       DateTime.MaxValue);
                    newKierownikEnd.BlackoutDates[2] = dzienKierownictwaEndBlackoutRange2;
                }
            }
            
        }


        private void Zmien(object sender, RoutedEventArgs e)
        {
            var db = new AEiI_2020_BD2_Drynda_FlotaEntities();
            var peoplePermission = db.PeoplesPermissions;

            bool newPermission = true;

            var personPermission = (from workerPermission in db.PeoplesPermissions
                                    where workerPermission.personId == toChange.id && workerPermission.Permission.name == "Kierownik"

                                    select workerPermission).FirstOrDefault();

                 if (personPermission != null)
                 {
                     personPermission.grantDate = (System.DateTime)newKierownikStart.SelectedDate;
                            if (newKierownikEnd.Text != "")
                    personPermission.revokeDate = (System.DateTime)newKierownikEnd.SelectedDate;
                            else
                    personPermission.revokeDate = null;

                            newPermission = false;
                 }

                    if (newPermission == true)  //jeśli nie jest kierownikiem dodajemy mu uprawnienie 
                    {
                        PeoplesPermission workerPermission = new PeoplesPermission();

                        workerPermission.grantDate = (System.DateTime)newKierownikStart.SelectedDate;
                        if (newKierownikEnd.Text != "")
                            workerPermission.revokeDate = (System.DateTime)newKierownikEnd.SelectedDate;
                        else
                            workerPermission.revokeDate = null;

                        workerPermission.personId = toChange.id;
                        peoplePermission.Add(workerPermission);
                    }
            MessageBox.Show("Zmienieono!", "Komunikat");
            db.SaveChanges();
        }
    }
}
