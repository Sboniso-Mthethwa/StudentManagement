using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "ID Number is required")]
    [StringLength(13, MinimumLength = 13, ErrorMessage = "ID Number must be exactly 13 digits.")]
    [RegularExpression(@"^\d{13}$", ErrorMessage = "ID Number must be 13 numeric digits.")]
    [Display(Name = "ID Number")]
    public string IdNumber { get; set; } // New property for ID number


    [Required(ErrorMessage = "Name is required")]
    [Display(Name = "Student Name")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email Address")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; }
    [Display(Name = "Selected Stream")]
    public string SelectedStream { get; set; } // Property for selected stream

    // New property to capture selected courses (this won't be saved in the database, just used for form data)
    public int[] SelectedCourses { get; set; }

    public virtual ICollection<Mark> Marks { get; set; }
}

public class Stream
{
    public int StreamId { get; set; } // Unique ID for the stream
    public string StreamName { get; set; } // Name of the stream (e.g., General, Physics, Accounting)

    // Navigation property to Courses
    public virtual ICollection<Course> Courses { get; set; } // Each stream can have multiple courses
}


public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Subject Name is required")]
    [Display(Name = "Subject Name")]
    [StringLength(100, ErrorMessage = "Course Name cannot be longer than 100 characters.")]
    public string CourseName { get; set; }

    [Required(ErrorMessage = "Credits are required")]
    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
    [Display(Name = "Subject Level")]
    public int Credits { get; set; }

    public virtual ICollection<Mark> Marks { get; set; }
}



public class Mark
{
    [Key]
    public int MarkId { get; set; }

    [Required(ErrorMessage = "Student is required")]
    [DisplayName("Student Name")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    [DisplayName("Subject")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Assignment 1 score is required")]
    [Range(0, 100, ErrorMessage = "Assignment 1 score must be between 0 and 100")]
    [DisplayName("Assignment 1 Marks")]
    public int Assignment1 { get; set; }


    [Required(ErrorMessage = "Total score is required")]
    [Range(0, 100, ErrorMessage = "Total score must be between 0 and 100")]
    [DisplayName("Test Marks")]
    public int Score { get; set; }

    public virtual Student Student { get; set; }
    public virtual Course Course { get; set; }
}
public class CoursePassFailViewModel
{
    public string CourseName { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
}

