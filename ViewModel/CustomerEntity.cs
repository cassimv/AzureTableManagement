using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTablesModel
{
    public class CustomerEntity : TableEntity
    {
        string _name, _surname;

        public CustomerEntity(string lastName, string firstName, string email, string phonenumber)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
            _name = firstName;
            _surname = lastName;
            Email = email;
            PhoneNumber = phonenumber;
        }

        public CustomerEntity() {}

        [Required]
        [Display(Name = "Last Name")]
        public string LastName
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                this.PartitionKey = value;
            }
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                this.RowKey = value;
            }
        }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required]
        public string Email { get; set; }
        [Phone]
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
