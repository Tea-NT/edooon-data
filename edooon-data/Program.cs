using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using Geo.Gps;
using Geo.Gps.Serialization;
using Geo.Gps.Serialization.Xml.Gpx.Gpx11;

namespace edooon_data
{
    internal class Program
    {
        public static string conn = @"Data Source=C:\Users\john\Desktop\MouseWithoutBorders\Cearch.db;";
        static void Main(string[] args)
        {
            GpsPoint point = new GpsPoint(31.1774276, 121.5272106, GpsEnum.WGS84);//测试坐标，用于输出测试结果。
            var newPoint = point.GetGCJ02();
            Console.WriteLine(newPoint.Latitude+"  "+newPoint.Longitude);

            if(args.Length > 0 )
            {
                var sourceGpx = args[0];
                Console.WriteLine("正在读取gpx文件:"+sourceGpx);
                if(sourceGpx != null ) {
                    if (!sourceGpx.Contains(".gpx"))
                    {
                        Console.WriteLine("请输入gpx文件名称！");
                        Console.ReadKey();
                        return;
                    }
                    if(File.Exists(sourceGpx))
                    {
                        GPXConverWCGtoGCJ(sourceGpx);
                        Console.WriteLine("转换成功！输出文件为:GCJ_02_"+sourceGpx);
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"文件{sourceGpx}不存在！");
                        Console.ReadKey();

                    }
                }
                Console.WriteLine("输入参数为空！");
                Console.ReadKey();
                return;
            }

            Dictionary<RecordDetails, List<RecordPoints>> records = new Dictionary<RecordDetails, List<RecordPoints>>();
            var items = GetAllRecordDetails();
            Console.WriteLine($"总找到{items.Count}条数据。");
            Console.WriteLine("前十条数据为:");
            //foreach(var item in items.Take(10) )
            foreach(var item in items.Take(1000))
                {
                //Console.WriteLine($"{item._Id},开始时间:{DateTimeOffset.FromUnixTimeSeconds(item.Start_Time).ToLocalTime().ToString("g")},距离 {item.Distance} 米。用时 {new TimeSpan(0,0,item.Duration).ToString(@"hh\:mm\:ss")} ");
                var points = GetRecordPointsByDetailId(item._Id);
                if(points.Count > 0 )
                {
                    //Console.WriteLine($"获取到轨迹点 {points.Count} 个。");
                    records.Add(item, points);
                }
            }
            Console.WriteLine($"包含轨迹点的记录条数 {records.Keys.Count}.");
            foreach (var key in records.Keys)
            {
                var points = records[key];
                GpsData gpsData = new GpsData();
                var recordTime = DateTimeOffset.FromUnixTimeSeconds(key.Start_Time).ToLocalTime().DateTime;
                gpsData.Tracks.Add(new Track());
                gpsData.Tracks[0].Segments.Add(new  TrackSegment());
                foreach (var record in points)
                {
                    //Gpx11Serializer gpx11Serializer = new Gpx11Serializer();
                    //gpsData.Waypoints.Add(new Waypoint(record.Latitude, record.Longitude, record.Altitude, recordTime.AddSeconds(record.Time)));
                    //gpsData.Tracks[0].Segments[0].Waypoints.Add(new Waypoint(record.Latitude, record.Longitude, record.Altitude, recordTime.AddSeconds(record.Time)));

                    //用GCJ-02格式输出
                    point = new GpsPoint(record.Latitude, record.Longitude, GpsEnum.WGS84);
                    newPoint = point.GetGCJ02();
                    gpsData.Tracks[0].Segments[0].Waypoints.Add(new Waypoint(newPoint.Latitude, newPoint.Longitude, record.Altitude, recordTime.AddSeconds(record.Time)));
                    //gpx11Serializer.Serialize(gpsData);
                }
                string content = gpsData.ToGpx(2);
                using (FileStream fs = new FileStream(recordTime.ToString("GCJ_yyyy-MM-dd_HH-mm-ss") + ".gpx", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(content);
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
            Console.ReadLine();
        }

        public static List<RecordDetails> GetAllRecordDetails()
        {

            using (IDbConnection cnn = new SQLiteConnection(conn))
            {
                cnn.Open();

                //var output = cnn.Query<RecordDetails>("select * from Record_Details ORDER BY distance desc LIMIT 10").ToList();
                var output = cnn.Query<RecordDetails>("select * from Record_Details ORDER BY distance desc").ToList();

                return output.ToList();
            }
        }

        public static List<RecordPoints> GetRecordPointsByDetailId(int detailId)
        {

            using (IDbConnection cnn = new SQLiteConnection(conn))
            {
                cnn.Open();

                //var output = cnn.Query<RecordDetails>("select * from Record_Details ORDER BY distance desc LIMIT 10").ToList();
                var output = cnn.Query<RecordPoints>("select * from record_points where recorddetail_id = "+detailId+" ORDER BY time").ToList();

                return output.ToList();
            }
        }


        public static void GPXConverWCGtoGCJ(string pathGpx)
        {
            //var gpxFileStr = File.ReadAllText(pathGpx);
            GpsData gpxData = new GpsData();
            using(FileStream fs = new FileStream(pathGpx, FileMode.Open, FileAccess.Read))
            {

                Gpx11Serializer gpx11Serializer = new Gpx11Serializer();
                gpxData = gpx11Serializer.DeSerialize(new StreamWrapper(fs));
            }

            foreach (var wcgPoint in gpxData.Tracks[0].Segments[0].Waypoints)
            {
                GpsPoint point = new GpsPoint(wcgPoint.Coordinate.Latitude, wcgPoint.Coordinate.Longitude, GpsEnum.WGS84);
                point = point.GetGCJ02();
                wcgPoint.Point = new Geo.Geometries.Point(point.Latitude, point.Longitude);
            }
            string content = gpxData.ToGpx(2);
            using (FileStream fs = new FileStream("GCJ-02_"+pathGpx, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                fs.Write(buffer, 0, buffer.Length);
            }

        }
    }
}
