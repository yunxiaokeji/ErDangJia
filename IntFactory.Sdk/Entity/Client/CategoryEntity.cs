using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class CategoryEntity
    {
        public string CategoryID;

        public string CategoryCode;

        public string CategoryName;

        public string PID;

        public string PIDList;

        public int Layers;

        public string SaleAttr;

        public string AttrList;

        public int Status;

        public string Description;

        public int CategoryType;

        public List<CategoryEntity> ChildCategory;

        public List<SaleAttr> SaleAttrs;

        public List<SaleAttr> AttrLists;
    }

    public class SaleAttr
    {
        public string AttrID;

        public string AttrName;

        public string Description;

        public int Type;

        public List<AttrValue> AttrValues;
    }
    public class AttrValue
    {
        public string ValueID;

        public string ValueName;

        public string AttrID;

        public int Sort; 
    }
}
