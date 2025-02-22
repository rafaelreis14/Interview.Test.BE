﻿using System;
using System.Collections.Generic;
using System.Linq;


namespace GraduationTracker
{
    public partial class GraduationTracker
    {
        // I think that was you guys wanted. I'm so desapointed with myself because i was so nervous and just could't devliver a clear code like that simple. :(
        public IEnumerable<Student> GetStudentsPassedMath(IEnumerable<Student> students)
        { 
            return from student in students
                    where student.Courses.Any(c => c.Name == "Math" && c.Mark >= 50)
                    select student;            

        }


        public IEnumerable<Student> GetStudentsFailedScience(IEnumerable<Student> students)
        {
            return from student in students
                   where student.Courses.Any(c => c.Name == "Science" && c.Mark < 50)
                   select student;
        }


        public ResultGraduationTraker HasGraduated(Diploma diploma, Student student)
        {
            IEnumerable<RequirementCoursesMark> requirementCoursesMark = this.GetRequirementCoursesMarkData(diploma, student);
            decimal averageMark = this.GetAvarageMark(requirementCoursesMark, student);
            int totalCredit = this.GetTotalCredit(requirementCoursesMark);
            student.Standing = this.GetStudentStanding(averageMark);

            return new ResultGraduationTraker{
                                                Graduated = this.IsGraduated(student.Standing),
                                                Standing = student.Standing,
                                                TotalCredits = totalCredit
                                              };
        }

        private IEnumerable<RequirementCoursesMark> GetRequirementCoursesMarkData(Diploma diploma, Student student)
        {
            var requirementCoursesMark = (from req in diploma.Requirements
                                          select new RequirementCoursesMark
                                          {
                                              MinimumMark = req.MinimumMark,
                                              Credits = req.Credits,
                                              CoursesMark = (from course in diploma.Requirements.SelectMany(reqCourse => reqCourse.Courses)
                                                             join courseStudent in student.Courses on course.Id equals courseStudent.Id
                                                             select new CourseMark
                                                             {
                                                                 Id = course.Id,
                                                                 Name = course.Name,
                                                                 Mark = courseStudent.Mark
                                                             }).ToArray()
                                          });

            return requirementCoursesMark;
        }

        private decimal GetAvarageMark(IEnumerable<RequirementCoursesMark> requirementCourses, Student student)
        {
            int totalMark = requirementCourses.Select(req => req.CoursesMark.Sum(course => course.Mark)).First();
            return Math.Round((decimal)totalMark / student.Courses.Length, 2);
        }

        private int GetTotalCredit(IEnumerable<RequirementCoursesMark> requirementCourses)
        {
            return requirementCourses.Where(req => req.CoursesMark.Any(course => course.Mark > req.MinimumMark)).Sum(r => r.Credits);
        }

        private Standing GetStudentStanding(decimal averageMark)
        {
            if (averageMark < 50)
                return Standing.Remedial;
            else if (averageMark < 80)
                return Standing.Average;
            else if (averageMark < 95)
                return Standing.MagnaCumLaude;
            else
                return Standing.SumaCumLaude;
        }

        private bool IsGraduated(Standing standing)
        {
            bool isGraduated = true;
            if (standing == Standing.Remedial || standing == Standing.None)
                isGraduated = false;

            return isGraduated;
        }
    }
}
