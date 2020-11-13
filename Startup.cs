using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Data;

namespace Shop
{
    public class Startup  //classe de inicialização...
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)//aqui informamos quais servicos iremos utilizar..
        {
            services.AddCors();//para fazer requisições localhost

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>(); //vai comprimir o json antes de mandar para a tela
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" }); //vai comprimir tudo que for json
            });

            //services.AddResponseCaching();

            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database")); //apontamos o DataContext, e em opt informamos que tipo estamos usando, poderia ser SqlServer, Postgres, etc
            //services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));

            services.AddScoped<DataContext, DataContext>();
            //AddScoped, garante que só tera um DataContext por requisição..
            //AddTransient, toda vez que criar o DataContext ira gerar um novo ao invés de buscar na memória como faz o Scoped
            //AddSingleton, vai criar uma instancia do DataContext por aplicação.. 

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo {Title = "PetShop Api", Version = "v1"}));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)//aqui configuramos como vamos utilizar os serviços..
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); //vai forçar utilização de HTTPS

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetShop API V1"));

            app.UseRouting();//utilizar o padrão de rotas do MVC

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => //enspoints são as urls, como o cliente vai acessar
            {
                endpoints.MapControllers();
            });
        }
    }
}
