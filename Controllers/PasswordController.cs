using ParkingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ParkingProject.Controllers
{
    public class PasswordController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public string Post(int id)
        {
            return "value";
        }

        public bool Get(string email)
        {
            return Password.SendinEmail(email);
        }

        public int PUT(string email, string currentPassword, string password1, string password2)
        {
            return Password.UpdatePassword(email, currentPassword, password1, password2);
        }

        public void Delete(int id)
        {
        }
    }
}