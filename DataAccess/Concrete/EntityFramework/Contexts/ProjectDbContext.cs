using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Concrete;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected ProjectDbContext(DbContextOptions options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GroupClaim> GroupClaims { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<MobileLogin> MobileLogins { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Translate> Translates { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }

        // School / Multi-Tenant Entities
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<TenantUser> TenantUsers { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<StudentBranch> StudentBranches { get; set; }
        public DbSet<TeacherBranch> TeacherBranches { get; set; }

        // Private Course & Fee Entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<FeeDue> FeeDues { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected IConfiguration Configuration { get; }

        public override int SaveChanges()
        {
            SetAuditAndTenantFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditAndTenantFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditAndTenantFields()
        {
            var userId = UserInfoExtensions.GetUserIdOrZero();
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            var now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (!baseEntity.CreatedDate.HasValue)
                            baseEntity.CreatedDate = now;

                        if (!baseEntity.CreatedBy.HasValue && userId > 0)
                            baseEntity.CreatedBy = userId;

                        if (!baseEntity.IsActive.HasValue)
                            baseEntity.IsActive = true;

                        if (!baseEntity.IsDeleted.HasValue)
                            baseEntity.IsDeleted = false;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        baseEntity.UpdatedDate = now;
                        if (userId > 0)
                            baseEntity.UpdatedBy = userId;
                    }
                }

                if (entry.Entity is TenantEntity tenantEntity)
                {
                    if (entry.State == EntityState.Added && tenantEntity.TenantId <= 0 && tenantId > 0)
                    {
                        tenantEntity.TenantId = tenantId;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<StudentParent>()
                .HasKey(sp => new { sp.StudentId, sp.ParentId });

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Student)
                .WithMany(s => s.Parents)
                .HasForeignKey(sp => sp.StudentId);

            modelBuilder.Entity<StudentParent>()
                .HasOne(sp => sp.Parent)
                .WithMany(p => p.Students)
                .HasForeignKey(sp => sp.ParentId);

            modelBuilder.Entity<StudentBranch>()
                .HasKey(sb => new { sb.StudentId, sb.BranchId });

            modelBuilder.Entity<StudentBranch>()
                .HasOne(sb => sb.Student)
                .WithMany(s => s.StudentBranches)
                .HasForeignKey(sb => sb.StudentId);

            modelBuilder.Entity<StudentBranch>()
                .HasOne(sb => sb.Branch)
                .WithMany(b => b.StudentBranches)
                .HasForeignKey(sb => sb.BranchId);

            modelBuilder.Entity<TeacherBranch>()
                .HasKey(tb => new { tb.TeacherId, tb.BranchId });

            modelBuilder.Entity<TeacherBranch>()
                .HasOne(tb => tb.Teacher)
                .WithMany(t => t.TeacherBranches)
                .HasForeignKey(tb => tb.TeacherId);

            modelBuilder.Entity<TeacherBranch>()
                .HasOne(tb => tb.Branch)
                .WithMany(b => b.TeacherBranches)
                .HasForeignKey(tb => tb.BranchId);

            // Course & Fee Configurations
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(ce => ce.Student)
                .WithMany()
                .HasForeignKey(ce => ce.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(ce => ce.Course)
                .WithMany(c => c.CourseEnrollments)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FeeDue>()
                .HasOne(fd => fd.CourseEnrollment)
                .WithMany(ce => ce.FeeDues)
                .HasForeignKey(fd => fd.CourseEnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FeeDue>()
                .HasOne(fd => fd.Student)
                .WithMany()
                .HasForeignKey(fd => fd.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.FeeDue)
                .WithMany(fd => fd.Payments)
                .HasForeignKey(p => p.FeeDueId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            if (!optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DArchPgContext")));
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    optionsBuilder.EnableSensitiveDataLogging();
                }
            }
        }
    }
}
