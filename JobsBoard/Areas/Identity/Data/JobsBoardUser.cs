using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using JobsBoard.Models;
using Microsoft.AspNetCore.Identity;

namespace JobsBoard.Areas.Identity.Data;

// Add profile data for application users by adding properties to the JobsBoardUser class
public class JobsBoardUser : IdentityUser
{


    [DisplayName("ألاسم الاول")]
    public string FirstName { get; set; }


    [DisplayName("الاسم الاخير")]
    public string LastName { get; set; }


    public string UserType { get; set; }

    //[DisplayName("نبذه عني")]
    //public string? about_user { get; set; }


    //[DisplayName("التعليم")]

    //public string Education { get; set; }


    //public string Experiences { get; set; }

    public virtual ICollection<Job> Jobs { get; set; }

    public virtual ICollection<ApplyForJob> ApplyForJobs { get; set; }

}

