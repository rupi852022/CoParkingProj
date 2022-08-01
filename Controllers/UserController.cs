using ParkingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ParkingProject.Controllers
{
    public class UsersController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public Tuple<User, Cars, Manufacture> Get(string email)
        {

            User user = Models.User.readUserMail(email);
            if (user is null) return null;
            if (user.Status == "off")
            {
                return new Tuple<User, Cars, Manufacture>(user, null, null);
            }
            else
            {
                Cars cars = Cars.ReadMainCar(user.Id);
                if (cars is null)
                {
                    return new Tuple<User, Cars, Manufacture>(user, null, null);
                }
                else
                {
                    Manufacture manufacture = Manufacture.ReadManufacture(cars.Idcar);
                    return new Tuple<User, Cars, Manufacture>(user, cars, manufacture);
                }

            }
        }

        public Tuple<User, Cars, Manufacture> Get(string email, string password)
        {

            User user = Models.User.readUser(email, password);
            if(user is null) { return null; }
            if (user.Status == "off")
            {
                return new Tuple<User, Cars, Manufacture>(user, null, null);
            }
            else
            {
                Cars cars = Cars.ReadMainCar(user.Id);
                if (cars is null)
                {
                    return new Tuple<User, Cars, Manufacture>(user, null, null);
                }
                else
                {
                    Manufacture manufacture = Manufacture.ReadManufacture(cars.Idcar);
                    return new Tuple<User, Cars, Manufacture>(user, cars, manufacture);
                }

            }
        }

        public User Get(int id)
        {
            return ParkingProject.Models.User.readUserId(id);
        }

        public User Post([FromBody] User U)
        {
            User id = U.Insert();
            return id;
        }

        public int Post(int userGattingRate, int rate)
        {
            User u = new User();
            int id = u.InsertRate(userGattingRate, rate);
            return id;
        }


        public string Put()
        {
            return "hello";
        }

        public User[] Delete()
        {
            return ParkingProject.Models.User.GetAll();
        }
    }
}