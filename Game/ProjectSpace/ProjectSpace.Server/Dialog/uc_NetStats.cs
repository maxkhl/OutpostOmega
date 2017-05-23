using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.Server.Dialog
{
    partial class uc_NetStats : UserControl
    {
        Task Worker; bool Work = true;
        Main MainForm;
        double DumpTime = 0;
        public uc_NetStats(Main MainForm)
        {
            InitializeComponent();
            Worker = new Task(new Action(UpdateStatistics));
            Worker.Start();
            this.MainForm = MainForm;
            this.MainForm.Statistic.StatisticsUpdated += UpdateStatistics;
            chart.Series.Clear();
            cB_Time.SelectedIndex = 0;
        }

        double LastUpdate = Environment.TickCount / 1000;
        public void UpdateStatistics()
        {
            if (MainForm != null && MainForm.Statistic != null)
            {
                foreach(var key in MainForm.Statistic.Data.Keys)
                {
                    if(!checkedListBox.Items.Contains(key))
                        checkedListBox.Items.Add(key);
                }

                if(chart.InvokeRequired)
                    chart.Invoke(new RefreshDelegate(RefreshChart));
                LastUpdate = (Environment.TickCount - MainForm.Statistic.StartTime) / 1000;
            }
        }

        private delegate void RefreshDelegate();
        private void RefreshChart()
        {
            DumpTime = LastUpdate;
            switch (cB_Time.Text)
            {
                case "10 Seconds":
                    DumpTime -= 10;
                    break;
                case "30 Seconds":
                    DumpTime -= 30;
                    break;
                case "1 Minute":
                    DumpTime -= 60;
                    break;
                case "5 Minutes":
                    DumpTime -= 300;
                    break;
                case "15 Minutes":
                    DumpTime -= 900;
                    break;
                case "1 Hour":
                    DumpTime -= 3600;
                    break;
                case "2 Hours":
                    DumpTime -= 7200;
                    break;
                case "5 Hours":
                    DumpTime -= 18000;
                    break;
                default:
                    DumpTime -= 10;
                    break;
            }
            MainForm.Statistic.DumpTime = DumpTime;

            foreach (var series in chart.Series)
            {
                var data = MainForm.Statistic.Data[series.Name];
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].Time > LastUpdate)
                        series.Points.AddXY(Math.Round(data[i].Time,0), data[i].Value);
                }
                for(int i = 0; i < series.Points.Count; i++)
                {
                    if (series.Points[i].XValue < DumpTime)
                        series.Points.RemoveAt(i);
                }
            }
            chart.ResetAutoValues();
        }

        private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                var name = checkedListBox.Items[e.Index].ToString();
                var Series = chart.Series.Add(name);
                Series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                var data = MainForm.Statistic.Data[Series.Name];
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i].Time > DumpTime)
                        Series.Points.AddXY(Math.Round(data[i].Time, 0), data[i].Value);
                }
            }
            else
            {
                var item = chart.Series.FindByName(checkedListBox.Items[e.Index].ToString());
                if(item != null)
                    chart.Series.Remove(item);
            }
        }
    }
}
