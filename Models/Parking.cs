using ParkingProject.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ParkingProject.Models
{
    public class Parking
    {
        private int parkingCode;
        private double locationLng;
        private double locationLat;
        private string locationName;
        private DateTime exitDate;
        private DateTime uploadDate;
        private int typeOfParking;
        private string signType;
        private int userCodeOut;
        private string numberCarOut;
        private int userCodeIn;
        private string numberCarIn;

        private static DataServices ds = new DataServices();

        public Parking() { }

        public Parking(double locationLng, double locationLat, string locationName, DateTime exitDate, int typeOfParking, string signType, int userCodeOut, string numberCarOut, int userCodeIn, string numberCarIn)
        {
            this.locationLat = locationLat;
            this.locationLng = locationLng;
            this.locationName = locationName;
            this.exitDate = exitDate;
            this.typeOfParking = typeOfParking;
            this.signType = signType;
            this.userCodeOut = userCodeOut;
            this.userCodeIn = userCodeIn;
            this.numberCarIn = numberCarIn;
            this.numberCarOut = numberCarOut;
        }

        public Parking(double locationLng, double locationLat, string locationName, DateTime exitDate, int typeOfParking, string signType, int userCodeOut, string numberCarOut)
        {
            this.locationLat = locationLat;
            this.locationLng = locationLng;
            this.locationName = locationName;
            this.exitDate = exitDate;
            this.typeOfParking = typeOfParking;
            this.signType = signType;
            this.userCodeOut = userCodeOut;
            this.numberCarOut = numberCarOut;
        }
        public Parking(int parkingCode, double locationLng, double locationLat, string locationName, DateTime exitDate, int typeOfParking, string signType, int userCodeOut, string numberCarOut, int userCodeIn, string numberCarIn, DateTime uploadDate)
        {
            this.parkingCode = parkingCode;
            this.locationLat = locationLat;
            this.locationLng = locationLng;
            this.locationName = locationName;
            this.exitDate = exitDate;
            this.typeOfParking = typeOfParking;
            this.signType = signType;
            this.userCodeOut = userCodeOut;
            this.numberCarOut = numberCarOut;
            this.userCodeIn = userCodeIn;
            this.numberCarIn = numberCarIn;
            this.uploadDate = uploadDate;


        }

        public Parking(int parkingCode, double locationLng, double locationLat, string locationName, DateTime exitDate, int typeOfParking, string signType, int userCodeOut, string numberCarOut, int userCodeIn, string numberCarIn)
        {
            this.parkingCode = parkingCode;
            this.locationLat = locationLat;
            this.locationLng = locationLng;
            this.locationName = locationName;
            this.exitDate = exitDate;
            this.typeOfParking = typeOfParking;
            this.signType = signType;
            this.userCodeOut = userCodeOut;
            this.numberCarOut = numberCarOut;
            this.userCodeIn = userCodeIn;
            this.numberCarIn = numberCarIn;


        }

        public Parking(int parkingCode, double locationLng, double locationLat, string locationName, DateTime exitDate, int typeOfParking, string signType, int userCodeOut, string numberCarOut)
        {
            this.parkingCode = parkingCode;
            this.locationLat = locationLat;
            this.locationLng = locationLng;
            this.locationName = locationName;
            this.exitDate = exitDate;
            this.typeOfParking = typeOfParking;
            this.signType = signType;
            this.userCodeOut = userCodeOut;
            this.numberCarOut = numberCarOut;

        }

        public Parking(int parkingCode, double locationLng, double locationLat, string locationName, DateTime exitDate, int typeOfParking, string signType, int userCodeOut, string numberCarOut, DateTime uploadDate)
        {
            this.parkingCode = parkingCode;
            this.locationLat = locationLat;
            this.locationLng = locationLng;
            this.locationName = locationName;
            this.exitDate = exitDate;
            this.typeOfParking = typeOfParking;
            this.signType = signType;
            this.userCodeOut = userCodeOut;
            this.numberCarOut = numberCarOut;
            this.uploadDate = uploadDate;

        }

        public int ParkingCode { get => parkingCode; set => parkingCode = value; }
        public double LocationLat { get => locationLat; set => locationLat = value; }
        public double LocationLng { get => locationLng; set => locationLng = value; }
        public string LocationName { get => locationName; set => locationName = value; }
        public DateTime ExitDate { get => exitDate; set => exitDate = value; }
        public int TypeOfParking { get => typeOfParking; set => typeOfParking = value; }
        public string SignType { get => signType; set => signType = value; }
        public int UserCodeOut { get => userCodeOut; set => userCodeOut = value; }
        public int UserCodeIn { get => userCodeIn; set => userCodeIn = value; }
        public string NumberCarOut { get => numberCarOut; set => numberCarOut = value; }
        public string NumberCarIn { get => numberCarIn; set => numberCarIn = value; }
        public DateTime UploadDate { get => uploadDate; set => uploadDate = value; }

        public int InsertUserIn(int idUser, int parkingCode)
        {
            if (ds.TakeParking(idUser, parkingCode) == true) { return 1; }
            return 0;
        }
        public Tuple<List<int>, int, DateTime> Insert()
        {
            Parking parking = (this);
            return ds.InsertParking(parking);
        }
        public int UpdateParking()
        {
            Parking parking = (this);
            int status = ds.UpdateParking(parking);
            return status;
        }

        public static Tuple<Parking, bool, DateTime, Cars, User>[] GetAll(int id)
        {
            return ds.GetAllParkings(id);
        }


        public static Tuple<Parking, Cars, Cars, User>[] GetAllParkingsUser(int id)
        {

            Tuple<Parking, Cars, Cars, User>[] P = ds.GetAllParkingsUser(id);

            Array.Sort(P, (x, y) => x.Item1.exitDate.CompareTo(y.Item1.exitDate));
            return P;

        }

        public static Tuple<Parking, Cars, Cars, User> GetParkingUser(int idUser, int idParking)
        {

            Tuple<Parking, Cars, Cars, User> P = ds.GetParkingUser(idUser, idParking);
            return P;

        }

        public static Parking[] GetAllOnlyParkingsUser(int id)
        {

            Parking[] P = ds.GetAllOnlyParkingsUser(id);

            Array.Sort(P, (x, y) => x.exitDate.CompareTo(y.exitDate));
            return P;

        }

        public static Tuple<Parking, Cars, Cars, User>[] GetAllParkingsUserFuture(int id)
        {

            Tuple<Parking, Cars, Cars, User>[] P = ds.GetAllParkingsUserFuture(id);

            Array.Sort(P, (x, y) => x.Item1.exitDate.CompareTo(y.Item1.exitDate));
            return P;

        }

        public static Parking GetParking(int parkingCode)
        {
            return ds.GetParking(parkingCode);
        }




        public static bool TakeParking(int idUser, int parkingCode)
        {
            bool status = ds.TakeParking(idUser, parkingCode);
            return status;
        }

        public static bool ApproveParking(int idUser, int parkingCode)
        {
            return ds.ApproveParking(idUser, parkingCode);
        }

        public static Tuple<bool, bool> ReportNotArrived(int idUser, int parkingCode)
        {
            return ds.ReportNotArrived(idUser, parkingCode);
        }



        public static int ReturnParking(int parkingCode)
        {
            int status = ds.ReturnParking(parkingCode);
            return status;
        }

        public static int UpdateParking(Parking parkingCode)
        {
            int status = ds.UpdateParking(parkingCode);
            return status;
        }

        public static bool DeleteParking(int parkingCode)
        {
            bool status = ds.DeleteParking(parkingCode);
            return status;
        }


        public static Tuple<List<int>, int, DateTime> DeleteUserIn(int parkingCode)
        {
            return ds.DeleteUserIn(parkingCode);
        }

    }
}
