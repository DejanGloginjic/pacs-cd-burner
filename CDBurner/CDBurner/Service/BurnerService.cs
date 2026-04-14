using CDBurner.Service.Common;
using System;
using System.Collections.Generic;
using System.IO;
using Shell32;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using Application = System.Windows.Application;

namespace CDBurner.Service
{
    public class BurnerService : IBurnerService
    {
        public async Task<bool> BurnFolderAsync(string folderPath, IProgress<double> progress = null)
        {
            // TESTIRAJ SVE
            string tempBurnFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempBurnFolder);

            try
            {
                // 🔹 Static files from bin
                string staticFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles");

                // 🔹 Merge folders
                CopyDirectory(folderPath, Path.Combine(tempBurnFolder, "DICOM"));
                CopyDirectory(staticFilesPath, tempBurnFolder);

                // 🔹 Find DVD drive (NO Shell32)
                var dvdDrive = DriveInfo
                    .GetDrives()
                    .FirstOrDefault(d => d.DriveType == DriveType.CDRom && d.IsReady);

                if (dvdDrive == null)
                    throw new Exception("No DVD drive found.");

                // 🔹 Start Windows burn staging
                var shell = new Shell32.Shell();
                var source = shell.NameSpace(tempBurnFolder);
                var target = shell.NameSpace(dvdDrive.RootDirectory.FullName);

                target.CopyHere(source.Items(), 20);

                // 🔹 Progress (staging estimation)
                await Task.Run(() =>
                {
                    int lastCount = -1;

                    while (true)
                    {
                        int count = Directory.GetFiles(tempBurnFolder, "*", SearchOption.AllDirectories).Length;

                        if (count != lastCount)
                        {
                            progress?.Report(100 - (count * 100.0 / (count + 1)));
                            lastCount = count;
                        }

                        if (count == 0)
                            break;

                        Thread.Sleep(500);
                    }
                });

                progress?.Report(100);
                return true;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempBurnFolder))
                        Directory.Delete(tempBurnFolder, true);
                }
                catch { }
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var dest = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var dest = Path.Combine(targetDir, Path.GetFileName(dir));
                CopyDirectory(dir, dest);
            }
        }
    }
}
