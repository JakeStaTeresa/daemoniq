/*
 *  Copyright 2009 Kriztian Jake Sta. Teresa
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Daemoniq.Configuration
{
    public class ConfigurationElementCollection<T> : 
        ConfigurationElementCollection, ICollection<T>
        where T : ConfigurationElement
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return typeof(T).GetConstructor(new Type[] { }).
                       Invoke(new object[] { }) as ConfigurationElement;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo keyProperty = null;
            foreach (PropertyInfo property in properties)
            {
                if (property.IsDefined(typeof(ConfigurationPropertyAttribute),
                                       true))
                {
                    ConfigurationPropertyAttribute attribute = property.GetCustomAttributes(typeof(ConfigurationPropertyAttribute),
                                                                                            true)[0] as ConfigurationPropertyAttribute;
                    
                    if (attribute != null &&
                        attribute.IsKey)
                    {
                        keyProperty = property;
                        break;
                    }
                }
            }
            object key = null;
            if (keyProperty != null)
            {
                key = keyProperty.GetValue(element, null);
            }
            return key;
        }

        public new int Count
        {
            get { return base.Count; }
        }

        public T this[int index]
        {
            get { return (T)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public T this[string name]
        {
            get { return (T)BaseGet(name); }
        }

        public int IndexOf(T element)
        {
            return BaseIndexOf(element);
        }

        public void Add(T element)
        {
            BaseAdd(element);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, true);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

        #region ICollection<T> Members

        public bool Contains(T item)
        {
            if (item == null)
            {
                for (int j = 0; j < Count; j++)
                {
                    if (this[j] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < Count; i++)
            {
                if (comparer.Equals(this[i], item))
                {
                    return true;
                }
            }
            return false;            
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);            
        }

        public new bool IsReadOnly
        {
            get { return base.IsReadOnly(); }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                BaseRemoveAt(index);
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }            
        }

        #endregion
    }
}