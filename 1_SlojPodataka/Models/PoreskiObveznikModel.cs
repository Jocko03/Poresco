using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1_SlojPodataka.Modeli
{
     [Table("PoreskiObveznik")]
     public class PoreskiObveznikModel
     {
          [Key]
          public int ID { get; set; }
          public string Ime { get; set; }
          public string Prezime { get; set; }
          public string Email { get; set; }
          public string JMBG { get; set; }
          public string Opstina { get; set; }
          public string Adresa { get; set; }
          public string Broj { get; set; }
          public string BrojTelefona { get; set; }
          public string Drzava { get; set; }
          public string JmbgPoreskogPunomocnika { get; set; }
     }
}