using Serilog.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.Views;

namespace Genesis
{
    public static class WpfSyncExtension
    {
        public static LoggerConfiguration WpfSync(this LoggerSinkConfiguration loggerConfiguration, MainWindow mw) =>
            loggerConfiguration.Sink(new WpfSync(mw));
    }
}
