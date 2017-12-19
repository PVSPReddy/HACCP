namespace HACCP.Core
{
    public class ShowCheckListReviewMessage
    {
        public ShowCheckListReviewMessage(CheckListResponse response, Question question)
        {
            Response = response;
            Question = question;
        }

        public CheckListResponse Response { get; set; }

        public Question Question { get; set; }
    }
}