using FirmaTransportowa.Views;
using System;
using System.Windows.Controls;

namespace FirmaTransportowa
{
    class CarComparator
    {
        public int CompareCarsByIdAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return first.carId.CompareTo(second.carId);
        }

        public int CompareCarsByIdDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return second.carId.CompareTo(first.carId);
        }

        public int CompareCarsByRegistrationAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(second.registration, first.registration);
        }

        public int CompareCarsByRegistrationDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(first.registration, second.registration);
        }

        public int CompareCarsBySaleDateAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.saleDate.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.saleDate);
            else
                firstDate = DateTime.MinValue;
            if (second.saleDate.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.saleDate);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(secondDate, firstDate);
        }

        public int CompareCarsBySaleDateDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            DateTime firstDate;
            DateTime secondDate;
            if (first.saleDate.CompareTo("") != 0)
                firstDate = Convert.ToDateTime(first.saleDate);
            else
                firstDate = DateTime.MinValue;
            if (second.saleDate.CompareTo("") != 0)
                secondDate = Convert.ToDateTime(second.saleDate);
            else
                secondDate = DateTime.MinValue;
            return DateTime.Compare(firstDate, secondDate);
        }

        public int CompareCarsBySupervisorAscending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(second.carSupervisor, first.carSupervisor);
        }

        public int CompareCarsBySupervisorDescending(ListViewItem a, ListViewItem b)
        {
            ItemList first = (ItemList)a.Content;
            ItemList second = (ItemList)b.Content;
            return String.Compare(first.carSupervisor, second.carSupervisor);
        }
    }
}
