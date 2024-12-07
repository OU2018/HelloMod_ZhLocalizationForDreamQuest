using System;
using System.Runtime.InteropServices;

namespace HelloMod
{
    public class ClipboardUtility
    {
        // 定义需要的 Windows API 函数
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetClipboardData(uint uFormat);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalSize(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        private const uint CF_UNICODETEXT = 13;
        private const uint GMEM_MOVEABLE = 0x0002;

        // 方法：将字符串复制到剪贴板
        public static void CopyToClipboard(string text)
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                throw new Exception("Unable to open clipboard");
            }

            try
            {
                EmptyClipboard();

                IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)((text.Length + 1) * 2));
                if (hGlobal == IntPtr.Zero)
                {
                    throw new Exception("GlobalAlloc failed");
                }

                IntPtr target = GlobalLock(hGlobal);
                if (target == IntPtr.Zero)
                {
                    throw new Exception("GlobalLock failed");
                }

                try
                {
                    Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
                    Marshal.WriteInt16(target, text.Length * 2, 0); // null 终止符
                }
                finally
                {
                    GlobalUnlock(hGlobal);
                }

                if (SetClipboardData(CF_UNICODETEXT, hGlobal) == IntPtr.Zero)
                {
                    throw new Exception("SetClipboardData failed");
                }
            }
            finally
            {
                CloseClipboard();
            }
        }

        // 方法：从剪贴板读取字符串
        // 方法：从剪贴板读取字符串，不抛出异常，返回空字符串
        public static string GetFromClipboard()
        {
            if (!OpenClipboard(IntPtr.Zero))
            {
                // 无法打开剪贴板时返回空字符串
                return string.Empty;
            }

            string result = string.Empty;

            try
            {
                // 获取剪贴板上的文本数据
                IntPtr handle = GetClipboardData(CF_UNICODETEXT);
                if (handle == IntPtr.Zero)
                {
                    // 剪贴板上没有文本数据，返回空字符串
                    return string.Empty;
                }

                // 锁定全局内存块并获取文本数据
                IntPtr pointer = GlobalLock(handle);
                if (pointer == IntPtr.Zero)
                {
                    // 无法锁定内存块时返回空字符串
                    return string.Empty;
                }

                try
                {
                    // 计算文本的大小，并将其转换为字符串
                    int size = (int)GlobalSize(handle);
                    char[] buffer = new char[size / 2]; // 每个字符占2个字节
                    Marshal.Copy(pointer, buffer, 0, buffer.Length);
                    result = new string(buffer).TrimEnd('\0'); // 去除末尾的null终止符
                }
                finally
                {
                    GlobalUnlock(handle);
                }
            }
            finally
            {
                CloseClipboard();
            }
            return result;
        }

    }


}
