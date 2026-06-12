using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using src.assistant;
using src.db;
using src.services;

namespace src.student
{
    public partial class student_dashboard : src.security.StudentPage
    {
        private Student _student;
        private Semester _semester;

        protected string Greeting
        {
            get
            {
                int hour = DateTime.Now.Hour;
                string greeting_msg = "Good";

                if (hour >= 5 && hour < 12)
                {
                    greeting_msg += " Morning";
                }else if(hour >= 12 && hour < 17)
                {
                    greeting_msg += " Afternoon";
                }
                else if (hour >= 17 && hour < 21)
                {
                    greeting_msg += " Evening";
                }
                else
                {
                    greeting_msg += " Night";
                }

                return greeting_msg;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            if (Session["user_id"] == null)
            {
                Response.Redirect("~/shared/login.aspx");
                return;
            }

            _student = StudentService.GetByUserId((int)Session["user_id"]);
            _semester = SemesterService.GetCurrent();
            if (_student != null)
            {
                coursesRepeater.DataSource = _student.CurrentCourses;
                coursesRepeater.DataBind();
                scheduleRepeater.DataSource = _student.TodayClasses;
                scheduleRepeater.DataBind();
                assignmentsRepeater.DataSource = _student.AssignmentsDueThisWeek;
                assignmentsRepeater.DataBind();
                announcementsRepeater.DataSource = _student.Announcements;
                announcementsRepeater.DataBind();
            }
        }

        protected string CurrentDateLabel
        {
            // e.g. "Sunday, 24 May 2026" (day-first, invariant so it doesn't shift with server culture).
            get { return DateTime.Now.ToString("dddd, d MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture); }
        }

        protected int SemesterWeek
        {
            get { return _semester != null ? _semester.CurrentWeek : 1; }
        }

        protected string SemesterNumber
        {
            get { return _student != null ? _student.CurrentSemesterNo.ToString(System.Globalization.CultureInfo.InvariantCulture) : ""; }
        }

        protected string GetUserName
        {

            get { return _student != null ? _student.FullName : "Student"; }
        }

        protected int TodayClassCount
        {
            // Today's classes come from the student profile loaded in Page_Load.
            get { return _student != null && _student.TodayClasses != null ? _student.TodayClasses.Count : 0; }
        }

        protected int AssignmentDueCount
        {
            // Assignments due this week, from the student profile loaded in Page_Load.
            get { return _student != null && _student.AssignmentsDueThisWeek != null ? _student.AssignmentsDueThisWeek.Count : 0; }
        }

        protected int PendingTaskCount
        {
            // Current-semester assignments the student has not yet submitted.
            get { return _student != null ? _student.PendingTaskCount : 0; }
        }

        protected string CgpaDisplay
        {
            // 2-decimal CGPA, or an em dash when the student has no published grades.
            get
            {
                return _student != null && _student.Cgpa.HasValue
                    ? _student.Cgpa.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                    : "—";
            }
        }

        protected string AttendanceDisplay
        {
            // Whole-percent attendance, or an em dash when there are no records.
            get
            {
                return _student != null && _student.AttendanceRate.HasValue
                    ? System.Math.Round(_student.AttendanceRate.Value * 100).ToString("0", System.Globalization.CultureInfo.InvariantCulture) + "%"
                    : "—";
            }
        }

        protected int CreditsEarnedValue
        {
            get { return _student != null ? _student.CreditsEarned : 0; }
        }

        protected string ClassColor(string color)
        {
            // Course color from the DB; falls back to neutral slate when unset.
            return string.IsNullOrEmpty(color) ? "#64748b" : color;
        }

        protected string FormatTimeRange(TimeSpan start, TimeSpan end)
        {
            // en dash (–) between the two HH:mm times, matching the markup.
            return start.ToString(@"hh\:mm") + " – " + end.ToString(@"hh\:mm");
        }

        protected bool IsLiveNow(TimeSpan start, TimeSpan end)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            return now >= start && now < end;
        }

        protected string TodayScheduleSubtitle
        {
            // e.g. "4 classes · 6h 30m total", or a friendly note when empty.
            get
            {
                if (_student == null || _student.TodayClasses == null || _student.TodayClasses.Count == 0)
                {
                    return "No classes today";
                }

                int count = _student.TodayClasses.Count;
                TimeSpan total = TimeSpan.Zero;
                foreach (var session in _student.TodayClasses)
                {
                    total += session.EndTime - session.StartTime;
                }

                string duration = (int)total.TotalHours + "h " + total.Minutes + "m";
                return count + (count == 1 ? " class" : " classes") + " · " + duration + " total";
            }
        }

        protected string FormatRelativeDue(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            if (days < 0) return "Overdue";
            if (days == 0) return "Due today";
            if (days == 1) return "Tomorrow";
            return "In " + days + " days";
        }

        protected string DueIcon(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            return days <= 1 ? "alert-circle" : "check-circle-2";
        }

        protected string DueBadgeClass(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            if (days <= 1) return "bg-[#e0162b]/10 text-[#e0162b]";
            if (days <= 3) return "bg-amber-50 text-amber-600";
            return "bg-emerald-50 text-emerald-600";
        }

        protected string DueTextClass(DateTime due)
        {
            int days = (due.Date - DateTime.Today).Days;
            return days <= 1 ? "text-[#e0162b] font-semibold" : "text-slate-500";
        }

        protected int AnnouncementCount
        {
            get { return _student != null && _student.Announcements != null ? _student.Announcements.Count : 0; }
        }

        protected string FormatRelativeTime(DateTime when)
        {
            TimeSpan ago = DateTime.Now - when;
            if (ago.TotalMinutes < 1) return "Just now";
            if (ago.TotalMinutes < 60) return (int)ago.TotalMinutes + "m ago";
            if (ago.TotalHours < 24) return (int)ago.TotalHours + "h ago";

            int days = (int)ago.TotalDays;
            if (days == 1) return "Yesterday";
            if (days < 7) return days + " days ago";
            return when.ToString("d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        // ------------------------------------------------------------------
        // Chat assistant endpoint
        // ------------------------------------------------------------------

        // Cap on tool-call rounds so a misbehaving model can't loop forever.
        private const int MaxAssistantTurns = 5;

        /// <summary>
        /// Page method backing the dashboard chat widget. Runs the OpenAI-compatible
        /// tool-call loop on the server: ask the model, run any tools it requests
        /// (scoped to the logged-in student), feed results back, repeat until the
        /// model answers in plain text. The userId comes only from the session.
        /// </summary>
        [WebMethod(EnableSession = true)]
        public static object Chat(List<Dictionary<string, object>> history, string message, string conversationId)
        {
            var ctx = HttpContext.Current;

            // Only an authenticated student may use the assistant.
            if (ctx.Session["user_id"] == null ||
                !string.Equals(ctx.Session["role"] as string, "STUDENT", StringComparison.OrdinalIgnoreCase))
            {
                ctx.Response.StatusCode = 401;
                ctx.Response.SuppressContent = true;
                return null;
            }

            int userId = (int)ctx.Session["user_id"];

            // One log row per widget session; an unparseable id degrades to per-call rows.
            Guid convId;
            if (!Guid.TryParse(conversationId, out convId)) convId = Guid.NewGuid();

            // System prompt + prior turns + the new user message.
            var messages = new List<object>
            {
                new Dictionary<string, object>
                {
                    ["role"] = "system",
                    ["content"] =
                        "You are the INTI student portal assistant. Only answer questions about the " +
                        "currently logged-in student's own academic information. When you need data, call " +
                        "the provided tools instead of guessing, and never invent information. Reply " +
                        "concisely and politely."
                }
            };

            if (history != null)
            {
                foreach (var turn in history)
                {
                    messages.Add(turn);
                }
            }

            messages.Add(new Dictionary<string, object>
            {
                ["role"] = "user",
                ["content"] = message ?? ""
            });

            var tools = AssistantTools.Definitions();
            var toolsUsed = new List<string>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                for (int turn = 0; turn < MaxAssistantTurns; turn++)
                {
                    var aiMessage = AiChatClient.Send(messages, tools);
                    messages.Add(aiMessage);

                    object toolCallsObj;
                    aiMessage.TryGetValue("tool_calls", out toolCallsObj);
                    var toolCalls = toolCallsObj as object[];

                    // No tool calls: the assistant has produced its final answer.
                    if (toolCalls == null || toolCalls.Length == 0)
                    {
                        object content;
                        aiMessage.TryGetValue("content", out content);
                        var reply = content as string ?? "";
                        WriteChatLog(convId, userId, message, reply, toolsUsed, stopwatch);
                        return new { reply };
                    }

                    // Run every requested tool and append its result for the next round.
                    foreach (var callObj in toolCalls)
                    {
                        var call = (Dictionary<string, object>)callObj;
                        var function = (Dictionary<string, object>)call["function"];
                        var name = function["name"] as string;

                        toolsUsed.Add(name);
                        string result = AssistantTools.Execute(name, userId);

                        messages.Add(new Dictionary<string, object>
                        {
                            ["role"] = "tool",
                            ["tool_call_id"] = call["id"],
                            ["content"] = result
                        });
                    }
                }

                var fallback = "Sorry, I couldn't complete that request.";
                WriteChatLog(convId, userId, message, fallback, toolsUsed, stopwatch);
                return new { reply = fallback };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Assistant Chat failed: " + ex.Message);
                ctx.Response.StatusCode = 500;
                ctx.Response.SuppressContent = true;
                return null;
            }
        }

        /// <summary>Best-effort chat logging; never lets a logging failure break the reply.</summary>
        private static void WriteChatLog(Guid conversationId, int userId, string question,
            string reply, List<string> toolsUsed, System.Diagnostics.Stopwatch stopwatch)
        {
            try
            {
                stopwatch.Stop();
                AiConsoleService.AppendLog(conversationId, userId, question, reply,
                    string.Join(", ", toolsUsed), (int)stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Chat log write failed: " + ex.Message);
            }
        }

    }
}