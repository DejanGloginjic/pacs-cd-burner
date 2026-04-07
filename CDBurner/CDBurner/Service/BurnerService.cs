using CDBurner.Service.Common;
using IMAPI2;
using IMAPI2FS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CDBurner.Service
{
    public class BurnerService : IBurnerService
    {
        public async Task<bool> BurnFolderAsync(string folderPath, IProgress<double> progress = null)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);

            try
            {
                //var recorder = new MsftDiscRecorder2();
                //recorder.InitializeDiscRecorder(null);

                //var fsImage = new MsftFileSystemImage();
                //fsImage.ChooseImageDefaults((IMAPI2FS.IDiscRecorder2)recorder);
                //fsImage.FileSystemsToCreate = FsiFileSystems.FsiFileSystemJoliet; // OVO PRAVI VELIKE PROBLEME
                //fsImage.VolumeName = "DICOM_CD";

                //fsImage.Root.AddTree(folderPath, false);
                //var resultImage = fsImage.CreateResultImage();


                var fsImage = new MsftFileSystemImage();
                fsImage.Root.AddTree(folderPath, false);   // add your folder
                fsImage.VolumeName = "DICOM_CD";           // optional
                var resultImage = fsImage.CreateResultImage();


                // --- TEST MODE: save ISO to disk instead of burning ---
                var isoStream = resultImage.ImageStream;
                string testIsoPath = Path.Combine("C:\\Users\\Haris Dzumhur (Work)\\Desktop", "Test.iso");

                await Task.Run(() =>
                {
                    byte[] buffer = new byte[4096];
                    uint bytesRead;

                    // Open the file once before reading
                    using (var file = File.Create(testIsoPath))
                    {
                        do
                        {
                            // COM interop requires 'out' on first element
                            isoStream.RemoteRead(out buffer[0], (uint)buffer.Length, out bytesRead);

                            if (bytesRead > 0)
                                file.Write(buffer, 0, (int)bytesRead);

                        } while (bytesRead > 0);
                    }
                });
                System.Diagnostics.Debug.WriteLine($"ISO saved to: {testIsoPath}");

                Marshal.ReleaseComObject(isoStream);
                Marshal.ReleaseComObject(resultImage);
                Marshal.ReleaseComObject(fsImage);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                // KRAJ

                //Marshal.ReleaseComObject(isoStream);
                //Marshal.ReleaseComObject(resultImage);
                //Marshal.ReleaseComObject(fsImage);

                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();

                //var discFormat = new MsftDiscFormat2Data
                //{
                //    Recorder = recorder,
                //    ClientName = "My CD Burner", // Racunari doo narezivac
                //    ForceMediaToBeClosed = true
                //};

                //discFormat.Update += (sender, eObj) =>
                //{
                //    if (eObj is IDiscFormat2DataEventArgs e && e.TotalTime > 0)
                //    {
                //        double percent = (double)e.ElapsedTime / e.TotalTime * 100;
                //        progress?.Report(percent);
                //    }
                //};

                //await Task.Run(() => discFormat.Write((IMAPI2.IStream)resultImage.ImageStream));
                //progress?.Report(100);

                return true;
            }
            catch (COMException exc) // ovo potencijalno brisemo
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                return false;
            }
            catch(Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.Message);
                return false;
            }
        }
    }
}
