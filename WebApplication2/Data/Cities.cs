using System.Collections.Generic;

public class Location
{
    public string Region { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CityMap
{
    public Dictionary<string, List<Location>> Map { get; set; }
}

public class MapData
{
    public static CityMap GetMap()
    {
        return new CityMap
        {
            Map = new Dictionary<string, List<Location>>
            {
                ["Cairo"] = new List<Location>
        {
            new Location { Region = "Giza", Latitude = 30.0131, Longitude = 31.2089 },
            new Location { Region = "Maadi", Latitude = 29.9594, Longitude = 31.2585 },
            new Location { Region = "Nasr City", Latitude = 30.0626, Longitude = 31.341 },
            new Location { Region = "Heliopolis", Latitude = 30.095, Longitude = 31.3322 },
            new Location { Region = "Zamalek", Latitude = 30.0657, Longitude = 31.2168 },
            new Location { Region = "6th of October City", Latitude = 29.9722, Longitude = 30.9614 }
        },
                ["Alexandria"] = new List<Location>
        {
            new Location { Region = "Al-Montaza", Latitude = 31.2316, Longitude = 29.9596 },
            new Location { Region = "Al-Ibrahimiyah", Latitude = 31.2001, Longitude = 29.9187 },
            new Location { Region = "Al-Azaritah", Latitude = 31.2001, Longitude = 29.9187 },
            new Location { Region = "Bolkly", Latitude = 31.2122, Longitude = 29.9443 },
            new Location { Region = "Sidi Gaber", Latitude = 31.2337, Longitude = 29.9528 },
            new Location { Region = "Miami", Latitude = 31.2341, Longitude = 29.9525 }
        },
                ["Luxor"] = new List<Location>
        {
            new Location { Region = "East Bank", Latitude = 25.6872, Longitude = 32.6396 },
            new Location { Region = "West Bank", Latitude = 25.7213, Longitude = 32.6528 },
            new Location { Region = "Karnak", Latitude = 25.7196, Longitude = 32.6556 },
            new Location { Region = "Deir el-Bahari", Latitude = 25.7333, Longitude = 32.6119 },
            new Location { Region = "Medinet Habu", Latitude = 25.7229, Longitude = 32.5921 },
            new Location { Region = "Ramesseum", Latitude = 25.7285, Longitude = 32.6089 }
        },
                ["Aswan"] = new List<Location>
        {
            new Location { Region = "Aswan City", Latitude = 24.0889, Longitude = 32.8998 },
            new Location { Region = "Elephantine Island", Latitude = 24.0953, Longitude = 32.8903 },
            new Location { Region = "Philae", Latitude = 24.0256, Longitude = 32.8847 },
            new Location { Region = "Nubian Village", Latitude = 24.1006, Longitude = 32.9043 },
            new Location { Region = "Kom Ombo", Latitude = 24.4697, Longitude = 32.9628 },
            new Location { Region = "Kalabsha", Latitude = 24.1392, Longitude = 32.8875 }
        },
                ["Sharm El Sheikh"] = new List<Location>
        {
            new Location { Region = "Naama Bay", Latitude = 27.9158, Longitude = 34.3333 },
            new Location { Region = "Ras Um Sid", Latitude = 27.8663, Longitude = 34.2969 },
            new Location { Region = "Sharks Bay", Latitude = 27.9049, Longitude = 34.3194 },
            new Location { Region = "Hadaba", Latitude = 27.8935, Longitude = 34.3289 },
            new Location { Region = "Nabq Bay", Latitude = 28.0034, Longitude = 34.4378 },
            new Location { Region = "Dahab", Latitude = 28.5004, Longitude = 34.5136 }
        },
                ["Hurghada"] = new List<Location>
        {
            new Location { Region = "Sakkala", Latitude = 27.238, Longitude = 33.8208 },
            new Location { Region = "Village Road", Latitude = 27.1671, Longitude = 33.8145 },
            new Location { Region = "Sheraton Road", Latitude = 27.2274, Longitude = 33.8331 },
            new Location { Region = "Downtown", Latitude = 27.2605, Longitude = 33.8106 },
            new Location { Region = "El Gouna", Latitude = 27.3956, Longitude = 33.6772 },
            new Location { Region = "Makadi Bay", Latitude = 27.28, Longitude = 33.8712 }
        }
            }

    };
    }
}
