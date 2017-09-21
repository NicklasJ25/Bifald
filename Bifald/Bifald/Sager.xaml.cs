using Bifald.DB;
using Bifald.Dialog;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bifald
{
    public partial class Sager : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        Validering validering = new Validering();

        public Sager()
        {
            InitializeComponent();
            List<DB.Sager> sager = database.Sager.Where(s => s.Afsluttet == false).ToList();
            opdaterListe(sager);
        }

        private void opdaterListe(List<DB.Sager> sager)
        {
            List<SagerListsViewModel> liste = new List<SagerListsViewModel>();

            foreach (DB.Sager sag in sager)
            {
                List<Pladser> pladser = database.Pladser.Where(p => p.Sagsnummer == sag.Sagsnummer).ToList();
                string pladsnumre = string.Join("; ", pladser.Select(p => p.Pladsnummer));
                Kunder kunde = database.Kunder.Find(sag.KundeId);
                liste.Add(new SagerListsViewModel
                {
                    sagsnummer = sag.Sagsnummer,
                    kunde = kunde.Navn,
                    adresseFra = kunde.Adresse_fra,
                    antalPladser = pladser.Count(p => p.Type == "Plads"),
                    antalLifte = pladser.Count(p => p.Type == "Lift")
                });
            }
            sagerListView.ItemsSource = liste;
        }

        private async void søgButton_Click(object sender, RoutedEventArgs e)
        {
            validering.ValiderSøgSager(sagsnummerTextbox.Text, pladsnummerTextbox.Text, kundeTextbox.Text, false);

            if (string.IsNullOrEmpty(validering.søgPladserValidering))
            {
                List<DB.Sager> sager = database.Sager.Where(s => s.Afsluttet == false).ToList();

                if (!string.IsNullOrEmpty(sagsnummerTextbox.Text))
                {
                    sager = sager.Where(s => s.Sagsnummer.Contains(sagsnummerTextbox.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(kundeTextbox.Text))
                {
                    List<Kunder> kunder = database.Kunder.Where(k => k.Navn.Contains(kundeTextbox.Text)).ToList();
                    List<DB.Sager> nySager = new List<DB.Sager>();
                    foreach (Kunder kunde in kunder)
                    {
                        nySager.AddRange(sager.Where(s => s.KundeId == kunde.Id).ToList());
                    }
                    sager = nySager;
                }
                if (!string.IsNullOrEmpty(pladsnummerTextbox.Text))
                {
                    List<Pladser> pladser = database.Pladser.Where(p => p.Pladsnummer.Contains(pladsnummerTextbox.Text)).ToList();
                    List<DB.Sager> nySager = new List<DB.Sager>();
                    foreach (Pladser plads in pladser)
                    {
                        nySager.AddRange(sager.Where(s => s.Sagsnummer == plads.Sagsnummer).ToList());
                    }
                    sager = nySager;
                }
                opdaterListe(sager);
            }
            else
            {
                var view = new StandardDialog();
                view.label.Content = validering.søgPladserValidering;
                view.cancelButton.Visibility = Visibility.Hidden;
                view.acceptButton.Content = "Ok";
                await DialogHost.Show(view, "RootDialog");
            }
        }

        private void sagerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sagerListView.SelectedItem != null)
            {
                SagerListsViewModel sagerJoin = (SagerListsViewModel)sagerListView.SelectedItem;
                Application.Current.Properties["Sagsnummer"] = sagerJoin.sagsnummer;
                Application.Current.Properties["FørSag"] = "Sager.xaml";

                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new Uri("sag.xaml", UriKind.Relative));
            }
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(sagerListView, "Print");
            }
        }
    }
}
