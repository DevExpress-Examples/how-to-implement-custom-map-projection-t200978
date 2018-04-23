using System;
using System.Windows;
using DevExpress.Xpf.Map;

namespace CustomProjection {
    class HammerAitoffProjection : IProjection {
        const double offsetX = 0.5;
        const double offsetY = 0.5;
        const double scaleX = 0.5;
        const double scaleY = -0.25;

        const double minLatitude = -90.0;
        const double maxLatitude = 90.0;
        const double minLongitude = -180.0;
        const double maxLongitude = 180.0;
        
        const double latitudeToKilometersRatio = 111.12;
        const double longitudeToKilometersRatio = 100;

        static double RadianToDegree(double value) {
            return value * 180.0 / Math.PI;
        }

        static double DegreeToRadian(double value) {
            return value * Math.PI / 180.0;
        }

        bool IsValidPoint(double x, double y) {
            if (Math.Pow(x, 2) + Math.Pow(y, 2) > 1)
                return false;

            return true;
        }

        public MapUnit GeoPointToMapUnit(GeoPoint geoPoint) {
            double lonInRadian = DegreeToRadian(
                Math.Min(
                    maxLongitude, 
                    Math.Max(minLongitude, geoPoint.Longitude)
                )
            );
            double latInRadian = DegreeToRadian(
                Math.Min(
                    maxLatitude, 
                    Math.Max(minLatitude, geoPoint.Latitude)
                )
            );
            double z = Math.Sqrt(1 + Math.Cos(latInRadian) * Math.Cos(lonInRadian / 2));
            double x = Math.Cos(latInRadian) * Math.Sin(lonInRadian / 2) / z;
            double y = Math.Sin(latInRadian) / z;

            return new MapUnit(x * scaleX + offsetX, y * scaleY + offsetY);
        }

        public GeoPoint MapUnitToGeoPoint(MapUnit mapUnit) {
            double x = (mapUnit.X - offsetX) / scaleX;
            double y = Math.Min(1, Math.Max(-1, (mapUnit.Y - offsetY) / scaleY));

            if (IsValidPoint(x, y)) {
                double z = Math.Sqrt(1 - 0.5 * Math.Pow(x, 2) - 0.5 * Math.Pow(y, 2));
                double c = Math.Sqrt(2) * z * x / (2 * Math.Pow(z, 2) - 1);
                double lon = 2 * Math.Atan(c);
                double lat = Math.Asin(Math.Min(Math.Max(Math.Sqrt(2) * z * y, -1), 1));
                double latInDegree = lat * 180.0 / Math.PI;
                double lonInDegree = lon * 180.0 / Math.PI;

                return new GeoPoint(
                    Math.Min(
                        maxLatitude, 
                        Math.Max(minLatitude, RadianToDegree(lat))
                    ), 
                    Math.Min(
                        maxLongitude, 
                        Math.Max(minLongitude, RadianToDegree(lon))
                    )
                );
            }
            else {
                int signX = (x < 0) ? -1 : 1;
                int signY = (y < 0) ? -1 : 1;
                return new GeoPoint(maxLatitude * signY, maxLongitude * signX);
            }
        }

        public Size GeoToKilometersSize(GeoPoint anchorPoint, Size size) {
            return new Size(
                size.Width * longitudeToKilometersRatio * Math.Cos(DegreeToRadian(anchorPoint.Latitude)), 
                size.Height * latitudeToKilometersRatio
            );
        }

        public Size KilometersToGeoSize(GeoPoint anchorPoint, Size size) {
            return new Size(
                size.Width / longitudeToKilometersRatio / Math.Cos(DegreeToRadian(anchorPoint.Latitude)), 
                size.Height / latitudeToKilometersRatio
            );
        }
    }
}
