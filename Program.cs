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

        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new MainForm(args));
        }

    }
}
