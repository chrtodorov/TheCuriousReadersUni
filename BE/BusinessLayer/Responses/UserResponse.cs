using BusinessLayer.Enumerations;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Responses
{
    public class UserResponse
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public AccountStatus Status { get; set; }
        public Address Address { get; set; } = null!;
    }
}