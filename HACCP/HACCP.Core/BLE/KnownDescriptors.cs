using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace HACCP.Core
{
    public static class KnownDescriptors
    {
        #region Member Variables

        private static Dictionary<Guid, KnownDescriptor> _items;
        private static readonly object Lock = new object();

        #endregion

        /// <summary>
        /// KnownDescriptors
        /// </summary>
        static KnownDescriptors()
        {
        }

        /// <summary>
        /// Lookup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static KnownDescriptor Lookup(Guid id)
        {
            lock (Lock)
            {
                if (_items == null)
                    LoadItemsFromJson();
            }

            if (_items != null && _items.ContainsKey(id))
                return _items[id];
            return new KnownDescriptor {Name = "Unknown", ID = Guid.Empty};
        }

        /// <summary>
        /// LoadItemsFromJson
        /// </summary>
        public static void LoadItemsFromJson()
        {
            _items = new Dictionary<Guid, KnownDescriptor>();
            //TODO: switch over to DescriptorStack.Text when it gets bound.
            KnownDescriptor descriptor = new KnownDescriptor();
            var itemsJson = ResourceLoader.GetEmbeddedResourceString(typeof(KnownDescriptors).GetTypeInfo().Assembly,
                "KnownDescriptors.json");
            var json = JToken.Parse(itemsJson);
            foreach (var item in json.Children())
            {
                var prop = item as JProperty;
                if (prop != null)
                    descriptor = new KnownDescriptor {Name = prop.Value.ToString(), ID = Guid.ParseExact(prop.Name, "d")};
                _items.Add(descriptor.ID, descriptor);
            }
        }
    }

    /// <summary>
    /// KnownDescriptor Struct
    /// </summary>
    public struct KnownDescriptor
    {
        public string Name;
        public Guid ID;
    }
}