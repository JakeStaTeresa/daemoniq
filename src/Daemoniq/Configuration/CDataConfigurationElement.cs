using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Daemoniq.Configuration
{
    public class CDataConfigurationElement
        : ConfigurationElement
    {
        private readonly Dictionary<string, bool> _cDataProperties;

        public CDataConfigurationElement()
        {
            _cDataProperties = new Dictionary<string, bool>();
            PropertyInfo[] properties = GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                object[] customAttributes = 
                    property.GetCustomAttributes(typeof (ConfigurationPropertyAttribute), true);
                ConfigurationPropertyAttribute[] configurationPropertyAttributes = 
                    Array.ConvertAll(customAttributes,
                                     o => o as ConfigurationPropertyAttribute);
                CDataConfigurationPropertyAttribute[] cDataConfigurationPropertyAttribute =
                    Array.ConvertAll(property.GetCustomAttributes(typeof(CDataConfigurationPropertyAttribute), true),
                                     o => o  as CDataConfigurationPropertyAttribute);

                if (configurationPropertyAttributes.Length == 0)
                {
                    continue;
                }

                if (cDataConfigurationPropertyAttribute.Length != 0)
                {
                    _cDataProperties.Add(configurationPropertyAttributes[0].Name, true);
                }
            }
        }                

        protected override bool SerializeElement(System.Xml.XmlWriter writer, bool serializeCollectionKey)
        {
            bool returnValue;
            if (_cDataProperties.Count == 0)
            {
                returnValue = base.SerializeElement(writer, serializeCollectionKey);
            }
            else
            {                
                foreach (ConfigurationProperty configurationProperty in Properties)
                {
                    string name = configurationProperty.Name;
                    string propertyValue = configurationProperty.Converter.ConvertToString(
                        base[name]);
                        
                    if (_cDataProperties.ContainsKey(name))
                    {
                        writer.WriteCData(propertyValue);
                    }
                    else
                    {
                        writer.WriteAttributeString("name", propertyValue); 
                    }
                }                                
                returnValue = true;
            }
            return returnValue;
        }

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            if(_cDataProperties.Count == 0)
            {
                base.DeserializeElement(reader, serializeCollectionKey);
            }
            else
            {
                foreach (ConfigurationProperty configurationProperty in Properties)
                {
                    string name = configurationProperty.Name;
                    if (_cDataProperties.ContainsKey(name))
                    {
                        string contentString = reader.ReadString();
                        base[name] = contentString;
                    }
                    else
                    {
                        string attributeValue = reader.GetAttribute(name);
                        base[name] = attributeValue;
                    }
                }
                reader.ReadEndElement();
            }            
        }
    }
}