using System;
using System.Collections.Generic;
using System.Text;

namespace AltSourceCore
{
    public class Account
    {
        public int ID { get; internal set; }
        public string OwnerUserID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; internal set; }
    }
}
