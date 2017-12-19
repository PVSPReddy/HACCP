using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using HACCP.Core;
using HACCP.WP;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResourceFileHelper))]

namespace HACCP.WP
{
    public class ResourceFileHelper : IResourceFileHelper
    {
        public string LoadResource(string filename)
        {
            return string.Empty;
        }


        public async Task<string> LoadResourceAsync(string filename)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var textFile = await localFolder.GetFileAsync(filename);

                using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                {
                    string contents;
                    using (var textReader = new DataReader(textStream))
                    {
                        var textLength = (uint) textStream.Size;
                        await textReader.LoadAsync(textLength);
                        contents = textReader.ReadString(textLength);
                    }
                    return contents;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on reading from resource file: " + ex.Message);
            }

            return "";
        }


        public async Task SaveResource(string filename, string text)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var textFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (var textStream = await textFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var textWriter = new DataWriter(textStream))
                    {
                        textWriter.WriteString(text);
                        await textWriter.StoreAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error on creating resource file: {0}" + ex.Message);
            }
        }

        //  //   Task.Delay(300);
        //{


        //public string LoadResource(string filename)


        //    Task<string> tasks = LoadTextAsync(filename);
        //    tasks.Wait();
        //    // HACK: to keep Interface return types simple (sorry!)
        //    return tasks.Result;
        //}
        //async Task<string> LoadTextAsync(string filename)
        //{
        //    string text = string.Empty;
        //    Stream stream = null;
        //    StreamReader reader = null;
        //    try
        //    {

        //        // access the local folder
        //        StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
        //        stream = await local.OpenStreamForReadAsync(filename);

        //        // copy the file contents into the string 'text'
        //        using (reader = new StreamReader(stream))
        //        {
        //            text = reader.ReadToEnd();
        //            return text;
        //        }
        //        await Task.Delay(300);
        //    }
        //    catch (Exception ex)
        //    {

        //        Debug.WriteLine("Error on reading resource file : " + ex.Message);
        //    }
        //   finally
        //    {

        //        stream = null;
        //        reader = null;
        //    }
        //    return text;
        //}
        //public async void SaveResource(string filename, string text)
        //{


        //    StreamWriter writer = null;
        //    //var file;
        //    try
        //    {
        //        StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
        //      var   file = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
        //        using ( writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
        //        {
        //            writer.Write(text);
        //        }
        //       await Task.Delay(700);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error on writing resource file : " + ex.Message);
        //    }
        //   finally
        //    {
        //        writer = null;

        //    }
        //}
    }
}