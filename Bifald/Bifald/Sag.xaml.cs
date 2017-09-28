using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Bifald.DB;
using MaterialDesignThemes.Wpf;
using Bifald.Dialog;
using Bifald.Model;

namespace Bifald
{
    public partial class Sag : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        List<string> gamlePladser = new List<string>();
        DB.Sager sag;
        string existsInOpenCase;

        public Sag()
        {
            InitializeComponent();
            string sagsnummer = (string)Application.Current.Properties["Sagsnummer"];
            sag = database.Sager.Include(s => s.Pladser).SingleOrDefault(s => s.Sagsnummer == sagsnummer);
            OpdaterSagsvisning();
        }

        private void OpdaterSagsvisning()
        {
            pladserTextbox.Text = "";
            typeTextbox.Text = "";
            sagsnummerTextbox.Text = sag.Sagsnummer;

            if (sag.Afsluttet == true)
            {
                afsluttetTextbox.Text = "Ja";
                afsluttetDatoDatePicker.SelectedDate = sag.Opbevaring_slutdato;
                afslutGenoptagButton.Content = "Genoptag sag";
                List<Plads_historik> pladser = database.Plads_historik.Include(ap => ap.Pladser).Where(ap => ap.Sagsnummer == sag.Sagsnummer && ap.Opret_afslut == "Afsluttet").OrderBy(ap => ap.Pladsnummer).ToList();
                pladser = pladser.Distinct(new PladsComparer()).ToList();
                foreach (Plads_historik plads in pladser)
                {
                    pladserTextbox.Text += "; " + plads.Pladsnummer;

                    if (!typeTextbox.Text.Contains(plads.Pladser.Type))
                    {
                        typeTextbox.Text += " & " + plads.Pladser.Type;
                    }
                }
                afsluttetDatoLabel.Visibility = Visibility.Visible;
                afsluttetDatoDatePicker.Visibility = Visibility.Visible;
                retGemButton.Visibility = Visibility.Hidden;
                delAfslutButton.Visibility = Visibility.Hidden;
            }
            else
            {
                afsluttetTextbox.Text = "Nej";
                afslutGenoptagButton.Content = "Afslut sag";
                foreach (Pladser plads in sag.Pladser.OrderBy(p => p.Pladsnummer))
                {
                    pladserTextbox.Text += "; " + plads.Pladsnummer;

                    if (!typeTextbox.Text.Contains(plads.Type))
                    {
                        typeTextbox.Text += " & " + plads.Type;
                    }
                }
                afsluttetDatoLabel.Visibility = Visibility.Hidden;
                afsluttetDatoDatePicker.Visibility = Visibility.Hidden;
                retGemButton.Visibility = Visibility.Visible;
                delAfslutButton.Visibility = Visibility.Visible;
            }
            if (!string.IsNullOrEmpty(typeTextbox.Text))
            {
                typeTextbox.Text = typeTextbox.Text.Remove(0, 3);
                pladserTextbox.Text = pladserTextbox.Text.Remove(0, 2);
            }
            opbevaringFraDatePicker.SelectedDate = sag.Opbevaring_startdato;
            int leveretKasser = database.Kasser.Where(k => k.Sagsnummer == sag.Sagsnummer && k.Hentet_leveret.Equals("Leveret")).ToList().Sum(k => (int?)k.Antal ?? 0);
            int hentetKasser = database.Kasser.Where(k => k.Sagsnummer == sag.Sagsnummer && k.Hentet_leveret.Equals("Hentet")).ToList().Sum(k => (int?)k.Antal ?? 0);
            kasserTextbox.Text = (leveretKasser - hentetKasser).ToString();
            noterTextbox.Text = sag.Noter;

            Kunder kunde = database.Kunder.Find(sag.KundeId);
            kundeTextbox.Text = kunde.Navn;
            startAdresseTextbox.Text = kunde.Adresse_fra;
            slutAdresseTextbox.Text = kunde.Adresse_til;
        }

        private void RetGemButton_Click(object sender, RoutedEventArgs e)
        {
            if (retGemButton.Content.Equals("Ret information"))
            {
                tilføjPladserButton.Visibility = Visibility.Visible;
                opbevaringFraDatePicker.IsEnabled = true;
                kundeTextbox.IsEnabled = true;
                startAdresseTextbox.IsEnabled = true;
                slutAdresseTextbox.IsEnabled = true;
                noterTextbox.IsEnabled = true;

                retGemButton.Content = "Gem information";
            }
            else if (retGemButton.Content.Equals("Gem information"))
            {
                tilføjPladserButton.Visibility = Visibility.Hidden;
                opbevaringFraDatePicker.IsEnabled = false;
                kundeTextbox.IsEnabled = false;
                startAdresseTextbox.IsEnabled = false;
                slutAdresseTextbox.IsEnabled = false;
                noterTextbox.IsEnabled = false;

                retGemButton.Content = "Ret information";

                DB.Sager rettetSag = database.Sager.Find(sag.Sagsnummer);

                rettetSag.Opbevaring_startdato = opbevaringFraDatePicker.SelectedDate.Value;
                rettetSag.Noter = noterTextbox.Text;

                Kunder kunde = database.Kunder.Find(rettetSag.KundeId);
                kunde.Navn = kundeTextbox.Text;
                kunde.Adresse_fra = startAdresseTextbox.Text;
                kunde.Adresse_til = slutAdresseTextbox.Text;

                database.SaveChanges();
            }
        }

        private async void AfslutGenoptagButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new StandardDialog();
            if (afslutGenoptagButton.Content.Equals("Afslut sag"))
            {
                view.label.Content = "Er du sikker på du vil aflutte sagen: " + sag.Sagsnummer + "?";

                await DialogHost.Show(view, "RootDialog", AfslutClosingEventHandler);
            }
            else if (afslutGenoptagButton.Content.Equals("Genoptag sag"))
            {
                view.label.Content = "Er du sikker på du vil genoptage sagen: " + sag.Sagsnummer + "?";

                await DialogHost.Show(view, "RootDialog", GenoptagClosingEventHandler);
                if (existsInOpenCase != "")
                {
                    existsInOpenCase = existsInOpenCase.Remove(0, 2);
                    view.label.Content = "En eller flere pladser bliver brugt i en åben sag:\n" + existsInOpenCase;
                    view.cancelButton.Visibility = Visibility.Hidden;
                    view.acceptButton.Content = "Ok";
                    await DialogHost.Show(view, "RootDialog");
                }
            }
        }

        private void AfslutClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool) eventArgs.Parameter)
            {
                List<Pladser> afslutPladser = sag.Pladser.ToList();
                foreach (Pladser plads in afslutPladser)
                {
                    Plads_historik plads_Historik = new Plads_historik
                    {
                        Sagsnummer = sag.Sagsnummer,
                        Pladsnummer = plads.Pladsnummer,
                        Opret_afslut = "Afsluttet",
                        Dato = DateTime.Now
                    };
                    database.Plads_historik.Add(plads_Historik);
                    plads.Sagsnummer = null;
                }
                sag.Afsluttet = true;
                sag.Opbevaring_slutdato = DateTime.Now;
                database.SaveChanges();

                OpdaterSagsvisning();
            }
        }

        private void GenoptagClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                sag.Afsluttet = false;

                existsInOpenCase = "";
                List<Plads_historik> afsluttedePladser = database.Plads_historik.Where(afs => afs.Sagsnummer == sag.Sagsnummer && afs.Opret_afslut == "Afsluttet").ToList();
                afsluttedePladser = afsluttedePladser.Distinct().ToList();
                foreach (Plads_historik afsluttetPlads in afsluttedePladser)
                {
                    Pladser plads = database.Pladser.Find(afsluttetPlads.Pladsnummer);
                    if (string.IsNullOrEmpty(plads.Sagsnummer))
                    {
                        plads.Sagsnummer = afsluttetPlads.Sagsnummer;
                        Plads_historik plads_Historik = new Plads_historik
                        {
                            Sagsnummer = sag.Sagsnummer,
                            Pladsnummer = plads.Pladsnummer,
                            Opret_afslut = "Tilføjet",
                            Dato = DateTime.Now
                        };
                        database.Plads_historik.Add(plads_Historik);
                    }
                    else if (afsluttedePladser.Where(afs => afs.Pladsnummer == plads.Pladsnummer).Count() == 1)
                    {
                        existsInOpenCase += "; " + plads.Pladsnummer;
                    }
                }
                database.SaveChanges();
                OpdaterSagsvisning();
            }
        }

        private async void TilføjFjernPladserButton_Click(object sender, RoutedEventArgs e)
        {
            var view = new ListDialog();
            view.cancelButton.Content = "Annuller";

            Button button = sender as Button;
            if (button.Name.Equals("tilføjPladserButton"))
            {
                view.label.Content = "Vælg de pladser/lifte der skal tilføjes til sagen";
                view.acceptButton.Content = "Tilføj";

                Pladser[] pladser = database.Pladser.Where(p => string.IsNullOrEmpty(p.Sagsnummer)).OrderBy(p => p.Pladsnummer).ToArray();
                foreach (Pladser plads in pladser)
                {
                    view.listView.Items.Add(plads.Pladsnummer);
                }
            }
            else
            {
                view.label.Content = "Vælg de pladser/lifte der skal fjernes fra sagen";
                view.acceptButton.Content = "Del afslut";

                foreach (Pladser plads in sag.Pladser.OrderBy(p => p.Pladsnummer))
                {
                    view.listView.Items.Add(plads.Pladsnummer);
                }
            }

            await DialogHost.Show(view, "RootDialog", TilføjFjernPladserClosingEventHandler);
        }

        private void TilføjFjernPladserClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!eventArgs.IsCancelled)
            {
                DialogHost dialogHost = sender as DialogHost;
                ListDialog listDialog = dialogHost.DialogContent as ListDialog;
                if (listDialog.acceptButton.Content.Equals("Tilføj"))
                {
                    foreach (String selectedItem in listDialog.listView.SelectedItems)
                    {
                        Pladser plads = database.Pladser.SingleOrDefault(p => p.Pladsnummer == selectedItem);
                        sag.Pladser.Add(plads);

                        Plads_historik plads_Historik = new Plads_historik
                        {
                            Sagsnummer = sag.Sagsnummer,
                            Pladsnummer = plads.Pladsnummer,
                            Opret_afslut = "Tilføjet",
                            Dato = DateTime.Now
                        };
                        database.Plads_historik.Add(plads_Historik);
                    }
                    database.SaveChanges();
                }
                else if (listDialog.acceptButton.Content.Equals("Del afslut"))
                {
                    foreach (String selectedItem in listDialog.listView.SelectedItems)
                    {
                        Pladser plads = sag.Pladser.SingleOrDefault(p => p.Pladsnummer == selectedItem);
                        sag.Pladser.Remove(plads);
                        Plads_historik plads_Historik = new Plads_historik
                        {
                            Sagsnummer = sagsnummerTextbox.Text,
                            Pladsnummer = plads.Pladsnummer,
                            Opret_afslut = "Afsluttet",
                            Dato = DateTime.Now
                        };
                        database.Plads_historik.Add(plads_Historik);
                        plads.Sagsnummer = null;
                    }
                    database.SaveChanges();
                }

                OpdaterSagsvisning();
            }
        }

        private void UdskrivSagButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(this, "Print");
            }
        }

        private void TilbageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            Button knap = sender as Button;
            ns.Navigate(new Uri((string)Application.Current.Properties["FørSag"], UriKind.Relative));
        }

        private async void PladsHistorikButton_Click(object sender, RoutedEventArgs e)
        {
            var pladsHistorik = database.Plads_historik
                            .Where(ph => ph.Sagsnummer == sag.Sagsnummer)
                            .GroupBy(ph => new { ph.Opret_afslut, ph.Dato })
                            .OrderBy(ph => ph.Key.Dato)
                            .ToList();

            PladsHistorikDialog dialog = new PladsHistorikDialog();
            dialog.label.Content = "Plads historik";

            foreach (var group in pladsHistorik)
            {
                string pladsJoin = "";
                foreach(Plads_historik plads in group.OrderBy(ph => ph.Pladsnummer))
                {
                    pladsJoin += "; " + plads.Pladsnummer;
                }
                pladsJoin = pladsJoin.Remove(0, 2);
                PladsHistorikJoin pladsHistorikJoin = new PladsHistorikJoin
                {
                    Pladser = pladsJoin,
                    Opret_afslut = group.Key.Opret_afslut,
                    Dato = group.Key.Dato
                };
                dialog.listView.Items.Add(pladsHistorikJoin);
            }
            dialog.cancelButton.Visibility = Visibility.Hidden;
            dialog.acceptButton.Content = "OK";

            await DialogHost.Show(dialog, "RootDialog");
        }
    }
}