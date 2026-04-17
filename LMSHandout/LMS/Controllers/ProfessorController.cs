using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LMS.Controllers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {

        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from c in db.Courses
                        where c.Subject == subject && c.Number == num
                        join ca in db.Classes
                        on c.CourseId equals ca.CourseId
                        where ca.Semester == season && ca.Year == year
                        join eg in db.EnrollmentGrades
                        on ca.ClassId equals eg.ClassId
                        join s in db.Students
                        on eg.UId equals s.UId
                        select new
                        {
                            fname = s.FirstName,
                            lname = s.LastName,
                            uid = s.UId,
                            dob = s.Dob,
                            grade = eg.Grade,
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if(category != null)
            {
                var query = from c in db.Courses
                            where c.Subject == subject && c.Number == num
                            join ca in db.Classes
                            on c.CourseId equals ca.CourseId
                            where ca.Semester == season && ca.Year == year
                            join ac in db.AssignmentCategories
                            on ca.ClassId equals ac.ClassId
                            where ac.Name == category
                            join a in db.Assignments
                            on ac.CategoryId equals a.CategoryId
                            select new
                            {
                                aname = a.Name,
                                cname = ac.Name,
                                due = a.Due,
                                submissions = (from sub in db.Submissions
                                              where sub.AssignmentId == a.AssignmentId
                                              select sub).Count()
                            };
                return Json(query.ToArray());

            }
            else
            {
                var query = from c in db.Courses
                            where c.Subject == subject && c.Number == num
                            join ca in db.Classes
                            on c.CourseId equals ca.CourseId
                            where ca.Semester == season && ca.Year == year
                            join ac in db.AssignmentCategories
                            on ca.ClassId equals ac.ClassId
                            join a in db.Assignments
                            on ac.CategoryId equals a.CategoryId
                            select new
                            {
                                aname = a.Name,
                                cname = ac.Name,
                                due = a.Due,
                                submissions = (from sub in db.Submissions
                                               where sub.AssignmentId == a.AssignmentId
                                               select sub).Count()
                            };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from c in db.Courses
                        where c.Subject == subject && c.Number == num
                        join ca in db.Classes
                        on c.CourseId equals ca.CourseId
                        where ca.Semester == season && ca.Year == year
                        join ac in db.AssignmentCategories
                        on ca.ClassId equals ac.ClassId
                        select new
                        {
                            name = ac.Name,
                            weight = ac.Weight,
                        };
            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {

            var classID = (from c in db.Courses
                           where c.Subject == subject && c.Number == num
                           join ca in db.Classes
                           on c.CourseId equals ca.CourseId
                           where ca.Semester == season && ca.Year == (uint)year
                           select ca.ClassId).FirstOrDefault();

            if (!db.AssignmentCategories.Any(c => (c.Name == category && c.ClassId == classID)))
            {
                var newCategory = new AssignmentCategory
                {
                    Name = category,
                    Weight = (short)catweight,
                    ClassId = classID
                };
                
                db.AssignmentCategories.Add(newCategory);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
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

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {       
            var newAssignment = new Assignment
                {
                    Name = asgname,
                    Contents = asgcontents,
                    Due = asgdue,
                    MaxPoints = (short)asgpoints,

                    CategoryId = (from c in db.Courses
                                  where c.Subject == subject && c.Number == num
                                  join ca in db.Classes
                                  on c.CourseId equals ca.CourseId
                                  where ca.Semester == season && ca.Year == year
                                  join ac in db.AssignmentCategories
                                  on ca.ClassId equals ac.ClassId
                                  where ac.Name == category 
                                  select ac.CategoryId).FirstOrDefault()
                };

            var studentQuery = (from c in db.Courses
                               where c.Subject == subject && c.Number == num
                               join ca in db.Classes
                               on c.CourseId equals ca.CourseId
                               where ca.Semester == season && ca.Year == year
                               join eg in db.EnrollmentGrades
                               on ca.ClassId equals eg.ClassId
                               join s in db.Students
                               on eg.UId equals s.UId
                               select s.UId).ToList();

            foreach (var uid in studentQuery)
            {
                var classIDs = (from eg in db.EnrollmentGrades
                                where eg.UId == uid
                                select eg.ClassId).ToList();  // ADD .ToList() Here to fix the connection error

                foreach (var classId in classIDs)
                {
                    // Pass db down to CalculateGrade
                    var cIDs = (from eg in db.EnrollmentGrades
                                where eg.UId == uid
                                select eg.ClassId).ToList();  // ADD .ToList() Here to fix the connection error

                    foreach (var cId in classIDs)
                    {
                        // Pass db down to CalculateGrade
                        CalculateGrade(cId, uid);
                    }
                }
            }

            db.Assignments.Add(newAssignment);
                db.SaveChanges();
                return Json(new { success = true });
         }
   
        


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query = from c in db.Courses
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
                        join s in db.Submissions
                        on a.AssignmentId equals s.AssignmentId
                        join st in db.Students
                        on s.UId equals st.UId
                        select new
                        {
                            fname = st.FirstName,
                            lname = st.LastName,
                            uid = st.UId,
                            time = s.Time,
                            score = s.Score
                        };
            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var query = from c in db.Courses
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
                        join s in db.Submissions
                        on a.AssignmentId equals s.AssignmentId
                        where s.UId == uid
                        select new { Submission = s, ClassId = ca.ClassId };

            var result = query.FirstOrDefault();
            if (result == null)
            {
                return Json(new { success = false });
            }

            result.Submission.Score = (short)score;
            db.SaveChanges(); 

            // Use the queried ClassId to update the overall grade
            CalculateGrade(result.ClassId, uid);

            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {

            var query = from p in db.Professors
                        where p.UId == uid
                        join ca in db.Classes
                        on p.UId equals ca.ProfessorUid
                        join c in db.Courses
                        on ca.CourseId equals c.CourseId
                        select new
                        {
                            subject = c.Subject,
                            number = c.Number,
                            name = c.Name,
                            season = ca.Semester,
                            year = ca.Year
                        };
            return Json(query.ToArray());
        }


        
        /*******End code to modify********/
    }
}

