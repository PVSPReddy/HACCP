namespace HACCP.Core
{
    public class ServiceResponse
    {
        public ServiceResponse()
        {
            IsSuccess = false;
            ErrorCode = 0;
            Message = null;
        }

        public bool IsSuccess { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public object Results { get; set; }
    }
}