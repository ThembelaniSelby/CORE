using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Models.Data
{
    public class GetQuery
    {

        public string Main()
        {
            int length = 20;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }



        public string GenCode()
        {
            int length = 10;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }


    }

    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserRole
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        public int RoleId { get; set; }


        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Id Number")]
        public string IdNumb { get; set; }

        public int Balance { get; set; }
        [Display(Name = "Application Date")]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Maritual Status")]
        public string Maritual { get; set; }
        [Required]
        [Display(Name = "Home Language")]
        public string HomeLang { get; set; }

        [Required]
        [Display(Name = "Postal Address")]
        public string PostalAd { get; set; }

        [Display(Name = "Home Address")]
        public string HomeAd { get; set; }
        [Required]
        [Display(Name = "Home Number")]
        public string HomeNumber { get; set; }


    }

    public class Child
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "First Name(s)")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string ParentEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        public string ChildEmail { get; set; }


        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Gender { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Application Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Home Language")]
        public string HomeLang { get; set; }

        [Display(Name = "Id Number")]
        public string IdNumb { get; set; }
        [Display(Name = "Grade")]
        public string Grade { get; set; }

        public string Status { get; set; }

        [Display(Name = "Certificate")]
        public string Certificate { get; set; }
        [Display(Name = "Last Report")]
        public string LastReport { get; set; }

        public string LeearnerCode { get; set; }

        public string LearnerPic { get; set; }

    }

    public class ChildReg
    {
        [Key]
        public int Id { get; set; }

        public string IdNumber { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Nationality { get; set; }

        public string PreviosSchool { get; set; }
        public string ReasonForLeaving { get; set; }
        public string TransferNumber { get; set; }
        public string Disability { get; set; }
        public string ClinicCard { get; set; }
    }

    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Attendancee { get; set; }
        public string ChildName { get; set; }
        public string ChildIdNumber { get; set; }

        public int ChildId { get; set; }
    }

    public class QRModel
    {
        public string Message { get; set; }
    }

    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Commentt { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Code")]
        public string CODE { get; set; }
        [Required]
        [Display(Name = "Coz Name")]
        public string NAME { get; set; }

        [Display(Name = "Coz Name Fix")]
        public string NameFix { get; set; }
        [Required]
        [Display(Name = "Minimum Points")]
        public int MinPoints { get; set; }

    }

    public class Quotation
    {
        [Key]
        public int Id { get; set; }
        public string Administration { get; set; }
        public int ChildId { get; set; }
        public string AfterHours { get; set; }
        public string Delivery { get; set; }
        public string Uniform { get; set; }
        public string Stationary { get; set; }
        public string Event_sports { get; set; }
        public string Feeding_scheme { get; set; }
        public string Status { get; set; }
        public int Total { get; set; }
    }

    public class Fee
    {
        [Key]

        public int Id { get; set; }
        public int AdminFee { get; set; }
        public int AfterHourFee { get; set; }
        public int UniformFee { get; set; }
        public int StationaryFee { get; set; }
        public int Event_sportFee { get; set; }
        public int Feeding_schemeFee { get; set; }
        public int OneWayDiv { get; set; }
        public int TwoWayDiv { get; set; }

    }

    public class Content
    {
        [Key]
        public int Id { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; }
        public string VideoLesson { get; set; }
        public string Type { get; set; }

    }

    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Range(typeof(int), "1", "12")]
        public int Number { get; set; }
    }

    public class Gradesubject
    {
        [Key]
        public int Id { get; set; }
        public int Subid { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameFix { get; set; }
        public int Gradeid { get; set; }
        public string GradeName { get; set; }
        public int GradeNumber { get; set; }

    }

    public class Subjectteacher
    {
        [Key]
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameFix { get; set; }
        public string GradeName { get; set; }
        public int GradeNumber { get; set; }
        public string TeacherName { get; set; }
        public string TeacherId { get; set; }
    }

    public class Classteacher
    {
        [Key]
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameFix { get; set; }
        public string GradeName { get; set; }
        public int GradeNumber { get; set; }
        public string TeacherName { get; set; }
        public string TeacherId { get; set; }
    }

    public class ClassTVM
    {

        public ClassTVM()
        {

        }


        public ClassTVM(Classteacher row)
        {
            Id = row.Id;
            SubjectName = row.SubjectName;
            SubjectNameFix = row.SubjectNameFix;
            GradeName = row.GradeName;
            GradeNumber = row.GradeNumber;
            TeacherName = row.TeacherName;
            TeacherId = row.TeacherId;
        }

        [Key]
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameFix { get; set; }
        public string GradeName { get; set; }
        public int GradeNumber { get; set; }
        public string TeacherName { get; set; }
        public string TeacherId { get; set; }

        public IEnumerable<SelectListItem> GradeTeachers { get; set; }

    }

    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameFix { get; set; }
    }

    public class Application
    {
        [Key]
        public int Id { get; set; }
        public string IdNumber { get; set; }
        public string CozCode { get; set; }
        public DateTime AppDate { get; set; }
        public string Status { get; set; }

        //public override int GetHashCode()
        //{
        //    return -144711466 + EqualityComparer<string>.Default.GetHashCode(CozCode);
        //}
    }

    public class SubTVM
    {

        public SubTVM()
        {

        }


        public SubTVM(Subjectteacher row)
        {
            Id = row.Id;
            SubjectName = row.SubjectName;
            SubjectNameFix = row.SubjectNameFix;
            GradeName = row.GradeName;
            GradeNumber = row.GradeNumber;
            TeacherName = row.TeacherName;
            TeacherId = row.TeacherId;
        }

        [Key]
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameFix { get; set; }
        public string GradeName { get; set; }
        public int GradeNumber { get; set; }
        public string TeacherName { get; set; }
        public string TeacherId { get; set; }

        public IEnumerable<SelectListItem> Teachers { get; set; }

    }

    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public System.DateTime Start { get; set; }
        public Nullable<System.DateTime> End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public int GradeNumber { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int subjectTeacherId { get; set; }
        public string link { get; set; }
        public string startlink { get; set; }

    }

    public class Day
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class Time
    {
        [Key]
        public int Id { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

    public class Slot
    {
        [Key]
        public int Id { get; set; }
        public string DayName { get; set; }
        public int DayNumber { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int GradeNumber { get; set; }
        public string ThemeColor { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int subjectTeacherId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int EventId { get; set; }
    }

    public class SlotVM
    {

        public SlotVM()
        {

        }


        public SlotVM(Slot row)
        {
            Id = row.Id;
            DayName = row.DayName;
            DayNumber = row.DayNumber;
            Start = row.Start;
            End = row.End;
            GradeNumber = row.GradeNumber;
            ThemeColor = row.ThemeColor;
            TeacherId = row.TeacherId;
            TeacherName = row.TeacherName;
            subjectTeacherId = row.subjectTeacherId;
            Subject = row.Subject;
            Description = row.Description;
            EventId = row.EventId;
        }



        [Key]
        public int Id { get; set; }
        public string DayName { get; set; }
        public int DayNumber { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int GradeNumber { get; set; }
        public string ThemeColor { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int subjectTeacherId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int EventId { get; set; }


        public IEnumerable<SelectListItem> SUBJECTS { get; set; }

    }

    public class Assessment
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Teacher { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public int WeightInPercent { get; set; }
        public DateTime AssessmentDate { get; set; }
        public DateTime AssessmentEndDate { get; set; }
        public int DurationInHours { get; set; }
        public int TotalMarks { get; set; }
        public string Status { get; set; }
    }

    public class AssessmentQuestion
    {
        [Key]
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string Subject { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Marks { get; set; }
    }

    public class QuestionPosibleAnswer
    {
        [Key]
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public int QuestionId { get; set; }
        public string Subject { get; set; }
        public string Answer { get; set; }
        public string Status { get; set; }


    }


    public class QuestionAndAnswers
    {

        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int Mark { get; set; }
        public int Status { get; set; }
        public Dictionary<string, string> ProductsAndQty { get; set; }
        public int AnswerId { get; set; }
    }


    public class AssessmentEvent
    {
        [Key]
        public int EventID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public string ThemeColor { get; set; }
        public bool IsFullDay { get; set; }
        public int GradeNumber { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int subjectTeacherId { get; set; }
        public string link { get; set; }
    }


    public class StudentAssessmentSession
    {
        [Key]
        public int Id { get; set; }
        public int EventID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public int GradeNumber { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int subjectTeacherId { get; set; }

        public string link { get; set; }

        public string Question { get; set; }
        public int QuestionId { get; set; }

        public string StudentAnswer { get; set; }

        public string CorrectAnswer { get; set; }
        public int Mark { get; set; }
        public string CorrectOrWrong { get; set; }
        public string LearnerName { get; set; }

    }




    public class AssessmentSecurity
    {
        [Key]
        public int Id { get; set; }
        public int AssId { get; set; }
        public string Status { get; set; }
        public string LName { get; set; }
    }

    public class AssessmentMark
    {
        [Key]
        public string link { get; set; }
        public string LearnerName { get; set; }
        public string Statust { get; set; }
        public int Mark { get; set; }
        public int PercentOf100 { get; set; }
    }


    public class SubjectMark
    {
        [Key]
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public string SubjectLower { get; set; }
        public string LearnerName { get; set; }
        public int Grade { get; set; }
        public int YearMark { get; set; }

    }

    public class Deregister
    {
        [Key]
        public int Id { get; set; }
        public string LearnerEmail { get; set; }
        public string Status { get; set; }
        public int StatusNum { get; set; }
        public string Reason { get; set; }
        public int LearnerId { get; set; }
        public string Pemail { get; set; }

    }


    public class DelVM
    {
        public DelVM()
        {

        }

        public DelVM(Deregister model)
        {
            Id = model.Id;
            LearnerEmail = model.LearnerEmail;
            LearnerId = model.LearnerId;
            Status = model.Status;
            StatusNum = model.StatusNum;
            Reason = model.Reason;
            Pemail = model.Pemail;
        }

        [Key]
        public int Id { get; set; }
        public string LearnerEmail { get; set; }
        public int LearnerId { get; set; }
        public string Status { get; set; }
        public int StatusNum { get; set; }
        public string Reason { get; set; }
        public string Pemail { get; set; }


        public IEnumerable<SelectListItem> MyChildren { get; set; }
    }



    //////////////////SYSTEM MODELS/////////////////////
    ///



}