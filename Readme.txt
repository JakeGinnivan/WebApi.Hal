Release Notes

Version 2.3.0
 - Serialization of one-item SimpleListRepresentation (now is always an array)
 - Support .net 4.5 (Currently is 4.5.1 for no reason)
 - Update WebApi references to 5.1.2

Version 2.2.0
- Deserialize HAL resources into C# objects
- Representation.Href and .Rel virtual
- Support absolute URIs
- A few minor fixes for better serialization

Version 2.1.0
- Enhancements to provide adherence to the HAL spec:
  - Support array of link objects for any given rel, instead of only allowing a single link object.
  - Support embedded single resource object for any given rel, instead of only allowing an array of resource objects.
  - Support multiple rels within embedded, instead of only allowing one rel for an array of resource objects.
  - Support RFC6570 URI Templates.
- Deprecated WebApi.Hal.RepresentationList: use WebApi.Hal.SimpleListRepresentation instead.
  - A future release will remove WebApi.Hal.RepresentationList.
  - SimpleListRepresentation removes the need to manually specify the list hypermedia
- Removed WebApi.Hal.Link.RegisterLinkWithWebApi.
  - Use ordinary MVC/WebApi machinery. HAL UriTemplates must comply with RFC6570, which was in conflict with route generation helpers.
