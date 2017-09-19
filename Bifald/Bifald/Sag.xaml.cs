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

namespace Bifald
{
    public partial class Sag : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        List<string> gamlePladser = new List<string>();
        DB.Sager sag;
        List<Pladser> pladser = new List<Pladser>();

        public Sag()
        {
            InitializeComponent();
            string sagsnummer = (string)Application.Current.Properties["Sagsnummer"];
            sag = database.Sager.Include(s => s.Pladser).SingleOrDefault(s => s.Sagsnummer == sagsnummer);
            if (sag.Afsluttet == true)
            {
                afslutGenoptagButton.Content = "Genoptag sag";
                retGemButton.Visibility = Visibility.Hidden;
            }
            else
            {
                afslutGenoptagButton.Content = "Afslut sag";
                retGemButton.Visibility = Visibility.Visible;
            }
            OpdaterSagsvisning();
        }

        private void OpdaterSagsvisning()
        {
            sagsnummerTextbox.Text = sag.Sagsnummer;

            if (sag.Afsluttet == true)
            {
                afsluttetTextbox.Text = "Ja";
                List<Afsluttede_pladser> pladser = database.Afsluttede_pladser.Include(af => af.Pladser).Where(af => af.Sagsnummer == sag.Sagsnummer).ToList();
                foreach (Afsluttede_pladser plads in pladser)
                {
                    pladserTextbox.Text += plads.Pladsnummer + "; ";
                    pladserTextbox.Text.Remove(pladserTextbox.Text.Length - 2);

                    if (!typeTextbox.Text.Contains(plads.Pladser.Type))
                    {
                        typeTextbox.Text += " & " + plads.Pladser.Type;
                    }
                }
                afsluttetDatoLabel.Visibility = Visibility.Visible;
                afsluttetDatoDatePicker.Visibility = Visibility.Visible;
            }
            else
            {
                afsluttetTextbox.Text = "Nej";
                foreach (Pladser plads in sag.Pladser)
                {
                    pladserTextbox.Text += plads.Pladsnummer + "; ";
                    pladserTextbox.Text.Remove(pladserTextbox.Text.Length - 2);

                    if (!typeTextbox.Text.Contains(plads.Type))
                    {
                        typeTextbox.Text += " & " + plads.Type;
                    }
                }
                afsluttetDatoLabel.Visibility = Visibility.Hidden;
                afsluttetDatoDatePicker.Visibility = Visibility.Hidden;
            }
            typeTextbox.Text = typeTextbox.Text.Remove(0, 3);
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
                fjernPladserButton.Visibility = Visibility.Visible;
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
                fjernPladserButton.Visibility = Visibility.Hidden;
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
            }
        }

        private void AfslutClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool) eventArgs.Parameter)
            {
                DB.Sager afslutSag = database.Sager.Include(s => s.Pladser).SingleOrDefault(s => s.Sagsnummer == sag.Sagsnummer);
                List<Pladser> pladser = afslutSag.Pladser.ToList();
                afslutSag.Afsluttet = true;

                foreach (Pladser plads in pladser)
                {
                    Afsluttede_pladser afsluttetPlads = new Afsluttede_pladser
                    {
                        Sagsnummer = afslutSag.Sagsnummer,
                        Pladsnummer = plads.Pladsnummer
                    };
                    database.Afsluttede_pladser.Add(afsluttetPlads);
                    plads.Sagsnummer = null;
                }
                database.SaveChanges();
                afsluttetTextbox.Text = "Ja";
                afslutGenoptagButton.Content = "Genoptag sag";
                retGemButton.Visibility = Visibility.Hidden;
                afsluttetDatoLabel.Visibility = Visibility.Visible;
                afsluttetDatoDatePicker.Visibility = Visibility.Visible;
            }
        }

        private void GenoptagClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                sag = database.Sager.Include(s => s.Pladser).SingleOrDefault(s => s.Sagsnummer == sag.Sagsnummer);
                sag.Afsluttet = false;

                List<Afsluttede_pladser> afsluttedePladser = database.Afsluttede_pladser.Where(afs => afs.Sagsnummer == sag.Sagsnummer).ToList();
                foreach (Afsluttede_pladser afsluttetPlads in afsluttedePladser)
                {
                    Pladser plads = database.Pladser.Find(afsluttetPlads.Pladsnummer);
                    plads.Sagsnummer = afsluttetPlads.Sagsnummer;
                    database.Afsluttede_pladser.Remove(afsluttetPlads);
                }
                database.SaveChanges();
                afsluttetTextbox.Text = "Nej";
                afslutGenoptagButton.Content = "Afslut sag";
                retGemButton.Visibility = Visibility.Visible;
                afsluttetDatoLabel.Visibility = Visibility.Hidden;
                afsluttetDatoDatePicker.Visibility = Visibility.Hidden;
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

                Pladser[] pladser = database.Pladser.Where(p => string.IsNullOrEmpty(p.Sagsnummer)).ToArray();
                foreach (Pladser plads in pladser)
                {
                    view.listView.Items.Add(plads.Pladsnummer);
                }
            }
            else
            {
                view.label.Content = "Vælg de pladser/lifte der skal fjernes fra sagen";
                view.acceptButton.Content = "Fjern";

                foreach (Pladser plads in pladser)
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
                ListDialog listDialog = dialogHost.Content as ListDialog;
                if (listDialog.acceptButton.Content.Equals("Tilføj"))
                {
                    foreach (String selectedItem in listDialog.listView.SelectedItems)
                    {
                        Pladser plads = database.Pladser.SingleOrDefault(p => p.Pladsnummer == selectedItem);
                        pladser.Add(plads);
                    }
                }
                else
                {
                    foreach (String selectedItem in listDialog.listView.SelectedItems)
                    {
                        Pladser plads = pladser.SingleOrDefault(p => p.Pladsnummer == selectedItem);
                        pladser.Remove(plads);
                    }
                }

                foreach (Pladser plads in pladser)
                {
                    pladserTextbox.Text += plads.Pladsnummer + "; ";
                }

                pladserTextbox.Text.Remove(pladserTextbox.Text.Length - 2);
            }
        }

        private void TilbageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            Button knap = sender as Button;
            ns.Navigate(new Uri((string)Application.Current.Properties["FørSag"], UriKind.Relative));
        }
    }
}
