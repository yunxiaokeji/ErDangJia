using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class CategorysResult
    {
        public List<CategoryEntity> result;

        public int error_code = 0;

        public string error_message;
    }
    public class CategoryResult
    {
        public CategoryEntity result;

        public int error_code = 0;

        public string error_message;
    }
}
