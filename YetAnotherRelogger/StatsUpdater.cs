using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using YetAnotherRelogger.Helpers;
using YetAnotherRelogger.Helpers.Bot;
using YetAnotherRelogger.Helpers.Stats;
using YetAnotherRelogger.Helpers.Tools;
using YetAnotherRelogger.Properties;

namespace YetAnotherRelogger
{
    public sealed class StatsUpdater
    {
        #region singleton
        private static StatsUpdater _instance = new StatsUpdater();
        public static StatsUpdater Instance => _instance ?? (_instance = new StatsUpdater());
        private StatsUpdater()
        {
        }
        #endregion

        private Thread _statsUpdater;

        public void Start()
        {
            if (_statsUpdater != null && _statsUpdater.IsAlive)
                return;
            _statsUpdater = new Thread(StatsUpdaterWorker) { IsBackground = true, Name = "StatsUpdaterWorker" };
            _statsUpdater.Start();
        }

        public void Stop()
        {
            _statsUpdater?.Abort();
        }

        public void StatsUpdaterWorker()
        {
            // Wait here till mainform is up
            while (Program.Mainform == null || !Program.Mainform.IsHandleCreated)
                Thread.Sleep(100);

            var usages = new CpuRamUsage();
            var totalRam = PerformanceInfo.GetTotalMemory();

            PrepareMainGraphCpu();
            PrepareMainGraphMemory();
            PrepareMainGraphGold();
            while (true)
            {
                // Update Cpu/Ram Usage
                usages.Update();

                double diabloCpuUsage = 0;
                long diabloRamUsage = 0;
                double demonbuddyCpuUsage = 0;
                long demonbuddyRamUsage = 0;
                double goldPerHour = 0;
                double totalGold = 0;
                lock (BotSettings.Instance)
                {
                    foreach (var bot in BotSettings.Instance.Bots.ToList())
                    {
                        var chartStats = bot.ChartStats;
                        // Update bot uptime
                        bot.RunningTime = bot.IsRunning
                            ? DateTime.UtcNow.Subtract(bot.StartTime).ToString(@"hh\hmm\mss\s")
                            : "";

                        // Update bot specific Chart stats
                        CreateChartStats(bot, Program.Mainform.GoldStats, ChartValueType.Double);

                        if (bot.IsRunning)
                        {
                            #region Calculate System Usage

                            if (bot.Diablo.IsRunning)
                            {
                                // Calculate total Cpu/Ram usage for Diablo
                                try
                                {
                                    var usage = usages.GetUsageById(bot.Diablo.Proc.Id);
                                    diabloCpuUsage += usage.Cpu;
                                    diabloRamUsage += usage.Memory;
                                }
                                catch(Exception)
                                {
                                }
                            }

                            if (bot.Demonbuddy.IsRunning)
                            {
                                // Calculate total Cpu/Ram usage for Demonbuddy
                                try
                                {
                                    var usage = usages.GetUsageById(bot.Demonbuddy.Proc.Id);
                                    demonbuddyCpuUsage += usage.Cpu;
                                    demonbuddyRamUsage += usage.Memory;
                                }
                                catch (Exception)
                                {
                                }
                            }

                            #endregion

                            #region Gold Stats

                            chartStats.GoldStats.Update(bot); // Update Current bot

                            // Calculate total gold for all bots
                            goldPerHour += chartStats.GoldStats.GoldPerHour;
                            totalGold += chartStats.GoldStats.LastCoinage;

                            var serie = Program.Mainform.GoldStats.Series.FirstOrDefault(x => x.Name == bot.Name);
                            if (serie != null)
                            {
                                UpdateMainformGraph(Program.Mainform.GoldStats, serie.Name,
                                    Math.Round(chartStats.GoldStats.GoldPerHour), (int)Settings.Default.StatsGphHistory,
                                    autoscale: true);
                            }

                            #endregion
                        }
                    }
                }
                try
                {
                    // add to Cpu graph
                    var graph = Program.Mainform.CpuUsage;
                    var allusage = diabloCpuUsage + demonbuddyCpuUsage;
                    UpdateMainformGraph(graph, "All Usage", allusage,
                        legend: $"All usage: {allusage,11:000.0}%",
                        limit: (int)Settings.Default.StatsCPUHistory);
                    UpdateMainformGraph(graph, "Diablo", diabloCpuUsage,
                        legend: $"Diablo: {diabloCpuUsage,16:000.0}%",
                        limit: (int)Settings.Default.StatsCPUHistory);
                    UpdateMainformGraph(graph, "Demonbuddy", demonbuddyCpuUsage,
                        legend: $"Demonbuddy: {demonbuddyCpuUsage,4:000.0}%",
                        limit: (int)Settings.Default.StatsCPUHistory);
                    UpdateMainformGraph(graph, "Total System", Math.Round(usages.TotalCpuUsage, 2),
                        legend: $"Total System: {usages.TotalCpuUsage,2:000.0}%",
                        limit: (int)Settings.Default.StatsCPUHistory);

                    // add to Memory graph
                    graph = Program.Mainform.MemoryUsage;
                    allusage = (double)(diabloRamUsage + demonbuddyRamUsage) / totalRam * 100;
                    var diablousage = (double)diabloRamUsage / totalRam * 100;
                    var demonbuddyusage = (double)demonbuddyRamUsage / totalRam * 100;
                    UpdateMainformGraph(graph, "All Usage", allusage,
                        legend:
                        $"All usage: {((double)(diabloRamUsage + demonbuddyRamUsage) / totalRam * 100),11:000.0}%",
                        limit: (int)Settings.Default.StatsMemoryHistory);
                    UpdateMainformGraph(graph, "Diablo", diablousage,
                        legend: $"Diablo: {diablousage,16:000.0}%",
                        limit: (int)Settings.Default.StatsMemoryHistory);
                    UpdateMainformGraph(graph, "Demonbuddy", demonbuddyusage,
                        legend: $"Demonbuddy: {demonbuddyusage,4:000.0}%",
                        limit: (int)Settings.Default.StatsMemoryHistory);
                    var mem = (double)PerformanceInfo.GetPhysicalUsedMemory() / totalRam * 100;
                    UpdateMainformGraph(graph, "Total System", mem,
                        legend: $"Total System: {mem,2:000.0}%",
                        limit: (int)Settings.Default.StatsMemoryHistory);

                    // add to Gold Graph
                    UpdateMainformGraph(Program.Mainform.GoldStats, "Gph", Math.Round(goldPerHour),
                        legend: $"Gph {Math.Round(goldPerHour)}", autoscale: true,
                        limit: (int)Settings.Default.StatsGphHistory);
                    UpdateMainformLabel(Program.Mainform.CashPerHour,
                        $"{(goldPerHour / 1000000 * (double)Settings.Default.StatsGoldPrice):C2}");
                    UpdateMainformLabel(Program.Mainform.CurrentCash,
                        $"{(totalGold / 1000000 * (double)Settings.Default.StatsGoldPrice):C2}");
                    UpdateMainformLabel(Program.Mainform.TotalGold, $"{totalGold:N0}");
                }
                catch (Exception ex)
                {
                    DebugHelper.Exception(ex);
                }
                Thread.Sleep((int)Settings.Default.StatsUpdateRate);
            }
        }

        private static void UpdateMainformGraph(Chart graph, string serie, double value, int limit = 120, string legend = null,
            bool autoscale = false)
        {
            if (Program.Mainform == null || graph == null)
                return;
            try
            {
                Program.Mainform.Invoke(new Action(() =>
                {
                    try
                    {
                        if (legend != null)
                            graph.Series[serie].LegendText = legend;

                        while (graph.Series[serie].Points.Count < limit)
                        {
                            graph.Series[serie].Points.Add(0);
                        }

                        if (value > 0)
                        {
                            graph.Series[serie].Points.Add(value);
                        }
                        else
                        {
                            graph.Series[serie].Points.Add(0);
                        }

                        while (graph.Series[serie].Points.Count > limit)
                        {
                            graph.Series[serie].Points.RemoveAt(0);
                        }
                        if (autoscale)
                        {
                            graph.ChartAreas[0].AxisY.Minimum = double.NaN;
                            graph.ChartAreas[0].AxisY.Maximum = double.NaN;
                            graph.ChartAreas[0].RecalculateAxesScale();
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Exception(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }

        private void UpdateMainformLabel(Label label, string value)
        {
            if (Program.Mainform == null || label == null)
                return;
            try
            {
                Program.Mainform.Invoke(new Action(() =>
                {
                    try
                    {
                        label.Text = value;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Exception(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }

        #region Chart Stats Per Bot Creation

        private static readonly Color[] s_chartColors =
        {
            Color.LightSteelBlue,
            Color.Teal,
            Color.Yellow,
            Color.Red,
            Color.LimeGreen,
            Color.Goldenrod,
            Color.DeepSkyBlue,
            Color.DeepPink,
            Color.Magenta,
            Color.DarkSeaGreen,
            Color.DarkRed,
            Color.DarkOrchid,
            Color.DarkOrange
        };

        private void CreateChartStats(Bot bot, Chart graph, ChartValueType valueType = ChartValueType.Auto)
        {
            if (Program.Mainform != null && graph != null)
            {
                try
                {
                    Program.Mainform.Invoke(new Action(() =>
                    {
                        try
                        {
                            if (bot.IsRunning)
                            {
                                var serie = graph.Series.FirstOrDefault(x => x.Name == bot.Name);
                                if (serie == null)
                                {
                                    // Add Series
                                    graph.Series.Add(bot.Name);
                                    graph.Series[bot.Name].ChartType = SeriesChartType.FastLine;
                                    graph.Series[bot.Name].Points.Add(0);
                                    graph.Series[bot.Name].YAxisType = AxisType.Primary;
                                    graph.Series[bot.Name].YValueType = valueType;
                                    graph.Series[bot.Name].IsXValueIndexed = false;

                                    graph.Series[bot.Name].Color = Color.Black;
                                    foreach (
                                        var color in
                                            s_chartColors.Where(color => graph.Series.All(x => x.Color != color))
                                        )
                                        graph.Series[bot.Name].Color = color;
                                    graph.Series[bot.Name].Name = bot.Name;
                                }
                            }
                            else
                            {
                                var serie = graph.Series.FirstOrDefault(x => x.Name == bot.Name);
                                if (serie != null)
                                    graph.Series.Remove(serie);
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugHelper.Exception(ex);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    DebugHelper.Exception(ex);
                }
            }
        }

        #endregion

        #region Gold Stats

        private void PrepareMainGraphGold()
        {
            if (Program.Mainform == null || Program.Mainform.GoldStats == null)
                return;
            try
            {
                Program.Mainform.Invoke(new Action(() =>
                {
                    try
                    {
                        // Clear mainform stats
                        var graph = Program.Mainform.GoldStats;
                        graph.Series.Clear();
                        graph.Palette = ChartColorPalette.Pastel;
                        graph.Titles.Clear();
                        graph.Titles.Add("Gold Statistics");
                        // Add Series
                        graph.Series.Add("Gph");
                        graph.Series["Gph"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Gph"].Points.Add(0);
                        graph.Series["Gph"].YAxisType = AxisType.Primary;
                        graph.Series["Gph"].YValueType = ChartValueType.Auto;
                        graph.Series["Gph"].IsXValueIndexed = false;
                        graph.Series["Gph"].Color = Color.DarkSlateBlue;

                        graph.ResetAutoValues();
                        graph.ChartAreas[0].AxisY.Minimum = 0;
                        graph.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
                        graph.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Exception(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }

        #endregion
        
        #region CPU Graph

        private void PrepareMainGraphCpu()
        {
            if (Program.Mainform == null || Program.Mainform.CpuUsage == null)
                return;
            try
            {
                Program.Mainform.Invoke(new Action(() =>
                {
                    try
                    {
                        // Clear mainform stats
                        var graph = Program.Mainform.CpuUsage;
                        graph.Series.Clear();
                        graph.Palette = ChartColorPalette.Pastel;
                        graph.Titles.Clear();
                        graph.Titles.Add("Processor Usage");
                        // Add Series
                        graph.Series.Add("All Usage");
                        graph.Series["All Usage"].ChartType = SeriesChartType.FastLine;
                        graph.Series["All Usage"].Points.Add(0);
                        graph.Series["All Usage"].YAxisType = AxisType.Primary;
                        graph.Series["All Usage"].YValueType = ChartValueType.Double;
                        graph.Series["All Usage"].IsXValueIndexed = false;
                        graph.Series["All Usage"].Color = Color.DarkSlateBlue;

                        graph.Series.Add("Demonbuddy");
                        graph.Series["Demonbuddy"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Demonbuddy"].Points.Add(0);
                        graph.Series["Demonbuddy"].YAxisType = AxisType.Primary;
                        graph.Series["Demonbuddy"].YValueType = ChartValueType.Double;
                        graph.Series["Demonbuddy"].IsXValueIndexed = false;
                        graph.Series["Demonbuddy"].Color = Color.Red;

                        graph.Series.Add("Diablo");
                        graph.Series["Diablo"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Diablo"].Points.Add(0);
                        graph.Series["Diablo"].YAxisType = AxisType.Primary;
                        graph.Series["Diablo"].YValueType = ChartValueType.Double;
                        graph.Series["Diablo"].IsXValueIndexed = false;
                        graph.Series["Diablo"].Color = Color.Green;

                        graph.Series.Add("Total System");
                        graph.Series["Total System"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Total System"].Points.Add(0);
                        graph.Series["Total System"].YAxisType = AxisType.Primary;
                        graph.Series["Total System"].YValueType = ChartValueType.Double;
                        graph.Series["Total System"].IsXValueIndexed = false;
                        graph.Series["Total System"].Color = Color.SpringGreen;

                        graph.ResetAutoValues();

                        graph.ChartAreas[0].AxisY.Maximum = 100; //Max Y 
                        graph.ChartAreas[0].AxisY.Minimum = 0;
                        graph.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
                        graph.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Exception(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }

        #endregion

        #region Memory Graph

        private void PrepareMainGraphMemory()
        {
            if (Program.Mainform == null || Program.Mainform.MemoryUsage == null)
                return;
            try
            {
                Program.Mainform.Invoke(new Action(() =>
                {
                    try
                    {
                        // Clear mainform stats
                        var graph = Program.Mainform.MemoryUsage;
                        graph.Series.Clear();
                        graph.Palette = ChartColorPalette.Pastel;
                        graph.Titles.Clear();
                        graph.Titles.Add("Memory Usage");
                        // Add Series
                        graph.Series.Add("All Usage");
                        graph.Series["All Usage"].ChartType = SeriesChartType.FastLine;
                        graph.Series["All Usage"].Points.Add(0);
                        graph.Series["All Usage"].YAxisType = AxisType.Primary;
                        graph.Series["All Usage"].YValueType = ChartValueType.Double;
                        graph.Series["All Usage"].IsXValueIndexed = false;
                        graph.Series["All Usage"].Color = Color.DarkSlateBlue;

                        graph.Series.Add("Demonbuddy");
                        graph.Series["Demonbuddy"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Demonbuddy"].Points.Add(0);
                        graph.Series["Demonbuddy"].YAxisType = AxisType.Primary;
                        graph.Series["Demonbuddy"].YValueType = ChartValueType.Double;
                        graph.Series["Demonbuddy"].IsXValueIndexed = false;
                        graph.Series["Demonbuddy"].Color = Color.Red;

                        graph.Series.Add("Diablo");
                        graph.Series["Diablo"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Diablo"].Points.Add(0);
                        graph.Series["Diablo"].YAxisType = AxisType.Primary;
                        graph.Series["Diablo"].YValueType = ChartValueType.Double;
                        graph.Series["Diablo"].IsXValueIndexed = false;
                        graph.Series["Diablo"].Color = Color.Green;

                        graph.Series.Add("Total System");
                        graph.Series["Total System"].ChartType = SeriesChartType.FastLine;
                        graph.Series["Total System"].Points.Add(0);
                        graph.Series["Total System"].YAxisType = AxisType.Primary;
                        graph.Series["Total System"].YValueType = ChartValueType.Double;
                        graph.Series["Total System"].IsXValueIndexed = false;
                        graph.Series["Total System"].Color = Color.SpringGreen;

                        graph.ResetAutoValues();

                        graph.ChartAreas[0].AxisY.Maximum = 100; //Max Y 
                        graph.ChartAreas[0].AxisY.Minimum = 0;
                        graph.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
                        graph.ChartAreas[0].AxisY.IntervalAutoMode = IntervalAutoMode.VariableCount;
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Exception(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                DebugHelper.Exception(ex);
            }
        }

        #endregion
    }
}