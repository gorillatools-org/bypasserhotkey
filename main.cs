using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using System.Security.Policy;

namespace FunnyClipboardProject
{
    public class Program
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Bypasser";

            ShowWindow(GetConsoleWindow(), 0);
            //RegisterHotKey(IntPtr.Zero, 1, 0x0002 | 0x0001, (int)Keys.V); // Ctrl + Alt + V
            RegisterHotKey(IntPtr.Zero, 1, 0x0002, (int)Keys.Oemtilde); // Ctrl + Tilda
            Application.Run(new CustomApplicationContext());
        }

        private class CustomApplicationContext : ApplicationContext
        {
            public CustomApplicationContext()
            {
                Application.AddMessageFilter(new HotkeyMessageFilter());
            }
        }

        private class HotkeyMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
                {
                    string ct = GetClipboardText();
                    if (Regex.IsMatch(clipboardText, @"^(http|https)://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?$"))
                    {
                        try
                        {
                            Process.Start("firefox.exe", $"https://bypass.city/bypass?bypass={ct}");
                        }
                        catch (Exception ex)
                        {
                            System.IO.File.AppendAllText("error_log.txt", $"{DateTime.Now}: Error opening Firefox: {ex.Message}\n");
                        }
                    }
                    return true;
                }
                return false;
            }

            private string GetClipboardText()
            {
                if (Clipboard.ContainsText())
                {
                    return Clipboard.GetText();
                }
                return string.Empty;
            }
        }
    }
}
