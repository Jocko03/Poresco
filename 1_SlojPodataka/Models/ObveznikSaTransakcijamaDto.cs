using System.Collections.Generic;

namespace _1_SlojPodataka.Modeli
{
     public class ObveznikSaTransakcijamaDto
     {
          public PoreskiObveznikModel Obveznik { get; set; }
          public List<TransakcijaModel> Transakcije { get; set; } = new List<TransakcijaModel>();
     }
}