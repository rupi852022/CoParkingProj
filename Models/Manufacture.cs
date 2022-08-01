using ParkingProject.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkingProject.Models
{
    public class Manufacture
    {
        private int idCar;
        private string manufacturer;

        public Manufacture() { }

        public Manufacture(string manufacturer)
        {
            this.manufacturer = manufacturer;
        }

        public Manufacture(int idCar, string manufacturer) : this(manufacturer)
        {
            this.idCar = idCar;
        }
        public int IdCar { get => idCar; set => idCar = value; }
        public string Manufacturer { get => manufacturer; set => manufacturer = value; }

        public static Manufacture[] GetAllManufacturer()
        {
            DataServices ds = new DataServices();
            return ds.GetAllManufacturer();
        }

        public static Manufacture ReadManufacture(int idCar)
        {
            DataServices ds = new DataServices();
            Manufacture manufacture = ds.ReadManufacture(idCar);
            return manufacture;
        }
    }
}