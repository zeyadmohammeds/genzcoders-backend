using genzcoders.Models;
using Microsoft.EntityFrameworkCore;

namespace genzcoders.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<EmailOtp> EmailOtps => Set<EmailOtp>();

    public DbSet<Level> Levels => Set<Level>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseRound> CourseRounds => Set<CourseRound>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<ZoomMeeting> ZoomMeetings => Set<ZoomMeeting>();
    public DbSet<Week> Weeks => Set<Week>();
    public DbSet<CourseRoundStudent> CourseRoundStudents => Set<CourseRoundStudent>();
    public DbSet<CourseInstructor> CourseInstructors => Set<CourseInstructor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.NationalId).IsUnique();
            entity.HasIndex(e => e.GoogleId).IsUnique().HasFilter("[GoogleId] IS NOT NULL");
            entity.HasIndex(e => e.PasswordResetToken).HasFilter("[PasswordResetToken] IS NOT NULL");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.NationalId).HasMaxLength(50);
            entity.Property(e => e.FullNameEn).HasColumnName("FullNameEN");
            entity.Property(e => e.FullNameAr).HasColumnName("FullNameAR");
            entity.Property(e => e.CreatedAt).HasColumnName("Created_at");
            
            entity.Property(e => e.GoogleId).HasMaxLength(100);
            entity.Property(e => e.GoogleEmail).HasMaxLength(100);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(500);
            entity.Property(e => e.TwoFactorSecret).HasMaxLength(500);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);

            entity.HasOne(d => d.Role)
                .WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Roles");

            entity.HasOne(d => d.Status)
                .WithMany(p => p.Accounts)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Status");
        });

        modelBuilder.Entity<EmailOtp>(entity =>
        {
            entity.ToTable("EmailOtp");
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.Email, e.OtpCode });
            entity.HasIndex(e => e.ExpiresAt);

            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.OtpCode).HasMaxLength(10).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.ToTable("Level");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Level)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_Level");
        });

        modelBuilder.Entity<CourseRound>(entity =>
        {
            entity.ToTable("CourseRound");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Course)
                .WithMany(p => p.CourseRounds)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRound_Course");

            entity.HasOne(d => d.MainInstructor)
                .WithMany(p => p.CourseRounds)
                .HasForeignKey(d => d.MainInstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRound_MainInstructor");

            entity.HasOne(d => d.Status)
                .WithMany(p => p.CourseRounds)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRound_Status");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ApplicationDate).HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(250);

            entity.HasOne(d => d.Course)
                .WithMany(p => p.Applications)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_Course");

            entity.HasOne(d => d.Status)
                .WithMany(p => p.Applications)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_Status");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Material");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Link).HasMaxLength(1000);
            entity.Property(e => e.MaterialType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CourseRound)
                .WithMany(p => p.Materials)
                .HasForeignKey(d => d.CourseRoundId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_Material_CourseRound");

            entity.HasOne(d => d.Instructor)
                .WithMany()
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_Instructor");
        });

        modelBuilder.Entity<ZoomMeeting>(entity =>
        {
            entity.ToTable("ZoomMeeting");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Topic).HasMaxLength(250);
            entity.Property(e => e.MeetingLink).HasMaxLength(1000);
            entity.Property(e => e.MeetingId).HasMaxLength(100);
            entity.Property(e => e.Passcode).HasMaxLength(50);
            entity.Property(e => e.MeetingDateTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CourseRound)
                .WithMany(p => p.ZoomMeetings)
                .HasForeignKey(d => d.CourseRoundId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_ZoomMeeting_CourseRound");

            entity.HasOne(d => d.Instructor)
                .WithMany()
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ZoomMeeting_Instructor");
        });

        modelBuilder.Entity<Week>(entity =>
        {
            entity.ToTable("Week");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).HasMaxLength(250).IsRequired();
            entity.Property(e => e.BusinessEntityName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");

            entity.HasIndex(e => new { e.CourseRoundId, e.BusinessEntityName });

            entity.HasOne(d => d.CourseRound)
                .WithMany(p => p.Weeks)
                .HasForeignKey(d => d.CourseRoundId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_Week_CourseRound");
        });

        modelBuilder.Entity<CourseRoundStudent>(entity =>
        {
            entity.ToTable("CourseRoundStudent");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AssignedAt).HasColumnType("datetime");
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");

            entity.HasIndex(e => new { e.CourseRoundId, e.StudentId }).IsUnique();

            entity.HasOne(d => d.CourseRound)
                .WithMany(p => p.Students)
                .HasForeignKey(d => d.CourseRoundId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_CourseRoundStudent_CourseRound");

            entity.HasOne(d => d.Student)
                .WithMany(p => p.CourseRoundStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseRoundStudent_Student");
        });

        modelBuilder.Entity<CourseInstructor>(entity =>
        {
            entity.ToTable("CourseInstructor");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AssignedAt).HasColumnType("datetime");

            entity.HasIndex(e => new { e.CourseId, e.InstructorId }).IsUnique();

            entity.HasOne(d => d.Course)
                .WithMany(p => p.CourseInstructors)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("FK_CourseInstructor_Course");

            entity.HasOne(d => d.Instructor)
                .WithMany(p => p.CourseInstructors)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseInstructor_Instructor");
        });

        base.OnModelCreating(modelBuilder);
    }
}
