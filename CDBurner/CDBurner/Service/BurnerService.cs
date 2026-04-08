using CDBurner.Service.Common;
using IMAPI2;
using IMAPI2FS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Application = System.Windows.Application;

namespace CDBurner.Service
{
    public class BurnerService : IBurnerService
    {
        public async Task<bool> BurnFolderAsync(string folderPath, IProgress<double> progress = null)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);

            MsftDiscRecorder2 recorder = null;
            MsftFileSystemImage fsImage = null;
            IFileSystemImageResult resultImage = null;
            MsftDiscFormat2Data discFormat = null;

            try
            {
                recorder = new MsftDiscRecorder2();
                recorder.InitializeDiscRecorder(null);

                fsImage = new MsftFileSystemImage();
                fsImage.ChooseImageDefaults((IMAPI2FS.IDiscRecorder2)recorder);
                fsImage.FileSystemsToCreate = FsiFileSystems.FsiFileSystemUDF | FsiFileSystems.FsiFileSystemJoliet;
                fsImage.VolumeName = "DICOM_CD";

                fsImage.Root.AddTree(folderPath, false);
                resultImage = fsImage.CreateResultImage();

                discFormat = new MsftDiscFormat2Data
                {
                    Recorder = recorder,
                    ClientName = Application.Current.Resources["CDBurnerAppName"] as string,
                    ForceMediaToBeClosed = true
                };

                discFormat.Update += (sender, eObj) =>
                {
                    if (eObj is IDiscFormat2DataEventArgs e && e.TotalTime > 0)
                    {
                        double percent = (double)e.ElapsedTime / e.TotalTime * 100;
                        progress?.Report(percent);
                    }
                };

                await Task.Run(() => discFormat.Write((IMAPI2.IStream)resultImage.ImageStream));
                progress?.Report(100);

                return true;
            }
            catch (COMException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Marshal.ReleaseComObject(resultImage);
                Marshal.ReleaseComObject(fsImage);
                Marshal.ReleaseComObject(discFormat);
                Marshal.ReleaseComObject(recorder);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}
