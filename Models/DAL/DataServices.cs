using ParkingProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace ParkingProject.Models.DAL
{
    public class DataServices
    {

        public string ErrorMessage = "";
        int M = 0;
        DateTime TimeToRelease;


        protected SqlConnection Connect(string connectionStringName)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

            SqlConnection con = new SqlConnection(connectionString);

            con.Open();

            return con;
        }

        public User[] GetAllUsers()
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT * FROM [CoParkingUsers_2022]", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<User> users = new List<User>();
            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["id"]);
                string currentEmail = (string)dr["email"];
                string password = (string)dr["password"];
                string fName = (string)dr["lName"];
                string lName = (string)dr["fName"];
                string phoneNumber = (string)dr["phoneNumber"];
                char gender = Convert.ToChar(dr["gender"]);
                string image = (string)dr["image"];
                int searchRadius = Convert.ToInt32(dr["searchRadius"]);
                int timeDelta = Convert.ToInt32(dr["timeDelta"]);
                string status = (string)dr["status"];
                int tokens = Convert.ToInt32(dr["tokens"]);



                User user = new User(id, fName, lName, currentEmail, password, gender, phoneNumber, image, searchRadius, timeDelta, status, tokens);
                users.Add(user);
            }

            return users.ToArray();
        }
        public Tuple<List<int>, int, DateTime> GetAllUsersVip(int parking)
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT distinct userCode,CONVERT(varchar(30), [releaseDate], 0) as [releaseDate],[CoParkingUsers_2022].email FROM [CoParkingUserVip_2022] left join [CoParkingUsers_2022] on [CoParkingUserVip_2022].userCode=[CoParkingUsers_2022].id  where parkingCode='" + parking + "' and userCode is not null", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<int> list = new List<int>();
            DateTime dt = new DateTime();
            while (dr.Read())
            {
                dt = DateTime.Parse((string)dr["releaseDate"]);
                int userCode = Convert.ToInt32(dr["userCode"]);
                string email = (string)(dr["email"]);
                User user = Models.User.readUserMail(email);
                list.Add(userCode);

            }
            return new Tuple<List<int>, int, DateTime>(list, parking, TimeToRelease);
        }

        public int GetAvgRate(int u)
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT AVG(rate) as 'rate' FROM [CoParkingReview_2022] WHERE userCode='" + u + "';", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            int rate = 0;

            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                rate = Convert.ToInt32(dr["rate"]);

            }

            return rate;
        }


        public DateTime GetDate(int parking)
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT distinct userCode,CONVERT(varchar(30), [releaseDate], 0) as [releaseDate],[CoParkingUsers_2022].email FROM [CoParkingUserVip_2022] left join [CoParkingUsers_2022] on [CoParkingUserVip_2022].userCode=[CoParkingUsers_2022].id  where parkingCode='" + parking + "' and userCode is not null", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<int> list = new List<int>();
            DateTime dt = new DateTime();
            while (dr.Read())
            {
                dt = DateTime.Parse((string)dr["releaseDate"]);
                int userCode = Convert.ToInt32(dr["userCode"]);
                string email = (string)(dr["email"]);
                User user = Models.User.readUserMail(email);
                list.Add(userCode);

            }
            return dt;
        }


        public Cars[] GetAllUserCars(int id)
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT * FROM[CoParkingUsersCars_2022] LEFT JOIN[CoParkingCars_2022] ON CoParkingUsersCars_2022.numberCar = [CoParkingCars_2022].numberCar LEFT JOIN[CoParkingManufacture_2022] ON[CoParkingCars_2022].idCar = [CoParkingManufacture_2022].idCar WHERE[CoParkingUsersCars_2022].id ="
                + id, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<Cars> carsList = new List<Cars>();
            while (dr.Read())
            {
                string numberCar = (string)(dr["numberCar"]);
                int idCar = Convert.ToInt32(dr["idCar"]);
                int year = Convert.ToInt32(dr["year"]);
                string model = (string)dr["model"];
                string color = (string)dr["color"];
                int size = Convert.ToInt32(dr["size"]);
                string manufacture = (string)dr["manufacturer"];

                bool currentIsmain = true;
                string isMain = (string)dr["isMain"];
                if (isMain == "F") { currentIsmain = false; }

                Cars cars = new Cars(id, numberCar, currentIsmain, idCar, year, model, color, size, manufacture);


                carsList.Add(cars);
            }

            return carsList.ToArray();
        }

        public int GetUserCar(int id)
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT * FROM[CoParkingUsersCars_2022] LEFT JOIN[CoParkingCars_2022] ON CoParkingUsersCars_2022.numberCar = [CoParkingCars_2022].numberCar LEFT JOIN[CoParkingManufacture_2022] ON[CoParkingCars_2022].idCar = [CoParkingManufacture_2022].idCar WHERE[CoParkingUsersCars_2022].id ="
                + id, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            int affected = command.ExecuteNonQuery();
            return affected;
        }

        public User InsertUser(User U, int typeOf)
        {
            SqlConnection con = null;
            try
            {
                User user = this.ReadUserEmail(U.Email, 2);

                if (U.Gender == 0)
                {
                    ErrorMessage = "the Gender need to be with one char";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }

                if (user != null)
                {
                    ErrorMessage = "The email is not exist";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }
                else
                {
                    if (ValidateEmail(U.Email) is false)
                    {
                        Exception ex = new Exception(ErrorMessage);
                        throw ex;
                    }
                    if (ValidatePassword(U.Password) is false)
                    {
                        Exception ex = new Exception(ErrorMessage);
                        throw ex;
                    }
                }
                con = Connect("webOsDB");
                SqlCommand command = CreateInsertUser(U, con);
                int affected = command.ExecuteNonQuery();

                return U;

            }
            catch(Exception)
            {
                return (null);
            }

            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public Tuple<Parking, bool, DateTime, Cars, User>[] GetAllParkings(int id)
        {

            SqlConnection con = this.Connect("webOsDB");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SqlCommand command = new SqlCommand(
    "select distinct [CoParkingParkings_2022].[parkingCode],[LocationLng],[LocationLat],[LocationName], CONVERT(varchar(30), [exitDate], 0) as [exitDate], CONVERT(varchar(30), [UploadDate], 0) as [UploadDate],[typeOfParking],[signType],[userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn], CoParkingUserVIP_2022.parkingCode as 'parkingCodeVip',CONVERT(varchar(30), [CoParkingUserVIP_2022].releaseDate , 0) as [releaseDate]  from [CoParkingParkings_2022] LEFT JOIN [CoParkingUserVIP_2022] ON [CoParkingParkings_2022].parkingCode = [CoParkingUserVIP_2022].parkingCode where [exitDate] >= '" + currentDate + "' AND [userCodeIn] IS NULL AND [userCodeOut] != '" + id + "';"
    , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            var tupleList = new List<Tuple<Parking, bool, DateTime, Cars, User>>();
            bool parkingforuser = false;


            while (dr.Read())
            {
                int parkingCode = Convert.ToInt32(dr["parkingCode"]);
                double locationLng = Convert.ToDouble((string)dr["LocationLng"]);
                double locationLat = Convert.ToDouble((string)dr["LocationLat"]);
                string locationName = (string)dr["LocationName"];
                DateTime exitDate = DateTime.Parse((string)dr["exitDate"]);
                DateTime uploadDate = DateTime.Parse((string)dr["uploadDate"]);
                int typeOfParking = Convert.ToInt32(dr["typeOfParking"]);
                string signType = (string)dr["signType"];
                int userCodeOut = Convert.ToInt32(dr["userCodeOut"]);
                string numberCarOut = (string)dr["numberCarOut"];
                DateTime releaseDate;
                if (dr["releaseDate"] != DBNull.Value)
                {
                    releaseDate = DateTime.Parse((string)dr["releaseDate"]);
                }
                else
                {
                    releaseDate = DateTime.MinValue;
                }

                if (dr["parkingCodeVip"] != DBNull.Value)
                {
                    parkingforuser = true;
                }

                if (dr["userCodeIn"] != DBNull.Value)
                {
                    int userCodeIn = Convert.ToInt32(dr["userCodeIn"]);
                    string numberCarIn = (string)dr["numberCarIn"];
                    Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut, userCodeIn, numberCarIn, uploadDate);
                    Cars c = ParkingProject.Models.Cars.readCar(parking.NumberCarOut, parking.UserCodeOut);
                    User u = ParkingProject.Models.User.readUserId(parking.UserCodeOut);
                    updateWithAlgoritems(parking);
                    if (checkIfParkingForUser(parking.ParkingCode, id) is true)
                    {
                        tupleList.Add(new Tuple<Parking, bool, DateTime, Cars, User>(parking, parkingforuser, releaseDate, c, u));
                    }

                }
                else
                {
                    Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut);
                    Cars c = ParkingProject.Models.Cars.readCar(parking.NumberCarOut, parking.UserCodeOut);
                    User u = ParkingProject.Models.User.readUserId(parking.UserCodeOut);
                    updateWithAlgoritems(parking);
                    if (checkIfParkingForUser(parking.ParkingCode, id) is true)
                    {
                        tupleList.Add(new Tuple<Parking, bool, DateTime, Cars, User>(parking, true, releaseDate, c, u));
                    }
                    else
                    {
                        tupleList.Add(new Tuple<Parking, bool, DateTime, Cars, User>(parking, false, releaseDate, c, u));
                    }

                }

            }
            return tupleList.ToArray();
        }


        public Tuple<Parking, Cars, Cars, User>[] GetAllParkingsUserFuture(int id)
        {
            var tupleList = new List<Tuple<Parking, Cars, Cars, User>>();
            SqlConnection con = this.Connect("webOsDB");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SqlCommand command = new SqlCommand(
"select[parkingCode],[LocationLng],[LocationLat],[LocationName], CONVERT(varchar(30), [exitDate], 0) as [exitDate], CONVERT(varchar(30), [UploadDate], 0) as [UploadDate],[typeOfParking],[signType],[userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn]  from[CoParkingParkings_2022] where isHistory = 'N' and(userCodeOut = " + id + " or userCodeIn = " + id + ") ORDER BY exitDate;"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<Parking> parkings = new List<Parking>();
            while (dr.Read())
            {
                int parkingCode = Convert.ToInt32(dr["parkingCode"]);
                double locationLng = Convert.ToDouble((string)dr["LocationLng"]);
                double locationLat = Convert.ToDouble((string)dr["LocationLat"]);
                string locationName = (string)dr["LocationName"];
                DateTime exitDate = DateTime.Parse((string)dr["exitDate"]);
                DateTime uploadDate = DateTime.Parse((string)dr["uploadDate"]);
                int typeOfParking = Convert.ToInt32(dr["typeOfParking"]);
                string signType = (string)dr["signType"];
                int userCodeOut = Convert.ToInt32(dr["userCodeOut"]);
                string numberCarOut = (string)dr["numberCarOut"];
                Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut, uploadDate);
                updateWithAlgoritems(parking);
                if (checkIfParkingForUser(parking.ParkingCode, id) is true)
                { parkings.Add(parking); }
                Cars c = ParkingProject.Models.Cars.readCar(parking.NumberCarOut, parking.UserCodeOut);
                Cars d = null;
                User u = null;
                if (parking.NumberCarIn != null)
                {
                    d = ParkingProject.Models.Cars.readCar(parking.NumberCarIn, parking.UserCodeIn);
                    u = ParkingProject.Models.User.readUserId(parking.UserCodeIn);
                }

                tupleList.Add(new Tuple<Parking, Cars, Cars, User>(parking, c, d, u));
            }


            return tupleList.ToArray();
        }

        public Parking[] GetAllOnlyParkingsUser(int id)
        {
            var List = new List<Parking>();
            SqlConnection con = this.Connect("webOsDB");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SqlCommand command = new SqlCommand(
"select[parkingCode],[LocationLng],[LocationLat],[LocationName], CONVERT(varchar(30), [exitDate], 0) as [exitDate], CONVERT(varchar(30), [UploadDate], 0) as [UploadDate],[typeOfParking],[signType],[userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn]  from[CoParkingParkings_2022] where[exitDate] >= '" + currentDate + "' AND isHistory = 'N' and(userCodeOut = " + id + " or userCodeIn = " + id + ") ORDER BY exitDate;"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<Parking> parkings = new List<Parking>();
            while (dr.Read())
            {
                int parkingCode = Convert.ToInt32(dr["parkingCode"]);
                double locationLng = Convert.ToDouble((string)dr["LocationLng"]);
                double locationLat = Convert.ToDouble((string)dr["LocationLat"]);
                string locationName = (string)dr["LocationName"];
                DateTime exitDate = DateTime.Parse((string)dr["exitDate"]);
                DateTime uploadDate = DateTime.Parse((string)dr["uploadDate"]);
                int typeOfParking = Convert.ToInt32(dr["typeOfParking"]);
                string signType = (string)dr["signType"];
                int userCodeOut = Convert.ToInt32(dr["userCodeOut"]);
                string numberCarOut = (string)dr["numberCarOut"];
                Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut, uploadDate);
                updateWithAlgoritems(parking);
                if (checkIfParkingForUser(parking.ParkingCode, id) is true)
                { parkings.Add(parking); }

                List.Add(parking);
            }


            return List.ToArray();
        }

        public Tuple<Parking, Cars, Cars, User>[] GetAllParkingsUser(int id)
        {
            var tupleList = new List<Tuple<Parking, Cars, Cars, User>>();
            SqlConnection con = this.Connect("webOsDB");
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SqlCommand command = new SqlCommand(
"select[parkingCode],[LocationLng],[LocationLat],[LocationName], CONVERT(varchar(30), [exitDate], 0) as [exitDate], CONVERT(varchar(30), [UploadDate], 0) as [UploadDate],[typeOfParking],[signType],[userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn]  from[CoParkingParkings_2022] where[exitDate] >= '" + currentDate + "' AND isHistory = 'N' and(userCodeOut = " + id + " or userCodeIn = " + id + ") ORDER BY exitDate;"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<Parking> parkings = new List<Parking>();
            while (dr.Read())
            {
                int parkingCode = Convert.ToInt32(dr["parkingCode"]);
                double locationLng = Convert.ToDouble((string)dr["LocationLng"]);
                double locationLat = Convert.ToDouble((string)dr["LocationLat"]);
                string locationName = (string)dr["LocationName"];
                DateTime exitDate = DateTime.Parse((string)dr["exitDate"]);
                DateTime uploadDate = DateTime.Parse((string)dr["uploadDate"]);
                int typeOfParking = Convert.ToInt32(dr["typeOfParking"]);
                string signType = (string)dr["signType"];
                int userCodeOut = Convert.ToInt32(dr["userCodeOut"]);
                string numberCarOut = (string)dr["numberCarOut"];
                Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut, uploadDate);
                updateWithAlgoritems(parking);
                if (checkIfParkingForUser(parking.ParkingCode, id) is true)
                { parkings.Add(parking); }
                Cars c = ParkingProject.Models.Cars.readCar(parking.NumberCarOut, parking.UserCodeOut);
                Cars d = null;
                User u = null;
                if (parking.NumberCarIn != null)
                {
                    d = ParkingProject.Models.Cars.readCar(parking.NumberCarIn, parking.UserCodeIn);
                    u = ParkingProject.Models.User.readUserId(parking.UserCodeIn);
                }

                tupleList.Add(new Tuple<Parking, Cars, Cars, User>(parking, c, d, u));
            }


            return tupleList.ToArray();
        }


        public Tuple<Parking, Cars, Cars, User> GetParkingUser(int idUser, int idParking)
        {
            SqlConnection con = this.Connect("webOsDB");
            int currentId = 0;
            int userCodeIn = 0;
            int userCodeOut = 0;
            string numberCarOut = "";
            string numberCarIn = "";
            string str = "SELECT [userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn] FROM [igroup85_test2].[dbo].[CoParkingParkings_2022] where [CoParkingParkings_2022].parkingCode='" + idParking + "'";
            SqlCommand command = new SqlCommand(str, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<Parking> parkings = new List<Parking>();
            while (dr.Read())
            {
                userCodeOut = Convert.ToInt32(dr["userCodeOut"]);
                numberCarOut = (string)dr["numberCarOut"];

                if (dr["userCodeIn"] != DBNull.Value)
                {
                    userCodeIn = Convert.ToInt32(dr["userCodeIn"]);
                    numberCarIn = (string)dr["numberCarIn"];
                }
            }

            if (userCodeIn == idUser)
            {
                if (userCodeOut != 0)
                { currentId = userCodeOut; }

            }
            else
            {
                currentId = userCodeIn;
            }

            Parking p = GetParking(idParking);
            Cars cOut = ReadCarAndPic(numberCarOut, userCodeOut);
            if (userCodeIn != 0 && currentId != 0)
            {
                Cars cIn = ReadCarAndPic(numberCarIn, userCodeIn);
                User u = ReadUserId(currentId);
                return new Tuple<Parking, Cars, Cars, User>(p, cIn, cOut, u);
            }
            else
            {
                return new Tuple<Parking, Cars, Cars, User>(p, null, cOut, null);
            }
        }











        public bool checkIfParkingForUser(int ParkingCode, int id)
        {
            SqlConnection con = Connect("webOsDB");
            SqlCommand command = new SqlCommand("select parkingCode from [CoParkingUserVIP_2022] where parkingCode='" + ParkingCode + "' and userCode='" + id + "';", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                if (con != null)
                    con.Close();
                if (dr != null)
                {
                    dr.Close();
                }
                return true;
            }
            if (con != null)
                con.Close();
            if (dr != null)
            {
                dr.Close();
            }
            return false;
        }

        public bool checkIfUserInExist(int parkingCode)
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select [parkingCode],[LocationLng],[LocationLat],[LocationName], CONVERT(varchar(30), [exitDate], 0) as [exitDate], CONVERT(varchar(30), [UploadDate], 0) as [UploadDate],[typeOfParking],[signType],[userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn]  from [CoParkingParkings_2022] where [parkingCode] = '" + parkingCode + "';"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                if (dr["userCodeIn"] != DBNull.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            Exception ex = new Exception("the Parking not exist");
            throw ex;
        }
        public Parking GetParking(int parkingCode)
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select [parkingCode],[LocationLng],[LocationLat],[LocationName], CONVERT(varchar(30), [exitDate], 0) as [exitDate], CONVERT(varchar(30), [UploadDate], 0) as [UploadDate],[typeOfParking],[signType],[userCodeOut],[numberCarOut],[userCodeIn],[numberCarIn]  from [CoParkingParkings_2022] where [parkingCode] = '" + parkingCode + "';"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                double locationLng = Convert.ToDouble((string)dr["LocationLng"]);
                double locationLat = Convert.ToDouble((string)dr["LocationLat"]);
                string locationName = (string)dr["LocationName"];
                DateTime exitDate = DateTime.Parse((string)dr["exitDate"]);
                DateTime uploadDate = DateTime.Parse((string)dr["uploadDate"]);
                int typeOfParking = Convert.ToInt32(dr["typeOfParking"]);
                string signType = (string)dr["signType"];
                int userCodeOut = Convert.ToInt32(dr["userCodeOut"]);
                string numberCarOut = (string)dr["numberCarOut"];
                if (dr["userCodeIn"] != DBNull.Value)
                {
                    int userCodeIn = Convert.ToInt32(dr["userCodeIn"]);
                    string numberCarIn = (string)dr["numberCarIn"];
                    Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut, userCodeIn, numberCarIn, uploadDate);
                    return parking;
                }
                else
                {
                    Parking parking = new Parking(parkingCode, locationLng, locationLat, locationName, exitDate, typeOfParking, signType, userCodeOut, numberCarOut, uploadDate);
                    return parking;
                }

            }

            Exception ex = new Exception("the Parking not exist");
            throw ex;
        }

        public int GetParkingId()
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "  select max(parkingCode) as 'parkingCode' from [CoParkingParkings_2022];"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                int parkingCode = Convert.ToInt32(dr["parkingCode"]);
                return parkingCode;
            }

            Exception ex = new Exception("the Parking Id not exist");
            throw ex;
        }


        public int howMuchUsers()
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select count(distinct id) as 'countUser' from CoParkingUsers_2022 where id is not null"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                return Convert.ToInt32(dr["countUser"]);

            }
            Exception ex = new Exception("problem");
            throw ex;
        }

        public int howMuchUsersVip(int parking)
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select count(distinct userCode) as 'countUser' from CoParkingUserVIP_2022 where userCode is not null and parkingCode = '" + parking + "'"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                return Convert.ToInt32(dr["countUser"]);

            }
            Exception ex = new Exception("problem");
            throw ex;
        }



        public Manufacture[] GetAllManufacturer()
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand("SELECT * FROM [CoParkingManufacture_2022]", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            List<Manufacture> manufactures = new List<Manufacture>();
            while (dr.Read())
            {
                int idCar = Convert.ToInt32(dr["idCar"]);
                string manufacturer1 = (string)dr["manufacturer"];
                Manufacture manufacturer = new Manufacture(idCar, manufacturer1);
                manufactures.Add(manufacturer);
            }

            return manufactures.ToArray();
        }

        public Tuple<List<int>, int, DateTime> InsertParking(Parking P)
        {
            Console.WriteLine(P.ExitDate);
            SqlConnection con = null;
            int idParkingCode;
            try
            {
                con = Connect("webOsDB");

                SqlCommand command = CreateInsertParking(P, con);
                int affected = command.ExecuteNonQuery();
                idParkingCode = GetParkingId();
                Parking parking = new Parking(idParkingCode, P.LocationLng, P.LocationLat, P.LocationName, P.ExitDate, P.TypeOfParking, P.SignType, P.UserCodeOut, P.NumberCarOut, P.UserCodeIn, P.NumberCarIn, GetParking(idParkingCode).UploadDate);
                bool forAll = updateWithAlgoritems(parking);
                if (forAll is true)
                {
                    return new Tuple<List<int>, int, DateTime>(null, idParkingCode, DateTime.MinValue);
                }

                else { return GetAllUsersVip(idParkingCode); };

            }
            catch (Exception ex)
            {
                ErrorMessage = "proble with insert parking";
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public int updateTokens(int UserId, int tokens)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");
                User U = ReadUserId(UserId);
                int oldTokens = U.Tokens;
                int newTokens = oldTokens + tokens;
                if (newTokens <= 0) { newTokens = 0; };

                SqlCommand command = CreateupdateTokens(UserId, newTokens, con);

                int affected = command.ExecuteNonQuery();

                return affected;

            }
            catch (Exception ex)
            {
                ErrorMessage = "proble with insert parking";
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }


        public bool updateWithAlgoritems(Parking P)
        {
            updatePriorityUsers();
            if (checkMinutes(P) == 1)
            {
                algoritem1(P);
                return true;
            }
            if (checkMinutes(P) == 2)
            {
                algoritem2(P);
                return false;
            }
            else
                algoritem1(P);
            return true;
        }

        public int UpdateParking(Parking P)
        {
            Console.WriteLine(P.ExitDate);
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");

                SqlCommand command = CreateUpdateParking(P, con);

                int affected = command.ExecuteNonQuery();

                return affected;

            }
            catch (Exception ex)
            {
                ErrorMessage = "problem with update parking";
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public int algoritem2(Parking P)
        {
            M = numberOfM();
            string str = "";
            SqlConnection con = this.Connect("webOsDB");
            str += "DELETE FROM [CoParkingUserVIP_2022] WHERE parkingCode = '" + P.ParkingCode + "';";

            if (P.TypeOfParking == 2)
            {
                str += "INSERT INTO [CoParkingUserVIP_2022] SELECT distinct TOP " + M + " '" + P.ParkingCode + "' as 'parking',[CoParkingUsers_2022].id, priorityLevel,DATEADD(minute, -30, [CoParkingParkings_2022].exitDate) as 'releaseDate' FROM[CoParkingUsers_2022] LEFT JOIN[CoParkingUsersCars_2022] ON[CoParkingUsers_2022].id = [CoParkingUsersCars_2022].id   LEFT JOIN [CoParkingParkings_2022] ON [CoParkingParkings_2022].parkingCode ='" + P.ParkingCode + "' where handicapped = 'T' and not priorityLevel=0 and not tokens<11 ORDER BY priorityLevel DESC";

            }
            else
            {
                str += "INSERT INTO [CoParkingUserVIP_2022] SELECT distinct TOP " + M + " '" + P.ParkingCode + "' as 'parking',[CoParkingUsers_2022].id, priorityLevel,DATEADD(minute, -30, [CoParkingParkings_2022].exitDate) as 'releaseDate' FROM[CoParkingUsers_2022] LEFT JOIN[CoParkingUsersCars_2022] ON[CoParkingUsers_2022].id = [CoParkingUsersCars_2022].id   LEFT JOIN [CoParkingParkings_2022] ON [CoParkingParkings_2022].parkingCode ='" + P.ParkingCode + "' where not priorityLevel=0 and not tokens<11 ORDER BY priorityLevel DESC";
            }

            SqlCommand command = new SqlCommand(str, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            SqlDataReader dr = command.ExecuteReader();
            if (con != null)
                con.Close();
            if (dr != null)
            {
                dr.Close();
            }

            return 1;
        }

        public int algoritem1(Parking P)
        {
            M = numberOfUsers();
            string str = "";
            SqlConnection con = this.Connect("webOsDB");
            str += "DELETE FROM [CoParkingUserVIP_2022] WHERE parkingCode = '" + P.ParkingCode + "';";
            if (P.TypeOfParking == 2)
            {
                str += "INSERT INTO [CoParkingUserVIP_2022] SELECT distinct '" + P.ParkingCode + "' as 'parking',[CoParkingUsers_2022].id, priorityLevel,DATEADD(minute, -30, [CoParkingParkings_2022].exitDate) as 'releaseDate' FROM[CoParkingUsers_2022] LEFT JOIN[CoParkingUsersCars_2022] ON[CoParkingUsers_2022].id = [CoParkingUsersCars_2022].id LEFT JOIN [CoParkingParkings_2022] ON [CoParkingParkings_2022].parkingCode ='" + P.ParkingCode + "' where handicapped = 'T' and not priorityLevel=0 ORDER BY priorityLevel DESC";

            }
            else
            {
                str += "INSERT INTO [CoParkingUserVIP_2022] SELECT distinct '" + P.ParkingCode + "' as 'parking',[CoParkingUsers_2022].id, priorityLevel,DATEADD(minute, -30, [CoParkingParkings_2022].exitDate) as 'releaseDate' FROM[CoParkingUsers_2022] LEFT JOIN[CoParkingUsersCars_2022] ON[CoParkingUsers_2022].id = [CoParkingUsersCars_2022].id LEFT JOIN [CoParkingParkings_2022] ON [CoParkingParkings_2022].parkingCode ='" + P.ParkingCode + "' where not priorityLevel=0 ORDER BY priorityLevel DESC";
            }

            SqlCommand command = new SqlCommand(str, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            SqlDataReader dr = command.ExecuteReader();
            if (con != null)
                con.Close();
            if (dr != null)
            {
                dr.Close();
            }

            return 1;
        }

        public void updatePriorityUsers()
        {
            string str = "";
            SqlConnection con = Connect("webOsDB");
            SqlCommand command = new SqlCommand("  select DISTINCT [CoParkingUsers_2022].[id],[CoParkingUsers_2022].[email],[CoParkingUsers_2022].[password],[CoParkingUsers_2022].[fName],[CoParkingUsers_2022].[lName],[CoParkingUsers_2022].[phoneNumber],[CoParkingUsers_2022].[gender],[CoParkingUsers_2022].[image],[CoParkingUsers_2022].[searchRadius],[CoParkingUsers_2022].[timeDelta],[CoParkingUsers_2022].[status],[CoParkingUsers_2022].[tokens],[CoParkingUsers_2022].[priorityLevel],[CoParkingUsersCars_2022].carPic from [CoParkingUsers_2022] LEFT JOIN CoParkingUsersCars_2022 ON[CoParkingUsers_2022].id = CoParkingUsersCars_2022.id;", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            SqlDataReader dr = command.ExecuteReader();
            int tmpPriority = 1;
            while (dr.Read())
            {
                tmpPriority = 1;
                int id = Convert.ToInt32(dr["id"]);
                string pic = "";
                if (dr["carPic"] != DBNull.Value)
                {
                    pic = (string)dr["carPic"];
                }

                if (String.IsNullOrEmpty(pic))
                { tmpPriority = 0; }

                int tokens = Convert.ToInt32(dr["tokens"]);
                string image = (string)dr["image"];
                int scoreUserPicture = 1;
                if (String.IsNullOrEmpty(image))
                { scoreUserPicture = 0; }
                double newPriority = 0.25 * (Phi((tokens - 30) / (10))) + 0.015 * scoreUserPicture + 0.035 * tmpPriority;
                str += " UPDATE[CoParkingUsers_2022] SET priorityLevel = " + newPriority + " WHERE id = " + id + " ";
            }
            if (con != null)
                con.Close();
            if (dr != null)
            {
                dr.Close();
            }
            command = null;
            dr = null;
            con = Connect("webOsDB");
            command = new SqlCommand(str, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            dr = command.ExecuteReader();
        }

        public int numberOfM()
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select count(id) as 'M'  from [CoParkingUsers_2022] where status='on' ", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                M = Convert.ToInt32(dr["M"]);
                M = M / 10;
            }
            if (M <= 1) { M = 0; }
            if (M >= 10) { M = 10; }
            return M;
        }

        public int numberOfUsers()
        {
            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select count(id) as 'M'  from [CoParkingUsers_2022]", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                M = Convert.ToInt32(dr["M"]);
            }
            if (M <= 1) { M = 0; }
            return M;
        }

        public int checkMinutes(Parking P)
        {
            string ParkingDate = P.ExitDate.ToString("yyyy-MM-dd HH:mm:ss");
            string time = ParkingDate + ",531";
            DateTime myDate = DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

            string uploadDate = P.UploadDate.ToString("yyyy-MM-dd HH:mm:ss");
            string timeUpload = uploadDate + ",531";
            DateTime mytimeUpload = DateTime.ParseExact(timeUpload, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);


            int totalMinute = (int)(myDate - (DateTime.Now)).TotalMinutes;
            int Timer30Min = (int)((DateTime.Now) - mytimeUpload).TotalMinutes;
            Console.WriteLine(totalMinute);

            int timeToRelease = Math.Min(30, (totalMinute / 4));
            if (totalMinute <= 30)
            {
                return 1;

            }
            if (totalMinute <= 690)
            {
                if (Timer30Min <= timeToRelease)
                {
                    TimeToRelease = (DateTime.Now).AddMinutes(timeToRelease);
                    return 2;

                }
                return 2;
            }

            else { return 2; }
        }

        SqlCommand CreateInsertParking(Parking P, SqlConnection con)
        {
            string insertStr = "";
            Console.WriteLine(P.ExitDate);
            string currentexitDate = P.ExitDate.ToString("yyyy-MM-dd HH:mm:ss");
            string uploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (P.UserCodeIn == 0)
            {
                insertStr += " INSERT INTO [CoParkingParkings_2022] ([LocationLng],[LocationLat],[LocationName], [exitDate], [typeOfParking], [signType], [userCodeOut], [numberCarOut],[UploadDate]) VALUES('" + P.LocationLng + "', '" + P.LocationLat + "', @LocationName, '" + currentexitDate + "', '" + P.TypeOfParking + "', '" + P.SignType + "', '" + P.UserCodeOut + "', '" + P.NumberCarOut + "','" + uploadDate + "')";

            }
            else
            {
                insertStr += " INSERT INTO [CoParkingParkings_2022] ([LocationLng],[LocationLat],[LocationName], [exitDate], [typeOfParking], [signType], [userCodeOut], [numberCarOut], [userCodeIn], [numberCarIn],[UploadDate]) VALUES('" + P.LocationLng + "', '" + P.LocationLat + "', @LocationName, '" + currentexitDate + "', '" + P.TypeOfParking + "', '" + P.SignType + "', '" + P.UserCodeOut + "', '" + P.NumberCarOut + "', '" + P.UserCodeIn + "', '" + P.NumberCarIn + "','" + uploadDate + "')";
            }
            SqlCommand command = new SqlCommand(insertStr, con);
            command.Parameters.AddWithValue("@LocationName", P.LocationName);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;


        }

        SqlCommand CreateupdateTokens(int UserId, int tokens, SqlConnection con)
        {
            string str;
            str = "UPDATE [CoParkingUsers_2022] SET tokens = " + tokens + " WHERE id = " + UserId + ";";


            SqlCommand command = new SqlCommand(str, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        SqlCommand CreateUpdateParking(Parking P, SqlConnection con)
        {
            string insertStr = "";
            Console.WriteLine(P);
            string currentexitDate = P.ExitDate.ToString("yyyy-MM-dd HH:mm:ss");
            if (P.UserCodeIn == 0)
            {
                insertStr += "UPDATE[CoParkingParkings_2022] SET[LocationLng] = '" + P.LocationLng + "', [LocationLat] = '" + P.LocationLat + "',[LocationName]= '" + P.LocationName + "',[exitDate]= '" + currentexitDate + "',[typeOfParking]= '" + P.TypeOfParking + "',[signType]= '" + P.SignType + "',[userCodeOut]= '" + P.UserCodeOut + "',[numberCarOut]= '" + P.NumberCarOut + "' WHERE[parkingCode] = '" + P.ParkingCode + "'";
            }
            else
            {
                insertStr += insertStr += "UPDATE[CoParkingParkings_2022] SET[LocationLng] = '" + P.LocationLng + "', [LocationLat] = '" + P.LocationLat + "',[LocationName]= '" + P.LocationName + "',[exitDate]= '" + currentexitDate + "',[typeOfParking]= '" + P.TypeOfParking + "',[signType]= '" + P.SignType + "',[userCodeOut]= '" + P.UserCodeOut + "',[numberCarOut]= '" + P.NumberCarOut + "',[numberCarIn]= '" + P.NumberCarIn + "',[userCodeIn]='" + P.UserCodeIn + "' WHERE[parkingCode] = '" + P.ParkingCode + "'";
            }
            SqlCommand command = new SqlCommand(insertStr, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }
        SqlCommand CreateInsertUserCar(Cars U, string currentMain, int carExist, SqlConnection con)
        {

            if (CheckUserAndNumberCar(U.Id, U.NumberCar) == 1)
            {
                Exception ex = new Exception("the User and the NumberCar are exist.");
                throw ex;
            }
            string insertStr = "";
            string Handicapped = "T";


            SqlCommand command = new SqlCommand("SELECT * FROM CoParkingUsersCars_2022 WHERE id=" + U.Id, con);
            command.Parameters.AddWithValue("@id", U.Id);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            if (U.Handicapped is false) { Handicapped = "F"; }

            currentMain = "T";
            insertStr += " UPDATE [CoParkingUsersCars_2022] SET [isMain] = 'F' where id = '" + U.Id + "'";
            insertStr += " UPDATE [CoParkingUsers_2022] SET [status] = 'on' where [id] = '" + U.Id + "'";

            if (carExist == 2)
            {
                insertStr += " INSERT INTO [CoParkingUsersCars_2022] ([id], [numberCar], [isMain], [handicapped], [carPic] , [canEditCar]) VALUES('" + U.Id + "', '" + U.NumberCar + "', '" + currentMain + "', '" + Handicapped + "', '" + U.CarPic + "','T')";
            }
            else
            {
                insertStr += " UPDATE [CoParkingUsersCars_2022] SET canEditCar = 'F' where numberCar = '" + U.NumberCar + "'";
                insertStr += " INSERT INTO [CoParkingUsersCars_2022] ([id], [numberCar], [isMain], [handicapped], [carPic] , [canEditCar]) VALUES('" + U.Id + "', '" + U.NumberCar + "', '" + currentMain + "', '" + Handicapped + "', '" + U.CarPic + "','F')";
            }
            SqlCommand command2 = new SqlCommand(insertStr, con);
            command2.CommandType = System.Data.CommandType.Text;
            command2.CommandTimeout = 30;
            return command2;

        }

        protected bool ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
                return true;
            else
                ErrorMessage = "The Email with Uncorrect format";
            Exception ex = new Exception(ErrorMessage);
            throw ex;
        }

        public Cars InsertCars(Cars C)
        {
            int CarExist = 1;

            if (ReadUser(C.NumberCar) is null)
            {
                CarExist = 2;
            }

            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");

                if (checkedNumberCar(C.NumberCar, C.Id) == 1)
                {
                    Exception ex = new Exception("the NumberCar is exist.");
                    throw ex;
                }

                if (C.Id == 0)
                {
                    SqlCommand command = CreateInsertCar(C, CarExist, con);
                    int affected = command.ExecuteNonQuery();

                    Exception ex = new Exception("Enter Id");
                    throw ex;
                }
                else
                {
                    if (C.Idcar != 0)     
                    {

                        SqlCommand command2 = CreateInsertCar(C, CarExist, con);
                        int affected2 = (command2.ExecuteNonQuery());
                        string currentMain = "F";
                        SqlDataReader dr = null;
                        string commandStr = "SELECT * FROM CoParkingUsersCars_2022 WHERE id=" + C.Id;
                        SqlCommand cmd = createCommand(con, commandStr);
                        cmd.Parameters.AddWithValue("@numberCar", C.Id);
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                        if (dr == null || !dr.Read())
                        {
                            currentMain = "T";

                        }
                        if (con != null)
                            con.Close();

                        con = null;
                        con = Connect("webOsDB");

                        SqlCommand command1 = CreateInsertUserCar(C, currentMain, CarExist, con);
                        int affected1 = (command1.ExecuteNonQuery());
                        int affected = affected2 * affected1;


                        return ReadUserAndCar(C.NumberCar, C.Id);
                    }
                    else
                    {
                        SqlCommand command = CreateInsertUserCar(C, "F", CarExist, con);

                        if (CarExist == 2)
                        { C.CanEditCar = true; }
                        else
                        { C.CanEditCar = false; }

                        return ReadUserAndCar(C.NumberCar, C.Id);

                    }
                }


            }
            catch(Exception)
            {
                return (null);
            }

            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public int checkedNumberCar(string numberCar, int id)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;
            con = Connect("webOsDB");

            SqlCommand selectCommand = creatSelectUserCommand3(con, numberCar, id);
            int affected = selectCommand.ExecuteNonQuery();
            dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

            if (dr == null || !dr.Read())
            {
                return -1;
            }

            if (affected <= 0) { return 1; }
            string number = (string)(dr["numberCar"]);
            if (number == null) { return 1; }
            return -1;

        }

        public int MakeMainCar(string numberCar, int id)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");
                SqlCommand command = CreateUpdateMainCar(numberCar, id, con);
                int affected = command.ExecuteNonQuery();

                return affected;

            }
            catch (Exception ex)
            {
                throw new Exception("Failed in Insert of Main Car", ex);
            }

            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public int UpdateCars(Cars C)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");
                SqlCommand command = CreateUpdateCar(C, con);
                int affected = command.ExecuteNonQuery();

                return affected;

            }
            catch (Exception ex)
            {
                throw new Exception("Failed in Insert of Car", ex);
            }

            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        SqlCommand CreateUpdateCar(Cars C, SqlConnection con)
        {
            string str = "";
            str += "UPDATE [CoParkingCars_2022] " + "SET [idCar] = '" + C.Idcar + "', [year] = '" + C.Year + "', [model] = '" + C.Model + "', [color] = '" + C.Color + "', [size] = '" + C.Size + "' WHERE [numberCar] = '" + C.NumberCar + "'";
            if (C.IsMain == true)
            {
                str += "update[CoParkingUsersCars_2022] set isMain = 'T' where numberCar = '" + C.NumberCar + "'";
            }

            if (C.IsMain == false)
            {
                str += "update[CoParkingUsersCars_2022] set isMain = 'F' where numberCar = '" + C.NumberCar + "'";
            }
            if (C.CarPic.Length >= 1)
            {
                str += "update[CoParkingUsersCars_2022] set carPic = '" + C.CarPic + "' where numberCar = '" + C.NumberCar + "'";
            }
            if (C.Handicapped == true)
            {
                str += "update[CoParkingUsersCars_2022] set handicapped = 'T' where numberCar = '" + C.NumberCar + "'";
            }

            if (C.Handicapped == false)
            {
                str += "update[CoParkingUsersCars_2022] set handicapped = 'F' where numberCar = '" + C.NumberCar + "'";
            }
            SqlCommand command = new SqlCommand(str, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        SqlCommand CreateUpdateMainCar(string numberCar, int id, SqlConnection con)
        {
            SqlCommand command = new SqlCommand(
                  "UPDATE [CoParkingUsersCars_2022] SET isMain = 'F' WHERE id = '" + id + "' UPDATE [CoParkingUsersCars_2022] SET isMain = 'T' WHERE id = '" + id + "' AND numberCar = '" + numberCar + "'", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        public int deleteCar(string numberCar, int id)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");
                SqlDataReader dr = null;
                SqlCommand selectCommand = checkCanEditCar(id, numberCar, con);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                selectCommand = null;
                con = Connect("webOsDB");
                string canEditCar = "";
                string isMain = "";
                if (dr.Read())
                {
                    canEditCar = (string)dr["canEditCar"];
                    isMain = (string)dr["isMain"];
                }
                if (con != null)
                    con.Close();

                con = null;
                con = Connect("webOsDB");
                SqlCommand command = DeleteNumberOfCar(numberCar, id, canEditCar, con);
                int affected = command.ExecuteNonQuery();
                if (con != null)
                    con.Close();
                if (dr != null)
                {
                    dr.Close();
                }
                dr = null;
                con = null;
                con = Connect("webOsDB");

                int status = GetUserCar(id);
                if (status <= 0)
                {
                    if (con != null)
                        con.Close();
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    dr = null;
                    con = null;
                    con = Connect("webOsDB");
                    SqlCommand command1 = MakeStatusOff(id, con);
                    affected = command1.ExecuteNonQuery();
                }
                return status;

            }
            catch (Exception ex)
            {
                throw new Exception("Failed in Insert of Car", ex);
            }

            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        SqlCommand DeleteNumberOfCar(string NumberCar, int id, string canEditCar, SqlConnection con)
        {
            string sub = "";
            if (canEditCar == "T")
            {
                sub = "DELETE FROM[CoParkingUsersCars_2022] WHERE[numberCar] = '" + NumberCar + "' AND [id] = '" + id + "'";
                sub += "delete from [CoParkingCars_2022] where [numberCar] = '" + NumberCar + "'";
            }
            else
            {
                sub = "DELETE FROM[CoParkingUsersCars_2022] WHERE[numberCar] = '" + NumberCar + "' AND [id] = '" + id + "'";
            }


            SqlCommand command = new SqlCommand(sub, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;
        }

        protected SqlCommand CreateInsertUser(User U, SqlConnection con)
        {

            string insertStr = "INSERT INTO [CoParkingUsers_2022] ([email], [password], [fName], [lName], [phoneNumber], [gender], [image],[searchRadius]) VALUES('" + U.Email + "', '" + U.Password + "', '" + U.FName + "', '" + U.LName + "', '" + U.PhoneNumber + "', '" + U.Gender + "', '" + U.Image + "','1000')";
            SqlCommand command = new SqlCommand(insertStr, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        SqlCommand CreateInsertCar(Cars C, int carExist, SqlConnection con)
        {
            string insertStr = "";
            if (carExist == 1)
            {
                insertStr = "SELECT * FROM CoParkingCars_2022 WHERE numberCar='" + ReadUser(C.NumberCar).NumberCar + "'";
            }
            else
            {
                insertStr = "INSERT INTO [CoParkingCars_2022] ([numberCar], [idCar], [year],[model], [color], [size]) VALUES('" + Convert.ToString(C.NumberCar) + "', '" + C.Idcar + "', '" + C.Year + "', '" + C.Model + "', '" + C.Color + "', '" + C.Size + "')";
            }


            SqlCommand command = new SqlCommand(insertStr, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        public User ReadUserEmail(string email, int TypeOf)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = creatSelectUserCommand(con, email);
                int affected = selectCommand.ExecuteNonQuery();
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (TypeOf == 1)
                {
                    ErrorMessage = dr.ToString();
                    if (dr == null || !dr.Read())
                    {
                        ErrorMessage += "               " + dr.ToString() + "        ";
                        ErrorMessage = "the Email is not exist.";
                        Exception ex = new Exception(ErrorMessage);
                        throw ex;
                    }
                }
                else if (TypeOf == 2)
                {
                    if (dr == null || !dr.Read())
                    {
                        return null;
                    }
                }

                int id = Convert.ToInt32(dr["id"]);
                string currentEmail = (string)dr["email"];
                string password = (string)dr["password"];
                string fName = (string)dr["fName"];
                string lName = (string)dr["lName"];
                string phoneNumber = (string)dr["phoneNumber"];
                char gender = char.Parse((string)dr["gender"]);
                string image = (string)dr["image"];
                int searchRadius = Convert.ToInt32(dr["searchRadius"]);
                int timeDelta = Convert.ToInt32(dr["timeDelta"]);
                string status = (string)dr["status"];
                int tokens = Convert.ToInt32(dr["tokens"]);
                User user = new User(id, fName, lName, currentEmail, password, gender, phoneNumber, image, searchRadius, timeDelta, status, tokens);
                if (dr.Read())
                {
                    return null;
                }

                return user;
            }

            catch (Exception)
            {
                return null;
            }

            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                    con.Close();
            }
        }


        public User ReadUserId(int id)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = creatSelectUserCommandId(con, id);
                int affected = selectCommand.ExecuteNonQuery();
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr == null || !dr.Read())
                {
                    ErrorMessage = "the Id is not exist.";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }

                string currentEmail = (string)dr["email"];
                string password = (string)dr["password"];
                string fName = (string)dr["fName"];
                string lName = (string)dr["lName"];
                string phoneNumber = (string)dr["phoneNumber"];
                char gender = char.Parse((string)dr["gender"]);
                string image = (string)dr["image"];
                int searchRadius = Convert.ToInt32(dr["searchRadius"]);
                int timeDelta = Convert.ToInt32(dr["timeDelta"]);
                string status = (string)dr["status"];
                int tokens = Convert.ToInt32(dr["tokens"]);
                User user = new User(id, fName, lName, currentEmail, password, gender, phoneNumber, image, searchRadius, timeDelta, status, tokens);
                if (dr.Read())
                {
                    return null;
                }

                return user;
            }
            catch (Exception)
            {
                return null;
            }

            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                    con.Close();
            }
        }

        public Cars ReadUser(string numberCar)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = creatSelectCarsCommand(con, numberCar);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr == null || !dr.Read())
                {
                    return null;
                }
                string CurrentnumberCar = (string)(dr["numberCar"]);
                int idCar = Convert.ToInt32(dr["idCar"]);
                int year = Convert.ToInt32(dr["year"]);
                string model = (string)dr["model"];
                string color = (string)dr["color"];
                int size = Convert.ToInt32(dr["size"]);

                Cars cars = new Cars(CurrentnumberCar, idCar, year, model, color, size);

                if (dr.Read())
                {
                    return null;
                }

                return cars;
            }


            catch (Exception ex)
            {
                throw new Exception("failed in Log In", ex);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                    con.Close();
            }
        }

        public Cars ReadUserAndCar(string numberCar, int id)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = GetCarAndId(con, numberCar, id);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr == null || !dr.Read())
                {
                    return null;
                }

                string CurrentnumberCar = (string)(dr["numberCar"]);
                int idCar = Convert.ToInt32(dr["idCar"]);
                int year = Convert.ToInt32(dr["year"]);
                string model = (string)dr["model"];
                string color = (string)dr["color"];
                int size = Convert.ToInt32(dr["size"]);
                bool isMain = false;
                if ((string)dr["isMain"] == "T")
                { isMain = true; }
                bool handicapped = false;
                if ((string)dr["handicapped"] == "T")
                { handicapped = true; }
                string carPic = (string)dr["carPic"];
                string manufacturer = (string)dr["manufacturer"];
                string canEditCar = (string)dr["canEditCar"];
                bool currentEditCar = false;
                if (canEditCar == "T")
                { currentEditCar = true; }

                Cars cars = new Cars(id, CurrentnumberCar, isMain, handicapped, carPic, idCar, year, model, color, size, manufacturer, currentEditCar);

                if (dr.Read())
                {
                    return null;
                }

                return cars;
            }


            catch (Exception ex)
            {
                throw new Exception("failed in Log In", ex);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }

                if (con != null)
                    con.Close();
            }
        }

        public Cars ReadCar(string numberCar)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = GetOneCar(con, numberCar);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr == null || !dr.Read())
                {
                    return null;
                }

                string CurrentnumberCar = (string)(dr["numberCar"]);
                int idCar = Convert.ToInt32(dr["idCar"]);
                int year = Convert.ToInt32(dr["year"]);
                string model = (string)dr["model"];
                string color = (string)dr["color"];
                int size = Convert.ToInt32(dr["size"]);
                string manufacturer = (string)dr["manufacturer"];

                Cars cars = new Cars(CurrentnumberCar, idCar, year, model, color, size, manufacturer);

                return cars;
            }


            catch (Exception ex)
            {
                throw new Exception("failed in Log In", ex);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                    con.Close();
            }
        }

        public Cars ReadCarAndPic(string numberCar, int userId)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = GetOneCarAndPic(con, numberCar, userId);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr == null || !dr.Read())
                {
                    return null;
                }

                string CurrentnumberCar = (string)(dr["numberCar"]);
                int idCar = Convert.ToInt32(dr["idCar"]);
                int year = Convert.ToInt32(dr["year"]);
                string model = (string)dr["model"];
                string color = (string)dr["color"];
                int size = Convert.ToInt32(dr["size"]);
                string manufacturer = (string)dr["manufacturer"];
                string carPic = (string)dr["carPic"];


                Cars cars = new Cars(CurrentnumberCar, idCar, year, model, color, size, manufacturer, carPic);

                return cars;
            }


            catch (Exception ex)
            {
                throw new Exception("failed in Log In", ex);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                    con.Close();
            }
        }

        public Cars ReadMainCar(int id)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");

                SqlCommand selectCommand = creatSelectMainCarCommand(con, id);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (dr == null || !dr.Read())
                {
                    return null;
                }

                string numberCar = (string)(dr["numberCar"]);
                bool isMain = true;
                string currenthandicapped = (string)dr["handicapped"];
                bool handicapped = true;
                if (currenthandicapped == "F") { handicapped = false; }
                string carPic = (string)dr["carPic"];
                int idCar = Convert.ToInt32(dr["idCar"]);
                int year = Convert.ToInt32(dr["year"]);
                string model = (string)dr["model"];
                string color = (string)dr["color"];
                int size = Convert.ToInt32(dr["size"]);

                Cars cars = new Cars(id, numberCar, isMain, handicapped, carPic, idCar, year, model, color, size);


                if (dr.Read())
                {
                    return null;
                }

                return cars;
            }


            catch (Exception ex)
            {
                throw new Exception("failed return Main Car", ex);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }

                if (con != null)
                    con.Close();
            }
        }

        public Manufacture ReadManufacture(int idCar)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;

            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = creatSelectManufactureCommand(con, idCar);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (dr == null || !dr.Read())
                {
                    return null;
                }

                string currentManufacturer = (string)dr["manufacturer"];

                Manufacture manufacturer = new Manufacture(idCar, currentManufacturer);


                return manufacturer;
            }


            catch (Exception ex)
            {
                throw new Exception("failed return manufacturer", ex);
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
                if (con != null)
                    con.Close();
            }
        }

        private SqlCommand creatSelectMainCarCommand(SqlConnection con, int id)
        {
            string commandStr =
            "SELECT* FROM[CoParkingUsersCars_2022] LEFT JOIN[CoParkingCars_2022] ON CoParkingUsersCars_2022.numberCar = [CoParkingCars_2022].numberCar WHERE[CoParkingUsersCars_2022].id = @id AND[CoParkingUsersCars_2022].isMain = 'T'";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd;
        }

        private SqlCommand creatSelectManufactureCommand(SqlConnection con, int idCar)
        {
            string commandStr = "SELECT * FROM[CoParkingManufacture_2022] WHERE idCar = @idCar";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@idCar", idCar);
            return cmd;
        }

        private SqlCommand creatSelectUserCommand(SqlConnection con, string email)
        {
            string commandStr = "SELECT * FROM CoParkingUsers_2022 WHERE email=@email";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@email", email);

            return cmd;
        }

        private SqlCommand creatSelectUserCommandId(SqlConnection con, int id)
        {
            string commandStr = "SELECT * FROM CoParkingUsers_2022 WHERE id=@id";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd;
        }

        private SqlCommand creatSelectUserCommand2(SqlConnection con, int id, string numberCar)
        {
            string commandStr = "SELECT * FROM CoParkingUsersCars_2022 WHERE id=@id AND numberCar=@numberCar";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            return cmd;
        }

        private SqlCommand creatSelectUserCommand3(SqlConnection con, string numberCar, int id)
        {
            string commandStr = " SELECT * FROM CoParkingUsersCars_2022 WHERE numberCar='" + numberCar + "' AND id='" + id + "'";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd;
        }

        private SqlCommand creatSelectCarsCommand(SqlConnection con, string numberCar)
        {
            string commandStr = "SELECT * FROM CoParkingCars_2022 WHERE numberCar=@numberCar";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            return cmd;
        }

        private SqlCommand GetCarAndId(SqlConnection con, string numberCar, int id)
        {
            string commandStr = "SELECT * FROM CoParkingUsersCars_2022 LEFT JOIN CoParkingCars_2022 ON CoParkingUsersCars_2022.numberCar = CoParkingCars_2022.numberCar LEFT JOIN CoParkingManufacture_2022 ON CoParkingCars_2022.idCar = CoParkingManufacture_2022.idCar where id = '" + id + "' AND CoParkingCars_2022.numberCar = '" + numberCar + "';";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            cmd.Parameters.AddWithValue("@id", id);


            return cmd;
        }

        private SqlCommand GetOneCar(SqlConnection con, string numberCar)
        {
            string commandStr = "  select * from CoParkingCars_2022 LEFT JOIN CoParkingManufacture_2022 ON CoParkingCars_2022.idCar = CoParkingManufacture_2022.idCar where CoParkingCars_2022.numberCar = '" + numberCar + "'";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            return cmd;
        }


        private SqlCommand GetOneCarAndPic(SqlConnection con, string numberCar, int UserId)
        {
            string commandStr = "select * from CoParkingCars_2022 LEFT JOIN CoParkingManufacture_2022 ON CoParkingCars_2022.idCar = CoParkingManufacture_2022.idCar left join CoParkingUsersCars_2022 on CoParkingCars_2022.numberCar=CoParkingUsersCars_2022.numberCar where CoParkingCars_2022.numberCar = '" + numberCar + "' and id='" + UserId + "'";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            return cmd;
        }

        SqlCommand createCommand(SqlConnection con, string CommandSTR)
        {

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = CommandSTR;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 5;

            return cmd;
        }

        public void CreateTables()
        {
            SqlConnection con = null;
            try
            {
                con = this.Connect("webOsDB");

                SqlCommand usersCmd = this.createUserTable(con);
                usersCmd.ExecuteNonQuery();
            }

            catch(Exception)
            {
                return;
            }

            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        protected bool ValidatePassword(string password)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one lower case letter.";
                Exception ex = new Exception(ErrorMessage);
                throw ex;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one upper case letter.";
                Exception ex = new Exception(ErrorMessage);
                throw ex;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be lesser than 8 or greater than 15 characters.";
                Exception ex = new Exception(ErrorMessage);
                throw ex;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one numeric value.";
                Exception ex = new Exception(ErrorMessage);
                throw ex;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain at least one special case character.";
                Exception ex = new Exception(ErrorMessage);
                throw ex;
            }
            else
            {
                return true;
            }
        }

        public string ReadPaswword(string email)
        {
            {
                SqlConnection con = null;
                SqlDataReader dr = null;
                con = Connect("webOsDB");

                SqlCommand selectCommand = creatSelectUserCommand(con, email);
                Random rnd = new Random();
                string tmpPassword = "CO" + rnd.Next(99) + "!" + rnd.Next(99) + rnd.Next(99);
                CreateNewPassword(email, tmpPassword);
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (dr == null || !dr.Read())
                {
                    Exception ex = new Exception("the Email is not exist.");
                    throw ex;
                }

                string password = (string)dr["password"];
                if (dr.Read())
                {
                    return null;
                }
                return password;
            }
        }

        SqlCommand createPassword(SqlConnection con, string email, string Password)
        {
            string insertStr = "UPDATE [CoParkingUsers_2022] " +
      "SET [password] = '" + Password + "' WHERE [email] = '" + email + "'";
            SqlCommand command = new SqlCommand(insertStr, con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }



        public int CreateNewPassword(string email, string Password)
        {

            SqlConnection con = null;
            SqlDataReader dr = null;
            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = createPassword(con, email, Password);
                int affected = selectCommand.ExecuteNonQuery();
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (affected != 1)
                {
                    ErrorMessage = "The email not exist";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }
                return affected;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }


        public int UpdatePassword(string mail, string currentPassword, string password1, string password2)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");
                int status = CreateUpdatePassword(mail, currentPassword, password1, password2, con);
                if (status == -1)
                {
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }
                else return status;

            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public bool TakeParking(int idUser, int parkingCode)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");
                string numberCar = ReadMainCar(idUser).NumberCar;

                bool status = TryTakePariking(idUser, parkingCode, numberCar, con);
                if (status == true)
                {
                    updateTokens(idUser, -10);
                }
                return status;

            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }


        public bool ApproveParking(int idUser, int parkingCode)
        {
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");

                bool userIn;
                Parking P = GetParking(parkingCode);
                if (idUser == P.UserCodeIn)
                { userIn = true; }
                else
                {
                    if (idUser == P.UserCodeOut)
                    { userIn = false; }
                    else
                    {
                        ErrorMessage = "The userId not exist";
                        Exception ex = new Exception(ErrorMessage);
                        throw ex;
                    }
                }

                if (checkApprove(parkingCode, userIn) == true)
                {
                    ErrorMessage = "The userId alredy approved";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }
                int status = UpdateApprove(parkingCode, userIn, con);
                if (userIn == true) { userIn = false; } else { userIn = true; };
                if (checkApprove(parkingCode, userIn) == true)
                {
                    updateTokens(P.UserCodeOut, 10);
                }
                return checkApprove(parkingCode, userIn);



            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }


        public Tuple<bool, bool> ReportNotArrived(int idUser, int parkingCode)
        {
            bool tokens = false;
            SqlConnection con = null;
            try
            {
                con = Connect("webOsDB");

                bool userIn;
                Parking P = GetParking(parkingCode);
                if (idUser == P.UserCodeIn)
                { userIn = true; }
                else
                {
                    if (idUser == P.UserCodeOut)
                    { userIn = false; }
                    else
                    {
                        ErrorMessage = "The userId not exist";
                        Exception ex = new Exception(ErrorMessage);
                        throw ex;
                    }
                }

                if (checkApprove(parkingCode, true) == false)
                {
                    int status = UpdateNotArrived(parkingCode, userIn, con);
                    if (userIn == true) { userIn = false; } else { userIn = true; };
                    if (checkApprove(parkingCode, userIn) == false)
                    {
                        if (checkIfUserIsUserIn(parkingCode, idUser) == true)
                        {
                            updateTokens(P.UserCodeIn, 10);
                            tokens = true;
                        }
                    }
                    return new Tuple<bool, bool>(true, tokens);
                }
                return new Tuple<bool, bool>(false, tokens);



            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        public bool checkApprove(int parkingCode, bool userIn)
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select [userCodeOutApprove],[userCodeInApprove] from [CoParkingParkings_2022] where [parkingCode] = '" + parkingCode + "';"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            bool userCodeOutApprove = false;
            bool userCodeInApprove = false;
            if (dr.Read())
            {
                string userCodeOut = (string)dr["userCodeOutApprove"];
                string userCodeIn = (string)dr["userCodeInApprove"];

                if (userCodeOut == "Y")
                { userCodeOutApprove = true; }
                if (userCodeIn == "Y")
                { userCodeInApprove = true; }
            }
            if (userIn == true)
            {
                if (userCodeInApprove == true)
                {
                    return true;
                }
            }
            if (userIn == false)
            {
                if (userCodeOutApprove == true)
                {
                    return true;
                }
            }

            return false;

        }
        public bool checkIfUserIsUserIn(int parkingCode, int userId)
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "  select * from [CoParkingParkings_2022] where parkingCode='" + parkingCode + "'"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                if (dr["userCodeIn"] != DBNull.Value)
                {
                    int userCodeIn = (int)dr["userCodeIn"];
                    if (userId == userCodeIn)
                    {
                        return true;
                    }

                }
                int userCodeOut = (int)dr["userCodeOut"];
                if (userId == userCodeOut)
                {
                    return false;
                }
                return false;

            }
            return false;
        }



        public bool checkNotArrived(int parkingCode)
        {

            SqlConnection con = this.Connect("webOsDB");
            SqlCommand command = new SqlCommand(
                "select [userCodeOutArrived],[userCodeInArrived] from [CoParkingParkings_2022] where [parkingCode] = '" + parkingCode + "';"
                , con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;

            SqlDataReader dr = command.ExecuteReader();
            if (dr.Read())
            {
                string userCodeOut = (string)dr["userCodeOutArrived"];
                string userCodeIn = (string)dr["userCodeInArrived"];

                if (userCodeOut == "N")
                { return true; }
                if (userCodeIn == "N")
                { return true; }
            }

            return false;

        }


        public bool DeleteParking(int parkingCode)
        {

            SqlConnection con = null;
            con = Connect("webOsDB");
            if (checkIfUserInExist(parkingCode) == false)
            {
                SqlCommand selectCommand = DeleteTheParking(parkingCode, con);
                int affected = selectCommand.ExecuteNonQuery();
                if (affected >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        public Tuple<List<int>, int, DateTime> DeleteUserIn(int parkingCode)
        {
            {
                SqlConnection con = null;
                try
                {
                    con = Connect("webOsDB");
                    Parking P = GetParking(parkingCode);
                    if (checkApprove(parkingCode, true) == true || checkApprove(parkingCode, false) == true || checkNotArrived(parkingCode) == true)
                    {
                        return new Tuple<List<int>, int, DateTime>(null, -1, DateTime.MinValue);
                    }
                    SqlCommand selectCommand = DeleteUserFromParking(parkingCode, con);
                    int affected = selectCommand.ExecuteNonQuery();
                    Parking parking = new Parking(parkingCode, P.LocationLng, P.LocationLat, P.LocationName, P.ExitDate, P.TypeOfParking, P.SignType, P.UserCodeOut, P.NumberCarOut, P.UserCodeIn, P.NumberCarIn, GetParking(parkingCode).UploadDate);
                    bool forAll = updateWithAlgoritems(parking);
                    updateTokens(P.UserCodeIn, 10);


                    if (forAll is true)
                    {
                        return new Tuple<List<int>, int, DateTime>(null, parkingCode, DateTime.MinValue);
                    }

                    else
                    {
                        return GetAllUsersVip(parkingCode);
                    };

                }
                catch (Exception ex)
                {
                    ErrorMessage = "proble with insert parking";
                    throw new Exception(ErrorMessage, ex);
                }
                finally
                {
                    if (con != null)
                        con.Close();
                }
            }
        }

        public int ReturnParking(int parkingCode)
        {

            SqlConnection con = null;
            SqlDataReader dr = null;
            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = CreateUpdateReturnCar(parkingCode, con);
                int affected = selectCommand.ExecuteNonQuery();
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (affected != 1)
                {
                    ErrorMessage = "The parking not exist";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }

                return affected;
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }




        public int InsertRate(int userGattingRate, int rate)
        {

            SqlConnection con = null;
            SqlDataReader dr = null;
            try
            {
                con = Connect("webOsDB");
                SqlCommand selectCommand = CreateInsertRate(userGattingRate, rate, con);
                int affected = selectCommand.ExecuteNonQuery();
                dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

                if (affected != 1)
                {
                    ErrorMessage = "The userCode not exist";
                    Exception ex = new Exception(ErrorMessage);
                    throw ex;
                }

                return GetAvgRate(userGattingRate);
            }
            catch (Exception ex)
            {
                throw new Exception(ErrorMessage, ex);
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        SqlCommand DeleteTheParking(int parkingCode, SqlConnection con)
        {
            SqlCommand command = new SqlCommand("DELETE FROM [CoParkingUserVIP_2022] WHERE parkingCode='" + parkingCode + "';DELETE FROM [CoParkingParkings_2022] WHERE parkingCode='" + parkingCode + "';", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }



        SqlCommand DeleteUserFromParking(int parkingCode, SqlConnection con)
        {
            SqlCommand command = new SqlCommand("UPDATE [CoParkingParkings_2022] SET userCodeIn = null, numberCarIn = null,userCodeInApprove = 'N',userCodeInArrived = 'Y' WHERE parkingCode='" + parkingCode + "';", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        SqlCommand CreateUpdateReturnCar(int parkingCode, SqlConnection con)
        {
            SqlCommand command = new SqlCommand(
                  "UPDATE [CoParkingParkings_2022] " +
                  "SET [userCodeIn] = null WHERE [parkingCode] = '" + parkingCode + "'",
                    con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }

        SqlCommand CreateInsertRate(int userGattingRate, int rate, SqlConnection con)
        {
            string uploadDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SqlCommand command = new SqlCommand("INSERT INTO[CoParkingReview_2022]([userCode],[Rate],[ReviewDate]) VALUES('" + userGattingRate + "', '" + rate + "', '" + uploadDate + "')", con);
            command.CommandType = System.Data.CommandType.Text;
            command.CommandTimeout = 30;
            return command;

        }



        public int CreateUpdatePassword(string mail, string currentPassword, string password1, string password2, SqlConnection con)
        {
            int affected = -1;
            if (!(ReadOnlyPaswword(mail).Equals(currentPassword)))
            {
                ErrorMessage = "the password from mail not correct";
                return affected;
            }
            if (!password1.Equals(password2))
            {
                ErrorMessage = "the passwords are not equal";
                return affected;
            }
            if (ValidatePassword(password1) is true)
            {
                SqlCommand command = createPassword(con, mail, password1);
                command.CommandType = System.Data.CommandType.Text;
                command.CommandTimeout = 30;
                affected = command.ExecuteNonQuery();
                return affected;
            }
            return affected;
        }

        public bool TryTakePariking(int idUser, int parkingCode, string numberCar, SqlConnection con)
        {
            SqlDataReader dr = null;
            SqlCommand selectCommand = checkParking(parkingCode, con);
            dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
            selectCommand = null;
            con = Connect("webOsDB");
            if (dr.Read())
            {
                return false;
            }

            string str = "UPDATE[CoParkingParkings_2022] SET[userCodeIn] = '" + idUser + "' WHERE[parkingCode] = '" + parkingCode + "';";
            str += "UPDATE[CoParkingParkings_2022] SET[numberCarIn] = '" + numberCar + "' WHERE[parkingCode] = '" + parkingCode + "';";
            selectCommand = new SqlCommand(str, con);
            int affected = selectCommand.ExecuteNonQuery();
            selectCommand.CommandType = System.Data.CommandType.Text;
            selectCommand.CommandTimeout = 30;
            return true;
        }

        public int UpdateApprove(int parkingCode, bool userIn, SqlConnection con)
        {
            SqlDataReader dr = null;
            SqlCommand selectCommand = checkParking(parkingCode, con);
            dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
            selectCommand = null;
            con = Connect("webOsDB");
            string str = "";
            if (userIn == true)
            {
                str = "UPDATE[CoParkingParkings_2022] SET [userCodeInApprove] = 'Y' WHERE[parkingCode] = '" + parkingCode + "'; ";
            }
            else
            {
                str = "UPDATE[CoParkingParkings_2022] SET [userCodeOutApprove] = 'Y' WHERE[parkingCode] = '" + parkingCode + "'; ";
            }
            selectCommand = new SqlCommand(str, con);
            int affected = selectCommand.ExecuteNonQuery();
            selectCommand.CommandType = System.Data.CommandType.Text;
            selectCommand.CommandTimeout = 30;
            return affected;
        }


        public int UpdateNotArrived(int parkingCode, bool userIn, SqlConnection con)
        {
            SqlDataReader dr = null;
            SqlCommand selectCommand = checkParking(parkingCode, con);
            dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
            selectCommand = null;
            con = Connect("webOsDB");
            string str = "";
            if (userIn == true)
            {
                str = "UPDATE[CoParkingParkings_2022] SET [userCodeOutArrived] = 'N' WHERE[parkingCode] = '" + parkingCode + "'; ";
            }
            else
            {
                str = "UPDATE[CoParkingParkings_2022] SET [userCodeInArrived] = 'N' WHERE[parkingCode] = '" + parkingCode + "'; ";
            }
            selectCommand = new SqlCommand(str, con);
            int affected = selectCommand.ExecuteNonQuery();
            selectCommand.CommandType = System.Data.CommandType.Text;
            selectCommand.CommandTimeout = 30;
            return affected;
        }


        private SqlCommand checkParking(int ParkingCode, SqlConnection con)
        {
            string commandStr = "SELECT * FROM CoParkingParkings_2022 WHERE parkingCode=@ParkingCode AND userCodeIn IS NOT NULL";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@ParkingCode", ParkingCode);
            return cmd;
        }
        private SqlCommand checkCanEditCar(int id, string numberCar, SqlConnection con)
        {
            string commandStr = "select * from[CoParkingUsersCars_2022] where id = '" + id + "' AND numberCar = '" + numberCar + "'";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@numberCar", numberCar);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd;
        }
        private SqlCommand MakeStatusOff(int id, SqlConnection con)
        {
            string commandStr = "  UPDATE [CoParkingUsers_2022] SET status = 'off' where id = '" + id + "'";
            SqlCommand cmd = createCommand(con, commandStr);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd;
        }

        public string ReadOnlyPaswword(string email)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;
            con = Connect("webOsDB");
            SqlCommand selectCommand = creatSelectUserCommand(con, email);
            dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);
            if (dr == null || !dr.Read())
            {
                ErrorMessage = "the Email is not exist.";
                Exception ex = new Exception(ErrorMessage);
                throw ex;
            }
            string password = (string)dr["password"];

            if (dr.Read())
            {
                return null;
            }

            return password;
        }

        public int CheckUserAndNumberCar(int id, string numberCar)
        {
            SqlConnection con = null;
            SqlDataReader dr = null;
            con = Connect("webOsDB");
            SqlCommand selectCommand = creatSelectUserCommand2(con, id, numberCar);
            dr = selectCommand.ExecuteReader(CommandBehavior.CloseConnection);

            if (dr == null || !dr.Read())
            {
                return -1;
            }

            string number = (string)(dr["numberCar"]);
            if (number == null) { return 1; }
            return -1;

        }

        SqlCommand createUserTable(SqlConnection con)
        {
            string commandStr = "IF OBJECT_ID (N'[CoParkingCars_2022]', N'U') IS NULL BEGIN " +
                "CREATE TABLE [CoParkingCars_2022] (" +
                "[numberCar] int NOT NULL," +
                "[idCar] int  NOT NULL," +
                "[year] int NOT NULL," +
                "[model] nvarchar (100) NOT NULL," +
                "[color] nvarchar (100) NOT NULL," +
                "[size] int NOT NULL," +
                "Primary key (numberCar));" +
                " END;";
            SqlCommand cmd = createCommand(con, commandStr);
            return cmd;
        }
        SqlCommand createCarsTable(SqlConnection con)
        {
            string commandStr = "IF OBJECT_ID (N'[CoParkingUsers_2022]', N'U') IS NULL BEGIN " +
                "CREATE TABLE [CoParkingUsers_2022] (" +
                "[id] int IDENTITY(1, 1) NOT NULL," +
                "[email] nvarchar(100) NOT NULL," +
                "[password] nvarchar(100) NOT NULL," +
                "[fName] nvarchar(100) NOT NULL," +
                "[lName] nvarchar(100) NOT NULL," +
                "[phoneNumber] nvarchar(100) NOT NULL," +
                "[gender] nvarchar(100) NOT NULL," +
                "[image] nvarchar(100)," +
                "[searchRadius] int DEFAULT (2000) NOT NULL," +
                "[timeDelta] int DEFAULT (10) NOT NULL," +
                "[status] nvarchar(100) DEFAULT 'Off' NOT NULL," +
                "[tokens] int DEFAULT(30) NOT NULL," +
                "Primary key(id));" +
                " END;";
            SqlCommand cmd = createCommand(con, commandStr);
            return cmd;
        }

        static double Phi(double x)
        {
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x) / Math.Sqrt(2.0);

            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return 0.5 * (1.0 + sign * y);


        }


    }


}

