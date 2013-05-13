#WebApi.Hal

Adds support for the Hal Media Type (and Hypermedia) to Asp.net Web Api

## HAL Specification
http://stateless.co/hal_specification.html  
https://github.com/mikekelly/hal_specification

## Getting Started with WebApi.Hal
First thing first, WebApi.Hal is a media formatter. So to get started you have to register the Hal Media formatters:

    GlobalConfiguration.Configuration.Formatters.Add(new JsonHalMediaTypeFormatter());
    GlobalConfiguration.Configuration.Formatters.Add(new XmlHalMediaTypeFormatter());

Once those are registered, you can start defining your resources. WebApi.Hal has some important classes you should be aware of.

### WebApi.Hal.Representation
This is the base class for all representations your api returns. It has an abstract method you must override, `abstract void CreateHypermedia();` 

In CreateHypermedia() you should register the **self** link, and any hypermedia that that resource should **always** have. For example (from the example project, the `BeerResource`):

    protected override void CreateHypermedia()
    {
        var selfLink = LinkTemplates.Beers.Beer.CreateLink(id => Id);
        Href = selfLink.Href;
        Rel = selfLink.Rel;

        if (StyleId != null)
            Links.Add(LinkTemplates.BeerStyles.Style.CreateLink(id => StyleId));
        if (BreweryId != null)
            Links.Add(LinkTemplates.Breweries.Brewery.CreateLink(id => BreweryId));
    }

### WebApi.Hal.RepresentationList<T>
Instead of returning a list you should return a RepresentationList, which allows you to specify hypermedia for your list (for paging, self, etc)

### WebApi.Hal.Link
The link class represents hypermedia on a representation. It looks like this:

    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public bool IsTemplated { get; set; }
        public Link CreateLink(...);
    }

The thing which makes this class special is it's support for templates. For example:

    var link = new Link("beers, "/breweries/{id}/beers", isTemplated: true);

Notice the {id}, this allows you to return a templated link as hypermedia. But you can also turn it into an absolute link really easily:

    Brewery brewery;
    link.CreateLink(id => brewery.Id);

## Link Template Definitions
One of the simplest ways I have found (so far, I welcome suggestions) is to define a LinkTemplates static class, which contains all your link templates for you to reuse across your API. For example:

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
            public static Link AssociatedBeers { get{return new Link("beers", "/breweries/{id}/beers");} }
        }

        // public static class Styles{...}
        // public static class Beers{...}
    }

And put it all together and an API action can look like this:

    public BreweryRepresentation Get(int id)
    {
        var brewery = beerDbContext.Breweries.Find(id);

        return new BreweryRepresentation
        {
            Id = brewery.Id,
            Name = brewery.Name,
            Links =
            {
                LinkTemplates.Breweries.AssociatedBeers.CreateLink(_id => id)
            }
        };
    }

**Note:** The link class supports prefixing a variable with `_` in case there is a conflicting variable name. 

## Sample Project
To run the sample project, update the connection string in web.config, then **create** the database. When you hit an API for the first time, the database will be setup using DbUp.

You can use fiddler to explore the API. Make sure you put in an accept header of `application/hal+json`. Try hitting `http://localhost:51665/api/beers` with that accept header, and see what happens

##Credits
I have more credits to add, but this is the most obvious (as I based my Xml formatter off this project)

https://bitbucket.org/smichelotti/hal-media-type