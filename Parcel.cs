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
        public ParcelType parcelType { get; set; }

        public enum ParcelType
        {
            Weapons = 1,
            CautiousParcels = 2,
            RefrigeratedGoods = 3

        }

        public Parcel(double parcelWeight, string parcelSize, ParcelType parcelType)
        {
            this.parcelWeight = parcelWeight;
            this.parcelSize = parcelSize;
            this.parcelType = parcelType;
        }


    }
}