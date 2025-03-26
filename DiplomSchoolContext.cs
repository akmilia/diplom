using diplom.Models;
using Microsoft.EntityFrameworkCore;

namespace diplom;

public partial class DiplomSchoolContext : DbContext
{
    public DiplomSchoolContext()
    {
    }

    public DiplomSchoolContext(DbContextOptions<DiplomSchoolContext> options)
        : base(options)
    {
    }
    public DbSet<subjectsshow> SubjectShowItems { get; set; }
    public DbSet<usersshow> userShowItems { get; set; }
    public DbSet<scheduleshow> schedulesShow { get; set; }

    public DbSet<schedule_with_attendance> attendanceShow { get; set; }
    
    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<BilNebil> BilNebils { get; set; }

    public virtual DbSet<Cabinet> Cabinets { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupsUser> GroupsUsers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<diplom.Models.Type> Types { get; set; }

    public virtual DbSet<types_subjects> TypesSubjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=diplom_school;UserId=postgres;Password=2006;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Idattendance).HasName("attendance_pkey");

            entity.ToTable("attendance");

            entity.Property(e => e.Idattendance).HasColumnName("idattendance");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Idschedule).HasColumnName("idschedule");

            entity.HasOne(d => d.IdscheduleNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.Idschedule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("attendance_schedule_idschedule");
        });

        modelBuilder.Entity<scheduleshow>().ToView("scheduleshow");
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<schedule_with_attendance>().ToView("schedule_with_attendance");
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BilNebil>(entity =>
        {
            entity.HasKey(e => new { e.Idattendance, e.Iduser }).HasName("bil_nebil_pkey");

            entity.ToTable("bil_nebil");

            entity.Property(e => e.Idattendance).HasColumnName("idattendance");
            entity.Property(e => e.Iduser).HasColumnName("iduser");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdattendanceNavigation).WithMany(p => p.BilNebils)
                .HasForeignKey(d => d.Idattendance)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bil_attendance_idattendance");
        });

        modelBuilder.Entity<Cabinet>(entity =>
        {
            entity.HasKey(e => e.Idcabinet).HasName("cabinets_pkey");

            entity.ToTable("cabinets");

            entity.Property(e => e.Idcabinet).HasColumnName("idcabinet");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Idgroups).HasName("groups_pkey");

            entity.ToTable("groups");

            entity.Property(e => e.Idgroups).HasColumnName("idgroups");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<GroupsUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("groups_users");

            entity.Property(e => e.GroupsIdgroups)
                .ValueGeneratedOnAdd()
                .HasColumnName("groups_idgroups");
            entity.Property(e => e.UsersIdusers)
                .ValueGeneratedOnAdd()
                .HasColumnName("users_idusers");

            entity.HasOne(d => d.GroupsIdgroupsNavigation).WithMany()
                .HasForeignKey(d => d.GroupsIdgroups)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("groups_users_groups_idgroups_fkey");

            entity.HasOne(d => d.UsersIdusersNavigation).WithMany()
                .HasPrincipalKey(p => p.Idusers)
                .HasForeignKey(d => d.UsersIdusers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("groups_users_users_idusers_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Idroles).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Idroles).HasColumnName("idroles");
            entity.Property(e => e.Name)
                .HasMaxLength(25)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Idschedule).HasName("schedule_pk");

            entity.ToTable("schedule");

            entity.Property(e => e.Idschedule)
                .ValueGeneratedNever()
                .HasColumnName("idschedule");
            entity.Property(e => e.CabinetsIdcabinet).HasColumnName("cabinets_idcabinet");
            entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");
            entity.Property(e => e.GroupsIdgroup).HasColumnName("groups_idgroup");
            entity.Property(e => e.SubjectsIdsubjects).HasColumnName("subjects_idsubjects");
            entity.Property(e => e.Time)
                .HasColumnType("time with time zone")
                .HasColumnName("time");
            entity.Property(e => e.UsersIdusers).HasColumnName("users_idusers");

            entity.HasOne(d => d.CabinetsIdcabinetNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CabinetsIdcabinet)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_cabinets_idcabinet_fkey");

            entity.HasOne(d => d.GroupsIdgroupNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.GroupsIdgroup)
                .HasConstraintName("schedule_group_idgroup_fkey");

            entity.HasOne(d => d.SubjectsIdsubjectsNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.SubjectsIdsubjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_subjects_idsubjects_fkey");

            entity.HasOne(d => d.UsersIdusersNavigation).WithMany(p => p.Schedules)
                .HasPrincipalKey(p => p.Idusers)
                .HasForeignKey(d => d.UsersIdusers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("schedule_users_idusers_fkey");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Idsubjects).HasName("subjects_pkey");

            entity.ToTable("subjects");

            entity.Property(e => e.Idsubjects).HasColumnName("idsubjects");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<diplom.Models.Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("types_pkey");

            entity.ToTable("types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type1)
                .HasMaxLength(45)
                .HasColumnName("type");
        });

        modelBuilder.Entity<types_subjects>()
             .HasKey(ts => new { ts.types_id, ts.subjects_idsubjects });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => new { e.Idusers}).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Idusers, "users_idusers_key").IsUnique();

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Idusers)
                .ValueGeneratedOnAdd()
                .HasColumnName("idusers");
            entity.Property(e => e.Login)
                .HasMaxLength(25)
                .HasColumnName("login");
            entity.Property(e => e.RolesIdroles).HasColumnName("roles_idroles");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(25)
                .HasColumnName("password");
            entity.Property(e => e.Paternity)
                .HasMaxLength(45)
                .HasColumnName("paternity");
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .HasColumnName("surname");

            entity.HasOne(d => d.RolesIdrolesNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RolesIdroles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roles_idroles_fkey");
        });

        OnModelCreatingPartial(modelBuilder);


    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
