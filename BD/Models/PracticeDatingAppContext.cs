using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BD.Models;

public partial class PracticeDatingAppContext : DbContext
{
    public PracticeDatingAppContext()
    {
    }

    public PracticeDatingAppContext(DbContextOptions<PracticeDatingAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Datauser> Datausers { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Interest> Interests { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Relationshipgoal> Relationshipgoals { get; set; }

    public virtual DbSet<Userdataregister> Userdataregisters { get; set; }

    public virtual DbSet<Userdescription> Userdescriptions { get; set; }

    public virtual DbSet<Usergoal> Usergoals { get; set; }

    public virtual DbSet<Userinterest> Userinterests { get; set; }

    public virtual DbSet<Userlike> Userlikes { get; set; }

    public virtual DbSet<Userphotopprofile> Userphotopprofiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Practice_dating_app;Username=postgres;Password=Elizaveta05");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("chat_pkey");

            entity.ToTable("chat");

            entity.Property(e => e.ChatId)
                .ValueGeneratedNever()
                .HasColumnName("chat_id");
            entity.Property(e => e.TimeCreated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_created");
            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");

            entity.HasOne(d => d.User1).WithMany(p => p.ChatUser1s)
                .HasForeignKey(d => d.User1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chat_user1_id_fkey");

            entity.HasOne(d => d.User2).WithMany(p => p.ChatUser2s)
                .HasForeignKey(d => d.User2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chat_user2_id_fkey");
        });

        modelBuilder.Entity<Datauser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("datausers_pkey");

            entity.ToTable("datausers");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("first_name");
            entity.Property(e => e.GenderId).HasColumnName("gender_id");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("last_name");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("patronymic");
            entity.Property(e => e.UdrId).HasColumnName("udr_id");

            entity.HasOne(d => d.Gender).WithMany(p => p.Datausers)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("datausers_gender_id_fkey");

            entity.HasOne(d => d.Location).WithMany(p => p.Datausers)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("datausers_location_id_fkey");

            entity.HasOne(d => d.Udr).WithMany(p => p.Datausers)
                .HasForeignKey(d => d.UdrId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("datausers_userdataregister_id_fkey");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("gender_pkey");

            entity.ToTable("gender");

            entity.Property(e => e.GenderId)
                .ValueGeneratedNever()
                .HasColumnName("gender_id");
            entity.Property(e => e.GenderName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("gender_name");
        });

        modelBuilder.Entity<Interest>(entity =>
        {
            entity.HasKey(e => e.InterestId).HasName("interest_pkey");

            entity.ToTable("interest");

            entity.Property(e => e.InterestId)
                .ValueGeneratedNever()
                .HasColumnName("interest_id");
            entity.Property(e => e.InterestName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("interest_name");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("location_pkey");

            entity.ToTable("location");

            entity.Property(e => e.LocationId)
                .ValueGeneratedNever()
                .HasColumnName("location_id");
            entity.Property(e => e.LocationName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("location_name");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("matches_pkey");

            entity.ToTable("matches");

            entity.Property(e => e.MatchId)
                .ValueGeneratedNever()
                .HasColumnName("match_id");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");

            entity.HasOne(d => d.User1).WithMany(p => p.MatchUser1s)
                .HasForeignKey(d => d.User1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("matches_user1_id_fkey");

            entity.HasOne(d => d.User2).WithMany(p => p.MatchUser2s)
                .HasForeignKey(d => d.User2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("matches_user2_id_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.MessageId)
                .ValueGeneratedNever()
                .HasColumnName("message_id");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.MessageText).HasColumnName("message_text");
            entity.Property(e => e.TimeCreated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_created");
            entity.Property(e => e.UserSendingId).HasColumnName("user_sending_id");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_chat_id_fkey");

            entity.HasOne(d => d.UserSending).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserSendingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_user_sending_id_fkey");
        });

        modelBuilder.Entity<Relationshipgoal>(entity =>
        {
            entity.HasKey(e => e.GoalId).HasName("relationshipgoals_pkey");

            entity.ToTable("relationshipgoals");

            entity.Property(e => e.GoalId)
                .ValueGeneratedNever()
                .HasColumnName("goal_id");
            entity.Property(e => e.GoalName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("goal_name");
        });

        modelBuilder.Entity<Userdataregister>(entity =>
        {
            entity.HasKey(e => e.UdrId).HasName("UserDataRegister_pkey");

            entity.ToTable("userdataregister");

            entity.Property(e => e.UdrId)
                .ValueGeneratedNever()
                .HasColumnName("udr_id");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .HasColumnName("password");
        });

        modelBuilder.Entity<Userdescription>(entity =>
        {
            entity.HasKey(e => e.UdId).HasName("userdescription_pkey");

            entity.ToTable("userdescription");

            entity.Property(e => e.UdId)
                .ValueGeneratedNever()
                .HasColumnName("ud_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Userdescriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userdescription_user_id_fkey");
        });

        modelBuilder.Entity<Usergoal>(entity =>
        {
            entity.HasKey(e => e.UgId).HasName("usergoal_pkey");

            entity.ToTable("usergoal");

            entity.Property(e => e.UgId)
                .ValueGeneratedNever()
                .HasColumnName("ug_id");
            entity.Property(e => e.GoalId).HasColumnName("goal_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Goal).WithMany(p => p.Usergoals)
                .HasForeignKey(d => d.GoalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usergoal_goal_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Usergoals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usergoal_user_id_fkey");
        });

        modelBuilder.Entity<Userinterest>(entity =>
        {
            entity.HasKey(e => e.UiId).HasName("userinterest_pkey");

            entity.ToTable("userinterest");

            entity.Property(e => e.UiId)
                .ValueGeneratedNever()
                .HasColumnName("ui_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Tag).WithMany(p => p.Userinterests)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userinterest_tag_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Userinterests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userinterest_user_id_fkey");
        });

        modelBuilder.Entity<Userlike>(entity =>
        {
            entity.HasKey(e => e.UlId).HasName("userlikes_pkey");

            entity.ToTable("userlikes");

            entity.Property(e => e.UlId)
                .ValueGeneratedNever()
                .HasColumnName("ul_id");
            entity.Property(e => e.LikedUserId).HasColumnName("liked_user_id");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.LikedUser).WithMany(p => p.UserlikeLikedUsers)
                .HasForeignKey(d => d.LikedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userlikes_liked_user_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserlikeUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userlikes_user_id_fkey");
        });

        modelBuilder.Entity<Userphotopprofile>(entity =>
        {
            entity.HasKey(e => e.UppId).HasName("userphotopprofile_pkey");

            entity.ToTable("userphotopprofile");

            entity.Property(e => e.UppId).HasColumnName("upp_id");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Userphotopprofiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("userphotopprofile_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
