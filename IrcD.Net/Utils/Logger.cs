using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrcD.Database;
using System.Diagnostics;

namespace IrcD.Utils
{
    class Logger
    {
        public static void Log(string message, int level = 4)
        {
            StackTrace stackTrace = new StackTrace();
            var callerFrame = stackTrace.GetFrame(1);
            
            var entity = new Log();
            
            entity.Level = level;
            entity.Message = message;
            entity.Location = callerFrame.GetFileName() + " on Line " + callerFrame.GetFileLineNumber();
            entity.Time = DateTime.Now;

            DatabaseCommon.Db.Logs.InsertOnSubmit(entity);
            DatabaseCommon.Db.SubmitChanges();
        }
    }
}
