using System.ComponentModel.DataAnnotations;

namespace _1_SlojPodataka.Modeli
{
     public class RegistracijaModel
     {
          [Key]
          public int ID { get; set; } 
          public string KorisnickoIme { get; set; }
          public string Email { get; set; }
          public string Lozinka { get; set; }
          public string PotvrdaLozinke { get; set; }
          public string Ime { get; set; }
          public string Prezime { get; set; }
          public string JMBG { get; set; }
          public string Opstina { get; set; }
          public string Adresa { get; set; }
          public string Broj { get; set; }
          public string BrojTelefona { get; set; }
          public string Drzava { get; set; }
          public string JmbgPoreskogPunomocnika { get; set; }
     }
}