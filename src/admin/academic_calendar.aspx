<%@ Page Language="C#" MasterPageFile="~/admin/AdminLayout.master" AutoEventWireup="true" CodeBehind="academic_calendar.aspx.cs" Inherits="src.admin.academic_calendar" Title="Academic Calendar - INTI Admin Portal" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div>
            <p class="text-slate-500" style="font-size:13px;font-weight:500">Admin</p>
            <h1 class="mt-1 text-slate-900" style="font-size:28px;font-weight:700">Intake &amp; Academic Calendar</h1>
            <p class="mt-1 text-slate-500" style="font-size:14px">Manage each cohort's semester lifecycle from registration through semester end.</p>
        </div>
        <button type="button" data-modal-open="semester-modal" data-new-semester class="inline-flex h-10 items-center gap-2 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">
            <i data-lucide="plus" class="h-4 w-4"></i> Add intake semester
        </button>
    </div>

    <div data-table data-page-size="30">
        <section class="mt-6 grid gap-6 xl:grid-cols-[minmax(0,1fr)_300px]">
            <div class="rounded-lg border border-slate-200 bg-white">
                <div class="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 px-6 py-4">
                    <div class="flex items-center gap-2">
                        <button type="button" data-calendar-prev class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-slate-100"><i data-lucide="chevron-left" class="h-4 w-4"></i></button>
                        <div data-calendar-title class="min-w-[130px] text-center text-slate-900" style="font-size:15px;font-weight:700"></div>
                        <button type="button" data-calendar-next class="inline-flex h-8 w-8 items-center justify-center rounded-md hover:bg-slate-100"><i data-lucide="chevron-right" class="h-4 w-4"></i></button>
                        <button type="button" data-calendar-today class="ml-2 h-8 rounded-md border border-slate-200 px-3 text-slate-600 hover:bg-slate-50" style="font-size:12px;font-weight:600">Today</button>
                    </div>
                    <div class="flex gap-2">
                        <select data-table-filter="intake" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700" style="font-size:12.5px"><%= IntakeFilterOptionsHtml %></select>
                        <select data-table-filter="sem" class="h-9 rounded-md border border-slate-200 bg-white px-3 text-slate-700" style="font-size:12.5px"><%= SemesterFilterOptionsHtml %></select>
                    </div>
                </div>
                <div class="grid grid-cols-7 gap-1 px-6 py-4 text-slate-500" style="font-size:11px;font-weight:600">
                    <div>Mon</div><div>Tue</div><div>Wed</div><div>Thu</div><div>Fri</div><div>Sat</div><div>Sun</div>
                </div>
                <div data-calendar-grid class="grid grid-cols-7 gap-1 px-6 pb-6"></div>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white">
                <div class="border-b border-slate-100 px-6 py-4">
                    <h2 class="text-slate-900" style="font-size:15px;font-weight:700">Lifecycle filters</h2>
                    <p class="mt-0.5 text-slate-500" style="font-size:12.5px">Intake first, then semester and milestone.</p>
                </div>
                <div class="space-y-4 px-6 py-5">
                    <label class="block"><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Intake</span><select data-table-filter="intake" class="mt-1.5 h-9 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:12.5px"><%= IntakeFilterOptionsHtml %></select></label>
                    <label class="block"><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Semester</span><select data-table-filter="sem" class="mt-1.5 h-9 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:12.5px"><%= SemesterFilterOptionsHtml %></select></label>
                    <label class="block"><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Milestone</span><select data-table-filter="type" class="mt-1.5 h-9 w-full rounded-md border border-slate-200 bg-white px-3" style="font-size:12.5px"><%= EventTypeFilterOptionsHtml %></select></label>
                    <button type="button" data-table-clear class="text-slate-500 hover:text-slate-900" style="font-size:12.5px;font-weight:600">Clear filters</button>
                    <div class="rounded-lg bg-slate-50 p-4 text-slate-600" style="font-size:12px;line-height:1.6">
                        Students only see registration for the open semester belonging to their assigned intake.
                    </div>
                </div>
            </div>
        </section>

        <section class="mt-6 rounded-lg border border-slate-200 bg-white">
            <div class="border-b border-slate-100 px-6 py-4"><h2 class="text-slate-900" style="font-size:15px;font-weight:700">Intake semester milestones</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px"><%= EventCount %> lifecycle milestones</p></div>
            <div class="overflow-x-auto"><table class="min-w-full">
                <thead class="border-b border-slate-100 bg-slate-50/60 text-slate-500"><tr><th class="px-6 py-3 text-left uppercase" style="font-size:11px">Milestone</th><th class="px-6 py-3 text-left uppercase" style="font-size:11px">Type</th><th class="px-6 py-3 text-left uppercase" style="font-size:11px">Date</th><th class="px-6 py-3 text-left uppercase" style="font-size:11px">End</th><th class="px-6 py-3 text-left uppercase" style="font-size:11px">Intake semester</th><th class="px-6 py-3 text-right uppercase" style="font-size:11px">Min credit</th><th class="px-6 py-3 text-right uppercase" style="font-size:11px">Max credit</th><th class="px-6 py-3 text-left uppercase" style="font-size:11px">Status</th><th class="px-6 py-3 text-right uppercase" style="font-size:11px">Action</th></tr></thead>
                <tbody><%= EventRowsHtml %></tbody>
            </table></div>
        </section>
    </div>

    <div id="semester-modal" data-modal class="fixed inset-0 z-[60] items-center justify-center p-4" style="display:none">
        <div data-modal-backdrop class="absolute inset-0 bg-slate-900/40 backdrop-blur-sm"></div>
        <div class="relative max-h-[92vh] w-full max-w-3xl overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xl">
            <div class="flex items-center justify-between border-b border-slate-100 px-6 py-4"><div><h2 class="text-slate-900" style="font-size:17px;font-weight:700">Intake semester setup</h2><p class="mt-0.5 text-slate-500" style="font-size:12.5px">One setup creates all five calendar milestones.</p></div><button type="button" data-modal-close class="h-8 w-8 rounded-md text-slate-400 hover:bg-slate-100"><i data-lucide="x" class="mx-auto h-4 w-4"></i></button></div>
            <div class="max-h-[72vh] overflow-y-auto px-6 py-5">
                <input type="hidden" data-field="sessionId" />
                <input type="hidden" data-field="intakeId" />
                <div class="grid gap-4 md:grid-cols-2">
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Intake month</span><input data-field="intakeMonth" type="month" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Semester</span><input data-field="semester" type="number" min="1" step="1" placeholder="e.g. 1" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Classes begin</span><input data-field="startDate" type="date" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Semester ends</span><input data-field="endDate" type="date" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Status</span><select data-field="status" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3"><%= EventStatusOptionsHtml %></select></label>
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Min credits</span><input data-field="minCredits" type="number" min="0" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                    <label><span class="text-slate-500 uppercase" style="font-size:11px;font-weight:600">Max credits</span><input data-field="maxCredits" type="number" min="0" class="mt-1.5 h-10 w-full rounded-md border border-slate-200 px-3" /></label>
                </div>
            </div>
            <div class="flex justify-end gap-2 border-t border-slate-100 bg-slate-50/50 px-6 py-4"><button type="button" data-modal-close class="h-10 rounded-md px-4 text-slate-600" style="font-size:13px;font-weight:600">Cancel</button><button type="button" data-semester-save class="h-10 rounded-md bg-[#e0162b] px-4 text-white hover:bg-[#a01020]" style="font-size:13px;font-weight:600">Save intake semester</button></div>
        </div>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="ScriptsPlaceholder" runat="server">
    <script src="<%= ResolveUrl("~/js/admin/shared/icons.js") %>"></script><script src="<%= ResolveUrl("~/js/admin/shared/toast.js") %>"></script><script src="<%= ResolveUrl("~/js/admin/shared/table.js") %>"></script><script src="<%= ResolveUrl("~/js/admin/shared/ui.js") %>"></script>
    <script>
    (function(){
      var events=<%= CalendarEventsJson %>, cursor=events.length?new Date(parse(events[0].StartDate)):new Date();
      cursor=new Date(cursor.getFullYear(),cursor.getMonth(),1);
      function parse(v){var m=/Date\((\d+)\)/.exec(v||"");return m?new Date(+m[1]):new Date(v)}
      function iso(d){return d.getFullYear()+"-"+String(d.getMonth()+1).padStart(2,"0")+"-"+String(d.getDate()).padStart(2,"0")}
      function field(n){return document.querySelector("#semester-modal [data-field='"+n+"']")}
      function set(n,v){if(field(n))field(n).value=v||""}
      // Flags the first empty required field (toast + focus) and returns false so save can stop before posting.
      function requireFields(specs){for(var i=0;i<specs.length;i++){var el=field(specs[i].name),val=el?String(el.value).trim():"";if(!val){if(window.toast)window.toast.error(specs[i].label+" is required");if(el&&el.focus)el.focus();return false}}return true}
      function selected(name){var x=document.querySelector("[data-table-filter='"+name+"']");return x?x.value:""}
      function sync(name,value){document.querySelectorAll("[data-table-filter='"+name+"']").forEach(function(x){x.value=value})}
      function fail(error,message){if(window.toast)window.toast.error(error&&error.message?error.message:message)}
      function post(method,payload){return fetch("academic_calendar.aspx/"+method,{method:"POST",headers:{"Content-Type":"application/json; charset=utf-8"},credentials:"same-origin",body:JSON.stringify(payload||{})}).then(function(r){return r.json().then(function(json){var data=json&&json.d?json.d:json;if(!r.ok||(data&&data.ok===false))throw new Error((data&&data.message)||"Request failed");return data})})}
      function updateDateLimits(){var start=field("startDate"),end=field("endDate");if(!start||!end)return;end.min=start.value||"";start.max=end.value||"";if(start.value&&end.value&&end.value<=start.value)end.setCustomValidity("Semester end date must be after the classes begin date.");else end.setCustomValidity("")}
      function validateDates(){updateDateLimits();var start=field("startDate"),end=field("endDate");if(!start.value){start.reportValidity();throw new Error("Classes begin date is required.");}if(!end.value){end.reportValidity();throw new Error("Semester end date is required.");}if(end.value<=start.value){end.reportValidity();throw new Error("Semester end date must be after the classes begin date.");}}
      function render(){var g=document.querySelector("[data-calendar-grid]"),t=document.querySelector("[data-calendar-title]");t.textContent=cursor.toLocaleDateString(undefined,{month:"long",year:"numeric"});var y=cursor.getFullYear(),m=cursor.getMonth(),off=(new Date(y,m,1).getDay()+6)%7,days=new Date(y,m+1,0).getDate(),html="",intake=selected("intake"),sem=selected("sem"),type=selected("type");for(var b=0;b<off;b++)html+='<div class="min-h-[90px] rounded-lg bg-slate-50/40"></div>';for(var d=1;d<=days;d++){var key=iso(new Date(y,m,d)),items=events.filter(function(e){return iso(parse(e.StartDate))===key&&(!intake||e.IntakeId===intake)&&(!sem||e.SessionId===sem)&&(!type||e.Type===type)});html+='<div class="min-h-[90px] rounded-lg border border-slate-200 p-1.5"><div class="text-slate-700" style="font-size:12px">'+d+'</div>';items.forEach(function(e){html+='<button type="button" data-edit-session="'+e.SessionId+'" class="mt-1 block w-full truncate rounded bg-[#e0162b]/10 px-1.5 py-1 text-left text-[#a01020]" style="font-size:10px;font-weight:600">'+e.Type+'</button>'});html+='</div>'}g.innerHTML=html}
      function semesterNumber(label){var m=/(\d+)/.exec(label||"");return m?m[1]:""}
      function loadSession(id){var e=events.find(function(x){return x.SessionId===id});if(!e)return;set("sessionId",e.SessionId);set("intakeId",e.IntakeId);set("intakeMonth",iso(parse(e.IntakeMonth)).slice(0,7));set("semester",semesterNumber(e.SemesterName));set("startDate",iso(parse(e.SessionStartDate)));set("endDate",iso(parse(e.SessionEndDate)));set("status",e.Status==="Completed"?"Completed":"Scheduled");set("minCredits",e.MinCredits);set("maxCredits",e.MaxCredits);updateDateLimits()}
      document.addEventListener("click",function(ev){var p=ev.target.closest("[data-calendar-prev]"),n=ev.target.closest("[data-calendar-next]"),today=ev.target.closest("[data-calendar-today]"),edit=ev.target.closest("[data-edit-session],[data-calendar-edit]"),del=ev.target.closest("[data-calendar-delete]");if(p){cursor.setMonth(cursor.getMonth()-1);render()}else if(n){cursor.setMonth(cursor.getMonth()+1);render()}else if(today){cursor=new Date(new Date().getFullYear(),new Date().getMonth(),1);render()}else if(edit){loadSession(edit.dataset.editSession||(edit.closest("tr")&&edit.closest("tr").dataset.sessionId))}else if(del){ev.preventDefault();ev.stopImmediatePropagation();if(confirm("Delete this intake semester and its lifecycle?"))post("DeleteAcademicSession",{sessionId:del.dataset.sessionId}).then(function(){location.reload()}).catch(function(error){fail(error,"Could not delete intake semester.")})}else if(ev.target.closest("[data-new-semester]")){["sessionId","intakeId","intakeMonth","startDate","endDate","minCredits","maxCredits"].forEach(function(x){set(x,"")});set("semester","1");set("status","Scheduled");updateDateLimits()}else if(ev.target.closest("[data-semester-save]")){try{validateDates()}catch(error){fail(error,"Check the dates and try again.");return}var month=field("intakeMonth").value,name=month?new Date(month+"-01T00:00:00").toLocaleDateString("en-US",{month:"long",year:"numeric"}).toUpperCase():"",semNum=field("semester").value;post("SaveAcademicSession",{request:{sessionId:field("sessionId").value,intakeId:field("intakeId").value,intakeName:name,intakeMonth:month+"-01",semester:semNum?"Semester "+semNum:"",startDate:field("startDate").value,endDate:field("endDate").value,status:field("status").value,minCredits:field("minCredits").value,maxCredits:field("maxCredits").value}}).then(function(){location.reload()}).catch(function(error){fail(error,"Check the dates and try again.")})}},true);
      document.addEventListener("change",function(ev){if(ev.target.matches("#semester-modal [data-field='startDate'],#semester-modal [data-field='endDate']"))updateDateLimits()});
      document.querySelectorAll("[data-table-filter]").forEach(function(x){x.addEventListener("change",function(){sync(x.dataset.tableFilter,x.value);render()})});render();
    })();
    </script>
</asp:Content>
