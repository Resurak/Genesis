using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisLib
{
    public class LogData
    {
        public LogData(LogEvent logEvent)
        {
            this.Timestamp = logEvent.Timestamp.DateTime;
            this.LogLevel = logEvent.Level;

            this.Message = logEvent.RenderMessage();
            this.Exception = logEvent.Exception;
        }

        public DateTime Timestamp { get; set; }
        public LogEventLevel LogLevel { get; set; }

        public string Message { get; set; }
        public Exception? Exception { get; set; }
    }
}
