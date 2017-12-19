namespace HACCP.Core
{
    public class NextPrevButtonClickMessage
    {
        public NextPrevButtonClickMessage(bool isNext)
        {
            IsNext = isNext;
        }

        public bool IsNext { get; set; }
    }
}