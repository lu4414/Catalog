using System;
using System.Text;
using Catalog.Repositories;
using Catalog.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Catalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   


            services.AddSingleton<IMongoClient>(ServiceProvider => {
                var settings = Configuration.
                GetSection(nameof(UserDbSettings)).
                Get<UserDbSettings>();
                return new MongoClient(settings.ConnectionString);
            });



            services.AddCors();
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes("fedaf7d8863b48e197b9287d492b708e"); //pequena trapaça
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
         
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String)); //make this data human friendly
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
              var mongoDbSettings = Configuration.
              GetSection(nameof(MongoDbSettings)).
              Get<MongoDbSettings>();
            services.AddSingleton<IMongoClient>(serviceProvider =>  //initialize the connection
            {              
                return new MongoClient(mongoDbSettings.ConnectionString);
            });
            services.AddSingleton<IItemsRepository, MongoDbItemsRepository>(); //Note that here we replace the IInmemoryItemsRepository
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });
            services.AddControllers(options => {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddHealthChecks()
                    .AddMongoDb(mongoDbSettings.ConnectionString, name: "mongodb", timeout: TimeSpan.FromSeconds(3));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
                //app.UseHttpsRedirection();
            }       
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
            
            
        }
    }
}
