namespace HACCP.Core.WP.ResourceFiles
{
    public class AppResourcesContainer
    {
        private static readonly AppResources Resources = new AppResources();

        public AppResources Strings
        {
            get { return Resources; }
        }
    }
}