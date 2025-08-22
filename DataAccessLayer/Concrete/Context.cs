
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-0CSOJOE;database=FikoDvds;integrated security=true");
        }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Dvd> Dvds { get; set; }
        public DbSet<DvdActor> DvdActors { get; set; }
        public DbSet<DvdCategory> DvdCategories { get; set; }
        public DbSet<DvdDirector> DvdDirectors { get; set; }
        public DbSet<DvdRental> DvdRentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DvdActor>()
                .HasKey(da => new { da.DvdId, da.ActorId });

            modelBuilder.Entity<DvdCategory>()
                .HasKey(dc => new { dc.DvdId, dc.CategoryId });

            modelBuilder.Entity<DvdDirector>() // Ara tablo için ilişki tanımlandı
                .HasKey(dd => new { dd.DvdId, dd.DirectorId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
