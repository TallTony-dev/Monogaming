using NativeFileDialogNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monogaming.Monogaming.Utils
{
    internal static class FileSystemUtils
    {
        public static async Task<(bool ok, string path)> AskUserOpenFile(string path)
        {
            using var selectFileDialog = new NativeFileDialog().SelectFile();
            return await ProcessDialog(selectFileDialog, path);
        }
        public static async Task<(bool ok, string path)> AskUserSaveFile(string path)
        {
            using var selectFileDialog = new NativeFileDialog().SaveFile();
            return await ProcessDialog(selectFileDialog, path);
        }

        private static async Task<(bool ok, string path)> ProcessDialog(NativeFileDialog dialog, string path)
        {
            string fullPath = Path.GetFullPath(path, AppContext.BaseDirectory);
            Directory.CreateDirectory(fullPath);

            var tcs = new TaskCompletionSource<(bool, string)>();
            var thread = new Thread(() =>
            {
                try
                {
                    DialogResult result = dialog.Open(out string output, fullPath);
                    tcs.SetResult((result == DialogResult.Okay, output));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            if (OperatingSystem.IsWindows())
            {
                thread.SetApartmentState(ApartmentState.STA);
            }
            thread.IsBackground = true;
            thread.Start();
            return await tcs.Task;
        }




    }
}
