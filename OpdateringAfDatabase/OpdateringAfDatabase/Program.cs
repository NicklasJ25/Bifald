using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OpdateringAfDatabase
{
    class Program
    {
        private static BifaldEntities database = new BifaldEntities();

        static void Main(string[] args)
        {
            try
            {
                List<Pladser> pladser = database.Pladser.Include(p => p.Sager).Where(p => p.Sagsnummer != null).ToList();
                foreach (Pladser plads in pladser)
                {
                    Plads_historik plads_Historik = new Plads_historik
                    {
                        Sagsnummer = plads.Sagsnummer,
                        Pladsnummer = plads.Pladsnummer,
                        Opret_afslut = "Oprettet",
                        Dato = plads.Sager.Opbevaring_startdato
                    };
                    database.Plads_historik.Add(plads_Historik);
                }
                database.SaveChanges();

                List<Afsluttede_pladser> afsluttet_pladser = database.Afsluttede_pladser.Include(p => p.Sager).ToList();
                foreach (Afsluttede_pladser plads in afsluttet_pladser)
                {
                    Plads_historik plads_Historik = new Plads_historik
                    {
                        Sagsnummer = plads.Sagsnummer,
                        Pladsnummer = plads.Pladsnummer,
                        Opret_afslut = "Afsluttet",
                        Dato = plads.Sager.Opbevaring_slutdato ?? DateTime.Now
                    };
                    database.Plads_historik.Add(plads_Historik);
                }
                database.SaveChanges();
                Console.WriteLine("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            Console.ReadLine();
        }
    }
}
