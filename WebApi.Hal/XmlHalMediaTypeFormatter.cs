using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace WebApi.Hal
{
    public class XmlHalMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        public XmlHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+xml"));
        }

        public override object ReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders, IFormatterLogger formatterLogger)
        {
            if (!typeof(HalResource).IsAssignableFrom(type))
            {
                return null;
            }

            var xml = XElement.Load(stream);
            return ReadHalResource(type, xml);
        }

        public override void WriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders)
        {
            var resource = value as HalResource;
            if (resource == null)
            {
                return;
            }

            var settings = new XmlWriterSettings { Indent = true };

            var writer = XmlWriter.Create(stream, settings);
            WriteHalResource(resource, writer);
            writer.Flush();
        }

        /// <summary>
        /// ReadHalResource will
        /// </summary>
        /// <param name="type">Type of resource - Must be of type ApiResource</param>
        /// <param name="xml">xelement for the type</param>
        /// <returns>returns deserialized object</returns>
        static object ReadHalResource(Type type, XElement xml)
        {
            HalResource resource;

            if (xml == null)
            {
                return null;
            }

            // First, determine if Resource of type Generic List and construct Instance with respective Parameters
            if (type.IsGenericResourceList())
            {
                var resourceListXml = xml.Elements("resource");  // .Where(x => x.Attribute("rel").Value == "item");
                var genericTypeArg = type.GetGenericArguments().Single();
                var resourceList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericTypeArg));

                foreach (var resourceListItem in resourceListXml.Select(resourceXml => ReadHalResource(genericTypeArg, resourceXml)))
                {
                    resourceList.Add(resourceListItem);
                }

                resource = Activator.CreateInstance(type, new object[] { resourceList }) as HalResource;
            }
            else
            {
                resource = Activator.CreateInstance(type) as HalResource;
            }

            // Second, set the well-known HAL properties ONLY if type of HalResource
            CreateSelfHypermedia(type, xml, resource);

            // Third, read/set the rest of the properties
            SetProperties(type, xml, resource);

            // Fourth, read each link element
            var links = xml.Elements("link");
            var linksList = links.Select(link => new Link { Rel = link.Attribute("rel").Value, Href = link.Attribute("href").Value }).ToList();

            type.GetProperty("Links").SetValue(resource, linksList, null);

            return resource;
        }

        static void SetProperties(Type type, XElement xml, HalResource resource)
        {
            foreach (var property in type.GetPublicInstanceProperties())
            {
                if (property.IsValidBasicType())
                {
                    type.SetPropertyValue(property.Name, xml.Element(property.Name), resource);
                }
                else if (typeof(HalResource).IsAssignableFrom(property.PropertyType) &&
                         property.GetIndexParameters().Length == 0)
                {
                    var resourceXml =
                        xml.Elements("resource").SingleOrDefault(x => x.Attribute("name").Value == property.Name);
                    var halResource = ReadHalResource(property.PropertyType, resourceXml);
                    property.SetValue(resource, halResource, null);
                }
            }
        }

        static void CreateSelfHypermedia(Type type, XElement xml, HalResource resource)
        {
            type.GetProperty("Rel").SetValue(resource, xml.Attribute("rel").Value, null);
            type.SetPropertyValue("Href", xml.Attribute("href"), resource);
            type.SetPropertyValue("LinkName", xml.Attribute("name"), resource);
        }

        static void WriteHalResource(HalResource resource, XmlWriter writer, string propertyName = null)
        {
            if (resource == null)
            {
                return;
            }

            // First write the well-known HAL properties
            writer.WriteStartElement("resource");
            writer.WriteAttributeString("rel", resource.Rel);
            writer.WriteAttributeString("href", resource.Href);
            if (resource.LinkName != null || propertyName != null)
            {
                writer.WriteAttributeString("name", resource.LinkName = resource.LinkName ?? propertyName);
            }

            // Second, determine if resource is of Generic Resource List Type , list out all the items
            if (resource.GetType().IsGenericResourceList())
            {
                var propertyValue = resource as IEnumerable;
                foreach (HalResource item in propertyValue)
                {
                    WriteHalResource(item, writer);
                }
            }

            //Third write out the links of the resource
            foreach (var link in resource.Links)
            {
                writer.WriteStartElement("link");
                writer.WriteAttributeString("rel", link.Rel);
                writer.WriteAttributeString("href", link.Href);
                writer.WriteEndElement();
            }

            // Fourth, write the rest of the properties
            WriteResourceProperties(resource, writer);

            writer.WriteEndElement();
        }

        static void WriteResourceProperties(HalResource resource, XmlWriter writer)
        {
// Only simple type and nested ApiResource type will be handled : for any other type, exception will be thrown
            // including List<ApiResource> as representation of List would require properties rel, href and linkname
            // To overcome the issue, use "ResourceList<T>"
            foreach (var property in resource.GetType().GetPublicInstanceProperties())
            {
                if (property.IsValidBasicType())
                {
                    var propertyString = GetPropertyString(property, resource);
                    if (propertyString != null)
                    {
                        writer.WriteElementString(property.Name, propertyString);
                    }
                }
                else if (typeof (HalResource).IsAssignableFrom(property.PropertyType) &&
                         property.GetIndexParameters().Length == 0)
                {
                    var halResource = property.GetValue(resource, null);
                    WriteHalResource((HalResource) halResource, writer, property.Name);
                }
            }
        }

        static string GetPropertyString(PropertyInfo property, object instance)
        {
            var propertyValue = property.GetValue(instance, null);
            if (propertyValue != null)
            {
                return propertyValue.ToString();
            }

            return null;
        }

        public override bool CanReadType(Type type)
        {
            return typeof(HalResource).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(HalResource).IsAssignableFrom(type);
        }
    }
}