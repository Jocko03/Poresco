using _1_SlojPodataka.Modeli;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace _2_SlojPoslovneLogike
{
     public class WashSaleLogika
     {
          private readonly int _washSalePeriodVid;

          public WashSaleLogika()
          {
               string baseDir = AppDomain.CurrentDomain.BaseDirectory;
               string xmlPutanja = null;

               string[] lokacije = new[] {
                   Path.Combine(baseDir, "PravilaPoslovanja.xml"),
                   Path.Combine(baseDir, "bin", "PravilaPoslovanja.xml"),
                   Path.Combine(baseDir, "..", "_2_SlojPoslovneLogike", "PravilaPoslovanja.xml"),
                   Path.Combine(baseDir, "..", "..", "_2_SlojPoslovneLogike", "PravilaPoslovanja.xml")
               };

               foreach (var putanja in lokacije)
               {
                    if (File.Exists(putanja))
                    {
                         xmlPutanja = putanja;
                         break;
                    }
               }

               _washSalePeriodVid = xmlPutanja != null ? UcitajPeriodIzXmla(xmlPutanja) : 30;
          }

          public WashSaleLogika(string xmlPutanja)
          {
               _washSalePeriodVid = File.Exists(xmlPutanja) ? UcitajPeriodIzXmla(xmlPutanja) : 30;
          }

          private int UcitajPeriodIzXmla(string xmlPutanja)
          {
               try
               {
                    XDocument doc = XDocument.Load(xmlPutanja);
                    return int.Parse(doc.Root.Element("WashSale").Element("PeriodUDanima").Value);
               }
               catch
               {
                    return 30;
               }
          }

          public decimal IzracunajOporeziviDobitak(List<TransakcijaModel> transakcije)
          {
               if (transakcije == null || !transakcije.Any()) return 0;

               decimal ukupniOporeziviIznos = 0;

               var grupisaneAkcije = transakcije.GroupBy(t => t.NazivAkcije?.Trim().ToUpper());

               foreach (var grupa in grupisaneAkcije)
               {
                    
                    var obradjeniRedovi = grupa.Select(t => new
                    {
                         Model = t,
                         KupovinaDT = ParsirajDatum(t.DatumSticanja), 
                         ProdajaDT = ParsirajDatum(t.DatumPrenosa),   
                         DobitakGubitak = (t.ProdajnaCena - t.NabavnaCena)
                    }).ToList();

                    for (int i = 0; i < obradjeniRedovi.Count; i++)
                    {
                         var trenutniRed = obradjeniRedovi[i];

                         if (trenutniRed.ProdajaDT == DateTime.MinValue || trenutniRed.KupovinaDT == DateTime.MinValue)
                         {
                              continue;
                         }

                         if (trenutniRed.DobitakGubitak >= 0)
                         {
                              
                              ukupniOporeziviIznos += trenutniRed.DobitakGubitak;
                         }
                         else
                         {

                              bool isWashSale = false;

                              foreach (var biloKojiRed in obradjeniRedovi)
                              {
                                   if (biloKojiRed == trenutniRed) continue;
                                   if (biloKojiRed.KupovinaDT == DateTime.MinValue) continue;

                                   
                                   double razlikaUDanima = (biloKojiRed.KupovinaDT - trenutniRed.ProdajaDT).TotalDays;

                                   if (Math.Abs(razlikaUDanima) <= _washSalePeriodVid)
                                   {
                                        isWashSale = true;
                                        break;
                                   }
                              }

                              if (isWashSale)
                              {

                                   System.Diagnostics.Debug.WriteLine($"Wash Sale aktiviran za {trenutniRed.Model.NazivAkcije}. Gubitak je uspešno ignorisan.");
                              }
                              else
                              {
                                   
                                   ukupniOporeziviIznos += trenutniRed.DobitakGubitak;
                              }
                         }
                    }
               }

               return ukupniOporeziviIznos > 0 ? ukupniOporeziviIznos : 0;
          }

          private DateTime ParsirajDatum(string datumStr)
          {
               if (string.IsNullOrWhiteSpace(datumStr)) return DateTime.MinValue;

               datumStr = datumStr.Trim();
               string[] dozvoljeniFormati = { "dd.MM.yyyy", "dd.MM.yyyy.", "d.M.yyyy", "d.M.yyyy.", "yyyy-MM-dd" };

               if (DateTime.TryParseExact(datumStr, dozvoljeniFormati, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
               {
                    return dt;
               }

               if (DateTime.TryParse(datumStr, out dt))
               {
                    return dt;
               }

               return DateTime.MinValue;
          }
     }
}