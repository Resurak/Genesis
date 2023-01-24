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
            Logs = new ObservableCollection<string>();
            TestCommand = new RelayCommand(async _ => await DoTest());
        }

        public ShareData ShareData { get; set; }

        public RelayCommand TestCommand { get; set; }
        public ObservableCollection<string> Logs { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task DoTest()
        {
            try
            {
                var root = @"C:\Users\danie\Desktop\VanillaExpandedFramework-061022";

                var sw = Stopwatch.StartNew();
                var share = new ShareData(root);
                await share.Update();

                sw.Stop();
                Log.Information("Share created in {ms}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                Log.Information("Total files processed: {num} || Total size: {size}", share.RootPathData.TotalFileCount, share.RootPathData.TotalFileSize);

                sw.Stop();
                Log.Information("Total time necessary to get data recursively: {ms}", sw.ElapsedMilliseconds);

                sw.Restart();
                var json = JsonConvert.SerializeObject(share, Formatting.None);

                sw.Stop();
                Log.Information("Total time to create json: {ms} || Size: {size}", sw.ElapsedMilliseconds, json.Length);

                sw.Restart();
                var data = share.Serialize();

                sw.Stop();
                Log.Information("Total time to create messagePack: {ms} || Size: {size}", sw.ElapsedMilliseconds, data.Length);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "boh");
            }
        }
    }
}
