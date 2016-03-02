using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rally
{
   
    
        public class Track
        {
            public Track(string name, Location startLocation, double startHeading)
            {
                Name = name;
                StartLocation = startLocation;
                StartHeading = startHeading;
            }

            public string Name { get; set; }
            public Location StartLocation { get; set; }
            public double StartHeading { get; set; }
        }
    
}
