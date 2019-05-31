using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class APIServices
    {
        public string usersAPI = "http://localhost:9001/api/users";
        public string adsAPI = "http://localhost:9003/api/ads";
        public string bookingsAPI = "http://localhost:9005/api/bookings";
        //public string authAPI = "http://localhost:8030/o"; //main
        public string gatewayAPI = "http://localhost:9010/api";
        public string authAPI = "http://localhost:9001/api/auth";

    }
}
