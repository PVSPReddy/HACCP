namespace HACCP.Core
{
    public interface IInfoService
    {
        string ApplicationVersion { get; }
        string GetAppVersion();
    }
}