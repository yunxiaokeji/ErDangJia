using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class ProductAttrEntity
    {
        public string attrID { get; set; }

        public string attrName { get; set; }

        public List<AttrValueEntity> attrValues { get; set; }
    }
}
