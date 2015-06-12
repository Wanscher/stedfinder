﻿using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GeodataStyrelsen.ArcMap.PlaceFinder
{
    public class AboutButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Danske stednavnes sted finder.");
            stringBuilder.AppendLine();

            // Version
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            stringBuilder.AppendLine("Version:");
            stringBuilder.AppendLine(fileVersionInfo.FileVersion);
            stringBuilder.AppendLine();
            
            // kontakt
            stringBuilder.AppendLine("Udvikling og kontaktpersoner:");
            stringBuilder.AppendLine("Steen Hulthin Rasmussen, stehr@gst.dk");
            stringBuilder.AppendLine("Bjørn Petersen, xbeje@gst.dk");
            stringBuilder.AppendLine();

            // attribution
            stringBuilder.AppendLine("Anvendte resourcer:");
            stringBuilder.AppendLine("Jack Cai's info_black button"); //http://findicons.com/icon/175921/info_black?id=362845

            MessageBox.Show(stringBuilder.ToString(), "Om danske stednavne sted finder", MessageBoxButtons.OK);
        }
    }
}
