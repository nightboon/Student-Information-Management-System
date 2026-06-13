using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using src.services;

namespace src.assistant
{
    /// <summary>
    /// Central, page-neutral endpoint for the chat assistant widget. The widget
    /// (controls/assistant_widget.ascx) can sit on any page or master and always
    /// POSTs here, so the assistant is no longer tied to the student dashboard.
    ///
    /// The endpoint is role-aware: it reads the role from the session, hands the
    /// model the matching tool set + system prompt, and injects userId from the
    /// session only. The model can never pass a userId or any tool argument.
    /// </summary>
    public partial class chat : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        // Cap on tool-call rounds so a misbehaving model can't loop forever.
        private const int MaxAssistantTurns = 5;

        /// <summary>
        /// Runs the OpenAI-compatible tool-call loop on the server: ask the model,
        /// run any tools it requests (scoped to the logged-in user and role), feed
        /// results back, repeat until the model answers in plain text.
        /// </summary>
        [WebMethod(EnableSession = true)]
        public static object Chat(List<Dictionary<string, object>> history, string message, string conversationId)
        {
            var ctx = HttpContext.Current;

            // Only an authenticated student or lecturer may use the assistant.
            string role = ctx.Session["role"] as string;
            bool isStudent = string.Equals(role, "STUDENT", StringComparison.OrdinalIgnoreCase);
            bool isLecturer = string.Equals(role, "LECTURER", StringComparison.OrdinalIgnoreCase);

            if (ctx.Session["user_id"] == null || (!isStudent && !isLecturer))
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
                    ["content"] = SystemPromptFor(isLecturer)
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

            var tools = AssistantTools.DefinitionsFor(role);
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
                        string result = AssistantTools.Execute(name, userId, role);

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

        private static string SystemPromptFor(bool isLecturer)
        {
            if (isLecturer)
            {
                return "You are the INTI lecturer portal assistant. Only answer questions about the " +
                       "currently logged-in lecturer's own teaching: the courses they teach and students " +
                       "who may be at risk. When you need data, call the provided tools instead of " +
                       "guessing, and never invent information. Reply concisely and politely.";
            }

            return "You are the INTI student portal assistant. Only answer questions about the " +
                   "currently logged-in student's own academic information. When you need data, call " +
                   "the provided tools instead of guessing, and never invent information. Reply " +
                   "concisely and politely.";
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
