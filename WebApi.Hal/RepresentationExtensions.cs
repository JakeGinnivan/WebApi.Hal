using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApi.Hal
{
    public static class RepresentationExtensions
    {
        /// <summary>
        /// This method looks for LinkAttributes on the Representation class, properties and sub-properties.
        /// It uses the LinkAttributes to add to the Links list on the current instance.   
        /// </summary>
        /// <param name="representation"></param>
        /// <param name="ignoreCurrentClassLinks"></param>
        /// <param name="ignoreLinksOnProperty"></param>
        /// <param name="ignoreLinksOnPropertyReturnType"></param>
        public static void BuildLinksFromAttributes(this Representation representation, bool ignoreCurrentClassLinks = false, bool ignoreLinksOnProperty = false, bool ignoreLinksOnPropertyReturnType = false)
        {
            if (representation.Links == null)
                representation.Links = new List<Link>();

            if (!ignoreCurrentClassLinks)
            {
                //Build Links from Attributes defined at the current Representation class level
                var currentClassAttributes =
                    representation.GetType()
                        .GetCustomAttributes<LinkAttribute>(true).Where(a => !(a is SelfAttribute));

                foreach (var linkAttribute in currentClassAttributes)
                {
                    var templateLink = linkAttribute.CreateLink();

                    //find properties in class
                    var processedLink = ProcessTemplateLinkForClass(representation, templateLink, linkAttribute.UrlParameterMappings);

                    if (processedLink != null)
                        representation.Links.Add(processedLink);
                }
            }

            var properties = representation.GetType().GetPropertiesHierarchical().Where(prop => prop.CustomAttributes.Any(att => att.AttributeType == typeof(LinkAttribute)));

            foreach (var prop in properties)
            {

                if (!ignoreLinksOnProperty)
                {
                    //Build Links from Attributes defined at the Representation class level for the current property
                    if (prop.PropertyType == typeof(Representation))
                    {
                        var propertyClassAttributes = prop.PropertyType.GetCustomAttributes<LinkAttribute>(true);

                        foreach (var linkAttribute in propertyClassAttributes)
                        {
                            var link = linkAttribute.CreateLink();

                            if (link != null)
                                representation.Links.Add(link);
                        }
                    }
                }

                if (!ignoreLinksOnPropertyReturnType)
                {
                    //Build Links from Attributes defined on the current property
                    var propertyAttributes = prop.GetCustomAttributes<LinkAttribute>(true);

                    foreach (var linkAttribute in propertyAttributes)
                    {
                        var templateLink = linkAttribute.CreateLink();

                        if (templateLink != null)
                        {
                            var link = ProcessTemplateLinkForProperty(representation, templateLink, prop);

                            representation.Links.Add(link);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method will find the SelfAttribute on the current Representation instance.
        /// </summary>
        /// <param name="representation"></param>
        /// <returns>A Link object.</returns>
        public static Link GetSelfLinkFromAttribute(this Representation representation)
        {
            var selfLinkAttribute = representation.GetType().GetCustomAttributes<SelfAttribute>(true).FirstOrDefault();

            if (selfLinkAttribute == null)
                throw new Exception("No SelfAttribute was found on this object or any of it's descendents.");

            var link = selfLinkAttribute.CreateLink();

            if (link != null)
                return link;
            else
            {
                var typeName = representation.GetType().Name;
                return new Link(typeName, String.Format("~/{0}/{id}", typeName));
            }

        }

        //public static void SetRelFromRelAttribute(this Representation representation)
        //{
        //    var relAttribute = representation.GetType().GetCustomAttribute<RelAttribute>();

        //    representation.Rel = relAttribute.Rel;
        //}
        
        /// <summary>
        /// This method looks for all properties on the Representation object that are of type Representation.
        /// Then it looks for a RelAttribute on the property.  If it finds one, it sets the Rel property of 
        /// that Representation instance.
        /// </summary>
        /// <param name="representation">An instance of a Representation object.</param>
        public static void SetRelOnRepresentationPropertiesUsingRelAttribute(this Representation representation)
        {
            var props =
                representation.GetType()
                    .GetPropertiesHierarchical()
                    .Where(
                        prop =>
                        {
                            if (prop.PropertyType.IsSubclassOf(typeof (Representation)))// || RepresentationExtensions.IsListOfRepresentations(prop))
                            {
                                if (prop.GetCustomAttribute<RelAttribute>(true) != null)
                                {
                                    return true;
                                }
                            }
                            else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) &&
                                     prop.PropertyType.IsGenericType &&
                                     prop.PropertyType.GetGenericArguments()[0].IsSubclassOf(typeof (Representation)))
                            {
                                if (prop.GetCustomAttribute<RelAttribute>(true) != null)
                                {
                                    return true;
                                }
                            }
                            return false;
                        });

            foreach (var prop in props)
            {
                var propValue = prop.GetValue(representation) as Representation;
                var relAttribute = prop.GetCustomAttribute<RelAttribute>(true);
                if (propValue != null)
                {
                    if (relAttribute != null)
                    {
                        propValue.Rel = relAttribute.Rel;
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    if (relAttribute != null)
                    {
                        var propListValues = prop.GetValue(representation) as IEnumerable<Representation>;
                        foreach (var rep in propListValues)
                        {
                            rep.Rel = relAttribute.Rel;
                        }
                    }
                }
                else
                {
                    propValue = (AttributeRepresentation) Activator.CreateInstance(prop.PropertyType);
                    if (propValue != null)
                    {
                        propValue.Rel = relAttribute.Rel;
                    }
                }
            }
        }

        /// <summary>
        /// This method searches the Representation instance for a property with the HrefKeyAttribute and returns that property value. 
        /// </summary>
        /// <param name="representation"></param>
        /// <returns>The value of the property with the HrefKeyAttribute applied to it.</returns>
        public static object GetHrefKeyPropertyValue(this Representation representation)
        {
            var hrefKeyProperty = representation.GetType().GetPropertiesHierarchical().FirstOrDefault(prop => prop.CustomAttributes.Any(att => att.AttributeType == typeof(HrefKeyAttribute)));

            if (hrefKeyProperty != null)
                return hrefKeyProperty.GetValue(representation);

            return null;
        }

        /// <summary>
        /// Returns the declared properties of a type or its base types.
        /// 
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>
        /// An enumerable of the <see cref="T:System.Reflection.PropertyInfo"/> objects.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetPropertiesHierarchical(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<PropertyInfo>();
            if (type.Equals(typeof(object)))
                return IntrospectionExtensions.GetTypeInfo(type).DeclaredProperties;
            else
                return Enumerable.Concat<PropertyInfo>(IntrospectionExtensions.GetTypeInfo(type).DeclaredProperties, RepresentationExtensions.GetPropertiesHierarchical(IntrospectionExtensions.GetTypeInfo(type).BaseType));
        }


        private static Link ProcessTemplateLinkForClass(Representation representation, Link templateLink, Dictionary<string, string> urlParameterMappings)
        {
            var uriTemplate = new UriTemplate(templateLink.Href);
            var uriParameterNames = uriTemplate.GetParameterNames();
            var unresolvedParameterNames = new List<string>();
            foreach (var uriParameterName in uriParameterNames)
            {
                //try to use the urlParameterMappings to get the name of the property to use for the value of the related url token (i.e. - {id} or {searchTerm})

                //check urlParameterMappings for a key matching uriParameterName
                var propString = urlParameterMappings[uriParameterName];

                if (propString != null)
                {
                    var propValue = GetPropertyValueFromString(representation, propString);
                    if (propValue != null)
                    {
                        if (!(propValue is string) && !(propValue is IList) && !(propValue is IDictionary))
                            uriTemplate.SetParameter(uriParameterName, propValue.ToString());
                        else
                            uriTemplate.SetParameter(uriParameterName, propValue);
                    }

                }
                else //if there is no matching parameter in the urlParameterMappings, then look for a property with the same name as the uriParameterName.
                {
                    var prop = representation.GetType()
                        .GetProperty(uriParameterName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null)
                    {
                        var propValue = prop.GetValue(representation);
                        if (!(propValue is string) && !(propValue is IList) && !(propValue is IDictionary))
                            uriTemplate.SetParameter(uriParameterName, propValue.ToString());
                        else
                            uriTemplate.SetParameter(uriParameterName, propValue);
                    }
                    else
                    {
                        unresolvedParameterNames.Add(uriParameterName);
                    }
                }
            }

            var parsedTemplateValue = uriTemplate.Resolve();

            var uri = new Uri(parsedTemplateValue, UriKind.RelativeOrAbsolute);


            var link = new Link(templateLink.Rel, uri.ToString(), templateLink.Title);
            return link;
        }

        private static object GetPropertyValueFromString(object srcobj, string propertyName)
        {
            if (srcobj == null)
                return null;

            object obj = srcobj;

            // Split property name to parts (propertyName could be hierarchical, like obj.subobj.subobj.property
            string[] propertyNameParts = propertyName.Split('.');

            foreach (string propertyNamePart in propertyNameParts)
            {
                if (obj == null) return null;

                // propertyNamePart could contain reference to specific 
                // element (by index) inside a collection
                if (!propertyNamePart.Contains("["))
                {
                    PropertyInfo pi = obj.GetType().GetProperty(propertyNamePart, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi == null) return null;
                    obj = pi.GetValue(obj, null);
                }
                else
                {   // propertyNamePart is areference to specific element 
                    // (by index) inside a collection
                    // like AggregatedCollection[123]
                    //   get collection name and element index
                    int indexStart = propertyNamePart.IndexOf("[") + 1;
                    string collectionPropertyName = propertyNamePart.Substring(0, indexStart - 1);
                    int collectionElementIndex = Int32.Parse(propertyNamePart.Substring(indexStart, propertyNamePart.Length - indexStart - 1));
                    //   get collection object
                    PropertyInfo pi = obj.GetType().GetProperty(collectionPropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi == null) return null;
                    object unknownCollection = pi.GetValue(obj, null);
                    //   try to process the collection as array
                    if (unknownCollection.GetType().IsArray)
                    {
                        object[] collectionAsArray = unknownCollection as Array[];
                        obj = collectionAsArray[collectionElementIndex];
                    }
                    else
                    {
                        //   try to process the collection as IList
                        System.Collections.IList collectionAsList = unknownCollection as System.Collections.IList;
                        if (collectionAsList != null)
                        {
                            obj = collectionAsList[collectionElementIndex];
                        }
                        else
                        {
                            // ??? Unsupported collection type
                        }
                    }
                }
            }

            return obj;
        }

        private static Link ProcessTemplateLinkForProperty(Representation representation, Link templateLink, PropertyInfo prop)
        {
            var uriTemplate = new UriTemplate(templateLink.Href);
            var uriParameterNames = uriTemplate.GetParameterNames();
            foreach (var uriParameterName in uriParameterNames)
            {
                var propValue = prop.GetValue(representation);
                if (prop.Name.ToLower() == uriParameterName)
                {
                    //try set the uriPropertyValue to the value of the current property
                    if (!(propValue is string) && !(propValue is IList) && !(propValue is IDictionary))
                        uriTemplate.SetParameter(uriParameterName, propValue.ToString());
                    else
                        uriTemplate.SetParameter(uriParameterName, propValue);
                }
                else
                {
                    var subProperty = propValue.GetType()
                        .GetProperty(uriParameterName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (subProperty != null)
                    {
                        var subPropertyValue = subProperty.GetValue(propValue);
                        //try to set the uriPropertyValue to a sub-property on the current property with a name matching the uriParameterName
                        if (!(subPropertyValue is string) && !(subPropertyValue is IList) &&
                            !(subPropertyValue is IDictionary))
                            uriTemplate.SetParameter(uriParameterName, subPropertyValue.ToString());
                        else
                            uriTemplate.SetParameter(uriParameterName, subPropertyValue);
                    }
                    else
                    {
                        //try to set the uriPropertyValue to a property on the current representation with a name matching the uriParameterName
                        var currentRepresentationMatchingProperty =
                            representation.GetType()
                                .GetPropertiesHierarchical()
                                .FirstOrDefault(repProp => repProp.Name.ToLower() == uriParameterName);

                        if (currentRepresentationMatchingProperty != null)
                        {
                            var currentRepresentationMatchingPropertyValue = currentRepresentationMatchingProperty.GetValue(representation);

                            if (!(currentRepresentationMatchingPropertyValue is string) && !(currentRepresentationMatchingPropertyValue is IList) &&
                                !(currentRepresentationMatchingPropertyValue is IDictionary))
                                uriTemplate.SetParameter(uriParameterName, currentRepresentationMatchingPropertyValue.ToString());
                            else
                                uriTemplate.SetParameter(uriParameterName, currentRepresentationMatchingPropertyValue);
                        }
                    }
                }
            }

            var parsedTemplateValue = uriTemplate.Resolve();

            var uri = new Uri(parsedTemplateValue, UriKind.RelativeOrAbsolute);

            var link = new Link(templateLink.Rel, uri.ToString(), templateLink.Title);
            return link;
        }
    }
}