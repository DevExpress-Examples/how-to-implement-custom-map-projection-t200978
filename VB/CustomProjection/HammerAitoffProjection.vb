Imports System
Imports System.Windows
Imports DevExpress.Xpf.Map

Namespace CustomProjection
    Friend Class HammerAitoffProjection
        Implements IProjection

        Const offsetX As Double = 0.5
        Const offsetY As Double = 0.5
        Const scaleX As Double = 0.5
        Const scaleY As Double = -0.25

        Const minLatitude As Double = -90.0
        Const maxLatitude As Double = 90.0
        Const minLongitude As Double = -180.0
        Const maxLongitude As Double = 180.0

        Const latitudeToKilometersRatio As Double = 111.12
        Const longitudeToKilometersRatio As Double = 100

        Private Shared Function RadianToDegree(ByVal value As Double) As Double
            Return value * 180.0 / Math.PI
        End Function

        Private Shared Function DegreeToRadian(ByVal value As Double) As Double
            Return value * Math.PI / 180.0
        End Function

        Private Function IsValidPoint(ByVal x As Double, ByVal y As Double) As Boolean
            If Math.Pow(x, 2) + Math.Pow(y, 2) > 1 Then
                Return False
            End If

            Return True
        End Function

        Public Function GeoPointToMapUnit(ByVal geoPoint As GeoPoint) As MapUnit _
            Implements IProjection.GeoPointToMapUnit

            Dim lonInRadian As Double = DegreeToRadian(
                Math.Min(
                    maxLongitude,
                    Math.Max(minLongitude, geoPoint.Longitude)
                )
            )
            Dim latInRadian As Double = DegreeToRadian(
                Math.Min(
                    maxLatitude,
                    Math.Max(minLatitude, geoPoint.Latitude)
                )
            )
            Dim z As Double = Math.Sqrt(1 + Math.Cos(latInRadian) * Math.Cos(lonInRadian / 2))
            Dim x As Double = Math.Cos(latInRadian) * Math.Sin(lonInRadian / 2) / z
            Dim y As Double = Math.Sin(latInRadian) / z

            Return New MapUnit(x * scaleX + offsetX, y * scaleY + offsetY)
        End Function

        Public Function MapUnitToGeoPoint(ByVal mapUnit As MapUnit) As GeoPoint _
            Implements IProjection.MapUnitToGeoPoint

            Dim x As Double = (mapUnit.X - offsetX) / scaleX
            Dim y As Double = Math.Min(1, Math.Max(-1, (mapUnit.Y - offsetY) / scaleY))

            If IsValidPoint(x, y) Then
                Dim z As Double = Math.Sqrt(1 - 0.5 * Math.Pow(x, 2) - 0.5 * Math.Pow(y, 2))
                Dim c As Double = Math.Sqrt(2) * z * x / (2 * Math.Pow(z, 2) - 1)
                Dim lon As Double = 2 * Math.Atan(c)
                Dim lat As Double = Math.Asin(Math.Min(Math.Max(Math.Sqrt(2) * z * y, -1), 1))
                Dim latInDegree As Double = lat * 180.0 / Math.PI
                Dim lonInDegree As Double = lon * 180.0 / Math.PI

                Return New GeoPoint(
                    Math.Min(
                        maxLatitude,
                        Math.Max(minLatitude, RadianToDegree(lat))
                    ),
                    Math.Min(
                        maxLongitude,
                        Math.Max(minLongitude, RadianToDegree(lon))
                    )
                )
            Else
                Dim signX As Integer = If(x < 0, -1, 1)
                Dim signY As Integer = If(y < 0, -1, 1)
                Return New GeoPoint(maxLatitude * signY, maxLongitude * signX)
            End If
        End Function

        Public Function GeoToKilometersSize(ByVal anchorPoint As GeoPoint, ByVal size As Size) As Size _
            Implements IProjection.GeoToKilometersSize

            Return New Size(
                size.Width * longitudeToKilometersRatio * Math.Cos(DegreeToRadian(anchorPoint.Latitude)),
                size.Height * latitudeToKilometersRatio
            )
        End Function

        Public Function KilometersToGeoSize(ByVal anchorPoint As GeoPoint, ByVal size As Size) As Size _
            Implements IProjection.KilometersToGeoSize

            Return New Size(
                size.Width / longitudeToKilometersRatio / Math.Cos(DegreeToRadian(anchorPoint.Latitude)),
                size.Height / latitudeToKilometersRatio
            )
        End Function
    End Class
End Namespace
