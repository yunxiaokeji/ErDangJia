using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class UserResult
    {
        public UserEntity user;

        public ClientEntity client { get; set; }

        public int error_code = 0;

        public string error_message;

    }
}
