using ParkingProject.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace ParkingProject.App_Start
{
    public class DataServicesConfig
    {
        public static void CreateTables()
        {
            DataServices ds = new DataServices();
            ds.CreateTables();

        }


    }
}