using Microsoft.EntityFrameworkCore;
using meetings_app_server.Models.Domain;
using System;

namespace meetings_app_server.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingAttendee> MeetingAttendees { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeetingAttendee>()
            .HasKey(ma => new { ma.MeetingId, ma.UserId });

        modelBuilder.Entity<MeetingAttendee>()
            .HasOne(ma => ma.Meeting)
            .WithMany(m => m.Attendees)
            .HasForeignKey(ma => ma.MeetingId);

        modelBuilder.Entity<MeetingAttendee>()
            .HasOne(ma => ma.User)
            .WithMany()
            .HasForeignKey(ma => ma.UserId);
    }
}
