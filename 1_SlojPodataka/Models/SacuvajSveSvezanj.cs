using _1_SlojPodataka.Modeli;
using System.Collections.Generic;

namespace Porescoo.Models
{
     public class SacuvajSveSvežanj
     {
          public PoreskiObveznikModel obveznik { get; set; }
          public List<TransakcijaModel> transakcije { get; set; }
     }
}