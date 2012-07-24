using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Serialization
{
    [OrchardFeature("Piedone.HelpfulLibraries.Serialization")]
    public class IdSerializer
    {
        private IEnumerable<int> _ids;
        public IEnumerable<int> Ids
        {
            get
            {
                if (_ids == null)
                {
                    _ids = DeserializeIds(_idsDefinition);
                }

                return _ids;
            }
            set
            {
                _ids = value;
                _idsDefinition = SerializeIds(_ids);
            }
        }


        private string _idsDefinition;

        /// <summary>
        /// JSON array of the ids
        /// </summary>
        public string IdsDefinition
        {
            get { return _idsDefinition; }
            set
            {
                _idsDefinition = value;
                _ids = null;
            }
        }


        public IdSerializer(string idsDefinition = "")
        {
            IdsDefinition = idsDefinition;
        }


        public static string SerializeIds(IEnumerable<int> ids)
        {
            if (ids == null) ids = Enumerable.Empty<int>();
            return new JavaScriptSerializer().Serialize(ids);
        }

        public static IEnumerable<int> DeserializeIds(string idsDefinition)
        {
            if (String.IsNullOrEmpty(idsDefinition))
            {
                return Enumerable.Empty<int>();
            }

            return new JavaScriptSerializer().Deserialize<IEnumerable<int>>(idsDefinition);
        }
    }
}
