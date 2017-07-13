﻿using Bifald.DB;
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

        public Database()
        {
            InitializeComponent();
        }

        private void backupButton_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("En backup af databasen er nu oprettet");
            }
        }

        private void genskabButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                DefaultExt = "sql",
                Filter = "Database backups (*.sql)|*.sql|All files (*.*)|*.*"
            };
            openFileDialog.ShowDialog();

            if (!openFileDialog.FileName.Equals(""))
            {
                if (MessageBox.Show("Er du sikker på du vil genskabbe denne database?", "Genskab database", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
                        MessageBox.Show("Database er nu genskabt");
                    }
                    catch (FileNotFoundException fnfe)
                    {
                        MessageBox.Show("Filen " + fnfe.FileName + " blev ikke fundet");
                    }
                }
            }
        }
    }
}
