namespace rmd_cs;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Form1 f = new();

        //f.BackColor = Color.Magenta;
        //f.TransparencyKey = Color.Magenta;
        //string toreProcess = "tore.exe";
        //foreach(string arg in args)
        //{
        //    Console.WriteLine(arg);
        //}
        //Thread.Sleep(1000);
        //return;
        //if(args.Length > 0)
        //{
        //    toreProcess = args[0];
        //}

        ProcessStartInfo psi = new("tore.exe");

        psi.RedirectStandardOutput = true;

        Process? tore;
        try
        {
            tore = Process.Start(psi);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] {0}", ex.Message);
            Thread.Sleep(1500);
            return;
        }

        Point sz = new(0);

        f.SuspendLayout();
        string? line;
        //while((line = tore.StandardOutput.ReadLine()) != null)
        for (int i = 0; (line = tore.StandardOutput.ReadLine()) != null; ++i)
        {
            Label lb = new();
            lb.Font = new Font("Segoe UI", 12F);
            lb.AutoSize = true;
            lb.Text = line;
            lb.Location = new Point(12, 9 + (i * lb.Height));
            lb.ForeColor = SystemColors.Control;
            Console.WriteLine("lb.Location[{1}]: {0}", lb.Location, i + 1);
            Console.WriteLine("lb.Size[{1}]: {0}", lb.Size, i + 1);

            f.Controls.Add(lb);
            sz.X = Math.Max(sz.X, lb.Width + 18);
            sz.Y = Math.Max(sz.Y, lb.Location.Y + lb.Height + 15);
            //Console.WriteLine()
            //Console.WriteLine(line);
        }
        f.ResumeLayout(false);


        if (!tore.HasExited)
            tore.Close();
        tore.Dispose();
        //return;

        if (Screen.PrimaryScreen != null)
        {
            Rectangle res = Screen.PrimaryScreen.Bounds;
            Console.WriteLine($"Resolution: ({res.Width}, {res.Height})");

            f.Width = sz.X;
            f.Height = sz.Y;
            Console.WriteLine("sz: {0}", sz);
            Console.WriteLine("f.Width:  {0}", f.Width);
            Console.WriteLine("f.Height: {0}", f.Height);

            int pad = Math.Min((int)((res.Width * .05f)), (int)(res.Height * .05f));

            Point p = new(res.Width - f.Width - pad, pad);

            f.StartPosition = FormStartPosition.Manual;
            Console.WriteLine("Old Form Location: {0}", f.Location);
            Console.WriteLine("New Form Location: {0}", p);

            f.Location = p;
            Console.WriteLine($"Frame Position\nTop: {f.Top}\nLeft: {f.Left}\nWidth: {f.Width}\nHeight: {f.Height}");
        }
        //f.Location = new Point(10, 10);
        Console.WriteLine(f.Location);
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        Application.Run(f);
    }
    public static readonly HWND HWNDNull;
    public static HWND GetWorkerWHWND()
    {
        HWND progman = PInvoke.FindWindow("Progman", null);
        if (progman == HWNDNull)
        {
            Console.WriteLine("[ERROR] FindWindow: {0}", Marshal.GetLastWin32Error());
            return HWNDNull;
        }
        unsafe
        {

            nuint rst = 0;
            LRESULT r = PInvoke.SendMessageTimeout(progman, 0x052C, 0, 0, SEND_MESSAGE_TIMEOUT_FLAGS.SMTO_NORMAL, 1000, &rst);
            Console.WriteLine($"r = {r} | rst = {rst}");
            if (rst != 0 && r != 1)
            {

                Console.WriteLine("[ERROR] SendMessageTimeout: {0}", Marshal.GetLastWin32Error());
                return HWNDNull;
            }

        }

        HWND workerw = HWNDNull;
        PInvoke.EnumWindows(new WNDENUMPROC((tophandle, topparamhandle) =>
        {
            HWND p = PInvoke.FindWindowEx(tophandle, HWNDNull, "SHELLDLL_DefView", null);
            if (p != HWNDNull)
            {
                workerw = PInvoke.FindWindowEx(HWNDNull, tophandle, "WorkerW", null);
                return false;
            }
            return true;
        }), 0);

        if (workerw == HWNDNull) return HWNDNull;
        return workerw;
    }
    /*
    public static bool AttachToDesktop(nint handle)
    {
        Console.WriteLine("Form Handle: 0x{0:X}", handle);
        HWND progman = PInvoke.FindWindow("Progman", null);
        
        if(progman == HWNDNull)
        {
            Console.WriteLine("[ERROR] FindWindow: {0}", Marshal.GetLastWin32Error());
            return false;
        }
        Console.WriteLine("progman: 0x{0:X}", progman);
        unsafe
        {

        nuint rst = 0;
        LRESULT r = PInvoke.SendMessageTimeout(progman, 0x052C, 0, 0, SEND_MESSAGE_TIMEOUT_FLAGS.SMTO_NORMAL, 1000, &rst);
            Console.WriteLine($"r = {r} | rst = {rst}");
            if (rst != 0 && r != 1)
            {

                Console.WriteLine("[ERROR] SendMessageTimeout: {0}", Marshal.GetLastWin32Error());
                return false;
            }

        }

        HWND workerw = HWNDNull;

        PInvoke.EnumWindows(new WNDENUMPROC((tophandle, topparamhandle) => {
            HWND p = PInvoke.FindWindowEx(tophandle, HWNDNull, "SHELLDLL_DefView", null);
            if(p != HWNDNull)
            {
                workerw = PInvoke.FindWindowEx(HWNDNull, tophandle, "WorkerW", null);
                return false;
            }
            return true; 
        }), 0);

        if (workerw == HWNDNull) return false;

        Console.WriteLine("workerw: 0x{0:X}", workerw);
        HWND formHandle;
        unsafe
        { formHandle = new HWND((void*)handle); }
        Console.WriteLine("formHandle: 0x{0:X}", formHandle);
        PInvoke.SetParent(formHandle, workerw);

        return true;
    }
    */
}