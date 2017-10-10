using Bifald.DB;
using Bifald.Dialog;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Bifald
{
    public partial class Database : Page
    {
        DatabaseEntities database = new DatabaseEntities();
        OpenFileDialog openFileDialog;

        bool backupGenskabt = false;

        public Database()
        {
            InitializeComponent();
        }

        private async void backupButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                DefaultExt = "sql",
                Filter = "Database backups (*.sql)|*.sql|All files (*.*)|*.*"
            };
            saveFileDialog.ShowDialog();

            if (!saveFileDialog.FileName.Equals(""))
            {
                string constring = "server=localhost;user id=root;password=root;persistsecurityinfo=True;database=bifald";
                string file = saveFileDialog.FileName;
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToFile(file);
                            conn.Close();
                        }
                    }
                }
                var view = new StandardDialog();
                view.label.Content = "En backup af databasen er nu oprettet";
                view.cancelButton.Visibility = Visibility.Hidden;
                view.acceptButton.Content = "Ok";
                await DialogHost.Show(view, "RootDialog");
            }
        }

        private async void genskabButton_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "sql",
                Filter = "Database backups (*.sql)|*.sql|All files (*.*)|*.*"
            };
            openFileDialog.ShowDialog();

            if (!openFileDialog.FileName.Equals(""))
            {
                StandardDialog view = new StandardDialog();
                view.label.Content = "Er du sikker på du vil genskabe denne database?";
                view.cancelButton.Content = "Nej";
                view.acceptButton.Content = "Ja";
                await DialogHost.Show(view, "RootDialog", GenskabClosingEventHandler);
                if (backupGenskabt)
                {
                    view.label.Content = "Database er nu genskabt";
                    view.cancelButton.Visibility = Visibility.Hidden;
                    view.acceptButton.Content = "Ok";
                    await DialogHost.Show(view, "RootDialog");
                }
                else
                {
                    view.label.Content = "Filen " + openFileDialog.FileName + " blev ikke fundet";
                    view.cancelButton.Visibility = Visibility.Hidden;
                    view.acceptButton.Content = "Ok";
                    await DialogHost.Show(view, "RootDialog");
                }
            }
        }

        private void GenskabClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                try
                {
                    string constring = "server=localhost;user id=root;password=root;persistsecurityinfo=True;database=bifald";
                    string file = openFileDialog.FileName;
                    using (MySqlConnection conn = new MySqlConnection(constring))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ImportFromFile(file);
                                conn.Close();
                            }
                        }
                    }
                    backupGenskabt = true;
                }
                catch (FileNotFoundException)
                {
                    backupGenskabt = false;
                }
            }
        }
    }
}
