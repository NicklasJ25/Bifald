using Bifald.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Bifald
{
    public partial class KasseHistorik : Page
    {
        DatabaseEntities database = new DatabaseEntities();

        public KasseHistorik()
        {
            InitializeComponent();
            List<DB.Kasser> kasser = database.Kasser.ToList();
            kasserListView.ItemsSource = kasser;
        }

        private void SøgButton_Click(object sender, RoutedEventArgs e)
        {
            List<DB.Kasser> kasser = database.Kasser.ToList();
            if (!string.IsNullOrEmpty(sagsnummerTextbox.Text))
            {
                kasser = kasser.Where(k => k.Sagsnummer.Contains(sagsnummerTextbox.Text)).ToList();
            }
            if (!string.IsNullOrEmpty(datoDatePicker.Text))
            {
                kasser = kasser.Where(k => DateTime.Equals(k.Hentet_leveret_dato, (DateTime)datoDatePicker.SelectedDate)).ToList();
            }
            kasserListView.ItemsSource = kasser;
        }

        private void TilbageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            Button knap = sender as Button;
            ns.Navigate(new Uri(knap.Tag.ToString(), UriKind.Relative));
        }
    }
}
