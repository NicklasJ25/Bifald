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
    public partial class OpretSletPladser : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        Validering validering = new Validering();

        public OpretSletPladser()
        {
            InitializeComponent();

            List<Pladser> pladser = database.Pladser.OrderBy(p => p.Pladsnummer).ToList();
            pladserListView.ItemsSource = pladser;
        }

        private void opretPladsButton_Click(object sender, RoutedEventArgs e)
        {
            validering.ValiderOpretPlads(pladsnummerTextBox.Text, pladsLiftComboBox.Text);
            if (validering.opretPladsValidering == null)
            {
                Pladser nyplads = new Pladser();

                nyplads.Pladsnummer = pladsnummerTextBox.Text;
                nyplads.Type = pladsLiftComboBox.Text;

                database.Pladser.Add(nyplads);
                database.SaveChanges();

                List<Pladser> pladser = database.Pladser.OrderBy(p => p.Pladsnummer).ToList();
                pladserListView.ItemsSource = pladser;
            }
            else
            {
                MessageBox.Show(validering.opretPladsValidering);
            }
        }

        private async void sletPladsButton_Click(object sender, RoutedEventArgs e)
        {
            

            if (pladserListView.SelectedItem != null)
            {
                Pladser plads = (Pladser)pladserListView.SelectedItem;
                var view = new CustomDialog();
                view.label.Content = "Er du sikker på du vil slette " + plads.Type + " " + plads.Pladsnummer + "?";

                await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            }
            else
            {
                MessageBox.Show("Vælg en plads der skal slettes");
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                Pladser plads = (Pladser)pladserListView.SelectedItem;
                Pladser sletPlads = database.Pladser.Find(plads.Pladsnummer);
                database.Pladser.Remove(sletPlads);
                database.SaveChanges();
                List<Pladser> pladser = database.Pladser.OrderBy(p => p.Pladsnummer).ToList();
                pladserListView.ItemsSource = pladser;
            }
        }

        private void søgButton_Click(object sender, RoutedEventArgs e)
        {
            List<Pladser> pladser = database.Pladser.Where(p =>
                           p.Pladsnummer.Contains(søgningTextBox.Text)
                        || p.Type.Contains(søgningTextBox.Text)).ToList();
            pladserListView.ItemsSource = pladser;
        }
    }
}
