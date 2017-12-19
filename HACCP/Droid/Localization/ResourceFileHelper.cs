using System;
using System.IO;
using System.Threading.Tasks;
using HACCP.Core;
using HACCP.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResourceFileHelper))]

namespace HACCP.Droid
{
    public class ResourceFileHelper : IResourceFileHelper
    {
        /// <summary>
        ///     Saves the resource.
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <param name="resourceXML">Resource XM.</param>
        public async Task SaveResource(string filename, string resourceXML)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            File.WriteAllText(filePath, resourceXML);
        }

        /// <summary>
        ///     Loads the resource.
        /// </summary>
        /// <returns>The resource.</returns>
        /// <param name="filename">Filename.</param>
        public async Task<string> LoadResourceAsync(string filename)
        {
            return string.Empty;
        }

        public string LoadResource(string filename)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);
                return File.ReadAllText(filePath);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}