using EVChargingStation.CARC.Application.HuyPD.Interfaces.Commons;
using EVChargingStation.CARC.Application.HuyPD.Utils;
using EVChargingStation.CARC.Domain.HuyPD;
using EVChargingStation.CARC.Domain.HuyPD.Entities;
using EVChargingStation.CARC.Domain.HuyPD.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace EVChargingStation.CARC.WebAPI.HuyPD.Controllers
{

    [ApiController]
    [Route("api/system")]
    public class SystemController : ControllerBase
    {
        private readonly FA25_SWD392_SE182400_G6_EvChargingStation _context;
        private readonly ILoggerService _logger;

        public SystemController(FA25_SWD392_SE182400_G6_EvChargingStation context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("seed-all-data")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                await ClearDatabase(_context);
                await SeedUserAsync();
                await SeedVehicleAsync();

                return Ok(ApiResult<object>.Success(new
                {
                    Message = "Data seeded successfully."
                }));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.Error("Database update error during data seeding: " + dbEx.Message);
                return StatusCode(500, "Database update error occurred while seeding data.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error during data seeding: " + ex.Message);
                return StatusCode(500, "An error occurred while seeding data.");
            }
        }

        private async Task ClearDatabase(FA25_SWD392_SE182400_G6_EvChargingStation context)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                //Delete data in order to avoid foreign key constraint issues
                await context.InvoiceTruongNN.ExecuteDeleteAsync();
                await context.Sessions.ExecuteDeleteAsync();
                await context.Connectors.ExecuteDeleteAsync();
                await context.StationAnhDHV.ExecuteDeleteAsync();
                await context.Locations.ExecuteDeleteAsync();
                await context.UserPlanHoaHTT.ExecuteDeleteAsync();
                await context.Plans.ExecuteDeleteAsync();
                await context.VehicleHuyPD.ExecuteDeleteAsync();
                await context.Users.ExecuteDeleteAsync();

                await transaction.CommitAsync();
                _logger.Success("Deleted data in database successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Error("Error clearing database: " + ex.Message);
                throw;
            }
        }

        //Seed methods go here
        private async Task SeedUserAsync()
        {
            var passwordHasher = new PasswordHasher();

            //Seed User
            var users = new List<User>
            {
                new()
                {
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = passwordHasher.HashPassword("Admin@123"),
                    DateOfBirth = DateTime.UtcNow.AddYears(-30),
                    Gender = Gender.Male,
                    Email = "Admin@gmail.com",
                    Phone = "1234567890",
                    Address = "123 Admin St, City, Country",
                    Role = RoleType.Admin,
                    Status = UserStatus.Active
                },
                new()
                {
                    FirstName = "Huy",
                    LastName = "Pham",
                    PasswordHash = passwordHasher.HashPassword("123456"),
                    DateOfBirth = DateTime.UtcNow.AddYears(-22),
                    Gender = Gender.Male,
                    Email = "huy@admin.com",
                    Phone = "0838255236",
                    Address = "123 Admin St, HCM, VietNam",
                    Role = RoleType.Admin,
                    Status = UserStatus.Active
                }
            };
            _logger.Info("Seeding users with roles...");

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            _logger.Success("Users seeded successfully.");
        }

        private async Task SeedVehicleAsync()
        {
            _logger.Info("Seeding vehicles...");

            //Get Admin User
            var adminUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == "Admin@gmail.com");

            if (adminUser == null)
            {
                _logger.Error("Admin user not found for vehicle seeding.");
                return;
            }

            //Create vehicles for Admin
            var vehicles = new List<VehicleHuyPD>
            {
                new()
                {
                    Make = "Tesla",
                    Model = "Model 3",
                    Year = 2023,
                    LicensePlate = "30A-1345",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "VinFast",
                    Model = "VF e34",
                    Year = 2024,
                    LicensePlate = "29B-67890",
                    ConnectorType = ConnectorType.CHAdeMO,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "BMW",
                    Model = "i4",
                    Year = 2023,
                    LicensePlate = "31C-54321",
                    ConnectorType = ConnectorType.AC,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Hyundai",
                    Model = "Ioniq 5",
                    Year = 2022,
                    LicensePlate = "30A-2456",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Nissan",
                    Model = "Leaf",
                    Year = 2021,
                    LicensePlate = "29B-9876",
                    ConnectorType = ConnectorType.CHAdeMO,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "BMW",
                    Model = "i4",
                    Year = 2023,
                    LicensePlate = "31C-4521",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Kia",
                    Model = "EV6",
                    Year = 2022,
                    LicensePlate = "88D-6543",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "VinFast",
                    Model = "VF 8",
                    Year = 2024,
                    LicensePlate = "99E-1122",
                    ConnectorType = ConnectorType.AC,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Porsche",
                    Model = "Taycan",
                    Year = 2023,
                    LicensePlate = "33F-7788",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Mitsubishi",
                    Model = "Outlander PHEV",
                    Year = 2020,
                    LicensePlate = "43G-3344",
                    ConnectorType = ConnectorType.CHAdeMO,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Mercedes-Benz",
                    Model = "EQS",
                    Year = 2023,
                    LicensePlate = "51H-5566",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "BYD",
                    Model = "Atto 3",
                    Year = 2024,
                    LicensePlate = "60K-7789",
                    ConnectorType = ConnectorType.AC,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Audi",
                    Model = "e-tron GT",
                    Year = 2023,
                    LicensePlate = "70L-9900",
                    ConnectorType = ConnectorType.CCS,
                    UserId = adminUser.HuyPDID
                },
                new()
                {
                    Make = "Toyota",
                    Model = "bZ4X",
                    Year = 2023,
                    LicensePlate = "80M-2233",
                    ConnectorType = ConnectorType.CHAdeMO,
                    UserId = adminUser.HuyPDID
                }

            };

            await _context.VehicleHuyPD.AddRangeAsync(vehicles);
            await _context.SaveChangesAsync();
            _logger.Success($"Seeded {vehicles.Count} vehicles seeded successfully.");
        }

    }
}