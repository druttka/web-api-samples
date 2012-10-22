using System.Data.Entity;

namespace WebApi.Data
{
    public class EFDemoContext : DbContext
    {
        public EFDemoContext() : base("EFDemo")
        {
            Database.SetInitializer(
                new DropCreateDatabaseIfModelChanges<EFDemoContext>());
        }

        public DbSet<Speaker> Speakers { get; set; }
    }
}
