using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private LMSContext db;
        public StudentController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from eg in db.EnrollmentGrades
                        where eg.UId == uid
                        join ca in db.Classes
                        on eg.ClassId equals ca.ClassId
                        join c in db.Courses
                        on ca.CourseId equals c.CourseId
                        select new
                        {
                            subject = c.Subject,
                            number = c.Number,
                            name = c.Name,
                            season = ca.Semester,
                            year = ca.Year,
                            grade = eg.Grade ?? "--"
                        };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            var query = from c in db.Courses
                        where c.Subject == subject && c.Number == num
                        join ca in db.Classes
                        on c.CourseId equals ca.CourseId
                        where ca.Semester == season && ca.Year == (uint)year
                        join ac in db.AssignmentCategories
                        on ca.ClassId equals ac.ClassId
                        join a in db.Assignments
                        on ac.CategoryId equals a.CategoryId
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.Due,
                            score = (from s in db.Submissions
                                     where s.AssignmentId == a.AssignmentId && s.UId == uid
                                     select s.Score).FirstOrDefault()
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            var assignment = (from c in db.Courses
                         where c.Subject == subject && c.Number == num
                         join ca in db.Classes
                         on c.CourseId equals ca.CourseId
                         where ca.Semester == season && ca.Year == year
                         join ac in db.AssignmentCategories
                         on ca.ClassId equals ac.ClassId
                         where ac.Name == category
                         join a in db.Assignments
                         on ac.CategoryId equals a.CategoryId
                         where a.Name == asgname
                         select a).FirstOrDefault();


            if (assignment == null)
            {
                return Json(new { success = false });
            }
            var updatedSubmission = db.Submissions.FirstOrDefault(s => s.AssignmentId == assignment.AssignmentId && s.UId == uid);

            if (updatedSubmission == null)
            {
                updatedSubmission = new Submission
                {
                    AssignmentId = assignment.AssignmentId,
                    UId = uid,
                    SubmissionContents = contents,
                    Time = DateTime.Now,
                    Score = 0,
                };
            
                db.Submissions.Add(updatedSubmission);
                db.SaveChanges();
                return Json(new { success = true });

            }
            else if( updatedSubmission != null)
            {
                updatedSubmission.SubmissionContents = contents;
                updatedSubmission.Time = DateTime.Now;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            var query = from c in db.Courses
                        where c.Subject == subject && c.Number == num
                        join ca in db.Classes
                        on c.CourseId equals ca.CourseId
                        where ca.Semester == season && ca.Year == year
                        join eg in db.EnrollmentGrades 
                        on ca.ClassId equals eg.ClassId into enrollments

                        from eg in enrollments.DefaultIfEmpty() // Left join to include classes with no enrollments
                        where eg == null || eg.UId == uid
                        select new
                        {
                            ClassId = ca.ClassId,   // need classID to enroll 
                            Enrollment = eg
                        };
           
            if (query.Any(x => x.Enrollment != null && x.Enrollment.UId == uid))
            {
                return Json(new { success = false });
            }
            else
            {
                var classQuery = query.FirstOrDefault();
                if (classQuery != null)
                {
                    var newEnrollmentGrade = new EnrollmentGrade
                    {
                        UId = uid,
                        ClassId = classQuery.ClassId,
                        Grade = "--"
                    };

                    db.EnrollmentGrades.Add(newEnrollmentGrade);
                    db.SaveChanges();
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
        }

        // Helper method to calculate a student's grade in a class
        public void CalculateGrade(uint classId, string uid)
        {
            var studentEnrollment = (from eg in db.EnrollmentGrades
                                     where eg.ClassId == classId && eg.UId == uid
                                     select eg).FirstOrDefault();

            if (studentEnrollment == null)
            {
                return; // Student is not enrolled in this class
            }

            // Get all categories from this class
            var categories = (from ac in db.AssignmentCategories
                             where ac.ClassId == classId
                             select ac).ToList(); // ADD .ToList() Here

            double weighted = 0;
            double scorePercentage = 0;
            double totalCatWeight = 0;
            double totalWeightedScore = 0;
            // Iterate through categories, getting each assignment 
            foreach (var category in categories)
            {
                // Get all assingments in this category
                var assignments = (from a in db.Assignments
                                    where a.CategoryId == category.CategoryId
                                    select a).ToList();  // ADD .ToList() Here

                if (!assignments.Any())
                {
                    continue; // No assignments in this category, skip 
                }
                else
                {
                    double totalPoints = 0.0;
                    double maxPoints = 0.0;

                    foreach (var assignment in assignments)
                    {
                        //  Points earned / total points for assignment
                        var submission = (from s in db.Submissions
                                          where s.AssignmentId == assignment.AssignmentId && s.UId == uid
                                          select s).FirstOrDefault();
             

                        if (submission != null)
                        {
                            totalPoints += submission.Score;
                            maxPoints += assignment.MaxPoints;
                        }

                        if (totalPoints == 0)
                        {
                            continue; // Skip to avoid divide +by zero
                        }

                    }
                    scorePercentage = totalPoints / maxPoints; // total for all assignments
                    weighted = scorePercentage * category.Weight; // weighted total for all assignments
                    
                }
                totalCatWeight += category.Weight; // weighted totals for all categories
                
            }
            totalWeightedScore += weighted;
            var grade = totalWeightedScore * (100 / totalCatWeight);
            if (grade >= 0.93 && grade < 1.0)
            {
                studentEnrollment.Grade = "A";
            }
            else if (grade >= .9 && grade < .93)
            {
                studentEnrollment.Grade = "A-";
            }
            else if (grade >= .87 && grade < .9)
            {
                studentEnrollment.Grade = "B+";
            }
            else if (grade >= .83 && grade < .87)
            {
                studentEnrollment.Grade = "B";
            }
            else if (grade >= .8 && grade < .83)
            {
                studentEnrollment.Grade = "B-";
            }
            else if (grade >= .77 && grade < .8)
            {
                studentEnrollment.Grade = "C+";
            }
            else if (grade >= .73 && grade < .77)
            {
                studentEnrollment.Grade = "C";
            }
            else if (grade >= .7 && grade < .73)
            {
                studentEnrollment.Grade = "C-";
            }
            else if (grade >= .67 && grade < .7)
            {
                studentEnrollment.Grade = "D+";
            }
            else if (grade >= .63 && grade < .67)
            {
                studentEnrollment.Grade = "D";
            }
            else if (grade >= .6 && grade < .63)
            {
                studentEnrollment.Grade = "D-";
            }
            else
            {
                studentEnrollment.Grade = "E";
            }
            db.SaveChanges();
        }

        //public void UpdateGrade(string uid)
        //{
        //    var classIDs = (from eg in db.EnrollmentGrades
        //                    where eg.UId == uid
        //                    select eg.ClassId).ToList();  // ADD .ToList() Here to fix the connection error

        //    foreach (var classId in classIDs)
        //    {
        //        // Pass db down to CalculateGrade
        //        CalculateGrade(classId, uid, db);
        //    }
        //}

        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            // Get all classes from student not "--"
            // Get avergae grade point value
            // for every assingment in 
            var classIDs = (from eg in db.EnrollmentGrades
                        where eg.UId == uid
                        select eg.ClassId).ToList();  // ADD .ToList() Here to fix the connection error

            foreach ( var classId in classIDs)
            {
                CalculateGrade(classId, uid);
            }

            var grades = (from eg in db.EnrollmentGrades
                        where eg.UId == uid && eg.Grade != "--"
                        select eg.Grade).ToList();    // Added .ToList() here for performance

            if (!grades.Any())
            {
                return Json(new { GPA = 0.0 });
            }

            double gradePoints = 0.0;
            foreach (var grade in grades)
            {
                // Convert letter grade to grade points and calculate GPA
                if (grade == "A")
                {
                    gradePoints += 4.0;
                }
                else if (grade == "A-")
                {
                    gradePoints += 3.7;
                }
                else if (grade == "B+")
                {
                    gradePoints += 3.3;
                }
                else if (grade == "B")
                {
                    gradePoints += 3.0;
                }
                else if (grade == "B-")
                {
                    gradePoints += 2.7;
                }
                else if (grade == "C+")
                {
                    gradePoints += 2.3;
                }
                else if (grade == "C")
                {
                    gradePoints += 2.0;
                }
                else if (grade == "C-")
                {
                    gradePoints += 1.7;
                }
                else if (grade == "D+")
                {
                    gradePoints += 1.3;
                }
                else if (grade == "D")
                {
                    gradePoints += 1.0;
                }
                else if (grade == "D-")
                {
                    gradePoints += 0.7;
                }
                else
                {
                    gradePoints += 0.0;
                }
            }
            double gpa = gradePoints / grades.Count; // Changed Count() to Count since it's a List now
            return Json(new { GPA = gpa });
        }
                
        /*******End code to modify********/

    }
}

