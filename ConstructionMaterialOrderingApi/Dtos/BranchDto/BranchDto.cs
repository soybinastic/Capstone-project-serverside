using System.Net.Mime;
using System;

namespace ConstructionMaterialOrderingApi.Dtos.BranchDto
{
    public class BranchDto
    {
        public int Id { get; set; }
        public int HardwareStoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateRegistered { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Range { get; set; }
        public string Image { get; set; }
        public double Distance { get; set; }

        public BranchDto(){}
        public BranchDto(int id, int hardwareStoreId, string name, 
            string address, bool isActive, DateTime dateRegistered, 
            double longitude, double latitude, double range,
            string image, double distance)
        {
            Id = id;
            HardwareStoreId = hardwareStoreId;
            Name = name;
            Address = address;
            IsActive = isActive;
            DateRegistered = dateRegistered;
            Longitude = longitude;
            Latitude = latitude;
            Range = range;
            Image = image;
            Distance = distance;
        }
    }
}