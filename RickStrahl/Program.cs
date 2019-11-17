using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RickStrahl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var database = new Database();
            await database.Database.EnsureCreatedAsync();

            var result = await database
                .UserInfos
                .Select(u => new UserInfo
                {
                    Id = u.Id,
                    Name = u.Name,
                    Priviliges = u.Priviliges.Where(p => p.AccessKey.StartsWith("FTDB_")).ToList()
                })
                .FirstOrDefaultAsync();

        }
    }

    public class Database : DbContext
    {
        private const string connectionString = "server=localhost,11433;database=RickStrahl;user=sa;password=Pass123!";
        
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Privilige> Priviliges { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>()
                .HasData(new UserInfo
                {
                    Id = 1,
                    Name = "Rick"
                });

            modelBuilder.Entity<Privilige>()
                .HasData(
                    new Privilige {Id = 1, UserInfoId = 1, AccessKey = "FTDB_One"},
                    new Privilige {Id = 2, UserInfoId = 1, AccessKey = "FTDB_Two"},
                    new Privilige {Id = 3, UserInfoId = 1, AccessKey = "Other_One"},
                    new Privilige {Id = 4, UserInfoId = 1, AccessKey = "Other_Two"});
        }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Privilige> Priviliges { get; set; }
    }
    
    public class Privilige {
        public int Id { get; set; }
        public int UserInfoId { get; set; }
        public string AccessKey { get; set; }
        public virtual UserInfo UserInfo { get; set; }
    }
}