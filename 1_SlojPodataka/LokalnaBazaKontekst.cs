using _1_SlojPodataka.Modeli;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace _1_SlojPodataka
{
     public class LokalnaBazaKontekst : DbContext
     {
          
          public LokalnaBazaKontekst() : base("name=BazaKonekcija")
          {

               Database.SetInitializer<LokalnaBazaKontekst>(null);
          }

        
          public DbSet<KorisnikModel> Korisnik { get; set; }
          public DbSet<PoreskiObveznikModel> PoreskiObveznik { get; set; }
          public DbSet<TransakcijaModel> Transakcija { get; set; }

          protected override void OnModelCreating(DbModelBuilder modelBuilder)
          {
               
               modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
          }
     }
}