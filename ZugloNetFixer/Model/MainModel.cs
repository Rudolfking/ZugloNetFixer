using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ZugloNetFixer.NetworkLayer;
using ZugloNetFixer.Utils;
using ZugloNetFixer.ViewModel;

namespace ZugloNetFixer.Model
{
    public class MainModel
    {
        public MainViewModel MainVM { get; private set; }

        private bool userChanged = false;
        private UseNetworkMode overridenChange = UseNetworkMode.None;

        public MainModel()
        {
            MainVM = new MainViewModel();

            MainVM.EnableMobilnetStatusCommand = new AsyncCommand(() => { return ChangeMobilnetStatusAsync(true, false); });
            MainVM.DisableMobilnetStatusCommand = new AsyncCommand(() => { return ChangeMobilnetStatusAsync(false, false); });

            MainVM.EnableZugloNetStatusCommand = new AsyncCommand(() => { return ChangeZugloNetStatusAsync(true, false); });
            MainVM.DisableZugloNetStatusCommand = new AsyncCommand(() => { return ChangeZugloNetStatusAsync(false, false); });

            MainVM.GoToZugloNetCommand = new AsyncCommand(() => { userChanged = true; return GoToZugloNet(); });
            MainVM.GoToMobilNetCommand = new AsyncCommand(() => { userChanged = true; return GoToMobilNet(); });

            MainVM.ReloadListCommand = new AsyncCommand(() => { return ReloadList(); });

            Init();

            StartTimerLogic();
        }

        private async void Init()
        {
            await ReloadList();
        }

        private void StartTimerLogic()
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += Dt_Tick;
            dt.Interval = TimeSpan.FromMinutes(1);
            dt.IsEnabled = true;
            dt.Start();
        }
        private bool alreadyDone = false;
        private static DayOfWeek[] weekends = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
        private async void Dt_Tick(object sender, EventArgs e)
        {
            await ReloadList();
            await Task.Delay(1000);
            if (MainVM.NetworkAdapters.Count(x => x.Name.ToLower().Contains("ethernet")) < 2)
            {
                // cannot change if no two adapters present, only enable SOMETHING at least
                var isChangedZ = await ChangeZugloNetStatusAsync(true, false);
                var isChanged = await ChangeMobilnetStatusAsync(true, false);
                App.Current.Log("Enabled someting because on two adapters are present: " + (isChanged == ChangeResult.Successful ? "MobilNet enabled" : " ") + (isChanged == ChangeResult.Successful ? "ZugloNet© enabled" : ""));
                return;
            }
            if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 18 && !weekends.Contains(DateTime.Now.DayOfWeek))
            {
                // weekdays, work time: zuglo!
                if (userChanged)
                {
                    if (overridenChange == UseNetworkMode.None)
                        overridenChange = UseNetworkMode.WorkDay;
                    else if (overridenChange != UseNetworkMode.WorkDay)
                    {
                        userChanged = false; // reset!!!!
                        overridenChange = UseNetworkMode.None;
                    }
                    return; // do not revert
                }
                try
                {

                    var isChangedZ = await ChangeZugloNetStatusAsync(true, false);
                    var isChanged = await ChangeMobilnetStatusAsync(false, false);
                    alreadyDone = true;
                    if ((isChanged == ChangeResult.Successful) || (isChangedZ == ChangeResult.Successful))
                        App.Current.Log($"Changed the status of the networks to ZugloNet© automatically!");
                }
                catch (Exception ex)
                {
                    App.Current.Log($"Cannot set automatic ZugloNet© for work: {ex.Message}, trying again later.");
                }
            }
            else if (!weekends.Contains(DateTime.Now.DayOfWeek))
            {
                // weekdays, home time: mobil!
                if (userChanged)
                {
                    if (overridenChange == UseNetworkMode.None)
                        overridenChange = UseNetworkMode.WorkHome;
                    else if (overridenChange != UseNetworkMode.WorkHome)
                    {
                        userChanged = false; // reset!!!!
                        overridenChange = UseNetworkMode.None;
                    }
                    return; // do not revert
                }
                try
                {
                    var isChangedZ = await ChangeZugloNetStatusAsync(false, false);
                    var isChanged = await ChangeMobilnetStatusAsync(true, false);
                    alreadyDone = true;
                    if ((isChanged == ChangeResult.Successful) || (isChangedZ == ChangeResult.Successful))
                        App.Current.Log($"Changed the status of the networks to MobilNet automatically!");
                }
                catch (Exception ex)
                {
                    App.Current.Log($"Cannot set automatic MobilNet for work: {ex.Message}, trying again later.");
                }
            }
            else if (weekends.Contains(DateTime.Now.DayOfWeek))
            {
                // weekends: mobil!
                if (userChanged)
                    if (userChanged)
                    {
                        if (overridenChange == UseNetworkMode.None)
                            overridenChange = UseNetworkMode.Weekend;
                        else if (overridenChange != UseNetworkMode.Weekend)
                        {
                            userChanged = false; // reset!!!!
                            overridenChange = UseNetworkMode.None;
                        }
                        return; // do not revert
                    }
                var isChangedZ = await ChangeZugloNetStatusAsync(false, false);
                var isChanged = await ChangeMobilnetStatusAsync(true, false);
            }
        }

        private async Task ReloadList()
        {
            var nw = new Network();
            var ni = await nw.GetNetworkInterfaces();
            ni.Sort(new NetworkMetricComparer());
            MainVM.NetworkAdapters.Clear();
            foreach (var item in ni)
            {
                App.Current.Log((item.ToString()));
                if (item.AddressFamily?.Contains("4") == true)
                    MainVM.NetworkAdapters.Add(new NetworkAdapterViewModel(item));
                else if (item.IsDisabled == true)
                {
                    // disabled
                    MainVM.NetworkAdapters.Add(new NetworkAdapterViewModel(item));
                }
            }
        }

        public async Task RefreshList()
        {
            await ReloadList();
        }

        private async Task GoToZugloNet()
        {
            var res = await ChangeZugloNetStatusAsync(true, false);
            if (res == ChangeResult.Unsuccessful || res == ChangeResult.NotFound)
                await ChangeMobilnetStatusAsync(true, true);
            else
                await ChangeMobilnetStatusAsync(false, false);
        }

        private async Task GoToMobilNet()
        {
            ChangeResult res = ChangeResult.Unsuccessful;
            try
            {
                res = await ChangeMobilnetStatusAsync(true, false);
            }
            catch (InvalidOperationException) { }
            if (res == ChangeResult.Unsuccessful || res == ChangeResult.NotFound)
                await ChangeZugloNetStatusAsync(true, true);
            else
                await ChangeZugloNetStatusAsync(false, false);
        }

        private async Task<ChangeResult> ChangeZugloNetStatusAsync(bool changeTo, bool forced = true)
        {
            var zNet = MainVM.NetworkAdapters.FirstOrDefault(x => x.Name.ToLower().Equals("ethernet"));
            if (zNet == null)
                return ChangeResult.NotFound;

            if (!forced && changeTo == !zNet.IsDisabled)
                return ChangeResult.AlreadyInTheState;

            try
            {
                if (changeTo)
                {
                    zNet.EnableCommand.Execute(this);
                }
                else
                {
                    zNet.DisableCommand.Execute(this);
                }
            }
            catch
            {
                return ChangeResult.Unsuccessful;
            }
            await ReloadList();
            return ChangeResult.Successful;
        }

        private async Task<ChangeResult> ChangeMobilnetStatusAsync(bool changeTo, bool forced = false)
        {
            ChangeResult changed = ChangeResult.NotFound;
            MainVM.NetworkAdapters.Where(x =>
            {
                var spl = x.Name.ToLower().Split(' ');
                if (spl.Length < 2)
                    return false;
                var rt = int.TryParse(spl[1], out _);
                return spl[0].Equals("ethernet") && rt == true;
            }).ForEach(x =>
            {
                if (!forced && changeTo == !x.IsDisabled)
                {
                    changed = ChangeResult.AlreadyInTheState;
                    return;
                }
                changed = ChangeResult.Successful;
                try
                {
                    if (changeTo)
                        x.EnableCommand.Execute(this);
                    else
                        x.DisableCommand.Execute(this);
                }
                catch
                {
                    changed = ChangeResult.Unsuccessful;
                }
            });

            if (changed == ChangeResult.Successful)
                await ReloadList();
            return changed;
        }
    }

    enum ChangeResult
    {
        AlreadyInTheState, NotFound, Unsuccessful, Successful
    }

    enum UseNetworkMode
    {
        WorkDay, WorkHome, Weekend, None
    }
}
