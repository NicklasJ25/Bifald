using Bifald.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Data.Entity;

namespace Bifald
{
    /// <summary>
    /// Interaction logic for AfsluttetSager.xaml
    /// </summary>
    public partial class AfsluttedeSager : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        Validering validering = new Validering();

        public AfsluttedeSager()
        {
            InitializeComponent();
            List<DB.Sager> sager = database.Sager.Where(s => s.Afsluttet == true).ToList();
            opdaterListe(sager);
        }

        private void opdaterListe(List<DB.Sager> sager)
        {
            List<SagerJoin> liste = new List<SagerJoin>();

            foreach (DB.Sager sag in sager)
            {
                List<Pladser> pladser = database.Pladser.Where(p => p.Sagsnummer == sag.Sagsnummer).ToList();
                string pladsnumre = string.Join("; ", pladser.Select(p => p.Pladsnummer));
                Kunder kunde = database.Kunder.Find(sag.KundeId);
                liste.Add(new SagerJoin
                {
                    sagsnummer = sag.Sagsnummer,
                    pladser = pladsnumre,
                    kunde = kunde.Navn,
                    adresseFra = kunde.Adresse_fra
                });
            }

            sagerListView.ItemsSource = liste;
        }

        private void søgButton_Click(object sender, RoutedEventArgs e)
        {
            validering.ValiderSøgSager(sagsnummerTextbox.Text, pladsnummerTextbox.Text, kundeTextbox.Text, true);

            if (string.IsNullOrEmpty(validering.søgPladserValidering))
            {
                List<DB.Sager> sager = database.Sager.Include(s => s.Pladser).Where(s => s.Afsluttet == true).ToList();

                if (!string.IsNullOrEmpty(sagsnummerTextbox.Text))
                {
                    sager = sager.Where(s => s.Sagsnummer.Contains(sagsnummerTextbox.Text)).ToList();
                }
                if (!string.IsNullOrEmpty(kundeTextbox.Text))
                {
                    List<Kunder> kunder = database.Kunder.Where(k => k.Navn.Contains(kundeTextbox.Text)).ToList();
                    List<DB.Sager> nySager = new List<DB.Sager>();
                    nySager.AddRange(sager);

                    foreach (DB.Sager sag in sager)
                    {
                        if (kunder.SingleOrDefault(k => k.Id == sag.KundeId) == null)
                        {
                            nySager.Remove(sag);
                        }
                    }
                    sager = nySager;
                }
                if (!string.IsNullOrEmpty(pladsnummerTextbox.Text))
                {
                    List<DB.Sager> nySager = new List<DB.Sager>();
                    nySager.AddRange(sager);

                    foreach (DB.Sager sag in sager)
                    {
                        if (sag.Pladser.Where(p => p.Pladsnummer.Contains(pladsnummerTextbox.Text)) == null)
                        {
                            nySager.Remove(sag);
                        }
                    }
                    sager = nySager;
                }
                opdaterListe(sager);
            }
            else
            {
                MessageBox.Show(validering.søgPladserValidering);
            }
        }

        private void sagerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sagerListView.SelectedItem != null)
            {
                SagerJoin sagerJoin = (SagerJoin)sagerListView.SelectedItem;
                Application.Current.Properties["Sagsnummer"] = sagerJoin.sagsnummer;
                Application.Current.Properties["FørSag"] = "AfsluttedeSager.xaml";

                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new Uri("sag.xaml", UriKind.Relative));
            }
        }
    }
}
