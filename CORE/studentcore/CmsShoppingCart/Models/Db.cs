using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CmsShoppingCart.Models.Data
{
    public class Db:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<ChildReg> ChildRegs { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Gradesubject> Gradesubjects { get; set; }
        public DbSet<Subjectteacher> Subjectteacher { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Time> Time { get; set; }
        public DbSet<Classteacher> Classteachers { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentQuestion> AssessmentQuestions { get; set; }
        public DbSet<QuestionPosibleAnswer> QuestionPosibleAnswers { get; set; }
        public DbSet<AssessmentEvent> AssessmentEvents { get; set; }
        public DbSet<StudentAssessmentSession> StudentAssessmentSessions { get; set; }
        public DbSet<AssessmentSecurity> AssessmentSecurities { get; set; }
        public DbSet<SubjectMark> SubjectMarks { get; set; }
        public DbSet<Deregister> Deregisters { get; set; }





        public System.Data.Entity.DbSet<CmsShoppingCart.Models.Data.Quotation> Quotations { get; set; }

        public System.Data.Entity.DbSet<CmsShoppingCart.Models.Data.AssessmentMark> AssessmentMarks { get; set; }

        public System.Data.Entity.DbSet<CmsShoppingCart.Models.Data.DelVM> DelVMs { get; set; }
    }
}