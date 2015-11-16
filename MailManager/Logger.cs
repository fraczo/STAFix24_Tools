using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace MailManager
{
    public class Logger
    {
        internal static void LogEvent(string subject, string body, TraceSeverity traceSeverity, EventSeverity eventSeverity, uint uid = 0)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;

            diagSvc.WriteTrace(uid,
                new SPDiagnosticsCategory("STAFix category", traceSeverity, eventSeverity),
                traceSeverity,
                subject.ToString() + ":  {0}",
                new object[] { body.ToString() });
        }

        internal static void LogEvent_Exception(string subject, string body, uint uid = 0)
        {
            LogEvent(subject, body, TraceSeverity.Unexpected, EventSeverity.Error, uid);
        }

        internal static void LogEvent_Event(string subject, string body, uint uid = 0)
        {
            LogEvent(subject, body, TraceSeverity.Monitorable, EventSeverity.Information, uid);
        }
    }
}
