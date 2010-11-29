using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DialUpMeter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("dude!, you need to enter an account name", "hey!");
                return;
            }
           
            backgroundWorker1.RunWorkerAsync();
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            listView1.Items.Clear();
            button1.Text = "Wait...";
            groupBox1.Enabled = false;
            EventLog l = new EventLog("Application", Environment.MachineName, "RasClient");
            var eventLogEntryCollection = l.Entries;
            var x = (from EventLogEntry v in eventLogEntryCollection
                     where v.Message.Contains(textBox1.Text) &&
                           (v.Message.Contains("connected") || v.Message.Contains("terminated")) &&
                           (v.TimeGenerated <= endTimePicker.Value &&
                            v.TimeGenerated >= startTimePicker.Value)
                     group v by v.Message.Substring(0, v.Message.IndexOf("}", 10))).ToArray();
            int total = 0;
            for (int i = 1; i < x.Length; i++)
            {
                if (x[i].Count() != 2) continue;
                var z = x[i].ToArray();
                var timeSpan = z[1].TimeGenerated - z[0].TimeGenerated;
                ListViewItem item = new ListViewItem(timeSpan.ToString());
                item.SubItems.Add(z[0].TimeGenerated.ToString());
                item.SubItems.Add(z[1].TimeGenerated.ToString());
                listView1.Items.Add(item);
                total += (int)Math.Ceiling(timeSpan.TotalMinutes);
            }

            label1.Text = "Total Hours: " + (total / 60f);
            this.Refresh();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Calculate";
            groupBox1.Enabled = true;
            if (e.Error!=null)
            {
                MessageBox.Show(e.Error.Message, "Error");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}

