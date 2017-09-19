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

        public ReserverPlads()
        {
            InitializeComponent();
            //setPladser();
            opbevaringFraDatePicker.SelectedDate = opbevaringFraDatePicker.DisplayDate;
        }

        private void setPladser()
        {
            pladsnummerComboBox.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "Vælg pladser";
            cbi.IsEnabled = false;
            cbi.IsSelected = true;
            pladsnummerComboBox.Items.Add(cbi);

            opbevaringFraDatePicker.SelectedDate = opbevaringFraDatePicker.DisplayDate;
            Pladser[] pladser = database.Pladser.Where(p => string.IsNullOrEmpty(p.Sagsnummer)).ToArray();
            foreach (Pladser plads in pladser)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = plads.Pladsnummer;
                pladsnummerComboBox.Items.Add(checkBox);
            }
        }

        private void opretSagButton_Click(object sender, RoutedEventArgs e)
        {
            validering.ValiderOpretSag(sagsnummerTextbox.Text, kundeTextbox.Text, pladsnummerComboBox, 
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
                MessageBox.Show(validering.opretSagValidering);
            }
        }

        private void OpretSag()
        {
            List<Pladser> pladser = new List<Pladser>();
            for (int i = 1; i < pladsnummerComboBox.Items.Count; i++)
            {
                CheckBox checkbox = pladsnummerComboBox.Items[i] as CheckBox;
                if (checkbox.IsChecked == true)
                {
                    string checkboxContent = checkbox.Content.ToString();
                    Pladser plads = database.Pladser.SingleOrDefault(p => p.Pladsnummer == checkboxContent);
                    pladser.Add(plads);
                }
            }

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
                        Noter = noterTextBlock.Text
                    }}
            });

            database.SaveChanges();

            sagsnummerTextbox.Text = null;
            kundeTextbox.Text = null;
            setPladser();
            startAddresseTextbox.Text = null;
            slutAddresseTextbox.Text = null;
            kasserTextbox.Text = null;
        }

        private void SagsnummerTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool exists = database.Sager.Any(s => s.Sagsnummer.Equals(sagsnummerTextbox.Text));
            if (exists)
            {
                sagsnummerErrorLabel.Content = "Sagsnummer findes i forvejen";
            }
            else
            {
                sagsnummerErrorLabel.Content = "";
            }
        }

        private async void tilføjPladserButton_Click(object sender, RoutedEventArgs e)
        {
            Pladser[] pladser = database.Pladser.Where(p => string.IsNullOrEmpty(p.Sagsnummer)).ToArray();

            var view = new ListDialog();
            view.label.Content = "Vælg de pladser/lifte der skal tilføjes til sagen";
            foreach (Pladser plads in pladser)
            {
                view.listView.Items.Add(plads.Pladsnummer);
            }
            view.cancelButton.Content = "Annuller";
            view.acceptButton.Content = "Tilføj";

            await DialogHost.Show(view, "RootDialog", AfslutClosingEventHandler);
        }

        private void AfslutClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            
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
