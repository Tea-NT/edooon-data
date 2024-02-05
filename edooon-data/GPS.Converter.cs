using System;

namespace edooon_data
{
    //
    // 摘要:
    //     GPS坐标系统类型
    public enum GpsEnum
    {
        //
        // 摘要:
        //     World Geodetic System 1984 国际上通用的GPS坐标系（地心坐标系） 例如： 普通GPS定位设备采集的原始坐标系
        WGS84,
        //
        // 摘要:
        //     GCJ-02是由中国国家测绘局制订的地理信息系统的坐标系统 例如：腾讯地图、高德地图等
        GCJ02,
        //
        // 摘要:
        //     百度地图专用坐标系 例如： 百度地图
        BD09,
        //
        // 摘要:
        //     墨卡托坐标系 例如：ArcGis
        Mercator
    }
    //
    // 摘要:
    //     通用GPS坐标点
    public class GpsPoint
    {
        private static readonly double a = 6378245.0;

        private static readonly double ee = 0.0066934216229659433;

        private static readonly double bd_pi = 52.359877559829883;

        //
        // 摘要:
        //     纬度
        public double Latitude { get; set; }

        //
        // 摘要:
        //     经度
        public double Longitude { get; set; }

        //
        // 摘要:
        //     GPS坐标系统
        public GpsEnum Type { get; set; }

        //
        // 摘要:
        //     通用GPS坐标点构造函数
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        //   type:
        //     GPS坐标系统
        public GpsPoint(double latitude, double longitude, GpsEnum type)
        {
            Latitude = latitude;
            Longitude = longitude;
            Type = type;
        }

        //
        // 摘要:
        //     获取WGS84的坐标点
        //
        // 返回结果:
        //     转换后的位置信息
        public GpsPoint GetWGS84()
        {
            switch( Type)
            {
                case GpsEnum.GCJ02:
                    return GCJ02_To_WGS84(Latitude, Longitude);
                    break;
                case GpsEnum.BD09:
                    return BD09_To_WGS84(Latitude, Longitude);
                    break;
                case GpsEnum.Mercator:
                    return Mercator_To_WGS84(Latitude, Longitude);
                    default: return null;
            };
        }

        //
        // 摘要:
        //     获取GCJ02的坐标点
        //
        // 返回结果:
        //     转换后的位置信息
        public GpsPoint GetGCJ02()
        {
            switch (Type)
            {
                case GpsEnum.WGS84:
                    return WGS84_To_GCJ02(Latitude, Longitude);
                    break;
                case GpsEnum.BD09:
                    return BD09_To_GCJ02(Latitude, Longitude);
                    break;
                case GpsEnum.Mercator:
                    return Mercator_To_WGS84(Latitude, Longitude).GetGCJ02();
                    default: return null;
            };
        }

        //
        // 摘要:
        //     获取BD09的坐标点
        //
        // 返回结果:
        //     转换后的位置信息
        public GpsPoint GetBD09()
        {
            switch (Type)
            {
                case GpsEnum.WGS84:
                    return WGS84_To_BD09(Latitude, Longitude);
                    break;
                    case GpsEnum.GCJ02:
                    return GCJ02_To_BD09(Latitude, Longitude);
                    break;
                    case GpsEnum.Mercator:
                    return Mercator_To_WGS84(Latitude, Longitude).GetBD09();
                    default: return null;
            };
        }

        //
        // 摘要:
        //     获取墨卡托坐标
        //
        // 返回结果:
        //     转换后的位置信息
        public GpsPoint GetMercator()
        {
            switch(Type )
            {
                case GpsEnum.WGS84:
                    return WGS84_To_Mercator(Latitude, Longitude);
                    break;
                    case GpsEnum.GCJ02:
                    return GCJ02_To_WGS84(Latitude, Longitude).GetMercator();
                        case GpsEnum.BD09:
                    return BD09_To_WGS84(Latitude, Longitude).GetMercator();
                    default: return null;
            };
        }

        //
        // 摘要:
        //     是否超出中国境内
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     bool
        private static bool OutOfChina(double latitude, double longitude)
        {
            if (longitude < 72.004 || longitude > 137.8347)
            {
                return true;
            }

            if (latitude < 0.8293 || latitude > 55.8271)
            {
                return true;
            }

            return false;
        }

        private static double TransformLat(double latitude, double longitude)
        {
            double num = -100.0 + 2.0 * latitude + 3.0 * longitude + 0.2 * longitude * longitude + 0.1 * longitude * latitude + 0.2 * Math.Sqrt(Math.Abs(latitude));
            num += (20.0 * Math.Sin(6.0 * latitude * Math.PI) + 20.0 * Math.Sin(2.0 * latitude * Math.PI)) * 2.0 / 3.0;
            num += (20.0 * Math.Sin(longitude * Math.PI) + 40.0 * Math.Sin(longitude / 3.0 * Math.PI)) * 2.0 / 3.0;
            return num + (160.0 * Math.Sin(longitude / 12.0 * Math.PI) + 320.0 * Math.Sin(longitude * Math.PI / 30.0)) * 2.0 / 3.0;
        }

        private static double TransformLon(double latitude, double longitude)
        {
            double num = 300.0 + latitude + 2.0 * longitude + 0.1 * latitude * latitude + 0.1 * longitude * latitude + 0.1 * Math.Sqrt(Math.Abs(latitude));
            num += (20.0 * Math.Sin(6.0 * latitude * Math.PI) + 20.0 * Math.Sin(2.0 * latitude * Math.PI)) * 2.0 / 3.0;
            num += (20.0 * Math.Sin(latitude * Math.PI) + 40.0 * Math.Sin(latitude / 3.0 * Math.PI)) * 2.0 / 3.0;
            return num + (150.0 * Math.Sin(latitude / 12.0 * Math.PI) + 300.0 * Math.Sin(latitude / 30.0 * Math.PI)) * 2.0 / 3.0;
        }

        //
        // 摘要:
        //     国际通用坐标系 (WGS84) to 火星坐标系 (GCJ-02) World Geodetic System ==> Mars Geodetic System
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint WGS84_To_GCJ02(double latitude, double longitude)
        {
            double num = TransformLat(longitude - 105.0, latitude - 35.0);
            double num2 = TransformLon(longitude - 105.0, latitude - 35.0);
            double d = latitude / 180.0 * Math.PI;
            double num3 = Math.Sin(d);
            num3 = 1.0 - ee * num3 * num3;
            double num4 = Math.Sqrt(num3);
            num = num * 180.0 / (a * (1.0 - ee) / (num3 * num4) * Math.PI);
            num2 = num2 * 180.0 / (a / num4 * Math.Cos(d) * Math.PI);
            double latitude2 = latitude + num;
            double longitude2 = longitude + num2;
            return new GpsPoint(latitude2, longitude2, GpsEnum.GCJ02);
        }

        //
        // 摘要:
        //     火星坐标系 (GCJ-02) to 国际通用坐标系 (WGS84) Mars Geodetic System ==> World Geodetic System
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint GCJ02_To_WGS84(double latitude, double longitude)
        {
            double num = TransformLat(longitude - 105.0, latitude - 35.0);
            double num2 = TransformLon(longitude - 105.0, latitude - 35.0);
            double d = latitude / 180.0 * Math.PI;
            double num3 = Math.Sin(d);
            num3 = 1.0 - ee * num3 * num3;
            double num4 = Math.Sqrt(num3);
            num = num * 180.0 / (a * (1.0 - ee) / (num3 * num4) * Math.PI);
            num2 = num2 * 180.0 / (a / num4 * Math.Cos(d) * Math.PI);
            double num5 = latitude + num;
            double num6 = longitude + num2;
            return new GpsPoint(latitude * 2.0 - num5, longitude * 2.0 - num6, GpsEnum.WGS84);
        }

        //
        // 摘要:
        //     火星坐标系 (GCJ-02) to 百度坐标系 (BD-09)
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint GCJ02_To_BD09(double latitude, double longitude)
        {
            double num = Math.Sqrt(longitude * longitude + latitude * latitude) + 2E-05 * Math.Sin(latitude * bd_pi);
            double d = Math.Atan2(latitude, longitude) + 3E-06 * Math.Cos(longitude * bd_pi);
            double longitude2 = num * Math.Cos(d) + 0.0065;
            double latitude2 = num * Math.Sin(d) + 0.006;
            return new GpsPoint(latitude2, longitude2, GpsEnum.BD09);
        }

        //
        // 摘要:
        //     火星坐标系 (GCJ-02) to 百度坐标系 (BD-09)
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint BD09_To_GCJ02(double latitude, double longitude)
        {
            double num = longitude - 0.0065;
            double num2 = latitude - 0.006;
            double num3 = Math.Sqrt(num * num + num2 * num2) - 2E-05 * Math.Sin(num2 * bd_pi);
            double d = Math.Atan2(num2, num) - 3E-06 * Math.Cos(num * bd_pi);
            double longitude2 = num3 * Math.Cos(d);
            double latitude2 = num3 * Math.Sin(d);
            return new GpsPoint(latitude2, longitude2, GpsEnum.GCJ02);
        }

        //
        // 摘要:
        //     百度坐标系 (BD-09) to 国际通用坐标系 (WGS84)
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint BD09_To_WGS84(double latitude, double longitude)
        {
            GpsPoint gpsPoint = BD09_To_GCJ02(latitude, longitude);
            return GCJ02_To_WGS84(gpsPoint.Latitude, gpsPoint.Longitude);
        }

        //
        // 摘要:
        //     国际通用坐标系 (WGS84) to 百度坐标系 (BD-09)
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint WGS84_To_BD09(double latitude, double longitude)
        {
            GpsPoint gpsPoint = WGS84_To_GCJ02(latitude, longitude);
            return GCJ02_To_BD09(gpsPoint.Latitude, gpsPoint.Longitude);
        }

        //
        // 摘要:
        //     MercatorToWGS84 墨卡托转WGS84
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint Mercator_To_WGS84(double latitude, double longitude)
        {
            latitude /= 3606751501.2;
            longitude /= 3606751501.2;
            latitude = 180.0 / Math.PI * (2.0 * Math.Atan(Math.Exp(latitude * Math.PI / 180.0)) - Math.PI / 2.0);
            return new GpsPoint(latitude, longitude, GpsEnum.WGS84);
        }

        //
        // 摘要:
        //     WGS84ToMercator WGS84转墨卡托
        //
        // 参数:
        //   latitude:
        //     纬度
        //
        //   longitude:
        //     经度
        //
        // 返回结果:
        //     转换后的位置信息
        public static GpsPoint WGS84_To_Mercator(double latitude, double longitude)
        {
            longitude *= 111319.49077777777;
            latitude = Math.Log(Math.Tan((90.0 + latitude) * Math.PI / 360.0)) / (Math.PI / 180.0);
            latitude *= 111319.49077777777;
            return new GpsPoint(latitude, longitude, GpsEnum.Mercator);
        }
    }
}
