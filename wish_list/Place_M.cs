using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelBot.wish_list
{
    public class Place_M : Element
    {

        public Geometry geometry { get; set; }
        public Properties properties { get; set; }

        public class Geometry
        {
            public List<double> Coordinates { get; set; }

        }
        public class Properties
        {
            public string Name { get; set; }
            public int Rate { get; set; }
            public string Kinds { get; set; }

        }
    }
}
