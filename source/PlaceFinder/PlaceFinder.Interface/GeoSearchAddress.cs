﻿using ESRI.ArcGIS.ArcMapUI;
using GeoJSON.Net;
using GeoJSON.Net.Converters;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace GeodataStyrelsen.ArcMap.PlaceFinder.Interface
{
    public class GeoSearchAddress
    {
        /// <summary>
        /// Primary display text for a hit (is available for all search results)
        /// </summary>
        [JsonProperty("visningstekst")]
        public string Visningstekst { get; set; }
        /// <summary>
        /// Unique id available for all search results
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// WKT built from other properties to ensure single access to geometry of hit (for zooming)  
        /// </summary>
        /// <remarks>author: jbw@hermestraffic.com</remarks>
        public string GeometryWkt
        {
            get
            {
                // Prioritized access order
                if (Geometry != null) return ConvertToWKT(Geometry);
                if (Vejpunkt_geometri != null) return ConvertToWKT(Vejpunkt_geometri);
                if (Adgangspunkt_geometri != null) return ConvertToWKT(Adgangspunkt_geometri);
                if (BBox != null) return ConvertToWKT(BBox);
                return null;
            }
        }
        /// <summary>
        /// Main geometric instance
        /// </summary>
        [JsonProperty("geometri")]
        [JsonConverter(typeof(GeometryConverter))]
        public GeoJSON.Net.GeoJSONObject Geometry { get; set; }
        /// <summary>
        /// Support geometric instance for some ressources
        /// </summary>
        [JsonProperty("vejpunkt_geometri")]
        [JsonConverter(typeof(GeometryConverter))]
        /// <summary>
        /// Support geometric instance for some ressources
        /// </summary>
        public GeoJSON.Net.GeoJSONObject Vejpunkt_geometri { get; set; }
        [JsonProperty("adgangspunkt_geometri")]
        [JsonConverter(typeof(GeometryConverter))]
        /// <summary>
        /// Support geometric instance for some ressources
        /// </summary>
        public GeoJSON.Net.GeoJSONObject Adgangspunkt_geometri { get; set; }
        [JsonProperty("bbox")]
        [JsonConverter(typeof(GeometryConverter))]
        public GeoJSON.Net.GeoJSONObject BBox { get; set; }

        public string Ressource { get; set; }

        // Generic properties that may be available for different result sets
        /* Deprecated as of version 2.0 (if reintroduced use https://docs.dataforsyningen.dk/#gsearch-schemas)
        public string streetName { get; set; }
        public string postCodeIdentifier { get; set; }
        public string districtName { get; set; }
        public bool validCoordinates { get; set; }
        public double xMax { get; set; }
        public double yMax { get; set; }
        public double xMin { get; set; }
        public double yMin { get; set; }
        public string municipalityCodes { get; set; }
        public string streetCodes { get; set; }
        public string id_lokalid { get; set; }
        public string name { get; set; }
        public string elavsnavn { get; set; }
        public string elavskode { get; set; }
        public string matrnr { get; set; }
        public string regionKoder { get; set; }
        public string featKode { get; set; }
        public object stednavnType { get; set; }
        public string valgkredsNr { get; set; }
        public string storkredsNr { get; set; }
        public string storkredsNavn { get; set; }
        public string landsdelsNr { get; set; }
        public string landsdelsNavn { get; set; }
        */

        /**
         * Supportive methods for efficiently generating the WKT for the GeoSearchAddress
         */

        ///<summary>Method for conversion of GeoJSONObjects to <c>WKT</c></summary>
        ///<param name="geojsonobject">The GeoJSONObject to represent by <c>WKT</c></param>
        ///<returns>The object as <c>WKT</c></returns>
        ///<remarks>author: jbw@hermestraffic.com</remarks>
        public string ConvertToWKT(GeoJSONObject geojsonobject)
        {
            string retVal = "Unknown type";
            string coordinates = null;
            switch (geojsonobject.Type)
            {
                case GeoJSONObjectType.Point:
                    coordinates = "(" + StringifyPosition(((GeoJSON.Net.Geometry.Point)geojsonobject).Coordinates) + ")";
                    break;
                case GeoJSONObjectType.LineString:
                    coordinates = StringifyPositions(((GeoJSON.Net.Geometry.LineString)geojsonobject).Coordinates);
                    break;
                case GeoJSONObjectType.Polygon:
                    coordinates = StringifyLinestrings(((GeoJSON.Net.Geometry.Polygon)geojsonobject).Coordinates);
                    break;
                case GeoJSONObjectType.MultiPoint:
                    coordinates = StringifyPositions(((GeoJSON.Net.Geometry.MultiPoint)geojsonobject).Coordinates.Select(a => a.Coordinates));
                    break;
                case GeoJSONObjectType.MultiPolygon:
                    coordinates = StringifyPolygons(((GeoJSON.Net.Geometry.MultiPolygon)geojsonobject).Coordinates);
                    break;
                case GeoJSONObjectType.MultiLineString:
                    coordinates = StringifyLinestrings(((GeoJSON.Net.Geometry.MultiLineString)geojsonobject).Coordinates);
                    break;
                default:
                    // Unrecognized
                    break;
            }
            if (coordinates != null)
                retVal = geojsonobject.Type.ToString().ToUpper() + coordinates;
            return retVal;
        }

        /// <summary>
        /// Converts an <c>IPosition</c> instance to a <c>WKT</c> coordinate set
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The coordinates as Lat/Lon string</returns>
        /// <remarks>author: jbw@hermestraffic.com</remarks>
        private string StringifyPosition(GeoJSON.Net.Geometry.IPosition position)
        {
            return string.Format("{0} {1}", position.Longitude, position.Latitude);
        }

        /// <summary>
        /// Converts an <c>IPosition</c> enumerable collection to a <c>WKT</c> coordinate set
        /// </summary>
        /// <param name="position">The position enumerable</param>
        /// <returns>The coordinates as Lat/Lon string</returns>
        /// <remarks>author: jbw@hermestraffic.com</remarks>
        private string StringifyPositions(System.Collections.Generic.IEnumerable<GeoJSON.Net.Geometry.IPosition> coordinates)
        {
            return "(" + string.Join(", ", coordinates.Select(c => StringifyPosition(c))) + ")";
        }

        /// <summary>
        /// Converts a <c>LineString</c> enumerable to a <c>WKT</c> coordinate set
        /// </summary>
        /// <param name="linestrings">The linestring enumerable</param>
        /// <returns>The coordinates as Lat/Lon string</returns>
        /// <remarks>author: jbw@hermestraffic.com</remarks>
        private string StringifyLinestrings(System.Collections.Generic.IEnumerable<GeoJSON.Net.Geometry.LineString> linestrings)
        {
            return "(" + string.Join(", ", linestrings.Select(ls => StringifyPositions(ls.Coordinates))) + ")";
        }

        /// <summary>
        /// Converts a <c>Polygon</c> enumerable to a <c>WKT</c> coordinate set
        /// </summary>
        /// <param name="polygons">The polygon enumerable</param>
        /// <returns>The coordinates as Lat/Lon string</returns>
        /// <remarks>author: jbw@hermestraffic.com</remarks>
        private string StringifyPolygons(System.Collections.Generic.IEnumerable<GeoJSON.Net.Geometry.Polygon> polygons) {
            return "(" + string.Join(", ", polygons.Select(poly => StringifyLinestrings(poly.Coordinates))) + ")";
        }
    }
}
