namespace WebApi.Hal.Tests.Representations
{
    public class OrganisationRepresentation : Representation
    {
        static readonly Link NoAppPath = new Link("organisation", "/api/organisations/{0}");

        public OrganisationRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string Rel
        {
            get { return NoAppPath.Rel; }
            set { }
        }

        public override string Href
        {
            get { return string.Format(NoAppPath.Href, Id); }
            set { }
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
        }
    }

    public class OrganisationWithAppPathRepresentation : Representation
    {
        static readonly Link WithAppPath = new Link("organisation", "~/api/organisations/{0}");

        public OrganisationWithAppPathRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string Rel
        {
            get { return WithAppPath.Rel; }
            set { }
        }

        public override string Href
        {
            get { return string.Format(WithAppPath.Href, Id); }
            set { }
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
        }
    }

    /// <summary>
    /// no self link is desired, as is the case when a client generates a represent to send to the server
    /// </summary>
    public class OrganisationWithNoHrefRepresentation : Representation
    {
        static readonly Link WithAppPath = new Link("organisation", "~/api/organisations/{0}");

        public OrganisationWithNoHrefRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string Rel
        {
            get { return WithAppPath.Rel; }
            set { }
        }

        public override string Href
        {
            get { return null; }
            set { }
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
        }
    }

    /// <summary>
    /// link title
    /// </summary>
    public class OrganisationWithLinkTitleRepresentation : Representation
    {
        static readonly Link WithAppPath = new Link("organisation", "~/api/organisations/{0}");

        public OrganisationWithLinkTitleRepresentation(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string Rel
        {
            get { return WithAppPath.Rel; }
            set { }
        }

        public override string Href
        {
            get { return null; }
            set { }
        }

        public int Id { get; set; }
        public string Name { get; set; }

        protected override void CreateHypermedia()
        {
            Links.Add(new Link("someRel", "someHref", "someTitle"));
        }
    }
}