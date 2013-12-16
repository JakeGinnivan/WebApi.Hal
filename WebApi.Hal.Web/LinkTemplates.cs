namespace WebApi.Hal.Web
{
    public static class LinkTemplates
    {
        public static class Breweries
        {
            /// <summary>
            /// /breweries
            /// </summary>
            public static Link GetBreweries { get { return new Link("breweries", "/breweries"); } }

            /// <summary>
            /// /breweries/{id}
            /// </summary>
            public static Link Brewery { get{return new Link("brewery", "/breweries/{id}");} }

            /// <summary>
            /// /breweries/{id}/beers
            /// </summary>
            public static Link AssociatedBeers { get{return new Link("beers", "/breweries/{id}/beers?page={page=1}");} }
        }

        public static class BeerStyles
        {
            /// <summary>
            /// /styles
            /// </summary>
            public static Link GetStyles { get { return new Link("styles", "/styles"); } }

            /// <summary>
            /// /styles/{id}/beers
            /// </summary>
            public static Link AssociatedBeers { get { return new Link("beers", "/styles/{id}/beers?page={page=1}"); } }

            /// <summary>
            /// /styles/{id}
            /// </summary>
            public static Link Style { get { return new Link("style", "/styles/{id}");}}
        }

        public static class Beers
        {
            /// <summary>
            /// /beers?page={page}
            /// </summary>
            public static Link GetBeers { get { return new Link("beers", "/beers?page={page=1}"); } }

            /// <summary>
            /// /beers?searchTerm={searchTerm}&amp;page={page}
            /// </summary>
            public static Link SearchBeers { get { return new Link("beerSearch", "/beers?searchTerm={searchTerm}&page={page}"); } }

            /// <summary>
            /// /beers/{id}
            /// </summary>
            public static Link Beer { get { return new Link("beer", "/beers/{id}"); } }
        }
    }
}