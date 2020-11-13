using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class DataContext : DbContext
    {
        /* 
        DataContext -> representação do nosso banco de dados em memória
        permite fazermos o mapeamento da nossa aplicação relacionada com o BD..
        instalamos:
        dotnet tool install --global dotnet-ef  -> p/usarmos as migrations
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer
        dotnet add package Microsoft.EntityFrameworkCore.Design -> ajuda informar como o EF gera o banco baseado na nossa aplicação

        dotnet ef migrations add InitialCreate - gera a pasta migrations 
        dotnet ef database update - executa os scripts da migration no banco

        dotnet add package Microsoft.AspNetCore.Authentication
        dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer -v 3.1.10 versão atual no momento 5.0.0 é incompativel com o .net 3.1 que estava usando..

        */
        

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {     }

        //DbSet -> representação das tabelas.. 
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        
    }
}
