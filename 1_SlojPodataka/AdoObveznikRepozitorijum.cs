using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using DBUtils;
using _1_SlojPodataka.Modeli;

namespace _1_SlojPodataka
{
     public class AdoObveznikRepozitorijum
     {
          private readonly string _connectionString;

          public AdoObveznikRepozitorijum(string connectionString)
          {
               _connectionString = connectionString;
          }

          public List<PoreskiObveznikModel> UcitajSve()
          {
               List<PoreskiObveznikModel> lista = new List<PoreskiObveznikModel>();
               KonekcijaKlasa kon = new KonekcijaKlasa(_connectionString);

               if (kon.OtvoriKonekciju())
               {
                    TabelaKlasa tabela = new TabelaKlasa(kon, "PoreskiObveznik");
                    DataSet ds = tabela.DajPodatke("SELECT * FROM PoreskiObveznik");

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                         foreach (DataRow row in ds.Tables[0].Rows)
                         {
                              lista.Add(new PoreskiObveznikModel
                              {
                                   ID = Convert.ToInt32(row["ID"]),
                                   Ime = row["Ime"].ToString(),
                                   Prezime = row["Prezime"].ToString(),
                                   Email = row["Email"].ToString(),
                                   JMBG = row["JMBG"].ToString(),
                                   Opstina = row["Opstina"].ToString(),
                                   Adresa = row["Adresa"].ToString(),
                                   Broj = row["Broj"].ToString(),
                                   BrojTelefona = row["BrojTelefona"].ToString(),
                                   Drzava = row["Drzava"].ToString(),
                                   JmbgPoreskogPunomocnika = row["JmbgPoreskogPunomocnika"].ToString()
                              });
                         }
                    }
                    kon.ZatvoriKonekciju();
               }
               return lista;
          }

          public List<TransakcijaModel> UcitajTransakcijeZaObveznika(string jmbg)
          {
               List<TransakcijaModel> lista = new List<TransakcijaModel>();
               KonekcijaKlasa kon = new KonekcijaKlasa(_connectionString);

               if (kon.OtvoriKonekciju())
               {
                    TabelaKlasa tabela = new TabelaKlasa(kon, "Transakcija");
                    string upit = $@"SELECT t.* FROM Transakcija t 
                                 INNER JOIN PoreskiObveznik o ON t.PoreskiObveznikID = o.ID 
                                 WHERE o.JMBG = '{jmbg.Trim()}'";

                    DataSet ds = tabela.DajPodatke(upit);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                         foreach (DataRow row in ds.Tables[0].Rows)
                         {
                              lista.Add(new TransakcijaModel
                              {
                                   RedniBroj = Convert.ToInt32(row["RedniBroj"]),
                                   NazivAkcije = row["NazivAkcije"].ToString(),
                                   DatumPrenosa = row["DatumPrenosa"].ToString(),
                                   ProdajnaCena = Convert.ToDecimal(row["ProdajnaCena"]),
                                   DatumSticanja = row["DatumSticanja"].ToString(),
                                   NabavnaCena = Convert.ToDecimal(row["NabavnaCena"]),
                                   PoreskiObveznikID = Convert.ToInt32(row["PoreskiObveznikID"])
                              });
                         }
                    }
                    kon.ZatvoriKonekciju();
               }
               return lista;
          }

          public bool SacuvajSve(PoreskiObveznikModel obveznik, List<TransakcijaModel> transakcije)
          {
               KonekcijaKlasa kon = new KonekcijaKlasa(_connectionString);
               if (!kon.OtvoriKonekciju()) return false;

               TabelaKlasa tabela = new TabelaKlasa(kon, "PoreskiObveznik");
               List<string> upiti = new List<string>();

               string proveraUpit = $"SELECT ID FROM PoreskiObveznik WHERE JMBG = '{obveznik.JMBG.Trim()}'";
               DataSet ds = tabela.DajPodatke(proveraUpit);
               bool postoji = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;

               if (!postoji)
               {
                    string insertObveznik = $@"INSERT INTO PoreskiObveznik 
                (Ime, Prezime, Email, JMBG, Opstina, Adresa, Broj, BrojTelefona, Drzava, JmbgPoreskogPunomocnika) 
                VALUES 
                ('{obveznik.Ime}', '{obveznik.Prezime}', '{obveznik.Email}', '{obveznik.JMBG}', 
                 '{obveznik.Opstina}', '{obveznik.Adresa}', '{obveznik.Broj}', '{obveznik.BrojTelefona}', 
                 '{obveznik.Drzava}', '{obveznik.JmbgPoreskogPunomocnika}')";
                    upiti.Add(insertObveznik);

                    foreach (var t in transakcije)
                    {
                         string pCena = t.ProdajnaCena.ToString(CultureInfo.InvariantCulture);
                         string nCena = t.NabavnaCena.ToString(CultureInfo.InvariantCulture);

                         string insertTransakcija = $@"INSERT INTO Transakcija 
                    (NazivAkcije, DatumPrenosa, ProdajnaCena, DatumSticanja, NabavnaCena, PoreskiObveznikID) 
                    VALUES 
                    ('{t.NazivAkcije}', '{t.DatumPrenosa}', {pCena}, '{t.DatumSticanja}', {nCena}, 
                     (SELECT TOP 1 ID FROM PoreskiObveznik WHERE JMBG = '{obveznik.JMBG}'))";
                         upiti.Add(insertTransakcija);
                    }
               }
               else
               {
                    int obveznikId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);

                    string updateObveznik = $@"UPDATE PoreskiObveznik SET 
                Ime = '{obveznik.Ime}', Prezime = '{obveznik.Prezime}', Email = '{obveznik.Email}', 
                Opstina = '{obveznik.Opstina}', Adresa = '{obveznik.Adresa}', Broj = '{obveznik.Broj}', 
                BrojTelefona = '{obveznik.BrojTelefona}', Drzava = '{obveznik.Drzava}', 
                JmbgPoreskogPunomocnika = '{obveznik.JmbgPoreskogPunomocnika}' 
                WHERE ID = {obveznikId}";
                    upiti.Add(updateObveznik);

                    string obrisiStare = $"DELETE FROM Transakcija WHERE PoreskiObveznikID = {obveznikId}";
                    upiti.Add(obrisiStare);

                    foreach (var t in transakcije)
                    {
                         string pCena = t.ProdajnaCena.ToString(CultureInfo.InvariantCulture);
                         string nCena = t.NabavnaCena.ToString(CultureInfo.InvariantCulture);

                         string insertTransakcija = $@"INSERT INTO Transakcija 
                    (NazivAkcije, DatumPrenosa, ProdajnaCena, DatumSticanja, NabavnaCena, PoreskiObveznikID) 
                    VALUES 
                    ('{t.NazivAkcije}', '{t.DatumPrenosa}', {pCena}, '{t.DatumSticanja}', {nCena}, {obveznikId})";
                         upiti.Add(insertTransakcija);
                    }
               }

               bool uspeh = tabela.IzvrsiAzuriranje(upiti);
               kon.ZatvoriKonekciju();
               return uspeh;
          }

          public bool ObrisiPoEmailu(string email)
          {
               KonekcijaKlasa kon = new KonekcijaKlasa(_connectionString);
               if (!kon.OtvoriKonekciju()) return false;

               TabelaKlasa tabela = new TabelaKlasa(kon, "PoreskiObveznik");
               string nadjiIdUpit = $"SELECT ID FROM PoreskiObveznik WHERE Email = '{email.Trim()}'";
               DataSet ds = tabela.DajPodatke(nadjiIdUpit);

               if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
               {
                    kon.ZatvoriKonekciju();
                    return false;
               }

               int obveznikId = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"]);
               List<string> upitiZaBrisanje = new List<string>
            {
                $"DELETE FROM Transakcija WHERE PoreskiObveznikID = {obveznikId}",
                $"DELETE FROM PoreskiObveznik WHERE ID = {obveznikId}"
            };

               bool uspeh = tabela.IzvrsiAzuriranje(upitiZaBrisanje);
               kon.ZatvoriKonekciju();
               return uspeh;
          }
     }
}