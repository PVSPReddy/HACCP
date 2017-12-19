namespace HACCP.Core
{
    public class ShowReviewMessage
    {
        public ShowReviewMessage(ItemTemperature item, LocationMenuItem menuitem)
        {
            Item = item;
            MenuItem = menuitem;
        }

        public ItemTemperature Item { get; set; }

        public LocationMenuItem MenuItem { get; set; }
    }
}