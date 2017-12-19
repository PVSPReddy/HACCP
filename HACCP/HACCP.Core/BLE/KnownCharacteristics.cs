using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace HACCP.Core
{
    public static class KnownCharacteristics
    {
        #region Member Variable

        private static Dictionary<Guid, KnownCharacteristic> _items;
        private static readonly object Lock = new object();

        #endregion

        /// <summary>
        /// KnownCharacteristics Constructor
        /// </summary>
        static KnownCharacteristics()
        {
        }

        /// <summary>
        /// Lookup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static KnownCharacteristic Lookup(Guid id)
        {
            lock (Lock)
            {
                if (_items == null)
                    LoadItemsFromJson();
            }

            if (_items != null && _items.ContainsKey(id))
                return _items[id];
            return new KnownCharacteristic {Name = "Unknown", ID = Guid.Empty};
        }

        /// <summary>
        /// LoadItemsFromJson
        /// </summary>
        public static void LoadItemsFromJson()
        {
            _items = new Dictionary<Guid, KnownCharacteristic>();
            //TODO: switch over to CharacteristicStack.Text when it gets bound.
            var itemsJson = ResourceLoader.GetEmbeddedResourceString(
                typeof(KnownCharacteristics).GetTypeInfo().Assembly, "KnownCharacteristics.json");
            var json = JToken.Parse(itemsJson);
            foreach (var item in json.Children())
            {
                var prop = item as JProperty;
                if (prop != null)
                {
                    var characteristic = new KnownCharacteristic
                    {
                        Name = prop.Value.ToString(),
                        ID = Guid.ParseExact(prop.Name, "d")
                    };
                    _items.Add(characteristic.ID, characteristic);
                }
            }
        }
    }

    /// <summary>
    /// KnownCharacteristic
    /// </summary>
    public struct KnownCharacteristic
    {
        public string Name;
        public Guid ID;
    }
}