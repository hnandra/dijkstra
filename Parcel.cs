using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dijkstra
{
    public class Parcel
    {
        public double parcelWeight { get; set; }
        public string parcelSize { get; set; }
        public string parcelType { get; set; }

        public Parcel(double parcelWeight, string parcelSize, string parcelType)
        {
            this.parcelWeight = parcelWeight;
            this.parcelSize = parcelSize;
            this.parcelType = parcelType;
        }
    }
}