using ForumFuncionario.Api.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForumFuncionario.Api.Context
{
    public class AppDbContext : IdentityDbContext<UserApp, IdentityRole<int>, int>
    {
        // Construtor que chama o construtor base do DbContext com as opções
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define as entidades que serão mapeadas para o banco de dados
        public DbSet<Post> Posts { get; set; }
        public DbSet<Ramal> Ramais { get; set; }
        public DbSet<IdentityRole<int>> Role { get; set; }
        public DbSet<UserApp> AppUser { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().HasKey(p => p.Id); // Define a chave primária da entidade Post
        }
    }
}
