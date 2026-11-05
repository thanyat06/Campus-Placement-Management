
using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<College> Colleges { get; set; }
    public DbSet<PlacementOfficer> PlacementOfficers { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyHR> CompanyHRs { get; set; }
    public DbSet<JobOpening> JobOpenings { get; set; }
    public DbSet<JobTargetCollege> JobTargetColleges { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<RecruitmentDrive> RecruitmentDrives { get; set; }
    public DbSet<InterviewRound> InterviewRounds { get; set; }
    public DbSet<InterviewSchedule> InterviewSchedules { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // College
        builder.Entity<College>()
            .HasIndex(c => c.CollegeCode)
            .IsUnique();

        // Placement Officer
        builder.Entity<PlacementOfficer>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        builder.Entity<PlacementOfficer>()
            .HasOne(p => p.User)
            .WithOne(u => u.PlacementOfficer)
            .HasForeignKey<PlacementOfficer>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PlacementOfficer>()
            .HasOne(p => p.ApprovedByUser)
            .WithMany()
            .HasForeignKey(p => p.ApprovedBy)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<PlacementOfficer>()
            .HasOne(p => p.College)
            .WithMany(c => c.PlacementOfficers)
            .HasForeignKey(p => p.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Student
        builder.Entity<Student>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        builder.Entity<Student>()
            .HasIndex(s => new { s.CollegeId, s.RegisterNumber })
            .IsUnique();

        builder.Entity<Student>()
            .Property(s => s.CGPA)
            .HasPrecision(4, 2);

        builder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Student>()
            .HasOne(s => s.College)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Student>()
            .HasOne(s => s.PlacementOfficer)
            .WithMany(p => p.Students)
            .HasForeignKey(s => s.PlacementOfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Company HR
        builder.Entity<CompanyHR>()
            .HasIndex(h => h.UserId)
            .IsUnique();

        builder.Entity<CompanyHR>()
            .HasOne(h => h.User)
            .WithOne(u => u.CompanyHR)
            .HasForeignKey<CompanyHR>(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CompanyHR>()
            .HasOne(h => h.Company)
            .WithMany(c => c.CompanyHRs)
            .HasForeignKey(h => h.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Job Opening
        builder.Entity<JobOpening>()
            .Property(j => j.SalaryPackage)
            .HasPrecision(12, 2);

        builder.Entity<JobOpening>()
            .Property(j => j.MinimumCGPA)
            .HasPrecision(4, 2);

        builder.Entity<JobOpening>()
            .HasOne(j => j.Company)
            .WithMany(c => c.JobOpenings)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobOpening>()
            .HasOne(j => j.CreatedByHR)
            .WithMany(h => h.JobOpenings)
            .HasForeignKey(j => j.CreatedByHRId)
            .OnDelete(DeleteBehavior.Restrict);

        // Job Target College
        builder.Entity<JobTargetCollege>()
            .HasIndex(j => new { j.JobId, j.CollegeId })
            .IsUnique();

        builder.Entity<JobTargetCollege>()
            .HasOne(j => j.JobOpening)
            .WithMany(j => j.TargetColleges)
            .HasForeignKey(j => j.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobTargetCollege>()
            .HasOne(j => j.College)
            .WithMany()
            .HasForeignKey(j => j.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Job Application
        builder.Entity<JobApplication>()
            .HasIndex(a => new { a.StudentId, a.JobId })
            .IsUnique();

        builder.Entity<JobApplication>()
            .HasOne(a => a.JobOpening)
            .WithMany(j => j.JobApplications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<JobApplication>()
            .HasOne(a => a.Student)
            .WithMany(s => s.JobApplications)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Recruitment Drive
        builder.Entity<RecruitmentDrive>()
            .HasOne(r => r.JobOpening)
            .WithMany(j => j.RecruitmentDrives)
            .HasForeignKey(r => r.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RecruitmentDrive>()
            .HasOne(r => r.College)
            .WithMany()
            .HasForeignKey(r => r.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Interview Round
        builder.Entity<InterviewRound>()
            .HasOne(i => i.JobOpening)
            .WithMany(j => j.InterviewRounds)
            .HasForeignKey(i => i.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // Interview Schedule
        builder.Entity<InterviewSchedule>()
            .HasOne(i => i.JobApplication)
            .WithMany(a => a.InterviewSchedules)
            .HasForeignKey(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InterviewSchedule>()
            .HasOne(i => i.InterviewRound)
            .WithMany(r => r.InterviewSchedules)
            .HasForeignKey(i => i.InterviewRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        // Application Status History
        builder.Entity<ApplicationStatusHistory>()
            .HasOne(h => h.JobApplication)
            .WithMany(a => a.StatusHistory)
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationStatusHistory>()
            .HasOne(h => h.UpdatedByUser)
            .WithMany()
            .HasForeignKey(h => h.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
