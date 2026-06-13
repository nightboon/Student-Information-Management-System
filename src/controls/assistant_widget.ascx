<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="assistant_widget.ascx.cs" Inherits="src.controls.assistant_widget" %>

<%-- Floating AI chat assistant. Embed it on any page (or a master page) with a
     single line: <uc:Assistant runat="server" />. The chat endpoint and the
     greeting/title are role-aware, resolved server-side from the session. --%>

<%-- Floating chat assistant --%>
<button id="assistant-fab" type="button" aria-label="Open assistant"
        class="fixed bottom-6 right-6 z-50 flex h-14 w-14 items-center justify-center rounded-full bg-[#e0162b] text-white shadow-lg shadow-[#e0162b]/30 hover:bg-[#a01020] transition-colors">
    <i data-lucide="message-circle" class="h-6 w-6"></i>
</button>

<div id="assistant-panel"
     data-endpoint="<%= EndpointUrl %>"
     class="fixed bottom-24 right-6 z-50 hidden h-[45rem] max-h-[80vh] w-[33rem] flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl">
    <header class="flex items-center justify-between bg-[#e0162b] px-4 py-3 text-white">
        <div class="flex items-center gap-2">
            <i data-lucide="sparkles" class="h-4 w-4"></i>
            <span style="font-size:14px;font-weight:600"><%= AssistantTitle %></span>
        </div>
        <button id="assistant-close" type="button" aria-label="Close assistant" class="text-white/80 hover:text-white">
            <i data-lucide="x" class="h-4 w-4"></i>
        </button>
    </header>

    <div id="assistant-messages" class="flex-1 space-y-3 overflow-y-auto bg-slate-50/60 p-4" style="font-size:13px;line-height:1.55">
        <div class="text-slate-500"><%= GreetingText %></div>
    </div>

    <div class="flex items-center gap-2 border-t border-slate-100 p-3">
        <input id="assistant-input" type="text" autocomplete="off"
               class="min-w-0 flex-1 rounded-xl border border-slate-200 px-3 py-2 text-slate-800 outline-none focus:border-[#e0162b]"
               style="font-size:13px" placeholder="<%= InputPlaceholder %>" />
        <button id="assistant-send" type="button" aria-label="Send"
                class="flex h-9 w-9 shrink-0 items-center justify-center rounded-xl bg-[#e0162b] text-white hover:bg-[#a01020] transition-colors disabled:opacity-50">
            <i data-lucide="send" class="h-4 w-4"></i>
        </button>
    </div>
</div>

<%-- Markdown rendering for assistant replies (marked) --%>
<style>
    #assistant-messages .assistant-md ul,
    #assistant-messages .assistant-md ol { padding-left: 1.1rem; list-style: revert; }
    #assistant-messages .assistant-md table { display: block; overflow-x: auto; border-collapse: collapse; font-size: 12px; }
    #assistant-messages .assistant-md th,
    #assistant-messages .assistant-md td { border: 1px solid #e2e8f0; padding: 3px 6px; white-space: nowrap; }
</style>
<script src="https://cdn.jsdelivr.net/npm/marked@12.0.0/marked.min.js"></script>
<script src="<%= ResolveUrl("~/js/shared/assistant/assistant.js") %>"></script>
