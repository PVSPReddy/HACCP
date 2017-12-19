namespace HACCP.Core.Models
{
    public class UserPasswordFocusMessage
    {
        public UserPasswordFocusMessage(bool needFocus)
        {
            NeedFocus = needFocus;
        }

        public bool NeedFocus { get; set; }
    }
}