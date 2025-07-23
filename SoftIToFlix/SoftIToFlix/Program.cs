using Microsoft.EntityFrameworkCore;
using SoftIToFlix.Data;
using SoftIToFlix.Models;
using Microsoft.AspNetCore.Identity;
namespace SoftIToFlix;

public class Program
{
    public static void Main(string[] args)
    {
        IdentityRole identityRole;
       
       
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
        builder.Services.AddDefaultIdentity<SoftIToFlixUser>(options => options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ApplicationContext>();
        builder.Services.AddIdentity<SoftIToFlixUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
        {
            RoleManager<IdentityRole> roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager != null)
            {
                if (roleManager.Roles.Count() == 0)
                {
                    identityRole = new IdentityRole("Administrator");
                    roleManager.CreateAsync(identityRole).Wait();
                    identityRole = new IdentityRole("ContentAdmin");
                    roleManager.CreateAsync(identityRole).Wait();
                    identityRole = new IdentityRole("FlixUser");
                    roleManager.CreateAsync(identityRole).Wait();


                }
                UserManager<SoftIToFlixUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<SoftIToFlixUser>>();
                if (userManager != null)
                {
                    if (userManager.Users.Count() == 0)
                    {
                        
                            SoftIToFlixUser admin = new SoftIToFlixUser();
                            admin.UserName = "Administrator";                          
                            admin.Name = "Administrator";
                            admin.Email = "admin@gmail.com";
                            admin.PhoneNumber = "1112223344";                           
                            admin.Passive = false;
                            userManager.CreateAsync(admin, "Admin123!").Wait();
                            userManager.AddToRoleAsync(admin, "Administrator").Wait();

                        SoftIToFlixUser contentAdmin = new SoftIToFlixUser();
                        contentAdmin.UserName = "ContentAdmin";
                        contentAdmin.Name = "ContentAdmin";
                        contentAdmin.Email = "contentadmin@gmail.com";
                        contentAdmin.PhoneNumber = "1234567890";
                        contentAdmin.Passive = false;
                        userManager.CreateAsync(contentAdmin, "Admin123!").Wait();
                        userManager.AddToRoleAsync(contentAdmin, "ContentAdmin").Wait();

                    }
                }
            }



        }

        app.Run();
    }
}

