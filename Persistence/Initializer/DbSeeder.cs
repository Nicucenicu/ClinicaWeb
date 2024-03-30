using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Enums;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace ClinicaWeb.Persistence.Initializer
{
    public static class DbSeeder
    {
        public static void SeedDb(AppDbContext appDbContext)
        {
            appDbContext.Database.Migrate();
            AddProductCategory(appDbContext);
            AddUser(appDbContext);
        }
        private static void AddUser(AppDbContext appDbContext)
        {
            if (appDbContext.Users.Count() == 0)
            {
                var admin = new User()
                {
                    FirstName = "admin",
                    LastName = "Template",
                    Email = "admin@",
                    Phone = "123456789",
                    IsEmailVerified= true,
                    IsBlocked=false,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now,
                    Password= "AA7K81530367D3n5yedJkG+KnczUiMh7hiMsVwzrvMGFL0s+VfFVYtJM6fIFtOC2Yw==",
                    LoginAttempts = 0,
                    Role=Role.Administrator,

                };
                
                appDbContext.Users.Add(admin);
            }
        }
        private static void AddProductCategory(AppDbContext appDbContext)
        {

        }
        private static void AddProcedureCategory(AppDbContext appDbContext) { }
        private static void AddPracticianRank(AppDbContext appDbContext) { }
        private static void AddRanks(AppDbContext appDbContext) { }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
