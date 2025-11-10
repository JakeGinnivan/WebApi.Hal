using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using WebApi.Hal.JsonConverters;

namespace WebApi.Hal;

public static class HalSerializerOptions
{
    public static JsonSerializerOptions Default => field ?? GetOrCreateSingleton(ref field);

    private static readonly LinksConverter _linksConverter = new LinksConverter();

    private static readonly ResourceConverter _resourceConverter = new ResourceConverter(); // TODO Support hypermedia resolver
    
    private static readonly EmbeddedResourceConverter _embeddedResourceConverter = new EmbeddedResourceConverter();

    public static JsonSerializerOptions GetOrCreateSingleton(ref JsonSerializerOptions defaultField)
    {
        defaultField ??= new JsonSerializerOptions();
        defaultField.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
        defaultField.Converters.Add(_linksConverter);
        defaultField.Converters.Add(_resourceConverter);
        defaultField.Converters.Add(_embeddedResourceConverter);
        return defaultField;
    }
}