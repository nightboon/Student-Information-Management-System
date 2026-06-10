using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Hosting;
using System.Xml;
using src.db;

namespace src.services
{
    /// <summary>One logged assistant conversation (all exchanges of one widget session).</summary>
    public class ChatLog
    {
        public int ChatLogId { get; set; }
        public Guid ConversationId { get; set; }
        public int UserId { get; set; }
        public int? StudentId { get; set; }
        public string StudentName { get; set; }
        public string Transcript { get; set; }
        public int Turns { get; set; }
        public string ToolsUsed { get; set; }
        public int DurationMs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>First question of the conversation, for the console list row.</summary>
        public string FirstQuestion
        {
            get
            {
                if (string.IsNullOrEmpty(Transcript)) return "";
                var firstLine = Transcript.Split('\n')[0];
                return firstLine.StartsWith("Q: ") ? firstLine.Substring(3) : firstLine;
            }
        }

        /// <summary>
        /// Who had the conversation, for the console list row: student id + name
        /// (e.g. "#1 Ong Zhi Bo"), falling back to the USERS id ("U4") when the
        /// account has no STUDENTS row.
        /// </summary>
        public string StudentLabel
        {
            get
            {
                if (StudentId == null) return "U" + UserId;
                return "#" + StudentId + " " + (StudentName ?? "");
            }
        }
    }

    /// <summary>
    /// Backing service for the AI Console page: reads/writes the AI settings in
    /// the external, gitignored ai.config file, and reads/writes CHAT_LOGS.
    ///
    /// Settings are read straight from ai.config on every call rather than via
    /// ConfigurationManager: the merged appSettings section is cached in-memory
    /// and RefreshSection does not reliably re-read a file=-referenced external
    /// file, so going to the file is the only way edits apply immediately.
    /// </summary>
    public static class AiConsoleService
    {
        private static readonly string[] SettingKeys = { "Ai:BaseUrl", "Ai:ApiKey", "Ai:Model" };

        /// <summary>
        /// Reads one AI setting directly from ai.config; falls back to
        /// ConfigurationManager (Web.config) when the file or key is absent.
        /// </summary>
        public static string GetSetting(string key)
        {
            try
            {
                string path = HostingEnvironment.MapPath("~/ai.config");
                if (System.IO.File.Exists(path))
                {
                    var doc = new XmlDocument();
                    doc.Load(path);
                    var node = doc.DocumentElement == null
                        ? null
                        : (XmlElement)doc.DocumentElement.SelectSingleNode("add[@key='" + key + "']");
                    if (node != null) return node.GetAttribute("value");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Reading ai.config failed: " + ex.Message);
            }
            return ConfigurationManager.AppSettings[key] ?? "";
        }

        /// <summary>Writes the three AI settings to ai.config; effective immediately.</summary>
        public static void SaveSettings(string baseUrl, string apiKey, string model)
        {
            string path = HostingEnvironment.MapPath("~/ai.config");
            var values = new Dictionary<string, string>
            {
                ["Ai:BaseUrl"] = (baseUrl ?? "").Trim(),
                ["Ai:ApiKey"] = (apiKey ?? "").Trim(),
                ["Ai:Model"] = (model ?? "").Trim()
            };

            var doc = new XmlDocument();
            if (System.IO.File.Exists(path))
            {
                doc.Load(path);
            }
            if (doc.DocumentElement == null || doc.DocumentElement.Name != "appSettings")
            {
                doc.LoadXml("<appSettings />");
            }

            foreach (var key in SettingKeys)
            {
                var node = (XmlElement)doc.DocumentElement.SelectSingleNode("add[@key='" + key + "']");
                if (node == null)
                {
                    node = doc.CreateElement("add");
                    node.SetAttribute("key", key);
                    doc.DocumentElement.AppendChild(node);
                }
                node.SetAttribute("value", values[key]);
            }

            doc.Save(path);
        }

        /// <summary>Clears all three AI settings (the console's "Reset" button).</summary>
        public static void ResetSettings()
        {
            SaveSettings("", "", "");
        }

        /// <summary>
        /// Appends one exchange to its conversation row, creating the row on the
        /// first exchange. Tools are merged (deduplicated) and durations summed.
        /// Callers are expected to swallow failures.
        /// </summary>
        public static void AppendLog(Guid conversationId, int userId, string question,
            string reply, string toolsUsed, int durationMs)
        {
            string entry = "Q: " + (question ?? "").Replace("\r", "") + "\n" +
                           "A: " + (reply ?? "").Replace("\r", "") + "\n" +
                           "[tools: " + (string.IsNullOrEmpty(toolsUsed) ? "none" : toolsUsed) +
                           "; " + durationMs + "ms]\n\n";

            using (var conn = Db.OpenConnection())
            {
                // Merge the tool list of the existing row (if any) with this turn's.
                string existingTools = null;
                bool exists = false;
                using (var cmd = new SqlCommand(
                    "SELECT tools_used FROM CHAT_LOGS WHERE conversation_id = @cid AND user_id = @uid", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", conversationId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            exists = true;
                            existingTools = reader["tools_used"] == DBNull.Value ? "" : reader["tools_used"].ToString();
                        }
                    }
                }

                string mergedTools = MergeTools(existingTools, toolsUsed);

                if (exists)
                {
                    using (var cmd = new SqlCommand(
                        "UPDATE CHAT_LOGS SET transcript = transcript + @entry, turns = turns + 1, " +
                        "tools_used = @tools, duration_ms = duration_ms + @ms, updated_at = GETDATE() " +
                        "WHERE conversation_id = @cid AND user_id = @uid", conn))
                    {
                        cmd.Parameters.AddWithValue("@entry", entry);
                        cmd.Parameters.AddWithValue("@tools", (object)mergedTools ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ms", durationMs);
                        cmd.Parameters.AddWithValue("@cid", conversationId);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(
                        "INSERT INTO CHAT_LOGS (conversation_id, user_id, transcript, tools_used, duration_ms) " +
                        "VALUES (@cid, @uid, @entry, @tools, @ms)", conn))
                    {
                        cmd.Parameters.AddWithValue("@cid", conversationId);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@entry", entry);
                        cmd.Parameters.AddWithValue("@tools", (object)mergedTools ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ms", durationMs);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>Latest conversations, most recently active first.</summary>
        public static List<ChatLog> GetRecentLogs(int top)
        {
            var logs = new List<ChatLog>();
            using (var conn = Db.OpenConnection())
            using (var cmd = new SqlCommand(
                "SELECT TOP (@top) cl.chat_log_id, cl.conversation_id, cl.user_id, " +
                "s.student_id, s.full_name, cl.transcript, cl.turns, " +
                "cl.tools_used, cl.duration_ms, cl.created_at, cl.updated_at " +
                "FROM CHAT_LOGS cl " +
                "LEFT JOIN STUDENTS s ON s.user_id = cl.user_id " +
                "ORDER BY cl.updated_at DESC, cl.chat_log_id DESC", conn))
            {
                cmd.Parameters.AddWithValue("@top", top);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new ChatLog
                        {
                            ChatLogId = (int)reader["chat_log_id"],
                            ConversationId = (Guid)reader["conversation_id"],
                            UserId = (int)reader["user_id"],
                            StudentId = reader["student_id"] == DBNull.Value ? (int?)null : (int)reader["student_id"],
                            StudentName = reader["full_name"] == DBNull.Value ? null : reader["full_name"].ToString(),
                            Transcript = reader["transcript"].ToString(),
                            Turns = (int)reader["turns"],
                            ToolsUsed = reader["tools_used"] == DBNull.Value ? "" : reader["tools_used"].ToString(),
                            DurationMs = (int)reader["duration_ms"],
                            CreatedAt = (DateTime)reader["created_at"],
                            UpdatedAt = (DateTime)reader["updated_at"]
                        });
                    }
                }
            }
            return logs;
        }

        /// <summary>Union of two comma-separated tool lists, order-preserving, deduplicated.</summary>
        private static string MergeTools(string existing, string added)
        {
            var merged = new List<string>();
            foreach (var part in ((existing ?? "") + "," + (added ?? "")).Split(','))
            {
                var name = part.Trim();
                if (name.Length > 0 && !merged.Contains(name)) merged.Add(name);
            }
            return merged.Count == 0 ? null : string.Join(", ", merged);
        }
    }
}
