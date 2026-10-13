# Learning Management System (LMS)

A full-stack Learning Management System developed using C#, ASP.NET Core MVC, Entity Framework Core, and MariaDB as part of my Computer Science coursework at the University of Utah.

## Overview

This project is a database-driven web application designed to manage university courses, class enrollment, assignments, and student grades.

The system supports three user roles—Administrators, Professors, and Students—each with different permissions and functionality.

Developed collaboratively by a two-person team, the project emphasizes backend development, relational database design, authentication, authorization, and server-side application logic.

## Technologies

- **C#** — Backend programming and application logic
- **ASP.NET Core MVC** — Model-View-Controller web architecture
- **Entity Framework Core** — Object-relational mapping and database operations
- **MariaDB / MySQL** — Relational database management
- **LINQ** — Database querying and data manipulation
- **ASP.NET Core Identity** — User authentication and role-based authorization
- **HTML / CSS / Razor Views** — Web interface development
- **Git** — Version control and collaboration

## Features

### Role-Based Authentication and Authorization

Implemented three user roles with different access permissions:

- **Administrators:** Manage academic information, courses, and class offerings.
- **Professors:** Manage class assignments and student grades.
- **Students:** Enroll in classes, view assignments, submit coursework, and monitor grades.

ASP.NET Core Identity manages user accounts, authentication, and role-based access control.

### Course and Class Management

- Organizes courses by subject, course number, and title.
- Supports class offerings associated with academic semesters and years.
- Associates class sections with professors, meeting times, and locations.
- Maintains relationships between courses, classes, students, and enrollments.

### Student Enrollment and Scheduling

- Allows students to enroll in available classes.
- Includes scheduling logic to detect potential conflicts between class meeting times.
- Maintains enrollment records linking students to their classes.

### Assignment and Grade Management

- Supports assignment categories and coursework organization.
- Tracks student assignment submissions and grades.
- Implements grade calculations, including weighted grading and GPA-related functionality.
- Connects grading information to student enrollment records.

## Software Architecture

### Model-View-Controller (MVC)

The application follows the MVC architectural pattern to separate responsibilities:

- **Models:** Represent academic entities and database relationships.
- **Views:** Render user interfaces using Razor and HTML.
- **Controllers:** Process HTTP requests, coordinate application logic, and interact with the database.

This separation makes the application easier to maintain, debug, and extend.

### Database Design

The application uses Entity Framework Core with a relational database backend.

Important entities include:

- **Course:** Stores course identification and descriptive information.
- **Class:** Represents a specific course offering, including semester, year, location, meeting time, and professor.
- **EnrollmentGrade:** Associates students with classes and tracks enrollment-related grading information.
- **Assignment Categories:** Organize graded coursework within a class.
- **User Accounts:** Represent authenticated users and their assigned roles.

The database uses primary keys, foreign keys, and entity relationships to organize academic information and maintain data consistency.

### Database Contexts

The application separates authentication-related data from academic application data using Entity Framework Core database contexts.

This supports a distinction between user identity management and the academic information managed by the LMS.

## Technical Highlights

### Entity Framework Core and LINQ

Used Entity Framework Core to interact with relational data through C# objects.

LINQ queries support operations such as retrieving course information, accessing enrollment records, and working with related academic entities.

### Authentication and Authorization

Integrated ASP.NET Core Identity to manage authentication and restrict application functionality according to user roles.

### Scheduling and Business Logic

Implemented application logic for class scheduling, enrollment management, and academic record processing.

### Debugging and Testing

Used debugging and application logging to investigate backend behavior, database interactions, and request processing.

## Skills Demonstrated

- C# and ASP.NET Core Development
- Backend Web Development
- MVC Architecture
- Relational Database Design
- Entity Framework Core and LINQ
- Authentication and Authorization
- Database Relationships and Data Modeling
- Business Logic Implementation
- Team Collaboration and Version Control

## Project Background

Developed as a two-person collaborative project for Computer Science coursework at the University of Utah.

Both team members contributed to the development and integration of the application.

The project provided practical experience designing a database-backed web application with multiple user roles, complex entity relationships, and academic business logic.

## Future Improvements

- Strengthen automated testing of grading and enrollment functionality.
- Improve validation and error handling for academic operations.
- Expand authorization testing to ensure users can only access permitted records.
- Improve the responsiveness and usability of the interface.

## Author

Sinahi Leon and Kalina Tran
B.S. Computer Science — University of Utah

Developed collaboratively with a project teammate.
