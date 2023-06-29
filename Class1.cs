using Npgsql;

using Multicad.Runtime;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.DatabaseServices.StandardObjects;

namespace library
{
    public class Class1
    {
        private static readonly string options = "Host=localhost;Username=postgres;Password=root;Database=graphics";
        [CommandMethod("checkdb", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void ReadDataFromPointTable()
        {
            using (var connection = new NpgsqlConnection(options))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT * FROM point", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var x = reader.GetDouble(0); 
                        var y = reader.GetDouble(1);
                        var z = reader.GetDouble(2);

                        Console.WriteLine($"Point: ({x}, {y}, {z})");
                    }
                }
            }
        }

        [CommandMethod("checkconsole", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void TestConsole()
        {
            Console.WriteLine("Hello World!");
        }

        [CommandMethod("checkdrawing", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
        public void DrawFace()
        {
            DbPolyline line = new DbPolyline();
            List<Point3d> points = new List<Point3d>() {
                new Point3d(100, 150, 0), new Point3d(200, 100, 0), new Point3d(350, 100, 0) };
            line.Polyline = new Polyline3d(points);
            line.Polyline.SetClosed(false);
            line.DbEntity.Transform(McDocumentsManager.GetActiveDoc().UCS); 
            line.DbEntity.AddToCurrentDocument();
        }
    }
}

