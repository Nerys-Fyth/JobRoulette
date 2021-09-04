using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace JobRoulette
{
    public partial class JobRoulette : Form
    {
        Random rnd = new Random();
        public string setFile = "JobRoulette.xml";
        public int maxLvl = 0;
        public char[] cetrim = { 'L', 'v', 'l' };

        public JobRoulette() { InitializeComponent(); LoadSettings(); }
        public void LoadSettings()
        {
            List<Job> jobList = new List<Job> { };
            List<JRsetting> setList = new List<JRsetting> { };

            if (File.Exists(setFile))
            {
                var xml = XDocument.Load(setFile);

                for (int h = 0; h < xml.Root.Descendants().Count(); h++)
                {
                    if (xml.Root.Descendants().ElementAt(h).Name.LocalName.Equals("job"))
                    {
                        string xjob = xml.Root.Descendants().ElementAt(h).Value + "Lvl";
                        string xlvl = xml.Root.Descendants().ElementAt(h).Attribute("level").Value;
                        jobList.Add(new Job() { job = xjob, level = xlvl });
                    }
                    if (xml.Root.Descendants().ElementAt(h).Name.LocalName.Equals("set"))
                    {
                        string xset = xml.Root.Descendants().ElementAt(h).Value;
                        string xval = xml.Root.Descendants().ElementAt(h).Attribute("setting").Value;

                        if (xset == "maxLevel")
                            maxLvl = Int32.Parse(xval);

                        setList.Add(new JRsetting() { name = xset, setting = xval });
                    }
                }

                foreach (var j in jobList)
                {
                    Control c = this.Controls.Find(j.job, false)[0];
                    c.Text = j.level;
                }

                foreach (var s in setList)
                {
                    Control c = setPanel.Controls.Find(s.name, false)[0];

                    if (c.Name.Equals("bluCheck") && s.setting == "False")
                        bluCheck.Checked = false;
                    if (c.Name.Equals("bluCheck") && s.setting == "True")
                        bluCheck.Checked = true;
                    if (!(c is CheckBox))
                        c.Text = s.setting;
                }
            }

            UpdateMaximums();
        }

        public void UpdateMaximums()
        {
            if (maxLvl != ((int)maxLevel.Value))
                maxLvl = ((int)maxLevel.Value);

            foreach (var nud in this.Controls.OfType<NumericUpDown>())
            {
                nud.Maximum = maxLvl;

                if (nud.Name.Equals("bluLvl"))
                    nud.Maximum = bluMaxLvl.Value;
            }
        }

        public void UpdateMenuBar() { tsFiftyPlus.Text = "50-" + (maxLvl - 1).ToString(); tsSixtyPlus.Text = "60-" + (maxLvl - 1).ToString(); }

        public void ResetHighlights()
        {
            System.Collections.IList list = this.Controls;

            foreach (var lbl in this.Controls.OfType<Label>())
            {
                if (lbl.ForeColor == SystemColors.Highlight)
                    lbl.ForeColor = SystemColors.ControlText;
            }
        }

        public void Roulette(int minLvl, int maxLvl)
        {
            ResetHighlights();

            int idx = 0;
            string[] jobs = new string[20];

            foreach (var nud in this.Controls.OfType<NumericUpDown>())
            {
                if (nud.Value <= maxLvl && nud.Value >= minLvl)
                {
                    if (nud.Name.Equals("bluLvl") && !(bluCheck.Checked))
                        continue;

                    jobs[idx] = nud.Name;
                    idx++;
                }

            }

            int classNo = rnd.Next(0, idx);
            string cname = jobs[classNo].TrimEnd(cetrim);

            foreach (var lbl in this.Controls.OfType<Label>())
            {
                if (lbl.Name.Equals(cname))
                    lbl.ForeColor = SystemColors.Highlight;
            }
        }

        public void SaveSettings()
        {
            XmlDocument xml = new XmlDocument();

            List<Job> jobList = new List<Job> { };
            List<JRsetting> setList = new List<JRsetting> { };

            foreach (var nud in this.Controls.OfType<NumericUpDown>())
            {
                string cname = nud.Name.TrimEnd(cetrim);
                string ctext = nud.Text;

                jobList.Add(new Job() { job = cname, level = ctext });
            }

            System.Collections.IList slist = setPanel.Controls;
            for (int i = 0; i < slist.Count; i++)
            {
                Control c = (Control)slist[i];

                if (!(c is NumericUpDown || c is CheckBox))
                    continue;

                string sname = c.Name.ToString();
                string stext = "";

                if (c.Name.Equals("bluCheck")) { stext = bluCheck.Checked.ToString(); }
                else { stext = c.Text.ToString(); }

                setList.Add(new JRsetting() { name = sname, setting = stext });
            }

            var jobElements =
                from j in jobList
                where j != null
                select
                    new XElement("job", j.job,
                        new XAttribute("level", j.level));

            var setElements =
                from s in setList
                select
                    new XElement("set", s.name,
                        new XAttribute("setting", s.setting));

            var doc = new XDocument(
                    new XComment("Settings file for JobRoulette.exe"),
                    new XElement("settings",
                        jobElements,
                        setElements
                    )
                );

            doc.Save(setFile);
        }

        private void tsExpert_Click(object sender, EventArgs e) { Roulette(maxLvl, maxLvl); }

        private void tsFiftyPlus_Click(object sender, EventArgs e) { Roulette(50, maxLvl - 1); }

        private void tsSixtyPlus_Click(object sender, EventArgs e) { Roulette(60, maxLvl - 1); }

        private void tsLeveling_Click(object sender, EventArgs e) { Roulette(16, maxLvl - 1); }

        private void tsAll_Click(object sender, EventArgs e) { Roulette(1, maxLvl); }

        private void tsReset_Click(object sender, EventArgs e) { ResetHighlights(); }

        private void tsSave_Click(object sender, EventArgs e) { SaveSettings(); }

        private void tsExit_Click(object sender, EventArgs e) { SaveSettings(); Application.Exit(); }

        private void maxLevel_ValueChanged(object sender, EventArgs e) { UpdateMaximums(); UpdateMenuBar(); }

        private void smnLvl_ValueChanged(object sender, EventArgs e) { schLvl.Value = smnLvl.Value; }

        private void schLvl_ValueChanged(object sender, EventArgs e) { smnLvl.Value = schLvl.Value; }

        private void tsAOT_Click(object sender, EventArgs e)
        {
            if (this.TopMost) { tsAOT.Checked = false; this.TopMost = false; }
            else { tsAOT.Checked = true; this.TopMost = true; }
        }
    }

    internal class Job
    {
        public string job { get; set; }
        public string level { get; set; }
    }

    internal class JRsetting
    {
        public string name { get; set; }
        public string setting { get; set; }
    }
}