using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace HACCP.Core
{
    public static class KnownServices
    {
        #region Member Variables

        private static Dictionary<Guid, KnownService> _items;
        private static readonly object Lock = new object();

        #endregion

        /// <summary>
        /// Known Services Conmstructor
        /// </summary>
        static KnownServices()
        {
        }

        #region Methods

        /// <summary>
        /// Lookup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static KnownService Lookup(Guid id)
        {
            lock (Lock)
            {
                if (_items == null)
                    LoadItemsFromJson();
            }

            if (_items != null && _items.ContainsKey(id))
                return _items[id];
            return new KnownService {Name = "Unknown", ID = Guid.Empty};
        }

        /// <summary>
        /// Load Items From Json
        /// </summary>
        public static void LoadItemsFromJson()
        {
            _items = new Dictionary<Guid, KnownService>();
            //TODO: switch over to ServiceStack.Text when it gets bound.
            KnownService service = new KnownService();
            var itemsJson = ResourceLoader.GetEmbeddedResourceString(typeof(KnownServices).GetTypeInfo().Assembly,
                "KnownServices.json");
            var json = JToken.Parse(itemsJson);
            foreach (var item in json.Children())
            {
                var prop = item as JProperty;
                if (prop != null)
                    service = new KnownService {Name = prop.Value.ToString(), ID = Guid.ParseExact(prop.Name, "d")};
                _items.Add(service.ID, service);
            }
        }

        #endregion
    }

    /// <summary>
    /// KnownService
    /// </summary>
    public struct KnownService
    {
        public string Name;
        public Guid ID;
    }
}