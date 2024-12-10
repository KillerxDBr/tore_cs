using System.Reflection.Metadata;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;


namespace rmd_cs;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }
    static readonly HWND HWNDNull;
    private void Form1_Load(object sender, EventArgs e)
    {

        //Console.WriteLine("Form Handle: 0x{0:X}", this.Handle);
        HWND progman = PInvoke.FindWindow("Progman", null);

        //if (progman == HWNDNull)
        //{
        //    Console.WriteLine("[ERROR] FindWindow: {0}", Marshal.GetLastWin32Error());
        //    return false;
        //}
        Console.WriteLine("progman: 0x{0:X}", progman);
        unsafe
        {

            nuint rst = 0;
            LRESULT r = PInvoke.SendMessageTimeout(progman, 0x052C, 0, 0, SEND_MESSAGE_TIMEOUT_FLAGS.SMTO_NORMAL, 1000, &rst);
            //Console.WriteLine($"r = {r} | rst = {rst}");
            //if (rst != 0 && r != 1)
            //{

            //    Console.WriteLine("[ERROR] SendMessageTimeout: {0}", Marshal.GetLastWin32Error());
            //    return false;
            //}

        }

        HWND workerw = HWNDNull;

        PInvoke.EnumWindows(new WNDENUMPROC((tophandle, topparamhandle) => {
            HWND p = PInvoke.FindWindowEx(tophandle, HWNDNull, "SHELLDLL_DefView", null);
            if (p != HWNDNull)
            {
                workerw = PInvoke.FindWindowEx(HWNDNull, tophandle, "WorkerW", null);
                return false;
            }
            return true;
        }), 0);

        //if (workerw == HWNDNull) return false;

        Console.WriteLine("workerw: 0x{0:X}", workerw);
        HWND formHandle;
        unsafe
        { formHandle = new HWND((void*)this.Handle); }
        Console.WriteLine("formHandle: 0x{0:X}", formHandle);
        PInvoke.SetParent(formHandle, workerw);

        //return true;
    }
}
