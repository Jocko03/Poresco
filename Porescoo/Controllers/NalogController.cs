using System;
using System.Web.Mvc;
using _1_SlojPodataka.Modeli;
using _3_SlojServisa; 
using _2_SlojPoslovneLogike;

namespace Porescoo.Controllers
{
     public class NalogController : Controller
     {
          private readonly NalogServis _nalogServis;

          public NalogController()
          {
               _nalogServis = new NalogServis();
          }

          [HttpGet]
          public ActionResult Registracija()
          {
               return View("~/Views/Nalog/Registracija.cshtml");
          }

          [HttpPost]
          [ValidateAntiForgeryToken]
          public ActionResult Registracija(KorisnikModel model)
          {
               if (ModelState.IsValid)
               {
                    
                    bool uspeh = _nalogServis.RegistrujNovogKorisnika(model);

                    if (uspeh)
                    {
                         return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                         ModelState.AddModelError("", "Došlo je do greške prilikom čuvanja podataka u bazu.");
                    }
               }

               return View("~/Views/Nalog/Registracija.cshtml", model);
          }

          [HttpGet]
          public ActionResult Prijava()
          {
               return View("~/Views/Nalog/Prijava.cshtml");
          }

          [HttpPost]
          [ValidateAntiForgeryToken]
          public ActionResult Prijava(PrijavaModel model)
          {
               if (ModelState.IsValid)
               {
                   
                    var korisnikIzBaze = _nalogServis.PrijaviKorisnika(model);

                    if (korisnikIzBaze != null)
                    {
                         
                         Session["Korisnik"] = korisnikIzBaze;
                         return RedirectToAction("MojNalog");
                    }
                    else
                    {
                         ModelState.AddModelError("", "Neispravan email ili lozinka.");
                    }
               }
               return View("~/Views/Nalog/Prijava.cshtml", model);
          }

          public ActionResult MojNalog()
          {
               
               var ulogovaniKorisnik = Session["Korisnik"] as KorisnikModel;
               if (ulogovaniKorisnik == null)
               {
                    return RedirectToAction("Prijava");
               }

               
               var prazanModel = new PoreskiObveznikModel();

              
               return View("~/Views/MojNalog/MojNalog.cshtml", prazanModel);
          }
     }
}