using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MCIB.Libs
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
        private IntPtr hHook { get; set; }
        private IntPtr Handle { get; }
        private LowLevelKeyboardProcDelegate hookProc { get; set; }
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
        private HookManager()
        {
            Handle = GetModuleHandle(IntPtr.Zero);
        }
        public void Hook()
        {
            hookProc = new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc);
            hHook = SetWindowsHookEx(13, hookProc, Handle, 0);
        }
        private int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    Debug.WriteLine($"vkCode:{lParam.vkCode};flags:{lParam.flags}");
                    switch (wParam)
                    {
                        case 256:
                        case 257:
                        case 260:
                        case 261:
                            if ((lParam.vkCode == 0x09 && lParam.flags == 32) || (lParam.vkCode == 0x1b && lParam.flags == 32)
                                || (lParam.vkCode == 0x20 && lParam.flags == 32) || (lParam.vkCode == 0x2e && lParam.flags == 32)
                              || (lParam.vkCode == 0x73 && lParam.flags == 32) || (lParam.vkCode == 0x1b && lParam.flags == 0)
                              || (lParam.vkCode == 0x5b && lParam.flags == 1) || (lParam.vkCode == 0x5c && lParam.flags == 1)
                              || (lParam.vkCode == 0x2e && lParam.flags == 0))
                                return 1;
                            break;
                    }
                }
            }
            catch { }
            return CallNextHookEx(0, nCode, wParam, ref lParam);
        }
        public void Release()
        {
            UnhookWindowsHookEx(hHook);
        }
        public bool GetTaskManagerStatus()
        {
            RegistryKey regkey = default;
            try
            {
                regkey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                var regValue = regkey.GetValue("DisableTaskMgr").ToString();
                int.TryParse(regValue, out int result);
                return result == 0;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("EnableTaskManager", ex);
                return false;
            }
            finally
            {
                regkey.Close();
            }
        }
        public void EnableTaskManager()
        {
            try
            {
                var regkey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                regkey.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
                regkey.Close();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("EnableTaskManager", ex);
            }
        }
        public void DisableTaskManager()
        {
            try
            {
                var regkey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                regkey.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                regkey.Close();
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogError("DisableTaskManager", ex);
            }
        }
    }
}