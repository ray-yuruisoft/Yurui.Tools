using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yurui.Tools
{
    public interface ILogger
    {
        void Debug(string msg, params object[] args);
        void Debug(string msg, Exception err);
        void Info(string msg, params object[] args);
        void Info(string msg, Exception err);
        void Trace(string msg, params object[] args);
        void Trace(string msg, Exception err);
        void Error(string msg, params object[] args);
        void Error(string msg, Exception err);
        void Fatal(string msg, params object[] args);
        void Fatal(string msg, Exception err);
    }
}
