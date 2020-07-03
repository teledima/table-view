using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace lab6
{
    public class Brainly_info:DbContext
    {
        public DbSet<questions> questions { get; set; }
        public DbSet<subjects> subjects { get; set; }
        public DbSet<users> users { get; set; }
        public DbSet<statuses> statuses { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(Properties.Settings.Default.brainly_info);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<subjects>()
                .HasKey(subject => subject.id)
                .HasName("subject_pk");
            builder.HasSequence<int>("subject_id_seq", "public");

            builder.Entity<subjects>()
                .HasIndex(subject => subject.name)
                .HasName("subject_subject_name_uindex")
                .IsUnique();

            builder.Entity<statuses>()
                .HasKey(status => status.id_status)
                .HasName("statuses_pk");
            builder.HasSequence<int>("statuses_id_status_seq", "public");

            builder.Entity<statuses>()
                .HasIndex(status => status.name)
                .HasName("statuses_name_uindex")
                .IsUnique();

            builder.Entity<users>()
                .HasKey(user => user.id_user)
                .HasName("users_pk");
            builder.HasSequence<int>("users_id_user_seq", "public");

            builder.Entity<users>()
                .HasIndex(user => user.login)
                .HasName("users_login_uindex")
                .IsUnique();

            builder.Entity<users>()
                .HasCheckConstraint("users_age_check", "age >= 0");

            builder.Entity<users>()
                .HasOne(user => user.statuses)
                .WithMany()
                .HasForeignKey(user => user.status_id);

            builder.Entity<questions>()
                .HasKey(question => question.id_question)
                .HasName("questions_pk");

            builder.Entity<questions>()
                .HasCheckConstraint("questions_count_view_check", "count_view >= 0")
                .HasCheckConstraint("questions_link_id_check", "link_id >= 0");

            builder.HasSequence<int>("questions_id_question_seq", "public");

            builder.Entity<questions>()
                .HasOne(question => question.subjects)
                .WithMany()
                .HasForeignKey(question => question.subject_id);

            builder.Entity<questions>()
                .HasOne(question => question.users)
                .WithMany()
                .HasForeignKey(question => question.user_id);


        }
    }
    public class users
    {
        public users() { }
        public users(string login, int age, int status_id)
        {
            this.login = login;
            this.age = age;
            this.status_id = status_id;
        }
        public int id_user { get; set; }
        public string login { get; set; }
        public int age { get; set; }
        public int status_id { get; set; }
        public statuses statuses { get; set; }
    }
    public class statuses
    {
        public int id_status { get; set; }
        public string name { get; set; }
    }
    public class questions
    {
        public questions() { }
        public questions(int link_id, int count_view, int subject_id, int user_id)
        {
            this.link_id = link_id;
            this.count_view = count_view;
            this.subject_id = subject_id;
            this.user_id = user_id;
        }
        public int id_question { get; set; }
        public int link_id { get; set; }
        public int count_view { get; set; }
        public int subject_id { get; set; }
        public int user_id { get; set; }
        public users users { get; set; }
        public subjects subjects { get; set; }
    }
    public class subjects
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
