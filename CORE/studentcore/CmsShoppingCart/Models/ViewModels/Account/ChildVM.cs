using CmsShoppingCart.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.ViewModels.Account
{
    public class ChildVM
    {

        public ChildVM()
        {

        }


        public ChildVM(Child row)
        {
            Id = row.Id;
            LastReport = row.LastReport;
            Cerficate = row.Certificate;
            Date = row.Date;



        }


        public int Id { get; set; }
        public string LastReport { get; set; }
        public string Cerficate { get; set; }
        [Display(Name = "Upload Date")]
        public DateTime Date { get; set; }

        public IEnumerable<string> GalleryImages { get; set; }


    }



    public class ContentVM
    {

        public ContentVM()
        {

        }


        public ContentVM(Content row)
        {
            Id = row.Id;
            Grade = row.Grade;
            Subject = row.Subject;
            VideoLesson = row.VideoLesson;
            Type = row.Type;
           


        }


        public int Id { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; }
        public string VideoLesson { get; set; }
        public string Type { get; set; }



        public IEnumerable<string> GalleryImages { get; set; }
     


    }
}