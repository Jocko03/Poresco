using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1_SlojPodataka.Modeli
{
     [Table("Korisnik")] 
     public class KorisnikModel
     {
          [Key]
          public int ID { get; set; }

          public string Email { get; set; }

          public string Lozinka { get; set; }

          public string KorisnickoIme { get; set; }
     }
}