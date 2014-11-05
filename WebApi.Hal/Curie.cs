using System.Web.Security;

namespace WebApi.Hal
{
    public class Curie : Link
    {
        /// <summary>
        /// the "namespace" for the rel links
        /// </summary>
        public string Name { get; set; }

        public override string Rel
        {
            get { return Name; }
            set { Name = value; }
        }
    }
}