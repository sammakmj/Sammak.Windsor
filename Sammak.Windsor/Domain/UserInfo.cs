using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sammak.Windsor.Domain
{
    public class UserInfo : EntityBase
    {
        public virtual int Email
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        public virtual int Username
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}