using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class APIServices
    {
        //public string usersAPI = "http://localhost:9001/api/users";
        public string usersAPI = "https://usersapiservice20190603102934.azurewebsites.net/api/users";
        //public string adsAPI = "http://localhost:9003/api/ads";
        public string adsAPI = "https://adsapiservice20190603102135.azurewebsites.net/api/ads";
        //public string bookingsAPI = "http://localhost:9005/api/bookings";
        public string bookingsAPI = "https://bookingsapiservice20190603102613.azurewebsites.net/api/bookings";
        //public string authAPI = "http://localhost:8030/o"; //main
        //public string gatewayAPI = "http://localhost:9010/api";
        public string gatewayAPI = "https://gateway20190603104549.azurewebsites.net/api";
        //public string authAPI = "http://localhost:9001/api/auth";
        public string authAPI = "https://usersapiservice20190603102934.azurewebsites.net/api/auth";

    }
}
