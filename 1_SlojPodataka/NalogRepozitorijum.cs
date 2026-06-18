using _1_SlojPodataka.Modeli;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace _1_SlojPodataka
{
     public class NalogRepozitorijum
     {
          private readonly LokalnaBazaKontekst _context;

          public NalogRepozitorijum()
          {
               _context = new LokalnaBazaKontekst();
          }

          
          public List<PoreskiObveznikModel> UzmiSveObveznike()
          {
               try
               {
                    return _context.PoreskiObveznik.ToList();
               }
               catch (Exception ex)
               {
                    throw new Exception("Greška pri dobavljanju obveznika kroz EF: " + ex.Message, ex);
               }
          }

          
          public bool RegistrujKorisnika(KorisnikModel podaci)
          {
               if (podaci == null) return false;

               try
               {
                    _context.Korisnik.Add(podaci);
                    int redova = _context.SaveChanges();
                    return redova > 0;
               }
               catch (Exception ex)
               {
                    throw new Exception("Greška pri registraciji korisnika kroz EF: " + ex.Message, ex);
               }
          }

         
          public KorisnikModel PrijaviKorisnika(PrijavaModel podaci)
          {
               if (podaci == null) return null;

               try
               {
                    return _context.Korisnik.FirstOrDefault(k =>
                         k.KorisnickoIme == podaci.KorisnickoIme &&
                         k.Lozinka == podaci.Lozinka);
               }
               catch (Exception ex)
               {
                    throw new Exception("Greška pri prijavi korisnika kroz EF: " + ex.Message, ex);
               }
          }

          
          public bool ObrisiObveznika(string email)
          {
               if (string.IsNullOrWhiteSpace(email)) return false;

               using (var dbTransakcija = _context.Database.BeginTransaction())
               {
                    try
                    {
                         var obveznik = _context.PoreskiObveznik.FirstOrDefault(o => o.Email.Trim() == email.Trim());

                         if (obveznik != null)
                         {
                              
                              var vezaneTransakcije = _context.Transakcija
                                   .Where(t => t.PoreskiObveznikID == obveznik.ID)
                                   .ToList();

                              if (vezaneTransakcije.Any())
                              {
                                   _context.Transakcija.RemoveRange(vezaneTransakcije);
                              }

                              _context.PoreskiObveznik.Remove(obveznik);

                              _context.SaveChanges();
                              dbTransakcija.Commit();
                              return true;
                         }

                         return false;
                    }
                    catch (Exception ex)
                    {
                         dbTransakcija.Rollback();
                         throw new Exception("Greška pri brisanju obveznika kroz EF: " + ex.Message, ex);
                    }
               }
          }
     }
}