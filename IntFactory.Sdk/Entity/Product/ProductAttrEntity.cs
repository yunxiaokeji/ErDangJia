using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class ProductAttrEntity
    {
        public string AttrID { get; set; }

        public string AttrName { get; set; }

        public List<AttrValueEntity> AttrValues { get; set; }
    }
}
