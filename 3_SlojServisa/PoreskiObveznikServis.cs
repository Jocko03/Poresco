using System;
using System.Collections.Generic;
using _1_SlojPodataka;
using _1_SlojPodataka.Modeli;
using _2_SlojPoslovneLogike;

namespace _2_SlojBiznisLogike.Servisi
{
     public class PoreskiObveznikServis
     {
          private readonly AdoObveznikRepozitorijum _repozitorijum;
          private readonly AdoProcedureRepozitorijum _procedureRepozitorijum;

          public PoreskiObveznikServis(string connectionString)
          {
               _repozitorijum = new AdoObveznikRepozitorijum(connectionString);
               _procedureRepozitorijum = new AdoProcedureRepozitorijum(connectionString);
          }

          public List<ObveznikSaTransakcijamaDto> FiltrirajObveznikeITransakcije(string filter)
          {
               return _procedureRepozitorijum.UcitajSaProcedurom(filter);
          }

          public List<PoreskiObveznikModel> PreuzmiSveObveznike()
          {
               return _repozitorijum.UcitajSve();
          }

          public List<TransakcijaModel> PreuzmiTransakcijeZaObveznika(string jmbg)
          {
               if (string.IsNullOrWhiteSpace(jmbg)) return new List<TransakcijaModel>();
               return _repozitorijum.UcitajTransakcijeZaObveznika(jmbg);
          }

          public bool SacuvajObveznikaITransakcije(PoreskiObveznikModel obveznik, List<TransakcijaModel> transakcije)
          {
               if (obveznik == null || string.IsNullOrWhiteSpace(obveznik.JMBG)) return false;
               return _repozitorijum.SacuvajSve(obveznik, transakcije);
          }

          public bool ObrisiObveznika(string email)
          {
               if (string.IsNullOrWhiteSpace(email)) return false;
               return _repozitorijum.ObrisiPoEmailu(email);
          }

          public decimal IzracunajKapitalniPorez(List<TransakcijaModel> transakcije)
          {
               if (transakcije == null || transakcije.Count == 0) return 0.00m;

               var washSaleLogika = new WashSaleLogika();
               decimal oporeziviDobitak = washSaleLogika.IzracunajOporeziviDobitak(transakcije);

               return oporeziviDobitak * 0.15m;
          }
     }
}