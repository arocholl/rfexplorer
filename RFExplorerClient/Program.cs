//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-15 Ariel Rocholl, www.rf-explorer.com
//
//This application is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 3.0 of the License, or (at your option) any later version.
//
//This software is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//General Public License for more details.
//
//You should have received a copy of the GNU General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace RFExplorerClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string sFile = "";
            if (args.Length > 0)
            {
                sFile = args[0];
            }

            MainForm mainForm = new MainForm(sFile);
            try
            {
                mainForm.m_winAboutModeless = new About_RFExplorer();
                mainForm.m_winAboutModeless.UseWaitCursor = true;
                mainForm.m_winAboutModeless.okButton.Visible = false;
                mainForm.m_winAboutModeless.Show(mainForm);
                Application.DoEvents();
                Thread.Sleep(1000);
            }
            catch (Exception obEx)
            {
                string sReportFile = Environment.GetEnvironmentVariable("APPDATA") + "\\rfexplorer_crash.log";

                using (StreamWriter sr = new StreamWriter(sReportFile, true))
                {
                    sr.WriteLine(Environment.NewLine + Environment.NewLine +
                        "------------------------------------------------------------" + Environment.NewLine +
                        "Report log date " + DateTime.Now.ToString() + Environment.NewLine);
                    sr.WriteLine("Exception: " + obEx.ToString());
                    sr.Flush();
                }
            }

            Application.Run(mainForm);
        }
    }
}
