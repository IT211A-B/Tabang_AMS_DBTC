using AMS.Data;
using AMS.DTOs;
using AMS.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace AMS.Services
{
    /// <summary>
    /// Aggregates today's stats, weekly trends, absence alerts,
    /// and recent activity for the teacher dashboard.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDTO> GetDashboardAsync(int sectionId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // ── Total enrolled students ────────────────────────────────────
            var totalStudents = await _context.Students
                .CountAsync(s => s.SectionId == sectionId);

            // ── Today's attendance counts ──────────────────────────────────
            var todayRecords = await _context.Attendances
                .Where(a => a.SectionId == sectionId && a.Date == today)
                .ToListAsync();

            var presentToday = todayRecords.Count(a => a.Status == "Present");
            var absentToday = todayRecords.Count(a => a.Status == "Absent");
            var lateToday = todayRecords.Count(a => a.Status == "Late");
            var excusedToday = todayRecords.Count(a => a.Status == "Excused");

            // ── Weekly breakdown (Mon–Fri of the current week) ─────────────
            var weekStart = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
            var weekEnd = weekStart.AddDays(4);                      // Friday

            var weekRecords = await _context.Attendances
                .Where(a => a.SectionId == sectionId
                         && a.Date >= weekStart
                         && a.Date <= weekEnd)
                .ToListAsync();

            // Build per-day dictionaries for the chart
            var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri" };
            var weeklyPresent = new Dictionary<string, int>();
            var weeklyAbsent = new Dictionary<string, int>();
            var weeklyLate = new Dictionary<string, int>();

            for (int i = 0; i < 5; i++)
            {
                var day = weekStart.AddDays(i);
                var dayName = days[i];
                var dayRecs = weekRecords.Where(a => a.Date == day).ToList();

                weeklyPresent[dayName] = dayRecs.Count(a => a.Status == "Present");
                weeklyAbsent[dayName] = dayRecs.Count(a => a.Status == "Absent");
                weeklyLate[dayName] = dayRecs.Count(a => a.Status == "Late");
            }

            // ── Absence alerts (students with ≥3 absences this period) ─────
            var absenceCounts = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.SectionId == sectionId && a.Status == "Absent")
                .GroupBy(a => new { a.StudentId, a.Student!.FirstName, a.Student.LastName })
                .Select(g => new AbsenceAlertDTO
                {
                    StudentId = g.Key.StudentId,
                    StudentName = $"{g.Key.FirstName} {g.Key.LastName}",
                    AbsenceCount = g.Count(),
                    // Build the alert message dynamically
                    Message = $"{g.Count()} absence{(g.Count() > 1 ? "s" : "")} this period"
                })
                .Where(a => a.AbsenceCount >= 3) // only flag students at risk
                .OrderByDescending(a => a.AbsenceCount)
                .ToListAsync();

            // ── Recent activity (last 10 attendance events) ────────────────
            var recentActivity = await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.SectionId == sectionId)
                .OrderByDescending(a => a.UpdatedAt)
                .Take(10)
                .Select(a => new ActivityLogDTO
                {
                    Description = $"{a.Student!.FirstName} {a.Student.LastName} marked {a.Status} on {a.Date}",
                    Timestamp = a.UpdatedAt
                })
                .ToListAsync();

            return new DashboardDTO
            {
                TotalStudents = totalStudents,
                PresentToday = presentToday,
                AbsentToday = absentToday,
                LateToday = lateToday,
                ExcusedToday = excusedToday,
                WeeklyPresent = weeklyPresent,
                WeeklyAbsent = weeklyAbsent,
                WeeklyLate = weeklyLate,
                AbsenceAlerts = absenceCounts,
                RecentActivity = recentActivity
            };
        }
    }
}