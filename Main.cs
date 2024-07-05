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

        public JobRoulette() { InitializeComponent(); ProfileCB.SelectedIndex = 0; LoadSettings(0, true); UpdateMaximums(); }

        public void LoadSettings(int charIdx, bool firstLoad)
        {
            List<Job> jobList = new List<Job> { };
            List<JRsetting> setList = new List<JRsetting> { };

            if (!(File.Exists(setFile)))
                SaveSettings(0, "Default");
            else
            {
                var xmltest = XDocument.Load(setFile);

                if (!(xmltest.Element("settings").HasAttributes)) { settingsUpdate(); }

                var xml = XDocument.Load(setFile);
                var charList = xml.Root.Elements("character");

                if (firstLoad)
                {
                    int k = 0;
                    foreach (var i in charList)
                    {
                        if (ProfileCB.Items.Count < (k + 1))
                            ProfileCB.Items.Add(i.FirstAttribute.Value);
                        else
                            ProfileCB.Items[k] = i.FirstAttribute.Value;

                        k++;
                    }
                }
    
                foreach (var i in charList.ToArray()[charIdx].Elements())
                {
                    if (i.Name.LocalName.Equals("job"))
                    {
                        string xjob = i.Value + "Lvl";
                        string xlvl = i.FirstAttribute.Value;
                        jobList.Add(new Job() { job = xjob, level = xlvl });
                    }
                    if (i.Name.LocalName.Equals("set"))
                    {
                        string xset = i.Value;
                        string xval = i.FirstAttribute.Value;
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
                    if (c.Name.Equals("bluCheck") && s.setting == "False") { bluCheck.Checked = false; }
                    if (c.Name.Equals("bluCheck") && s.setting == "True") { bluCheck.Checked = true; }
                    if (!(c is CheckBox)) { c.Text = s.setting; }
                }
            }

            UpdateMaximums();
        }

        public void UpdateMaximums()
        {
            if (maxLvl != ((int)maxLevel.Value)) { maxLvl = ((int)maxLevel.Value); }

            foreach (var nud in this.Controls.OfType<NumericUpDown>())
            {
                nud.Maximum = maxLvl;
                if (nud.Name.Equals("bluLvl")) { nud.Maximum = bluMaxLvl.Value; }
            }
        }

        public void UpdateMenuBar() { tsFiftyPlus.Text = "50-" + (maxLvl - 1).ToString(); tsSixtyPlus.Text = "60-" + (maxLvl - 1).ToString(); }

        public void ResetHighlights()
        {
            System.Collections.IList list = this.Controls;

            foreach (var lbl in this.Controls.OfType<Label>())
            {
                if (lbl.ForeColor == SystemColors.Highlight) { lbl.ForeColor = SystemColors.ControlText; }
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
                    if (nud.Name.Equals("bluLvl") && !(bluCheck.Checked)) { continue; }
                    if (nud.Tag.Equals("Tank") && !(cbTank.Checked)) { continue; }
                    if (nud.Tag.Equals("Heal") && !(cbHeal.Checked)) { continue; }
                    if (nud.Tag.Equals("DPS") && !(cbDPS.Checked)) { continue; }
                    jobs[idx] = nud.Name;
                    idx++;
                }
            }

            if (idx == 0)
            {
                ShowToolTip("No Jobs between " + minLvl + " and " + maxLvl + ".", 2000);
                return;
            }

            int classNo = rnd.Next(0, idx);
            string cname = jobs[classNo].TrimEnd(cetrim);

            foreach (var lbl in this.Controls.OfType<Label>()) { if (lbl.Name.Equals(cname)) { lbl.ForeColor = SystemColors.Highlight; } }
        }

        public void SaveSettings(int pIdx, string profile)
        {
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

                if (!(c is NumericUpDown || c is CheckBox)) { continue; }

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

            if (!(File.Exists(setFile)))
            {
                pIdx = 0; profile = "Default";
                var doc = new XDocument(
                        new XComment("Settings file for JobRoulette.exe"),
                        new XElement("settings",
                            new XAttribute("version", this.ProductVersion),
                                new XElement("character",
                                    new XAttribute("name", profile),
                                    jobElements,
                                    setElements
                                )
                            )
                        );
                doc.Save(setFile);
            }
            else
            {
                var doc = XDocument.Load(setFile);
                doc.Element("settings").SetAttributeValue("version", ProductVersion);
                var oldEle = doc.Element("settings").Elements("character");

                var newEle = new XElement("character",
                                new XAttribute("name", profile),
                                jobElements,
                                setElements
                             );

                int x = 0;
                foreach (XElement oe in oldEle)
                {
                    if (pIdx == 0 && oe.Attribute("name").Value == "Default") { oe.ReplaceWith(newEle); x++; continue; }
                    if (oe.Attribute("name").Value == profile) { oe.ReplaceWith(newEle); x++; continue; }
                }

                if (x == 0)
                    doc.Element("settings").Add(newEle);
                
                doc.Save(setFile);
            }
        }

        public bool CheckCheck()
        {
            int idx = 0;
            foreach (var cb in this.Controls.OfType<CheckBox>()) { if (cb.Checked) { idx++; } }
            if (idx == 0) { return false; }
            else { return true; }
        }

        private void ShowToolTip(string message, int duration) { new ToolTip().Show(message, this, Cursor.Position.X - this.Location.X, Cursor.Position.Y - this.Location.Y, duration); }

        private void tsExpert_Click(object sender, EventArgs e) { Roulette(maxLvl, maxLvl); }

        private void tsFiftyPlus_Click(object sender, EventArgs e) { Roulette(50, maxLvl - 1); }

        private void tsSixtyPlus_Click(object sender, EventArgs e) { Roulette(60, maxLvl - 1); }

        private void tsLeveling_Click(object sender, EventArgs e) { Roulette(16, maxLvl - 1); }

        private void tsAll_Click(object sender, EventArgs e) { Roulette(1, maxLvl); }

        private void tsReset_Click(object sender, EventArgs e) { ResetHighlights(); }

        private void tsSave_Click(object sender, EventArgs e) { SaveSettings(ProfileCB.SelectedIndex, ProfileCB.SelectedItem.ToString()); }

        private void tsExit_Click(object sender, EventArgs e) { SaveSettings(ProfileCB.SelectedIndex, ProfileCB.SelectedItem.ToString()); Application.Exit(); }

        private void maxLevel_ValueChanged(object sender, EventArgs e) { UpdateMaximums(); UpdateMenuBar(); }

        private void smnLvl_ValueChanged(object sender, EventArgs e) { schLvl.Value = smnLvl.Value; }

        private void schLvl_ValueChanged(object sender, EventArgs e) { smnLvl.Value = schLvl.Value; }

        private void minLvl_ValueChanged(object sender, EventArgs e)
        {
            foreach (var nud in this.Controls.OfType<NumericUpDown>())
            {
                if (nud.Name.Equals("drkLvl") && nud.Value > 0 && nud.Value < 30) { drkLvl.Minimum = 30; }
                if (nud.Name.Equals("astLvl") && nud.Value > 0 && nud.Value < 30) { astLvl.Minimum = 30; }
                if (nud.Name.Equals("mchLvl") && nud.Value > 0 && nud.Value < 30) { mchLvl.Minimum = 30; }
                if (nud.Name.Equals("rdmLvl") && nud.Value > 0 && nud.Value < 50) { rdmLvl.Minimum = 50; }
                if (nud.Name.Equals("samLvl") && nud.Value > 0 && nud.Value < 50) { samLvl.Minimum = 50; }
                if (nud.Name.Equals("gnbLvl") && nud.Value > 0 && nud.Value < 60) { gnbLvl.Minimum = 60; }
                if (nud.Name.Equals("dncLvl") && nud.Value > 0 && nud.Value < 60) { dncLvl.Minimum = 60; }
                if (nud.Name.Equals("sgeLvl") && nud.Value > 0 && nud.Value < 70) { sgeLvl.Minimum = 70; }
                if (nud.Name.Equals("rprLvl") && nud.Value > 0 && nud.Value < 70) { rprLvl.Minimum = 70; }
                if (nud.Name.Equals("vprLvl") && nud.Value > 0 && nud.Value < 80) { vprLvl.Minimum = 80; }
                if (nud.Name.Equals("pctLvl") && nud.Value > 0 && nud.Value < 80) { pctLvl.Minimum = 80; }
            }
        }

        private void tsAOT_Click(object sender, EventArgs e)
        {
            if (this.TopMost) { tsAOT.Checked = false; this.TopMost = false; }
            else { tsAOT.Checked = true; this.TopMost = true; }
        }

        private void cbCheckClick(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            bool response = CheckCheck();
            if (!chk.Checked && !response) { chk.Checked = !chk.Checked; }
        }

        private void ProfileCB_SelectedIndexChanged(object sender, EventArgs e) { ResetHighlights(); LoadSettings(ProfileCB.SelectedIndex, false); }

        private void pcbAdd_Click(object sender, EventArgs e)
        {
            int pcbLen = ProfileCB.Items.Count; string psdInput = ""; var psdForm = new psdForm();
            psdForm.Location = new Point(this.Location.X + ((this.Width / 2) - (psdForm.Width / 2)), this.Location.Y + this.Height);
            psdForm.ShowDialog();
            foreach (var psd in psdForm.Controls.OfType<TextBox>())
                if (psd.Name.Equals("psdInput")) { psdInput = psd.Text; continue; }

            if (psdInput == "") { return; }
            
            if (ProfileCB.Items[0].ToString() == "Default")
            {
                ProfileCB.Items.Insert(0, psdInput);
                ProfileCB.Items.RemoveAt(1);
                SaveSettings(0, psdInput);
                ProfileCB.SelectedIndex = pcbLen - 1;
            }
            else
            {
                ProfileCB.Items.Insert(pcbLen, psdInput);
                SaveSettings(pcbLen - 1, psdInput);
                ProfileCB.SelectedIndex = pcbLen;
            }
        }

        private void tsFormReset_Click(object sender, EventArgs e)
        {
            foreach (var nud in this.Controls.OfType<NumericUpDown>()) { nud.Minimum = 0;  nud.Value = 0; }
            foreach (var ctl in setPanel.Controls.OfType<NumericUpDown>())
            {
                if (ctl.Name.Equals("maxLevel"))
                    ctl.Value = 100;
                if (ctl.Name.Equals("bluMaxLvl"))
                    ctl.Value = 80;
            }
            foreach (var ctl in setPanel.Controls.OfType<CheckBox>())
                ctl.Checked = false;
        }

        private void settingsUpdate()
        {
            List<Job> jobList = new List<Job> { };
            List<JRsetting> setList = new List<JRsetting> { };

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
                    if (xset == "maxLevel") { maxLvl = Int32.Parse(xval); }
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
                if (c.Name.Equals("bluCheck") && s.setting == "False") { bluCheck.Checked = false; }
                if (c.Name.Equals("bluCheck") && s.setting == "True") { bluCheck.Checked = true; }
                if (!(c is CheckBox)) { c.Text = s.setting; }
            }

            File.Delete(setFile);
            SaveSettings(0, "Default");
        }

        private void anyLvl_OnFocus(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            nud.Select(0, 2);
        }

        private void blu_Click(object sender, EventArgs e)
        {

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