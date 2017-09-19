using Bifald.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Bifald
{
    class Validering
    {
        DatabaseEntities database = new DatabaseEntities();

        public string opretPladsValidering;
        public string opretSagValidering;
        public string søgPladserValidering;
        public string hentLeverKasserValidering;

        public void ValiderOpretPlads(string pladsnummer, string type)
        {
            opretPladsValidering = null;

            if (string.IsNullOrEmpty(pladsnummer))
            {
                opretPladsValidering += "Husk at udfylde pladsnummer.";
            }
            if (type == "Vælg type")
            {
                opretPladsValidering += "\nHusk at vælge en type.";
            }
            if (database.Pladser.Find(pladsnummer) != null)
            {
                opretPladsValidering += "\nDenne plads er allerede oprettet.";
            }
        }

        public void ValiderOpretSag(string sagsnummer, string kunde, List<Pladser> pladser, string addresseFra, string addresseTil, string kasser)
        {
            opretSagValidering = null;

            if (string.IsNullOrEmpty(sagsnummer))
            {
                opretSagValidering += "Husk at udfylde sagsnummer";
            }
            if (string.IsNullOrEmpty(kunde))
            {
                opretSagValidering += "\nHusk at udfylde kundes navn";
            }
            if (pladser.Count == 0)
            {
                opretSagValidering += "\nHusk at vælge en eller flere pladser";
            }
            if (string.IsNullOrEmpty(addresseFra))
            {
                opretSagValidering += "\nHusk at udfylde addresse fra";
            }
            Regex regexPostnummer = new Regex("^[0-9]{0,}$");
            if (string.IsNullOrEmpty(kasser))
            {
                opretSagValidering += "\nHusk at udfylde antallet af kasser";
            }
            else if (!regexPostnummer.IsMatch(kasser))
            {
                opretSagValidering += "\nKasser skal være et tal";
            }
        }

        public void ValiderSøgSager(string sagsnummer, string pladsnummer, string kunde, bool afsluttet)
        {
            søgPladserValidering = null;

            if (!string.IsNullOrEmpty(sagsnummer))
            {
                if (database.Sager.Where(s => s.Afsluttet == afsluttet).Where(s => s.Sagsnummer.Contains(sagsnummer)).Count() == 0)
                {
                    søgPladserValidering += "Sagsnummer findes ikke";
                }
            }
            if (!string.IsNullOrEmpty(pladsnummer))
            {
                if (database.Pladser.Where(p => p.Pladsnummer.Contains(pladsnummer)).Count() == 0)
                {
                    søgPladserValidering += "\nPladsnummer findes ikke";
                }
                else
                {
                    bool findesSagsnummer = false;
                    List<Pladser> pladser = database.Pladser.Where(p => p.Pladsnummer.Contains(pladsnummer)).ToList();
                    foreach (Pladser plads in pladser)
                    {
                        if (!string.IsNullOrEmpty(plads.Sagsnummer))
                        {
                            findesSagsnummer = true;
                            break;
                        }
                    }
                    if (!findesSagsnummer)
                    {
                        søgPladserValidering += "\nPladsnummer har ikke noget sagsnummer";
                    }
                }
            }
            if (!string.IsNullOrEmpty(kunde))
            {
                if (database.Kunder.Where(k => k.Navn.Contains(kunde)) == null)
                {
                    søgPladserValidering += "Kunde findes ikke";
                }
            }
        }

        public void ValiderHentLeverKasser(ComboBox sagsnummer, ComboBox hentetLeveret, string dato, string antal)
        {
            hentLeverKasserValidering = null;

            if (sagsnummer.SelectedIndex == 0)
            {
                hentLeverKasserValidering += "Husk at vælge et sagsnummer";
            }
            if (hentetLeveret.SelectedIndex == 0)
            {
                hentLeverKasserValidering += "\nHusk at vælge om kasserne skal hentes eller leveres";
            }
            if (string.IsNullOrEmpty(dato))
            {
                hentLeverKasserValidering += "\nHusk at udfylde datoen";
            }
            Regex regexPostnummer = new Regex("^[0-9]{0,}$");
            int leveretKasser = database.Kasser.Where(k => k.Sagsnummer == sagsnummer.SelectionBoxItem.ToString() && k.Hentet_leveret.Equals("Leveret")).ToList().Sum(k => (int?)k.Antal ?? 0);
            int hentetKasser = database.Kasser.Where(k => k.Sagsnummer == sagsnummer.SelectionBoxItem.ToString() && k.Hentet_leveret.Equals("Hentet")).ToList().Sum(k => (int?)k.Antal ?? 0);
            leveretKasser -= hentetKasser;
            if (string.IsNullOrEmpty(antal))
            {
                hentLeverKasserValidering += "\nHusk at udfylde antallet";
            }
            else if (!regexPostnummer.IsMatch(antal))
            {
                hentLeverKasserValidering += "\nAntallet skal være et tal";
            }
            else if (hentetLeveret.SelectionBoxItem.Equals("Hentes") && int.Parse(antal) > leveretKasser)
            {
                hentLeverKasserValidering += "\nDer kan ikke hentes flere kasser end der er leveret";
            }
        }
    }
}
