using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk.Entity.Client
{
    public class ProcessCategoryResult
    {
        public List<ProcessCategoryEntity> result;

        public int error_code = 0;

        public string error_message;
    }
}
