﻿using ECOLOGCarSimulator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOLOGCarSimulator.Daos
{
    class TripsRawDopplerDao
    {
        private static readonly string TableName = "TRIPS_RAW_Doppler";
        public static readonly string ColumnDriverId = "driver_id";
        public static readonly string ColumnCarId = "car_id";
        public static readonly string ColumnSensorId = "sensor_id";
        public static readonly string ColumnStartTime = "start_time";
        public static readonly string ColumnEndTime = "end_time";
        public static readonly string ColumnStartLatitude = "start_latitude";
        public static readonly string ColumnStartLongitude = "start_longitude";
        public static readonly string ColumnEndLatitude = "end_latitude";
        public static readonly string ColumnEndLongitude = "end_longitude";

        public static void Insert(DataTable dataTable)
        {
            DatabaseAccesser.Insert(TableName, dataTable);
        }

        public static DataTable Get()
        {
            string query = "SELECT * FROM " + TableName;

            return DatabaseAccesser.GetResult(query);
        }

        public static DataTable Get(InsertDatum datum)
        {
            var query = new StringBuilder();

            query.AppendLine("SELECT *");
            query.AppendLine($"FROM {TripsRawDopplerDao.TableName}");
            query.AppendLine($"WHERE {TripsRawDopplerDao.ColumnDriverId} = {datum.DriverId}");
            query.AppendLine($"AND {TripsRawDopplerDao.ColumnCarId} = {datum.CarId}");
            query.AppendLine($"AND {TripsRawDopplerDao.ColumnSensorId} = {datum.SensorId}");
            query.AppendLine($"AND {TripsRawDopplerDao.ColumnStartTime} >= '{datum.StartTime}'");
            query.AppendLine($"AND {TripsRawDopplerDao.ColumnEndTime} <= '{datum.EndTime}'");
            query.AppendLine($"ORDER BY {ColumnStartTime}");

            return DatabaseAccesser.GetResult(query.ToString());
        }
    }
}
