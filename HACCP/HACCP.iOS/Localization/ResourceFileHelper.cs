using System;
using System.IO;
using System.Threading.Tasks;
using HACCP.Core;
using HACCP.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResourceFileHelper))]

namespace HACCP.iOS
{
    public class ResourceFileHelper : IResourceFileHelper
    {
        public async Task SaveResource(string filename, string resourceXML)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            File.WriteAllText(filePath, resourceXML);
        }

        public async Task<string> LoadResourceAsync(string filename)
        {
            return string.Empty;
        }

        public string LoadResource(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return File.ReadAllText(filePath);
        }
    }
}