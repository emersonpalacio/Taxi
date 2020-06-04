using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enum;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context,
                      IUserHelper userHelper)
        {
            this._context = context;
            this._userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Emerson", "palacio", "emerson@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Admin);
            UserEntity driver = await CheckUserAsync("2020", "sara", "palacio", "sara@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Driver);
            UserEntity user1 = await CheckUserAsync("3030", "emmanuel", "palacio", "emmanuel@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            UserEntity user2 = await CheckUserAsync("5050", "Camila", "Cifuentes", "camila@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            UserEntity user3 = await CheckUserAsync("6060", "Sandra", "Usuga", "sandra@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            UserEntity user4 = await CheckUserAsync("7070", "Lisa", "Marin", "luisa@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User);
            await CheckTaxisAsync(driver, user1, user2);
            await CheckUserGroups(user1, user2, user3, user4);
        }

        private async Task CheckUserGroups(UserEntity user1, UserEntity user2, UserEntity user3, UserEntity user4)
        {
            if (!_context.UserGroups.Any())
            {
                _context.UserGroups.Add(new UserGroupEntity
                {
                    User = user1,
                    Users = new List<UserGroupDetailEntity>
                    {
                        new UserGroupDetailEntity { User = user2 },
                        new UserGroupDetailEntity { User = user3 },
                        new UserGroupDetailEntity { User = user4 }
                   }
                });

                _context.UserGroups.Add(new UserGroupEntity
                {
                    User = user2,
                    Users = new List<UserGroupDetailEntity>
                    {
                        new UserGroupDetailEntity { User = user1 },
                        new UserGroupDetailEntity { User = user3 },
                        new UserGroupDetailEntity { User = user4 }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }


        private async Task<UserEntity> CheckUserAsync(string document, string firstName, string lastName, string email,
                                                                                string phone, string address, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }



        private async Task CheckTaxisAsync(UserEntity driver, UserEntity user1, UserEntity user2)
        {
            if (!_context.Taxis.Any())
            {
                _context.Taxis.Add(new TaxiEntity
                {
                    User = driver,
                    Plaque = "TPQ123",
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Muy buen servicio",
                            User = user1
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Conductor muy amable",
                            User = user1
                        }
                    }
                });

                _context.Taxis.Add(new TaxiEntity
                {
                    Plaque = "THW321",
                    User = driver,
                    Trips = new List<TripEntity>
                    {
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.5f,
                            Source = "ITM Fraternidad",
                            Target = "ITM Robledo",
                            Remarks = "Muy buen servicio",
                            User = user2
                        },
                        new TripEntity
                        {
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddMinutes(30),
                            Qualification = 4.8f,
                            Source = "ITM Robledo",
                            Target = "ITM Fraternidad",
                            Remarks = "Conductor muy amable",
                            User = user2
                        }
                    }
                });

                await _context.SaveChangesAsync();
            }
        }



    }
}
