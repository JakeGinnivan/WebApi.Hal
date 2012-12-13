using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public class XmlHalMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        public XmlHalMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+xml"));
        }

        public override object ReadFromStream(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (!typeof(Representation).IsAssignableFrom(type))
            {
                return null;
            }

            var xml = XElement.Load(stream);
            return ReadHalResource(type, xml);
        }

        public override void WriteToStream(Type type, object value, Stream stream, HttpContent content)
        {
            var resource = value as Representation;
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
            Representation representation;

            if (xml == null)
            {
                return null;
            }

            // First, determine if Resource of type Generic List and construct Instance with respective Parameters
            if (typeof(IRepresentationList).IsAssignableFrom(type))
            {
                var resourceListXml = xml.Elements("resource");  // .Where(x => x.Attribute("rel").Value == "item");
                var genericTypeArg = type.GetGenericArguments().Single();
                var resourceList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(genericTypeArg));

                foreach (var resourceListItem in resourceListXml.Select(resourceXml => ReadHalResource(genericTypeArg, resourceXml)))
                {
                    resourceList.Add(resourceListItem);
                }

                representation = Activator.CreateInstance(type, new object[] { resourceList }) as Representation;
            }
            else
            {
                representation = Activator.CreateInstance(type) as Representation;
            }

            // Second, set the well-known HAL properties ONLY if type of Resource
            CreateSelfHypermedia(type, xml, representation);

            // Third, read/set the rest of the properties
            SetProperties(type, xml, representation);

            // Fourth, read each link element
            var links = xml.Elements("link");
            var linksList = links.Select(link => new Link
            {
                Rel = link.Attribute("rel").Value,
                Href = link.Attribute("href").Value
            }).ToList();

            type.GetProperty("Links").SetValue(representation, linksList, null);

            return representation;
        }

        static void SetProperties(Type type, XElement xml, Representation representation)
        {
            foreach (var property in type.GetPublicInstanceProperties())
            {
                if (property.IsValidBasicType())
                {
                    type.SetPropertyValue(property.Name, xml.Element(property.Name), representation);
                }
                else if (typeof(Representation).IsAssignableFrom(property.PropertyType) &&
                         property.GetIndexParameters().Length == 0)
                {
                    var resourceXml =
                        xml.Elements("resource").SingleOrDefault(x => x.Attribute("name").Value == property.Name);
                    var halResource = ReadHalResource(property.PropertyType, resourceXml);
                    property.SetValue(representation, halResource, null);
                }
            }
        }

        static void CreateSelfHypermedia(Type type, XElement xml, Representation representation)
        {
            type.GetProperty("Rel").SetValue(representation, xml.Attribute("rel").Value, null);
            type.SetPropertyValue("Href", xml.Attribute("href"), representation);
            type.SetPropertyValue("LinkName", xml.Attribute("name"), representation);
        }

        static void WriteHalResource(Representation representation, XmlWriter writer, string propertyName = null)
        {
            if (representation == null)
            {
                return;
            }

            // First write the well-known HAL properties
            writer.WriteStartElement("resource");
            writer.WriteAttributeString("rel", representation.Rel);
            writer.WriteAttributeString("href", representation.Href);
            if (representation.LinkName != null || propertyName != null)
            {
                writer.WriteAttributeString("name", representation.LinkName = representation.LinkName ?? propertyName);
            }

            // Second, determine if resource is of Generic Resource List Type , list out all the items
            var representationList = representation as IRepresentationList;
            if (representationList != null)
            {
                foreach (var item in representationList.Cast<Representation>())
                {
                    WriteHalResource(item, writer);
                }
            }

            //Third write out the links of the resource
            foreach (var link in representation.Links)
            {
                writer.WriteStartElement("link");
                writer.WriteAttributeString("rel", link.Rel);
                writer.WriteAttributeString("href", link.Href);
                writer.WriteEndElement();
            }

            // Fourth, write the rest of the properties
            WriteResourceProperties(representation, writer);

            writer.WriteEndElement();
        }

        static void WriteResourceProperties(Representation representation, XmlWriter writer)
        {
// Only simple type and nested ApiResource type will be handled : for any other type, exception will be thrown
            // including List<ApiResource> as representation of List would require properties rel, href and linkname
            // To overcome the issue, use "RepresentationList<T>"
            foreach (var property in representation.GetType().GetPublicInstanceProperties())
            {
                if (property.IsValidBasicType())
                {
                    var propertyString = GetPropertyString(property, representation);
                    if (propertyString != null)
                    {
                        writer.WriteElementString(property.Name, propertyString);
                    }
                }
                else if (typeof (Representation).IsAssignableFrom(property.PropertyType) &&
                         property.GetIndexParameters().Length == 0)
                {
                    var halResource = property.GetValue(representation, null);
                    WriteHalResource((Representation) halResource, writer, property.Name);
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
            return typeof(Representation).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(Representation).IsAssignableFrom(type);
        }
    }
}