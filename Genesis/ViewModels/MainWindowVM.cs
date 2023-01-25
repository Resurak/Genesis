using GenesisLib;
using GenesisLib.Sync;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public MainWindowVM() 
        {
            Logs = new ObservableCollection<LogData>();
            TestCommand = new RelayCommand(async _ => await DoTest());
        }

        public ShareData ShareData { get; set; }

        public RelayCommand TestCommand { get; set; }
        public ObservableCollection<LogData> Logs { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task DoTest()
        {
            try
            {
                var sourceRoot = @"C:\Users\danie\Desktop\root";
                var destinationRoot = @"C:\Users\danie\Desktop\share test";

                var sw = Stopwatch.StartNew();
                var source = new ShareData(sourceRoot);
                var destination = new ShareData(destinationRoot);

                await source.Update();
                await destination.Update();

                sw.Stop();
                Log.Information("Shares created in {ms}ms", sw.ElapsedMilliseconds);

                await destination.CompareShares(source);
                var json = JsonConvert.SerializeObject(destination, Formatting.Indented);
                Log.Information("{json}", json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "boh");
            }
        }
    }
}
