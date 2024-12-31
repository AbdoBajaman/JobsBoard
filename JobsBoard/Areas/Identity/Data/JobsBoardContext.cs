using JobsBoard.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JobsBoard.Models;

namespace JobsBoard.Data;

public class JobsBoardContext : IdentityDbContext<JobsBoardUser>
{
    public JobsBoardContext(DbContextOptions<JobsBoardContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Define composite key for IdentityUserRole
        builder.Entity<IdentityUserRole<string>>()
            .HasKey(r => new { r.UserId, r.RoleId });

        // Configure the relationship between IdentityUserRole and Role
        builder.Entity<IdentityUserRole<string>>()
            .HasOne<IdentityRole>()
            .WithMany()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between IdentityUserRole and User
        builder.Entity<IdentityUserRole<string>>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<Job>()
           .HasOne(j => j.Category) // Navigation property in Job
           .WithMany(c => c.Jobs)   // Navigation property in Category
           .HasForeignKey(j => j.CategoryId) // Foreign key in Job
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplyForJob>()
        .HasOne(a => a.Job)
        .WithMany(j => j.ApplyForJobs) // A job can have many applications
        .HasForeignKey(a => a.JobId)
        .OnDelete(DeleteBehavior.Cascade); // Delete applications when the job is deleted

        builder.Entity<ApplyForJob>()
            .HasOne(a => a.User)
            .WithMany(u => u.ApplyForJobs) // A user can apply for many jobs
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Prevent cascading conflicts

    }


    public DbSet<Category>? Category { get; set; }

    public DbSet<JobsBoard.Models.Job>? Job { get; set; }

    public DbSet<JobsBoard.Models.ApplyForJob>? ApplyForJob { get; set; }


}
