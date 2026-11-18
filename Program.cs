using System;
using Microsoft.Data.Sqlite;

namespace DemoSqlite
{
    class Program
    {
        static void Main()
        {
            // 1. La base de datos SQLite se crea como archivo si no existe
            string cadenaConexion = "Data Source=estudiantes.db";

            // 2. Conexión con SqliteConnection (using libera el recurso al terminar)
            using (var conexion = new SqliteConnection(cadenaConexion))
            {
                conexion.Open();
                Console.WriteLine("Conexión abierta correctamente.\n");

                // 3. Crear tabla con 4 columnas
                using (var cmdCrear = new SqliteCommand(
                    @"CREATE TABLE IF NOT EXISTS Estudiantes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Carrera TEXT NOT NULL,
                        Semestre INTEGER NOT NULL
                      );", conexion))
                {
                    cmdCrear.ExecuteNonQuery();
                }

                // Limpiar para que el ejemplo sea repetible
                using (var cmdLimpiar = new SqliteCommand("DELETE FROM Estudiantes;", conexion))
                {
                    cmdLimpiar.ExecuteNonQuery();
                }

                // 4. Insertar 3 registros con SqliteCommand y parámetros
                var datos = new (string Nombre, string Carrera, int Semestre)[]
                {
                    ("Ana Gómez", "Mecatrónica", 4),
                    ("Carlos Pérez", "Ingeniería de Sistemas", 6),
                    ("Laura Rojas", "Electrónica", 3)
                };

                foreach (var d in datos)
                {
                    using (var cmdInsertar = new SqliteCommand(
                        "INSERT INTO Estudiantes (Nombre, Carrera, Semestre) VALUES (@nombre, @carrera, @semestre);",
                        conexion))
                    {
                        cmdInsertar.Parameters.AddWithValue("@nombre", d.Nombre);
                        cmdInsertar.Parameters.AddWithValue("@carrera", d.Carrera);
                        cmdInsertar.Parameters.AddWithValue("@semestre", d.Semestre);
                        cmdInsertar.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Registros insertados.\n");

                // 5. Consultar y mostrar con SqliteDataReader
                using (var cmdConsulta = new SqliteCommand(
                    "SELECT Id, Nombre, Carrera, Semestre FROM Estudiantes;", conexion))
                using (SqliteDataReader lector = cmdConsulta.ExecuteReader())
                {
                    Console.WriteLine($"{"Id",-4}{"Nombre",-18}{"Carrera",-26}{"Semestre"}");
                    Console.WriteLine(new string('-', 58));

                    while (lector.Read())
                    {
                        Console.WriteLine(
                            $"{lector.GetInt32(0),-4}{lector.GetString(1),-18}{lector.GetString(2),-26}{lector.GetInt32(3)}");
                    }
                }
            } // 6. La conexión se cierra y libera automáticamente aquí

            Console.WriteLine("\nPresiona una tecla para salir...");
            Console.ReadKey();
        }
    }
}
