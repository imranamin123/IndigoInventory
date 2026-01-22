using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public static class SessionManager
    {
        private static readonly HashSet<string> ActiveSessions = new HashSet<string>();

        public static void AddSession(string sessionId)
        {
            lock (ActiveSessions)
            {
                ActiveSessions.Add(sessionId);
            }
        }

        public static void RemoveSession(string sessionId)
        {
            lock (ActiveSessions)
            {
                ActiveSessions.Remove(sessionId);
            }
        }

        public static int GetActiveSessionCount()
        {
            lock (ActiveSessions)
            {
                return ActiveSessions.Count;
            }
        }
    }
}