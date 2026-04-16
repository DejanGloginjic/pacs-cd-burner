using CDBurner.Model;
using CDBurner.Service.Common;
using IMAPI2;
using IMAPI2FS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;
using Application = System.Windows.Application;

namespace CDBurner.Service
{
    public class BurnerService : IBurnerService
    {
        public async Task<bool> BurnFolderAsync(string dicomPath, long dicomFolderSize, IProgress<double> progress = null)
        {
            if (!Directory.Exists(dicomPath))
                throw new DirectoryNotFoundException(dicomPath);

            MsftDiscMaster2 discMaster = null;
            MsftDiscRecorder2 recorder = null;
            MsftFileSystemImage fsImage = null;
            IFileSystemImageResult result = null;
            MsftDiscFormat2Data dataWriter = null;

            string staticFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles");

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfigModel>(json);

            string weasisPath = Path.Combine(staticFilesPath, config.WeasisFolderName);
            string launcherPath = Path.Combine(staticFilesPath, config.LauncherFolderName);

            try
            {
                discMaster = new MsftDiscMaster2();

                if (discMaster.Count == 0)
                    throw new Exception(Application.Current.Resources["DriveNotFound"] as string);

                string recorderId = discMaster[0];

                recorder = new MsftDiscRecorder2();
                recorder.InitializeDiscRecorder(recorderId);

                fsImage = new MsftFileSystemImage();
                fsImage.ChooseImageDefaults((IMAPI2FS.IDiscRecorder2)recorder);

                fsImage.FileSystemsToCreate = FsiFileSystems.FsiFileSystemUDF;
                fsImage.VolumeName = Application.Current.Resources["DiscVolumeName"] as string;

                try
                {
                    fsImage.Root.AddTree(dicomPath, true);
                    fsImage.Root.AddTree(weasisPath, true);
                    fsImage.Root.AddTree(launcherPath, false);
                }
                catch (COMException ex)
                {
                    if (ex.Message.Contains("larger than the current configured limit"))
                    {
                        throw new Exception(Application.Current.Resources["DiscLimitExceeded"] as string);
                    }

                    throw;
                }

                result = fsImage.CreateResultImage();

                dataWriter = new MsftDiscFormat2Data();
                dataWriter.Recorder = recorder;
                dataWriter.ClientName = config.ClientName;
                dataWriter.ForceMediaToBeClosed = true;

                IMAPI2.IMAPI_MEDIA_PHYSICAL_TYPE mediaType;

                try
                {
                    mediaType = dataWriter.CurrentPhysicalMediaType;
                    Debug.WriteLine(mediaType);
                }
                catch (COMException)
                {
                    throw new Exception(Application.Current.Resources["DiscNotFound"] as string);
                }
                
                if (!dataWriter.MediaHeuristicallyBlank)
                {
                    throw new Exception(Application.Current.Resources["DiscNotEmpty"] as string);
                }


                var writeTask = Task.Run(() =>
                {
                    dataWriter.Write((IMAPI2.IStream)result.ImageStream);
                });

                double progressValue = 0;
                while (!writeTask.IsCompleted)
                {
                    if (progressValue < 90)
                        progressValue += 0.3;

                    progress?.Report(progressValue);

                    await Task.Delay(200);
                }

                await writeTask;
                progress?.Report(100);

                return true;
            }
            catch (COMException ex)
            {
                System.Diagnostics.Debug.WriteLine("IMAPI COM error: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Burn error: " + ex.Message);
                throw;
            }
            finally
            {
                if (dataWriter != null) Marshal.ReleaseComObject(dataWriter);
                if (result != null) Marshal.ReleaseComObject(result);
                if (fsImage != null) Marshal.ReleaseComObject(fsImage);
                if (recorder != null) Marshal.ReleaseComObject(recorder);
                if (discMaster != null) Marshal.ReleaseComObject(discMaster);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}
