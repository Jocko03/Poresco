using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1_SlojPodataka.Modeli
{
     [Table("Transakcija")]
     public class TransakcijaModel
     {
          [Key] 
          public int RedniBroj { get; set; }

          public int PoreskiObveznikID { get; set; }

          public string NazivAkcije { get; set; }

          public string DatumPrenosa { get; set; }

          public decimal ProdajnaCena { get; set; }

          public string DatumSticanja { get; set; }

          public decimal NabavnaCena { get; set; }
     }
}