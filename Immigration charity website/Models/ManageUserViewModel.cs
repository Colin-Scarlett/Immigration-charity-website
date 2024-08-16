using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Immigration_charity_website.Models
{
    // ViewModel to manage user-related actions
    public class ManageUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public IEnumerable<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }

        // Property to hold user details
        public UserDetailViewModel UserDetails { get; set; }

        private readonly ApplicationDbContext _context;

        public ManageUserViewModel()
        {
            _context = new ApplicationDbContext();
        }

        // Method to add a new user
        public void AddUser(ManageUserViewModel user)
        {
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(), // Generate a new unique ID
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.IsEmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactor
                // Add more properties as needed
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }

        // Method to delete a user by ID
        public void DeleteUser(string userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        // Method to view user details by ID
        public UserDetailViewModel GetUserDetails(string userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                return new UserDetailViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    DateCreated = user.LockoutEndDateUtc?.AddMonths(-3) ?? DateTime.Now.AddMonths(-3), // Example data
                    LastLogin = user.LockoutEndDateUtc?.AddDays(-5) ?? DateTime.Now.AddDays(-5) // Example data
                };
            }
            return null; // Return null if user is not found
        }
    }

    // ViewModel to represent user details
    public class UserDetailViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLogin { get; set; }
        // Add more details as required
    }
}
