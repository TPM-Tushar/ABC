using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CustomModels.Security
{
    public class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name)
        {
            this.Name = name;
        }

        public string AuthenticationType
        {
            get
            {
                return "Custom";
            }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(this.Name); }
        }

        public string Name
        {
            get;
            private set;
        }
    }
}