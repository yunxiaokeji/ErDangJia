using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class UserEntity
    {
        public string userID
        {
            get;
            set;
        }

        public string clientID
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string avatar
        {
            get;
            set;
        }

        public string email
        {
            get;
            set;
        }

        public string mobilePhone
        {
            get;
            set;
        }

        public string work_phone
        {
            get;
            set;
        }

        public bool  isSystemAdmin { get; set; }


    }
}
