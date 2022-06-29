using System.Collections.Generic;

namespace TravelBot.Models
{
    public class Places
    {
        public List<Features> features { get; set; }
        public class Features
        {
            public Geometry geometry { get; set; }
            public Properties properties { get; set; }

        }

    }
    
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
