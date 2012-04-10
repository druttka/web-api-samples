using System.Data.Entity;

namespace WebApi.Common
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
