using System;
using System.Runtime.InteropServices;

namespace MCIB
{
    public sealed class HookManager
    {
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        private delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(IntPtr path);
        private IntPtr hHook;
        private LowLevelKeyboardProcDelegate hookProc;
        private static HookManager instance;
        public static HookManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new HookManager();
                return instance;
            }
        }
        private HookManager() { }
        public void Hook()
        {
            IntPtr hModule = GetModuleHandle(IntPtr.Zero);
            hookProc = new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc);
            hHook = SetWindowsHookEx(13, hookProc, hModule, 0);
        }
        private int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
            {
                switch (wParam)
                {
                    //屏蔽下面的快捷键
                    case 256:
                    case 257:
                    case 260:
                    case 261:
                        if ((lParam.vkCode == 0x1b && lParam.flags == 32) || (lParam.vkCode == 0x20 && lParam.flags == 32) ||
                            (lParam.vkCode == 0x73 && lParam.flags == 32) || (lParam.vkCode == 0x1b && lParam.flags == 0) ||
                            (lParam.vkCode == 0x5b && lParam.flags == 1) || (lParam.vkCode == 0x5c && lParam.flags == 1))
                            return 1;
                        break;
                }
            }
            return CallNextHookEx(0, nCode, wParam, ref lParam);
        }
        public void Release()
        {
            UnhookWindowsHookEx(hHook);
        }
    }
}