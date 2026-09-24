using System;
using System.Linq;
using EventHub.Data;
using EventHub.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHub
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================================================");
            Console.WriteLine("=== EventHub Platform - EF Core Assignment 02 (All 3 Techniques) ===");
            Console.WriteLine("==========================================================================\n");

            using (var context = new EventHubDbContext())
            {
                Console.WriteLine("[Step 1] Initializing fresh database 'EventHubDB' on SQL Server...");
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Console.WriteLine("Database and tables created successfully!\n");

                Console.WriteLine("[Step 2] Seeding data demonstrating all entities and configuration techniques...");

                // 1. Create Organizer and OrganizerProfile (Configured via Technique 1: Data Annotations)
                var organizer = new Organizer
                {
                    Name = "TechCon Global",
                    CompanyName = "TechCon Innovations Ltd.",
                    IsVerified = true,
                    Profile = new OrganizerProfile
                    {
                        Bio = "Premier international tech conference organizers hosting flagship developer summits worldwide.",
                        Website = "https://techcon.global",
                        LogoUrl = "https://techcon.global/assets/logo.png"
                    }
                };

                context.Organizers.Add(organizer);
                context.SaveChanges();

                // 2. Create Events with Self-referencing Sessions and Shadow Properties (Configured via Technique 3: Fluent API)
                var conferenceEvent = new Event
                {
                    Title = "Global .NET and Cloud Summit 2026",
                    Description = "Annual premier conference covering cloud-native architecture, EF Core, and modern .NET development.",
                    StartDate = new DateTime(2026, 11, 15, 9, 0, 0),
                    EndDate = new DateTime(2026, 11, 17, 18, 0, 0),
                    MaxAttendees = 500,
                    OrganizerId = organizer.Id
                };

                context.Events.Add(conferenceEvent);
                context.SaveChanges();

                // Setting Shadow Properties on Event
                context.Entry(conferenceEvent).Property("CreatedAt").CurrentValue = DateTime.UtcNow.AddDays(-10);
                context.Entry(conferenceEvent).Property("LastModified").CurrentValue = DateTime.UtcNow;

                // Add Child Breakout Sessions nested under the main conference event
                var session1 = new Event
                {
                    Title = "Deep Dive into EF Core 9 Performance Optimization",
                    Description = "Breakout workshop covering query compilation, shadow properties, and batching.",
                    StartDate = new DateTime(2026, 11, 15, 11, 0, 0),
                    EndDate = new DateTime(2026, 11, 15, 13, 0, 0),
                    MaxAttendees = 80,
                    OrganizerId = organizer.Id,
                    ParentEventId = conferenceEvent.Id
                };

                var session2 = new Event
                {
                    Title = "Microservices Architecture with ASP.NET Core",
                    Description = "Hands-on architectural patterns, event-driven communication, and containerization.",
                    StartDate = new DateTime(2026, 11, 15, 14, 0, 0),
                    EndDate = new DateTime(2026, 11, 15, 16, 30, 0),
                    MaxAttendees = 100,
                    OrganizerId = organizer.Id,
                    ParentEventId = conferenceEvent.Id
                };

                context.Events.AddRange(session1, session2);
                context.SaveChanges();

                // Set shadow properties on sessions
                context.Entry(session1).Property("CreatedAt").CurrentValue = DateTime.UtcNow.AddDays(-5);
                context.Entry(session1).Property("LastModified").CurrentValue = DateTime.UtcNow;
                context.Entry(session2).Property("CreatedAt").CurrentValue = DateTime.UtcNow.AddDays(-5);
                context.Entry(session2).Property("LastModified").CurrentValue = DateTime.UtcNow;
                context.SaveChanges();

                // 3. Create Attendees with Owned Address and Badges (Configured via Technique 2: IEntityTypeConfiguration)
                var attendee1 = new Attendee
                {
                    FullName = "Mohamed Shalaby",
                    Email = "mohamed.shalaby@route.edu.eg",
                    HomeAddress = new Address
                    {
                        Street = "15 Tahrir St.",
                        City = "Cairo",
                        Country = "Egypt",
                        PostalCode = "11511"
                    },
                    Badge = new Badge
                    {
                        BadgeNumber = "BDG-2026-001",
                        IssueDate = DateTime.UtcNow,
                        Tier = BadgeTier.VIP
                    }
                };

                var attendee2 = new Attendee
                {
                    FullName = "Sara Ahmed",
                    Email = "sara.ahmed@example.com",
                    HomeAddress = new Address
                    {
                        Street = "42 Corniche Road",
                        City = "Alexandria",
                        Country = "Egypt",
                        PostalCode = "21500"
                    },
                    Badge = new Badge
                    {
                        BadgeNumber = "BDG-2026-002",
                        IssueDate = DateTime.UtcNow,
                        Tier = BadgeTier.Standard
                    }
                };

                context.Attendees.AddRange(attendee1, attendee2);
                context.SaveChanges();

                // 4. Register Attendees for Events (Technique 3: Fluent API composite key with payload)
                var reg1 = new EventRegistration
                {
                    EventId = conferenceEvent.Id,
                    AttendeeId = attendee1.Id,
                    RegistrationDate = DateTime.UtcNow,
                    Notes = "Interested in keynote presentation and VIP networking lounge."
                };

                var reg2 = new EventRegistration
                {
                    EventId = session1.Id,
                    AttendeeId = attendee1.Id,
                    RegistrationDate = DateTime.UtcNow,
                    Notes = "Attending EF Core performance workshop."
                };

                var reg3 = new EventRegistration
                {
                    EventId = conferenceEvent.Id,
                    AttendeeId = attendee2.Id,
                    RegistrationDate = DateTime.UtcNow,
                    Notes = "General admission pass."
                };

                context.EventRegistrations.AddRange(reg1, reg2, reg3);
                context.SaveChanges();

                Console.WriteLine("All entities and relationships seeded successfully!\n");

                // Step 3: Query and Display Results
                Console.WriteLine("========================================================================");
                Console.WriteLine("=== EVENT & ORGANIZER HIERARCHY (with Shadow Timestamps) ===");
                Console.WriteLine("========================================================================");

                var mainEvents = context.Events
                    .Include(e => e.Organizer)
                        .ThenInclude(o => o.Profile)
                    .Include(e => e.Sessions)
                    .Include(e => e.Registrations)
                        .ThenInclude(r => r.Attendee)
                            .ThenInclude(a => a.Badge)
                    .Where(e => e.ParentEventId == null)
                    .ToList();

                foreach (var ev in mainEvents)
                {
                    var createdAt = context.Entry(ev).Property<DateTime>("CreatedAt").CurrentValue;
                    var lastModified = context.Entry(ev).Property<DateTime>("LastModified").CurrentValue;

                    Console.WriteLine($"\n[MAIN EVENT] #{ev.Id}: {ev.Title}");
                    Console.WriteLine($"  Description   : {ev.Description}");
                    Console.WriteLine($"  Schedule      : {ev.StartDate:yyyy-MM-dd HH:mm} to {ev.EndDate:yyyy-MM-dd HH:mm}");
                    Console.WriteLine($"  Max Attendees : {ev.MaxAttendees}");
                    Console.WriteLine($"  Organizer     : {ev.Organizer.Name} ({ev.Organizer.CompanyName}) [Verified: {ev.Organizer.IsVerified}]");
                    Console.WriteLine($"  Profile Bio   : {ev.Organizer.Profile.Bio}");
                    Console.WriteLine($"  Website       : {ev.Organizer.Profile.Website}");
                    Console.WriteLine($"  Shadow Props  : CreatedAt = {createdAt:yyyy-MM-dd HH:mm:ss UTC}, LastModified = {lastModified:yyyy-MM-dd HH:mm:ss UTC}");

                    Console.WriteLine("  Breakout Sessions (Self-Referencing):");
                    foreach (var session in ev.Sessions)
                    {
                        var sCreatedAt = context.Entry(session).Property<DateTime>("CreatedAt").CurrentValue;
                        Console.WriteLine($"    -> Session #{session.Id}: {session.Title} ({session.StartDate:HH:mm} - {session.EndDate:HH:mm}) [Shadow Created: {sCreatedAt:yyyy-MM-dd}]");
                    }

                    Console.WriteLine("  Registered Attendees:");
                    foreach (var reg in ev.Registrations)
                    {
                        var att = reg.Attendee;
                        Console.WriteLine($"    * {att.FullName} ({att.Email}) | Address: {att.HomeAddress.Street}, {att.HomeAddress.City}, {att.HomeAddress.Country}");
                        Console.WriteLine($"      Badge: {att.Badge?.BadgeNumber} [{att.Badge?.Tier}] | Note: \"{reg.Notes}\"");
                    }
                }

                Console.WriteLine("\n========================================================================");
                Console.WriteLine("=== ASSIGNMENT 02 COMPLETED SUCCESSFULLY WITH 0 ERRORS! ===");
                Console.WriteLine("========================================================================");
            }
        }
    }
}
