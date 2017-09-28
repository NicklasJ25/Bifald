using Bifald;
using Bifald.DB;
using Bifald.Dialog;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Bifald
{
    public partial class ReserverPlads : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        Validering validering = new Validering();

        string nytSagsnummer = "";
        List<Pladser> pladser = new List<Pladser>();

        public ReserverPlads()
        {
            InitializeComponent();
            opbevaringFraDatePicker.SelectedDate = opbevaringFraDatePicker.DisplayDate;
        }

        private async void opretSagButton_Click(object sender, RoutedEventArgs e)
        {
            validering.ValiderOpretSag(sagsnummerTextbox.Text, kundeTextbox.Text, pladser, 
                startAddresseTextbox.Text, slutAddresseTextbox.Text, kasserTextbox.Text);

            if (string.IsNullOrEmpty(validering.opretSagValidering))
            {
                if (!SagsnummerExists())
                {
                    OpretSag();
                }
            }
            else
            {
                var view = new StandardDialog();
                view.label.Content = validering.opretSagValidering;
                view.cancelButton.Visibility = Visibility.Hidden;
                view.acceptButton.Content = "Ok";
                await DialogHost.Show(view, "RootDialog");
            }
        }

        private void OpretSag()
        {
            database.Kunder.Add(new Kunder
            {
                Navn = kundeTextbox.Text,
                Adresse_fra = startAddresseTextbox.Text,
                Adresse_til = slutAddresseTextbox.Text,
                Sager = new List<DB.Sager>{ new DB.Sager
                    {
                        Sagsnummer = sagsnummerTextbox.Text,
                        Opbevaring_startdato = opbevaringFraDatePicker.DisplayDate,
                        Pladser = pladser,
                        Kasser = new List<DB.Kasser>{ new DB.Kasser
                        {
                        Sagsnummer = sagsnummerTextbox.Text,
                        Antal = int.Parse(kasserTextbox.Text),
                        Hentet_leveret_dato = opbevaringFraDatePicker.DisplayDate,
                        Hentet_leveret = "Leveret"
                        }},
                        Noter = noterTextbox.Text
                    }}
            });
            foreach (Pladser plads in pladser)
            {
                Plads_historik plads_Historik = new Plads_historik
                {
                    Sagsnummer = sagsnummerTextbox.Text,
                    Pladsnummer = plads.Pladsnummer,
                    Opret_afslut = "Tilføjet",
                    Dato = DateTime.Now
                };
                database.Plads_historik.Add(plads_Historik);
            }

            database.SaveChanges();

            NavigationService.Navigate(new Sager());
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
                    if (!this.pladser.Contains(plads))
                    {
                        view.listView.Items.Add(plads.Pladsnummer);
                    }
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
                ListDialog listDialog = dialogHost.DialogContent as ListDialog;
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

                pladserTextbox.Text = "";
                pladser = pladser.OrderBy(p => p.Pladsnummer).ToList();
                foreach (Pladser plads in pladser)
                {
                    pladserTextbox.Text += plads.Pladsnummer + "; ";
                }
                if (pladserTextbox.Text.Length != 0)
                {
                    pladserTextbox.Text = pladserTextbox.Text.Remove(pladserTextbox.Text.Length - 2);
                }
            }
        }

        private bool SagsnummerExists()
        {
            string sagsnummer = sagsnummerTextbox.Text;
            int i = 0;
            bool exists = database.Sager.Any(s => s.Sagsnummer == sagsnummer);
            while (exists)
            {
                i++;
                nytSagsnummer = string.Format("{0} ({1})", sagsnummer, i);
                exists = database.Sager.Any(s => s.Sagsnummer == nytSagsnummer);
            }
            if (i != 0)
            {
                var view = new StandardDialog();
                view.label.Content = "Sagsnummer findes i forvejen, vil du omdømme til " + nytSagsnummer + "?";
                DialogHost.Show(view, "RootDialog", StandardDialogClosingEventHandler);
                return true;
            }
            return false;
        }

        private void StandardDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                sagsnummerTextbox.Text = nytSagsnummer;
                OpretSag();
            }
        }
    }
}
