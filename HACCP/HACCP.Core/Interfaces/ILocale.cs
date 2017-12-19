using System.Globalization;

namespace HACCP.Core
{
    public interface ILocale
    {
        CultureInfo GetCurrent();

        void SetLocale();
    }
}