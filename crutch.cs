using Microsoft.VisualBasic;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace library
{
    public class crutch
    {
        private static readonly string options = "Host=localhost;Username=postgres;Password=root;Database=pepe";
        public static void Main()
        {
            List<string> points = new List<string>();

            using (var connection = new NpgsqlConnection(options))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM point", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var x = reader.GetDouble(1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                        var y = reader.GetDouble(2).ToString(System.Globalization.CultureInfo.InvariantCulture);
                        var z = reader.GetDouble(3).ToString(System.Globalization.CultureInfo.InvariantCulture);

                        string s = $"new Point3d({x},{y},{z})";

                        points.Add(s);
                    }
                }
                string code = "List<List<Point3d>> lines = new List<List<Point3d>>{";

                using (var command = new NpgsqlCommand("SELECT * FROM polyline", connection))
                using (var reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        var idStart = reader.GetInt16(1);
                        var idEnd = reader.GetInt16(2);

                        List<string> subList = points.GetRange(idStart - 1, idEnd - idStart + 1);
                        string lineString = string.Join(", ", subList);

                        code += "new List<Point3d>{" + lineString + "} ,";
                    }
                }
                code += "};";

                System.Console.WriteLine(code);

            }
        }
    }
}
