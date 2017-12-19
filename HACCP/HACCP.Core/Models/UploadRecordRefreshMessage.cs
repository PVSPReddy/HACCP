namespace HACCP.Core
{
	public class UploadRecordRefreshMessage
	{
	}

	public class UploadProgressMessage
	{
		public bool IsVisible { get; set; }
		public UploadProgressMessage (bool val)
		{
			IsVisible = val;
		}

	}
}