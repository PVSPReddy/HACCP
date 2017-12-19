namespace HACCP.Core
{
    public class ProgressMessage
    {
        public ProgressMessage(int uploadcount, int totalcount)
        {
            TotalCount = totalcount;
            UploadCount = uploadcount;
        }

        public int TotalCount { get; set; }
        public int UploadCount { get; set; }
    }
}