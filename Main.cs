using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using Tomlyn;
using Cursor = System.Windows.Forms.Cursor;

namespace JobRoulette
{
    public partial class JobRoulette : Form
    {
        Random rnd = new Random();
        readonly string tomlFile = "JobRoulette.toml";
        public int maxLvl = 0;
        readonly char[] cetrim = { 'L', 'v', 'l' };
        readonly string[] skips = { "name", "index", "bluenable", "tanks", "heals", "dps" };
        private List<CharSettings> charList = new List<CharSettings>();
        private AppSettings appSettings = new AppSettings();
        private CharSettings resetChar;
        private bool firstload = true;
        private bool recentreset = false;
        public JobRoulette()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            
            if (File.Exists("JobRoulette.xml")) { ImportXml("JobRoulette.xml"); }
            else if (!File.Exists(tomlFile)) { SaveSettings(0, "Default"); };

            LoadSettings(0);
        }

        public void LoadSettings(int charIdx) //, bool firstLoad)
        {
            if (firstload)
            {
                appSettings = Toml.ToModel<AppSettings>(File.ReadAllText(tomlFile));

                firstload = false;
                
                int k = 0;
                foreach (var c in appSettings.Characters)
                {
                    string charName = c.Key;
                    CharSettings character = c.Value;

                    if (k == 0)
                        ProfileCB.Items[0] = charName;
                    else
                        ProfileCB.Items.Add(charName);

                    charList.Add(character);
                    k++;
                }

                tsAOT.Checked = appSettings.OnTop; this.TopMost = appSettings.OnTop;
                this.Location = new Point(appSettings.WindowX, appSettings.WindowY);
                maxLevel.Value = appSettings.MaxLevel; bluMaxLvl.Value = appSettings.MaxBluLevel;
                ProfileCB.SelectedIndex = appSettings.LastSelectedChar; charIdx = appSettings.LastSelectedChar;
            }

            var stringPropertyNamesAndValues = charList[charIdx].GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType == typeof(int) && pi.GetGetMethod() != null)
                .Select(pi => new
                {
                    Name = pi.Name.ToLower(),
                    Value = pi.GetGetMethod().Invoke(charList[charIdx], null)
                });

            foreach (var pair in stringPropertyNamesAndValues)
            {
                if (skips.Contains(pair.Name))
                    continue;

                Control c = this.Controls.Find($"{pair.Name}Lvl", false)[0];
                c.Text = pair.Value.ToString();
            }

            var thisChar = GetChar(charList[charIdx].Name, appSettings);

            bluCheck.Checked = thisChar.BLUenable;
            cbTank.Checked = thisChar.Tanks;
            cbHeal.Checked = thisChar.Heals;
            cbDPS.Checked = thisChar.DPS;

            UpdateMaximums();
        }

        public void UpdateMaximums()
        {
            if (maxLvl != ((int)maxLevel.Value)) { maxLvl = ((int)maxLevel.Value); }
            foreach (var nud in this.Controls.OfType<NumericUpDown>()) { nud.Maximum = maxLvl; if (nud.Name.Equals("bluLvl")) { nud.Maximum = bluMaxLvl.Value; } }
        }

        public void ResetHighlights()
        {
            System.Collections.IList list = this.Controls;

            foreach (var lbl in this.Controls.OfType<Label>())
                if (lbl.ForeColor == SystemColors.Highlight)
                    lbl.ForeColor = SystemColors.ControlText;
        }

        public void Roulette(int minLvl, int maxLvl)
        {
            ResetHighlights();

            List<string> jobs = new List<string>();
            string message;

            foreach (var nud in this.Controls.OfType<NumericUpDown>())
                if (nud.Value <= maxLvl && nud.Value >= minLvl)
                {
                    if (nud.Name.Equals("bluLvl") && !(bluCheck.Checked)) { continue; }
                    if (nud.Tag.Equals("Tank") && !(cbTank.Checked)) { continue; }
                    if (nud.Tag.Equals("Heal") && !(cbHeal.Checked)) { continue; }
                    if (nud.Tag.Equals("DPS") && !(cbDPS.Checked)) { continue; }
                    jobs.Add(nud.Name);
                }

            if (jobs.Count == 0)
            {
                if (minLvl == maxLvl) { message = $"No Jobs at level {maxLvl}."; }
                else { message = $"No Jobs between level {minLvl} and {maxLvl}."; }
                ShowToolTip(message, 2000);
            }
            else
            {
                int classNo = rnd.Next(0, jobs.Count);
                string cname = jobs[classNo].TrimEnd(cetrim);

                foreach (var lbl in this.Controls.OfType<Label>())
                    if (lbl.Name.Equals(cname))
                        lbl.ForeColor = SystemColors.Highlight;
            }
        }

        public void SaveSettings(int pIdx, string profile)
        {
            DialogResult r = new DialogResult();
            if (recentreset)
                r = AlertMessage("Form was recently cleared. Do you want to save?\n\nClicking Cancel will restore your settings.");

            if (r == DialogResult.OK)
                recentreset = false;
            else if (r == DialogResult.Cancel)
            {
                charList[ProfileCB.SelectedIndex] = new CharSettings()
                {
                    Name = resetChar.Name,  AST = resetChar.AST, BLM = resetChar.BLM, BLU = resetChar.BLU, BRD = resetChar.BRD,
                    DNC = resetChar.DNC, DRG = resetChar.DRG, DRK = resetChar.DRK, GNB = resetChar.GNB, MCH = resetChar.MCH,
                    MNK = resetChar.MNK, NIN = resetChar.NIN, PCT = resetChar.PCT, PLD = resetChar.PLD, RDM = resetChar.RDM,
                    RPR = resetChar.RPR, SAM = resetChar.SAM, SCH = resetChar.SCH, SGE = resetChar.SGE, SMN = resetChar.SMN,
                    VPR = resetChar.VPR, WAR = resetChar.WAR, WHM = resetChar.WHM, Index = resetChar.Index, Tanks = resetChar.Tanks,
                    Heals = resetChar.Heals, DPS = resetChar.DPS, BLUenable = resetChar.BLUenable
                };
                LoadSettings(ProfileCB.SelectedIndex);
                recentreset = false;
                return;
            }

            appSettings.WindowX = this.Location.X;
            appSettings.WindowY = this.Location.Y;
            appSettings.LastSelectedChar = ProfileCB.SelectedIndex == -1 ? 0 : ProfileCB.SelectedIndex;

            if (pIdx == 0 && profile == "Default")
                charList.Add(new CharSettings());
            else if (pIdx == 0 && profile != "Default")
                charList[0].Name = profile;

            if (appSettings.Characters == null)
                appSettings.Characters = new Dictionary<string, CharSettings>() { { "Default", new CharSettings() } };
            else if (charList.Count > appSettings.Characters.Count)
            {
                int dupes = charList
                    .Where(n => n.Name == "Default")
                    .Count();

                if (dupes > 1)
                    charList.RemoveRange(appSettings.Characters.Count, charList.Count - appSettings.Characters.Count);
            }

            appSettings.Characters.Clear();
            foreach (CharSettings c in charList)
                appSettings.Characters.Add(c.Name, c);

            string toml = Toml.FromModel(appSettings);

            File.WriteAllText(tomlFile, toml);
        }

        public bool CheckCheck()
        {
            int idx = 0;
            foreach (var cb in this.Controls.OfType<CheckBox>()) { if (cb.Checked) { idx++; } }
            if (idx == 0) { return false; }
            else { return true; }
        }
        public void UpdateMenuBar() { tsFiftyPlus.Text = $"50-{(maxLvl - 1).ToString()}"; tsSixtyPlus.Text = $"60-{(maxLvl - 1).ToString()}"; }
        private void ShowToolTip(string message, int duration) { Point mp = Cursor.Position; new ToolTip().Show(message, this, mp.X - this.Location.X + 16, mp.Y - this.Location.Y, duration); }
        private void tsExpert_Click(object sender, EventArgs e) { Roulette(maxLvl, maxLvl); }
        private void tsFiftyPlus_Click(object sender, EventArgs e) { Roulette(50, maxLvl - 1); }
        private void tsSixtyPlus_Click(object sender, EventArgs e) { Roulette(60, maxLvl - 1); }
        private void tsLeveling_Click(object sender, EventArgs e) { Roulette(16, maxLvl - 1); }
        private void tsAll_Click(object sender, EventArgs e) { Roulette(1, maxLvl); }
        private void tsReset_Click(object sender, EventArgs e) { ResetHighlights(); }
        private void ProfileCB_Enter(object sender, EventArgs e) { ShowToolTip("Saving...", 1000); SaveSettings(ProfileCB.SelectedIndex, ProfileCB.SelectedItem.ToString()); }
        private void tsSave_Click(object sender, EventArgs e) { SaveSettings(ProfileCB.SelectedIndex, ProfileCB.SelectedItem.ToString()); }
        private void tsExit_Click(object sender, EventArgs e) { SaveSettings(ProfileCB.SelectedIndex, ProfileCB.SelectedItem.ToString()); Application.Exit(); }
        private void maxLevel_ValueChanged(object sender, EventArgs e) { UpdateMaximums(); UpdateMenuBar(); }
        private void ProfileCB_SelectedIndexChanged(object sender, EventArgs e) { ResetHighlights(); LoadSettings(ProfileCB.SelectedIndex); }

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

            anyLvl_ValueChanged(sender, e);
        }

        private void tsAOT_Click(object sender, EventArgs e)
        {
            if (this.TopMost) { tsAOT.Checked = false; this.TopMost = false; appSettings.OnTop = false; }
            else { tsAOT.Checked = true; this.TopMost = true; appSettings.OnTop = true; }
        }

        private void cbCheckClick(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            bool response = CheckCheck();
            if (!chk.Checked && !response)
                chk.Checked = !chk.Checked;
            
            charList[ProfileCB.SelectedIndex].Tanks = cbTank.Checked;
            charList[ProfileCB.SelectedIndex].Heals = cbHeal.Checked;
            charList[ProfileCB.SelectedIndex].DPS = cbDPS.Checked;
        }

        private void bluCheck_Click(object sender, EventArgs e)
        {
            if (bluLvl.Value == 0)
                bluCheck.Checked = false;
            charList[ProfileCB.SelectedIndex].BLUenable = bluCheck.Checked;
        }

        private void pcbAdd_Click(object sender, EventArgs e)
        {
            SaveSettings(ProfileCB.SelectedIndex, ProfileCB.SelectedItem.ToString());
            int pcbLen = ProfileCB.Items.Count; string psdInput = ""; var psdForm = new psdForm();
            psdForm.Location = new Point(this.Location.X + ((this.Width / 2) - (psdForm.Width / 2)), this.Location.Y + this.Height);
            psdForm.ShowDialog();
            foreach (var psd in psdForm.Controls.OfType<TextBox>())
                if (psd.Name.Equals("psdInput")) { psdInput = psd.Text; continue; }

            if (psdInput == "") { return; }
            else if (ProfileCB.Items.Contains(psdInput)) { ShowToolTip("You can't have two with the same name!", 2000); return; }

            if (ProfileCB.Items[0].ToString() == "Default")
            {
                ProfileCB.Items[0] = psdInput;
                SaveSettings(0, psdInput);
            }
            else
            {
                ProfileCB.Items.Add(psdInput);    //Insert(pcbLen, psdInput);
                charList.Add(new CharSettings() { Name = psdInput, Index = pcbLen });
                SaveSettings(1, psdInput);
                ProfileCB.SelectedIndex = pcbLen;
            }
        }

        private void pcbDel_Click(object sender, EventArgs e)
        {
            if (ProfileCB.Items.Count <= 1) { ShowToolTip("Unable to delete the last character profile.", 2000); return; }
            else
            {
                DialogResult r = AlertMessage("Do you want to remove this character?");
                if (r == DialogResult.Yes)
                {
                    int idx = ProfileCB.SelectedIndex;
                    charList.RemoveAt(idx);
                    for (int i = 0; i < charList.Count; i++) { charList[i].Index = i; }
                    ProfileCB.Items.RemoveAt(idx);
                    SaveSettings(0, ProfileCB.Items[0].ToString());
                    ProfileCB.SelectedIndex = 0;
                }
            }
        }

        private DialogResult AlertMessage(string message)
        {
            using (var mbForm = new mbBody())
            {
                foreach (var item in mbForm.Controls.OfType<Label>())
                    if (item.Name.Equals("mbText"))
                        item.Text = message;

                var result = mbForm.ShowDialog();
                
                return result;
            }
        }

        private void tsFormReset_Click(object sender, EventArgs e)
        {
            resetChar = new CharSettings() 
            { 
                Name = charList[ProfileCB.SelectedIndex].Name, AST = charList[ProfileCB.SelectedIndex].AST, BLM = charList[ProfileCB.SelectedIndex].BLM,
                BLU = charList[ProfileCB.SelectedIndex].BLU, BRD = charList[ProfileCB.SelectedIndex].BRD, DNC = charList[ProfileCB.SelectedIndex].DNC,
                DRG = charList[ProfileCB.SelectedIndex].DRG, DRK = charList[ProfileCB.SelectedIndex].DRK, GNB = charList[ProfileCB.SelectedIndex].GNB,
                MCH = charList[ProfileCB.SelectedIndex].MCH, MNK = charList[ProfileCB.SelectedIndex].MNK, NIN = charList[ProfileCB.SelectedIndex].NIN,
                PCT = charList[ProfileCB.SelectedIndex].PCT, PLD = charList[ProfileCB.SelectedIndex].PLD, RDM = charList[ProfileCB.SelectedIndex].RDM,
                RPR = charList[ProfileCB.SelectedIndex].RPR, SAM = charList[ProfileCB.SelectedIndex].SAM, SCH = charList[ProfileCB.SelectedIndex].SCH,
                SGE = charList[ProfileCB.SelectedIndex].SGE, SMN = charList[ProfileCB.SelectedIndex].SMN, VPR = charList[ProfileCB.SelectedIndex].VPR,
                WAR = charList[ProfileCB.SelectedIndex].WAR, WHM = charList[ProfileCB.SelectedIndex].WHM, Index = charList[ProfileCB.SelectedIndex].Index,
                Tanks = charList[ProfileCB.SelectedIndex].Tanks, Heals = charList[ProfileCB.SelectedIndex].Heals, DPS = charList[ProfileCB.SelectedIndex].DPS,
                BLUenable = charList[ProfileCB.SelectedIndex].BLUenable
            };
           
            DialogResult r = AlertMessage("Are you sure you want to reset this character?");
            if (r == DialogResult.OK)
            {
                recentreset = true;
                foreach (var nud in this.Controls.OfType<NumericUpDown>()) { nud.Minimum = 0; nud.Value = 0; }
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
            else
                return;
        }
        
        private void ImportXml(string xmlFile)
        {
            List<Job> jobList = new List<Job> { };
            List<JRsetting> setList = new List<JRsetting> { };

            var xml = XDocument.Load(xmlFile);
            var cList = xml.Root.Elements("character");

            if (firstload)
            {
                int k = 0;
                foreach (var i in cList)
                {
                    if (ProfileCB.Items.Count < (k + 1))
                        ProfileCB.Items.Add(i.FirstAttribute.Value);
                    else
                        ProfileCB.Items[k] = i.FirstAttribute.Value;

                    k++;
                }
            }

            for (int x = 0; x < cList.ToList().Count; x++)
            {
                CharSettings newChar = new CharSettings() { Name = cList.ToArray()[x].FirstAttribute.Value, Index = x };
                foreach (var i in cList.ToArray()[x].Elements())
                    if (i.Name.LocalName.Equals("job"))
                        newChar.GetType().GetProperty(i.Value.ToUpper()).SetValue(newChar, Int32.Parse(i.FirstAttribute.Value));

                charList.Add(newChar);

                SaveSettings(x, cList.ToArray()[x].FirstAttribute.Value);
            }
            File.Delete(xmlFile);
            firstload = false;
        }

        private void anyLvl_OnFocus(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            nud.Select(0, 3);
        }

        private void anyLvl_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            string job = nud.AccessibleName;

            if (job == "SMN")
                schLvl.Value = smnLvl.Value;
            else if (job == "SCH")
                smnLvl.Value = schLvl.Value;

            if (ProfileCB.SelectedIndex < 0) { ProfileCB.SelectedIndex = 0; }
            //if (!recentreset)
            //{
                CharSettings c = charList[ProfileCB.SelectedIndex];
                c.GetType().GetProperty(job).SetValue(c, Int32.Parse(nud.Value.ToString()));
            //}
        }

        public CharSettings GetChar(string charName, AppSettings settings) { return settings.Characters.TryGetValue(charName, out var user) ? user : null; }
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

    public class AppSettings
    {
        public int MaxLevel { get; set; } = 100;
        public int MaxBluLevel { get; set; } = 80;
        public int WindowX { get; set; } = 0;
        public int WindowY { get; set; } = 0;
        public bool OnTop { get; set; } = false;
        public int LastSelectedChar { get; set; } = 0;
        public Dictionary<string, CharSettings> Characters { get; set; } // = new Dictionary<string, CharSettings>() { { "Default", new CharSettings() } };
    }

    public class CharSettings
    {
        public string Name { get; set; } = "Default";
        public int Index { get; set; } = 0;
        public int PLD { get; set; } = 0;
        public int WAR { get; set; } = 0;
        public int DRK { get; set; } = 0;
        public int GNB { get; set; } = 0;
        public int WHM { get; set; } = 0;
        public int SCH { get; set; } = 0;
        public int AST { get; set; } = 0;
        public int SGE { get; set; } = 0;
        public int MNK { get; set; } = 0;
        public int DRG { get; set; } = 0;
        public int NIN { get; set; } = 0;
        public int SAM { get; set; } = 0;
        public int RPR { get; set; } = 0;
        public int VPR { get; set; } = 0;
        public int BRD { get; set; } = 0;
        public int MCH { get; set; } = 0;
        public int DNC { get; set; } = 0;
        public int BLM { get; set; } = 0;
        public int SMN { get; set; } = 0;
        public int RDM { get; set; } = 0;
        public int PCT { get; set; } = 0;
        public int BLU { get; set; } = 0;
        public bool BLUenable { get; set; } = false;
        public bool Tanks { get; set; } = true;
        public bool Heals { get; set; } = true;
        public bool DPS { get; set; } = true;
    }
}