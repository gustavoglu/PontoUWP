using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Ponto.APIGoogle
{
   public class Result
    {
        [JsonProperty("results")]
        public List<WebLocal> results { get; set; }
    }

   public  class WebLocal
    {
        
        public List<Address_Component> address_components { get; set; }

        public string formatted_address { get; set; }

        public Geometry geometry { get; set; }

        public string place_id { get; set; }

        public List<string> types { get; set; }

        public override string ToString()
        {
            return formatted_address;
        }

    }

    public class Address_Component
    {
        public string long_name { get; set; }

        public string short_name { get; set; }

        public List<string> types { get; set; }
    }
    
    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }

        public double lng { get; set; }
    }
}
