using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using _1_SlojPodataka.Modeli;

namespace _1_SlojPodataka
{
     public class AdoProcedureRepozitorijum
     {
          private readonly string _connectionString;

          public AdoProcedureRepozitorijum(string connectionString)
          {
               _connectionString = connectionString;
          }

          public List<ObveznikSaTransakcijamaDto> UcitajSaProcedurom(string filter)
          {
               // Mapa sada koristi ID obveznika kao ključ, a vrednost je naš novi DTO
               var mapa = new Dictionary<int, ObveznikSaTransakcijamaDto>();

               using (SqlConnection kon = new SqlConnection(_connectionString))
               {
                    using (SqlCommand cmd = new SqlCommand("sp_UcitajObveznikeITransakcije", kon))
                    {
                         cmd.CommandType = CommandType.StoredProcedure;

                         if (string.IsNullOrWhiteSpace(filter))
                              cmd.Parameters.AddWithValue("@Filter", DBNull.Value);
                         else
                              cmd.Parameters.AddWithValue("@Filter", filter.Trim());

                         kon.Open();
                         using (SqlDataReader dr = cmd.ExecuteReader())
                         {
                              while (dr.Read())
                              {
                                   int obveznikId = Convert.ToInt32(dr["ObveznikID"]);

                                   
                                   if (!mapa.ContainsKey(obveznikId))
                                   {
                                        mapa[obveznikId] = new ObveznikSaTransakcijamaDto
                                        {
                                             Obveznik = new PoreskiObveznikModel
                                             {
                                                  ID = obveznikId,
                                                  Ime = dr["Ime"].ToString(),
                                                  Prezime = dr["Prezime"].ToString(),
                                                  Email = dr["Email"].ToString(),
                                                  JMBG = dr["JMBG"].ToString(),
                                                  Opstina = dr["Opstina"].ToString(),
                                                  Adresa = dr["Adresa"].ToString(),
                                                  Broj = dr["Broj"].ToString(),
                                                  BrojTelefona = dr["BrojTelefona"].ToString(),
                                                  Drzava = dr["Drzava"].ToString(),
                                                  JmbgPoreskogPunomocnika = dr["JmbgPoreskogPunomocnika"].ToString()
                                             },
                                             Transakcije = new List<TransakcijaModel>()
                                        };
                                   }

                                   
                                   if (dr["TransakcijaID"] != DBNull.Value)
                                   {
                                        mapa[obveznikId].Transakcije.Add(new TransakcijaModel
                                        {
                                             RedniBroj = Convert.ToInt32(dr["TransakcijaID"]),
                                             PoreskiObveznikID = obveznikId,
                                             NazivAkcije = dr["NazivAkcije"].ToString(),
                                             DatumPrenosa = dr["DatumPrenosa"].ToString(),
                                             ProdajnaCena = Convert.ToDecimal(dr["ProdajnaCena"]),
                                             DatumSticanja = dr["DatumSticanja"].ToString(),
                                             NabavnaCena = Convert.ToDecimal(dr["NabavnaCena"])
                                        });
                                   }
                              }
                         }
                    }
               }
               return new List<ObveznikSaTransakcijamaDto>(mapa.Values);
          }
     }
}