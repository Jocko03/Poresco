using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using _1_SlojPodataka;
using _1_SlojPodataka.Modeli;
using _2_SlojBiznisLogike.Servisi;

namespace Poresco.Controllers
{
     public class MojNalogController : Controller
     {
          private readonly string connString = ConfigurationManager.ConnectionStrings["BazaKonekcija"].ConnectionString;

         
          [HttpGet]
          public ActionResult MojNalog()
          {
               PoreskiObveznikServis servis = new PoreskiObveznikServis(connString);

               
               ViewBag.SviObveznici = servis.FiltrirajObveznikeITransakcije(null);
               return View();
          }

          [HttpGet]
          public JsonResult FiltrirajTabelu(string tekst)
          {
               try
               {
                    PoreskiObveznikServis servis = new PoreskiObveznikServis(connString);
                    var rezultati = servis.FiltrirajObveznikeITransakcije(tekst);
                    return Json(rezultati, JsonRequestBehavior.AllowGet);
               }
               catch (Exception ex)
               {
                    return Json(new List<ObveznikSaTransakcijamaDto>(), JsonRequestBehavior.AllowGet);
               }
          }

          [HttpGet]
          public JsonResult DajPodatke(string jmbg)
          {
               if (string.IsNullOrEmpty(jmbg)) return Json(null, JsonRequestBehavior.AllowGet);

               PoreskiObveznikServis servis = new PoreskiObveznikServis(connString);
               var transakcije = servis.PreuzmiTransakcijeZaObveznika(jmbg);
               return Json(transakcije, JsonRequestBehavior.AllowGet);
          }

          [HttpPost]
          public JsonResult SacuvajSve(PoreskiObveznikModel obveznik, List<TransakcijaModel> transakcije)
          {
               if (obveznik == null || string.IsNullOrEmpty(obveznik.JMBG))
               {
                    return Json(new { uspeh = false, poruka = "Greška: Podaci o obvezniku ili JMBG nisu validni!" });
               }

               if (transakcije == null) transakcije = new List<TransakcijaModel>();

               PoreskiObveznikServis servis = new PoreskiObveznikServis(connString);


               decimal konacanPorez = servis.IzracunajKapitalniPorez(transakcije);

               bool uspeh = servis.SacuvajObveznikaITransakcije(obveznik, transakcije);

               if (uspeh)
               {
                    return Json(new { uspeh = true, poruka = "Uspešno sačuvano u bazu podataka!", porez = konacanPorez });
               }
               return Json(new { uspeh = false, poruka = "Greška pri upisu u bazu preko ADO.NET-a." });
          }

          [HttpGet]
          public ActionResult Obrisi(string email)
          {
               if (!string.IsNullOrEmpty(email))
               {
                    PoreskiObveznikServis servis = new PoreskiObveznikServis(connString);
                    servis.ObrisiObveznika(email);
               }
               return RedirectToAction("MojNalog");
          }

          [HttpPost]
          public JsonResult IzracunajBllPorez(List<TransakcijaModel> transakcije)
          {
               if (transakcije == null || transakcije.Count == 0)
               {
                    return Json(new { uspeh = true, iznosPoreza = 0.00 });
               }

               PoreskiObveznikServis servis = new PoreskiObveznikServis(connString);
               decimal porez = servis.IzracunajKapitalniPorez(transakcije);

               return Json(new { uspeh = true, iznosPoreza = porez });
          }
     }
}