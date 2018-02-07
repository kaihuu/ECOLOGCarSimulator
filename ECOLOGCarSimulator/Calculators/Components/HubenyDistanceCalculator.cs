using ECOLOGCarSimulator.Utils;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOLOGCarSimulator.Calculators.CalculatorComponents
{
    static class HubenyDistanceCalculator
    {
        private static double LongRadius = 6378137; // 長半径
        private static double ShortRadius = 6356752.314245; // 短半径

        //ヒュベニの公式
        public static double CalcHubenyFormula(double latitudeFirst, double longitudeFirst, double latitudeSecond, double longitudeSecond)
        {
            double differenceLattitude = MathUtil.ConvertDegreeToRadian(latitudeFirst - latitudeSecond); // 緯度の差
            double differenceLongitude = MathUtil.ConvertDegreeToRadian(longitudeFirst - longitudeSecond); // 経度の差

            double M = CalcMeridianCurvature(latitudeFirst, latitudeSecond); // 子午線曲率半径
            double N = CalcPrimeVerticalCircleCurvature(latitudeFirst, latitudeSecond); // 卯酉線曲率半径

            double yFirst = MathUtil.ConvertDegreeToRadian(latitudeFirst); // 緯度をラジアンに変換
            double ySecond = MathUtil.ConvertDegreeToRadian(latitudeSecond); // 緯度をラジアンに変換

            double cosMyuY = Math.Cos((yFirst + ySecond) / 2); // 緯度の平均値のコサイン

            double distance = Math.Sqrt(Math.Pow(differenceLattitude * M, 2) + Math.Pow(differenceLongitude * N * cosMyuY, 2));//距離の計算

            return distance;
        }

        //出発地点の座標と出発地点からの距離と方位から出発地点の緯度と経度を計算
        public static GeoCoordinate CalcCoordinateFromHubenyFormula(GeoCoordinate startCoordinate, double distance, GeoCoordinate endCoordinate)
        {
            double orientationAngle = CalcOrientationAngle(startCoordinate, endCoordinate);
            GeoCoordinate resultCoordinate = new GeoCoordinate();
            double M = CalcMeridianCurvature(startCoordinate.Latitude, startCoordinate.Latitude);//仮の子午線曲率半径を計算
            double orientationRadian = MathUtil.ConvertDegreeToRadian(orientationAngle); //方角を角度からラジアンに変換
            double di = distance * Math.Cos(orientationRadian) / M;//仮の緯度の変化量の計算
            double endLattitude = startCoordinate.Latitude + MathUtil.ConvertRadianToDegree(di);//仮の目標地点の緯度を計算

            M = CalcMeridianCurvature(endLattitude, endLattitude);//子午線曲率半径を再計算
            double N = CalcPrimeVerticalCircleCurvature(endLattitude, endLattitude); //卯酉線曲率半径
            di = distance * Math.Cos(orientationRadian) / M; //緯度の変化量を再計算
            double dk = distance * Math.Sin(orientationRadian) / (N * Math.Cos(MathUtil.ConvertDegreeToRadian(endLattitude))); //経度の変化量を計算

            resultCoordinate.Latitude = startCoordinate.Latitude + MathUtil.ConvertRadianToDegree(di); //目標地点の緯度を計算
            resultCoordinate.Longitude = startCoordinate.Longitude + MathUtil.ConvertRadianToDegree(dk); //目標地点の経度を計算

            return resultCoordinate;
        }

        //子午線曲率半径
        private static double CalcMeridianCurvature(double lattitudeFirst, double lattitudeSecond)
        {

            double W = CalcW(lattitudeFirst, lattitudeSecond); // Wを計算
            double e2 = CalcE2(); // 第一離心率の２乗を計算
            double M = LongRadius * (1 - e2) / Math.Pow(W, 3); // 子午線曲率半径を計算

            return M;
        }

        //卯酉線曲率半径
        private static double CalcPrimeVerticalCircleCurvature(double lattitudeFirst, double lattitudeSecond)
        {
            double W = CalcW(lattitudeFirst, lattitudeSecond); // Wを計算
            double N = LongRadius / W; // 卯酉線曲率半径を計算

            return N;
        }

        //ヒュベニの公式のWを導出
        private static double CalcW(double lattitudeFirst, double lattitudeSecond)
        {

            double yFirst = MathUtil.ConvertDegreeToRadian(lattitudeFirst); // 緯度をラジアンに変換
            double ySecond = MathUtil.ConvertDegreeToRadian(lattitudeSecond); // 緯度をラジアンに変換

            double sinMyuY2 = Math.Pow(Math.Sin((yFirst + ySecond) / 2), 2); // 緯度の平均値のサインの２乗
            double e2 = CalcE2(); //第一離心率の２乗

            double W = Math.Sqrt(1 - e2 * sinMyuY2);//Wを計算

            return W;
        }

        //第一離心率の２乗
        private static double CalcE2()
        {
            return (Math.Pow(LongRadius, 2) - Math.Pow(ShortRadius, 2)) / Math.Pow(LongRadius, 2);
        }

        public static double CalcOrientationAngle(GeoCoordinate startCoordinate, GeoCoordinate endCoordinate)
        {
            double startLattitude = MathUtil.ConvertDegreeToRadian(startCoordinate.Latitude);
            double startLongitude = MathUtil.ConvertDegreeToRadian(startCoordinate.Longitude);
            double endLattitude = MathUtil.ConvertDegreeToRadian(endCoordinate.Latitude);
            double endLongitude = MathUtil.ConvertDegreeToRadian(endCoordinate.Longitude);

            double x = Math.Cos(endLattitude) * Math.Sin(endLongitude - startLongitude);
            double y = Math.Cos(startLattitude) * Math.Sin(endLattitude) - Math.Sin(startLattitude) * Math.Cos(endLongitude - startLongitude) * Math.Cos(endLattitude);
            double heading = MathUtil.ConvertRadianToDegree(Math.Atan2(x, y));
            return heading;
        }
    }
}
