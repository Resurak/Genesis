using Genesis.ViewModels;
using Genesis.Views;
using GenesisLib;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Genesis
{
    public class WpfSync : ILogEventSink
    {
        public WpfSync(MainWindow window) 
        {
            this.DataContext = window.DataContext as MainWindowVM;
        }

        MainWindowVM? DataContext;

        public void Emit(LogEvent logEvent)
        {
            if (DataContext != null)
            {
                App.Current.Dispatcher.Invoke(new Action(() => DataContext.Logs.Add(new LogData(logEvent))));
            }
        }
    }
}
