using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RecycleBinManager
{
    class Program
    {
        const string RecycleBinRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel";
        const string RecycleBinRegistryValue = "{645FF040-5081-101B-9F08-00AA002F954E}";

        [DllImport("shell32.dll")]
        public static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("show", StringComparison.OrdinalIgnoreCase))
                {
                    ShowRecycleBin();
                }
                else if (args[0].Equals("hide", StringComparison.OrdinalIgnoreCase))
                {
                    HideRecycleBin();
                }
                else
                {
                    RunRecycleBinManager();
                }
            }
            else
            {
                RunRecycleBinManager();
            }
        }

        static void RefreshDesktop()
        {
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
        }

        static void HideRecycleBin()
        {
            SetRecycleBinVisibility(1);
            RefreshDesktop();
        }

        static void ShowRecycleBin()
        {
            SetRecycleBinVisibility(0);
            RefreshDesktop();
        }

        static void SetRecycleBinVisibility(int visibility)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RecycleBinRegistryPath, true);
            if (key != null)
            {
                key.SetValue(RecycleBinRegistryValue, visibility, RegistryValueKind.DWord);
            }
        }

        static bool IsRecycleBinShowing()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RecycleBinRegistryPath);
            if (key != null)
            {
                object value = key.GetValue(RecycleBinRegistryValue);
                if (value != null && (int)value == 0)
                {
                    return true;
                }
            }
            return false;
        }

        static void RunRecycleBinManager()
        {
            while (true)
            {
                Console.WriteLine(IsRecycleBinShowing() ? "Сейчас отображена иконка корзины" : "Сейчас скрыта иконка корзины");
                Console.WriteLine("1- Показать корзину  2- Скрыть корзину  0- Выход");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    ShowRecycleBin();
                }
                else if (choice == "2")
                {
                    HideRecycleBin();
                }
                else if (choice == "0")
                {
                    break;
                }
            }
        }
    }
}
