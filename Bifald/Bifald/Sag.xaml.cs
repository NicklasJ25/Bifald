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

            pladsnummerComboBox.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "Pladser";
            cbi.IsEnabled = false;
            cbi.IsSelected = true;
            pladsnummerComboBox.Items.Add(cbi);

            if (sag.Afsluttet == true)
            {
                afsluttetTextbox.Text = "Ja";
                List<Afsluttede_pladser> pladser = database.Afsluttede_pladser.Include(af => af.Pladser).Where(af => af.Sagsnummer == sag.Sagsnummer).ToList();
                foreach (Afsluttede_pladser plads in pladser)
                {
                    CheckBox checkBox = new CheckBox()
                    {
                        Content = plads.Pladsnummer,
                        IsEnabled = false
                    };
                    pladsnummerComboBox.Items.Add(checkBox);

                    if (!typeTextbox.Text.Contains(plads.Pladser.Type))
                    {
                        typeTextbox.Text += " & " + plads.Pladser.Type;
                    }
                }
            }
            else
            {
                afsluttetTextbox.Text = "Nej";
                foreach (dynamic plads in sag.Pladser)
                {
                    CheckBox checkBox = new CheckBox()
                    {
                        Content = plads.Pladsnummer,
                        IsEnabled = false,
                        IsChecked = true
                    };
                    pladsnummerComboBox.Items.Add(checkBox);

                    if (!typeTextbox.Text.Contains(plads.Type))
                    {
                        typeTextbox.Text += " & " + plads.Type;
                    }
                }
            }
            typeTextbox.Text = typeTextbox.Text.Remove(0, 3);
            opbevaringFraDatePicker.SelectedDate = sag.Opbevaring_startdato;
            int leveretKasser = database.Kasser.Where(k => k.Sagsnummer == sag.Sagsnummer && k.Hentet_leveret.Equals("Leveret")).ToList().Sum(k => (int?)k.Antal ?? 0);
            int hentetKasser = database.Kasser.Where(k => k.Sagsnummer == sag.Sagsnummer && k.Hentet_leveret.Equals("Hentet")).ToList().Sum(k => (int?)k.Antal ?? 0);
            kasserTextbox.Text = (leveretKasser - hentetKasser).ToString();
            noterTextBlock.Text = sag.Noter;

            Kunder kunde = database.Kunder.Find(sag.KundeId);
            kundeTextbox.Text = kunde.Navn;
            startAdresseTextbox.Text = kunde.Adresse_fra;
            slutAdresseTextbox.Text = kunde.Adresse_til;
        }


        private void RetGemButton_Click(object sender, RoutedEventArgs e)
        {
            if (retGemButton.Content.Equals("Ret information"))
            {
                foreach (object item in pladsnummerComboBox.Items)
                {
                    if (item.ToString().Contains("Pladser")) continue;

                    CheckBox checkBox = item as CheckBox;
                    checkBox.IsEnabled = true;
                }
                List<Pladser> pladser = database.Pladser.Where(p => string.IsNullOrEmpty(p.Sagsnummer)).ToList();
                foreach (Pladser plads in pladser)
                {
                    CheckBox checkBox = new CheckBox()
                    {
                        Content = plads.Pladsnummer
                    };
                    pladsnummerComboBox.Items.Add(checkBox);
                }
                opbevaringFraDatePicker.IsEnabled = true;

                kundeTextbox.IsEnabled = true;
                startAdresseTextbox.IsEnabled = true;
                slutAdresseTextbox.IsEnabled = true;
                noterTextBlock.IsEnabled = true;

                retGemButton.Content = "Gem information";
            }
            else if (retGemButton.Content.Equals("Gem information"))
            {
                DB.Sager rettetSag = database.Sager.Find(sag.Sagsnummer);

                opbevaringFraDatePicker.IsEnabled = false;
                kundeTextbox.IsEnabled = false;
                startAdresseTextbox.IsEnabled = false;
                slutAdresseTextbox.IsEnabled = false;
                noterTextBlock.IsEnabled = false;

                retGemButton.Content = "Ret information";
                int iteration = pladsnummerComboBox.Items.Count - 1;
                for (int i = iteration; i >= 0; i--)
                {
                    if (pladsnummerComboBox.Items[i].ToString().Contains("Pladser")) continue;

                    CheckBox checkBox = pladsnummerComboBox.Items[i] as CheckBox;
                    if (checkBox.IsChecked == true)
                    {
                        Pladser plads = database.Pladser.Find(checkBox.Content);
                        plads.Sagsnummer = rettetSag.Sagsnummer;
                        checkBox.IsEnabled = false;
                    }
                    else
                    {
                        Pladser plads = database.Pladser.Find(checkBox.Content);
                        plads.Sagsnummer = null;
                        pladsnummerComboBox.Items.RemoveAt(i);
                    }
                }

                rettetSag.Opbevaring_startdato = opbevaringFraDatePicker.SelectedDate.Value;
                rettetSag.Noter = noterTextBlock.Text;

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
