using _1_SlojPodataka;
using _1_SlojPodataka.Modeli;

namespace _3_SlojServisa
{
     public class NalogServis
     {
          
          private readonly NalogRepozitorijum _nalogSloj;

          public NalogServis()
          {
               _nalogSloj = new NalogRepozitorijum();
          }

          public bool RegistrujNovogKorisnika(KorisnikModel model)
          {
               if (model == null) return false;
               return _nalogSloj.RegistrujKorisnika(model);
          }

          public KorisnikModel PrijaviKorisnika(PrijavaModel model)
          {
               if (model == null || string.IsNullOrEmpty(model.KorisnickoIme) || string.IsNullOrEmpty(model.Lozinka))
               {
                    return null;
               }

               return _nalogSloj.PrijaviKorisnika(model);
          }

          public bool ObrisiKorisnikaPoEmailu(string email)
          {
               if (string.IsNullOrEmpty(email)) return false;
               return _nalogSloj.ObrisiObveznika(email);
          }
     }
}