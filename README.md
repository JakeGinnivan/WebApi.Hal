WebApi.Hal
==========

Adds support for the Hal Media Type (and Hypermedia) to Asp.net Web Api

HAL Specification
-----------------
http://stateless.co/hal_specification.html  
https://github.com/mikekelly/hal_specification


Getting Started with WebApi.Hal
-------------------------------
First thing first, WebApi.Hal is a media formatter. So to get started you have to register the Hal Media formatters:

	GlobalConfiguration.Configuration.Formatters.Add(new JsonHalMediaTypeFormatter());
	GlobalConfiguration.Configuration.Formatters.Add(new XmlHalMediaTypeFormatter());

Once those are registered, you can start defining your resources. In WebAPI.Hal, you always return `Representations` from your ApiControllers.

Your representations should all inherit from `Representation` or if you are returning a collection 
a `SimpleListRepresentation<TRepresentation>` or if you are returning a resource with multiple embedded
resources, inherit from `Representation` and add properties for the embedded resources which might
include lists of resources of the form `List<MyRepresentation>`. See the sample.

WebApi.Hal.Link
---------------
The link class represents hypermedia on a representation. It looks like this:

	public class Link
	{
		public string Rel { get; set; }
		public string Href { get; set; }
		public string Title { get; set; }
		public bool IsTemplated { get; set; }
		public Link CreateLink(...);
	}

The thing which makes this class special is it's support for templates. For example:

	var link = new Link("beers", "/breweries/{id}/beers");

Notice the {id}, this allows you to return a templated link as hypermedia. But you can also turn it into an absolute link really easily:

	Brewery brewery;
	link.CreateLink(new { id = brewery.Id });

URI Templates adhere to [RFC6570](http://tools.ietf.org/html/rfc6570), as per the HAL specification.

Register WebAPI routes
----------------------
Once you have done the work of defining all your link templates, register your routes in the ordinary MVC WebApi manner.
See the sample project for an example.

LinkTemplates class
-------------------
A nice place to keep your Links is in a static class called LinkTemplates, with nested classes for each resource your application has, for example:

    public static class LinkTemplates {
		public static class Beers {
			/// <summary>
			/// /beers/{id}
			public static Link Beer { get { return new Link("beer", "/beers/{id}"); } }

			/// <summary>
            /// /beers?page={page}
            /// </summary>
            public static Link GetBeers { get { return new Link("beers", "/beers{?page}"); } }
		}
	}

The sample is available at: https://github.com/JakeGinnivan/WebApi.Hal/blob/master/WebApi.Hal.Web/LinkTemplates.cs

WebApi.Hal.Representation
-------------------------
This is the base class for all representations your api returns. It has an abstract method you must override, `abstract void CreateHypermedia();` 

In the constructor, set the Rel property, as this generally doesn't change based on context.

In CreateHypermedia() you could register a **self** link, but it's done automatically for you if you don't.
Register other hypermedia that your resource should **always** have.
Other context sensitive hypermedia should be added in the API controller.

Here is an example of the Beer CreateHypermedia override (from the example project, the `BeerResource`):

	public Beer()
	{
		Rel = LinkTemplates.Beers.Beer.Rel;
	}
	protected override void CreateHypermedia()
	{
		Href = LinkTemplates.Beers.Beer.CreateLink(new { id = Id }).Href;

		if (StyleId != null)
			Links.Add(LinkTemplates.BeerStyles.Style.CreateLink(new { id = StyleId }));
		if (BreweryId != null)
			Links.Add(LinkTemplates.Breweries.Brewery.CreateLink(new { id = BreweryId }));
	}

Sample controller action
------------------------

	public BreweryRepresentation Get(int id)
	{
		var brewery = beerDbContext.Breweries.Find(id);

		return new BreweryRepresentation
		{
			Id = brewery.Id,
			Name = brewery.Name,
			Links =
			{
				LinkTemplates.Breweries.AssociatedBeers.CreateLink(new { id })
			}
		};
	}

## Sample Project
To run the sample project, update the connection string in web.config, then **create** the database. When you hit an API for the first time, the database will be setup using DbUp.

You can use fiddler to explore the API. Make sure you put in an accept header of `application/hal+json`. Try hitting `http://localhost:51665/beers` with that accept header, and see what happens

## Credits
I have more credits to add, but this is the most obvious (as I based my Xml formatter off this project)

https://bitbucket.org/smichelotti/hal-media-type

## Release Notes
See Readme.txt at http://github.com/JakeGinnivan/WebApi.Hal
