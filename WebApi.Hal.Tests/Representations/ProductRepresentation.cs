namespace WebApi.Hal.Tests.Representations
{
    public class ProductRepresentation : Representation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public CategoryRepresentation Category { get; set; }
    }
}