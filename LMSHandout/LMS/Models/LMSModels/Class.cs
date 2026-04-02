using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            EnrollmentGrades = new HashSet<EnrollmentGrade>();
        }

        public uint Year { get; set; }
        public string Semester { get; set; } = null!;
        public string Location { get; set; } = null!;
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
        public uint CourseId { get; set; }
        public uint ClassId { get; set; }
        public string ProfessorUid { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;
        public virtual Professor ProfessorU { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<EnrollmentGrade> EnrollmentGrades { get; set; }
    }
}
