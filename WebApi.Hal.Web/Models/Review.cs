namespace WebApi.Hal.Web.Models
{
    public class Review
    {
        public int Id { get; protected set; }
        public int Beer_Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}