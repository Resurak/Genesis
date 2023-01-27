using GenesisLib;
using GenesisLib.Sync;
using GenesisLib.Sync_old;
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

        public RelayCommand TestCommand { get; set; }
        public ObservableCollection<LogData> Logs { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task DoTest()
        {
            var sourceRoot = @"C:\Users\danie\Desktop\root";
            var destinationRoot = @"C:\Users\danie\Desktop\share test";

            var sourceShare = new Share(sourceRoot);
            var destinationShare = new Share(destinationRoot);

            await sourceShare.Update();
            await destinationShare.Update();

            destinationShare.CompareShare(sourceShare);
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(destinationShare, Formatting.Indented);
                Log.Information("{json}", json);
            });
        }
    }
}
