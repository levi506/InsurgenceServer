using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsurgenceServerWebsite.Models
{
    public class UserRequest
    {
        public UserInfo UserInfo { get; set; }
        public string FriendSafari { get; set; }
        public List<IPInfo> IPs { get; set; }
        public List<UserInfo> Alts { get; set; }
        public List<Trade> Trades {get;set;}
    }
    public class UserInfo
    {
        public string Username { get; set; }
        public uint User_Id { get; set; }
        public bool Banned {get;set;}
    }
    public class IPInfo
    {
        public string IP{get;set;}
        public bool Banned{get;set;}
    }
}
