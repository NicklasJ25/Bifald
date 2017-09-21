using Bifald.DB;
using Bifald.Dialog;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Bifald
{
    public partial class Kasser : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        Validering validering = new Validering();

        public Kasser()
        {
            InitializeComponent();
            //List<DB.Sager> sager = database.Sager.Include(s => s.Kasser).Where(s => s.Afsluttet == false && s.Kasser.Where(k => k.Hentet_leveret.Equals("Leveret")).Sum(k => k.Antal) > 0).ToList();
            List<DB.Sager> sager = database.Sager.ToList();
            foreach (DB.Sager sag in sager)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = sag.Sagsnummer;
                sagsnummerComboBox.Items.Add(comboBoxItem);
            }
            datoDatePicker.SelectedDate = datoDatePicker.DisplayDate;
        }

        private async void HentLeverButton_Click(object sender, RoutedEventArgs e)
        {
            validering.ValiderHentLeverKasser(sagsnummerComboBox, hentetLeveretComboBox, datoDatePicker.Text, antalTextbox.Text);

            if (string.IsNullOrEmpty(validering.hentLeverKasserValidering))
            {
                string noget = ((ComboBoxItem)sagsnummerComboBox.SelectedItem).Content.ToString();
                database.Kasser.Add(new DB.Kasser
                {
                    Sagsnummer = ((ComboBoxItem)sagsnummerComboBox.SelectedItem).Content.ToString(),
                    Antal = int.Parse(antalTextbox.Text),
                    Hentet_leveret = ((ComboBoxItem)hentetLeveretComboBox.SelectedItem).Tag.ToString(),
                    Hentet_leveret_dato = datoDatePicker.DisplayDate                    
                });

                DB.Sager sag = database.Sager.Find(((ComboBoxItem)sagsnummerComboBox.SelectedItem).Content.ToString());
                Kunder kunde = database.Kunder.Find(sag.KundeId);
                database.SaveChanges();

                sagsnummerComboBox.SelectedIndex = 0;
                hentetLeveretComboBox.SelectedIndex = 0;
                datoDatePicker.SelectedDate = null;
                antalTextbox.Text = null;
            }
            else
            {
                var view = new StandardDialog();
                view.label.Content = validering.hentLeverKasserValidering;
                view.cancelButton.Visibility = Visibility.Hidden;
                view.acceptButton.Content = "Ok";
                await DialogHost.Show(view, "RootDialog");
            }
        }

        private void SagsnummerComboBox_DropDownClosed(object sender, EventArgs e)
        {
            int leveretKasser = database.Kasser.Where(k => k.Sagsnummer == sagsnummerComboBox.SelectionBoxItem.ToString() && k.Hentet_leveret.Equals("Leveret")).ToList().Sum(k => (int?)k.Antal ?? 0);
            int hentetKasser = database.Kasser.Where(k => k.Sagsnummer == sagsnummerComboBox.SelectionBoxItem.ToString() && k.Hentet_leveret.Equals("Hentet")).ToList().Sum(k => (int?)k.Antal ?? 0);
            leveretKasser -= hentetKasser;
            leveretKasserLabel.Content = string.Format("Der er leveret {0} kasser til denne sag", leveretKasser);
        }

        private void HentetLeveretKasserButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            Button knap = sender as Button;
            ns.Navigate(new Uri(knap.Tag.ToString(), UriKind.Relative));
        }
    }
}
