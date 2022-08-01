using ParkingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ParkingProject.Controllers
{
    public class CarsController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public Cars Get(string numberCar, int id)
        {
            return ParkingProject.Models.Cars.readCar(numberCar, id);
        }

        public Cars Get(string numberCar)
        {
            return ParkingProject.Models.Cars.readOneCar(numberCar);
        }

        public Cars[] Get(int id)
        {
            return Cars.GetAllUserCars(id);
        }

        public Cars Post([FromBody] Cars C)
        {
            return Cars.InsertCar(C);
        }

        public int Put([FromBody] Cars C)
        {
            int id = Cars.UpdateCar(C);
            return id;
        }
        public Cars Put(string numberCar, int id)
        {
            return Cars.MakeMainCar(numberCar, id);
        }

        public int Delete(string numberCar, int id)
        {
            int satatus = Cars.DeleteCar(numberCar, id);
            return satatus;
        }
    }
}