using System.Threading.Tasks;

namespace HACCP.Core
{
    public interface IResourceFileHelper
    {
        Task SaveResource(string filename, string resourceXml);
        string LoadResource(string filename);
        Task<string> LoadResourceAsync(string filename);
    }
}