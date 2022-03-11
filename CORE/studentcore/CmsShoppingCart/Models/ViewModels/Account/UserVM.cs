using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.ViewModels.Account
{
    public class UserVM
    {

        public UserVM()
        {

        }


        public UserVM(User row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            Username = row.Username;
            Password = row.Password;
            PhoneNumber = row.PhoneNumber;
            Gender = row.Gender;
            DateOfBirth = row.DateOfBirth;
            IdNumb = row.IdNumb;
            Balance = row.Balance;
            Maritual = row.Maritual;
            HomeLang = row.HomeLang;
            PostalAd = row.PostalAd;
            HomeAd = row.HomeAd;
            HomeNumber = row.HomeNumber;

        }

        public int Id { get; set; }
        [Display(Name = "First Name(s)")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]

        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Id Number")]
        public string IdNumb { get; set; }

        public int Balance { get; set; }
        [Display(Name = "Application Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Maritual Status")]
        public string Maritual { get; set; }
        [Display(Name = "Home Language")]
        public string HomeLang { get; set; }

        [Display(Name = "Postal Address")]
        public string PostalAd { get; set; }

        [Display(Name = "Home Address")]
        public string HomeAd { get; set; }
        [Display(Name = "Home Number")]
        public string HomeNumber { get; set; }


    }
}