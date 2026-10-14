using System.Text.Json;
using MySqlConnector;
using ZapateriaLosPasos;

const string servidor = "217.154.101.61";
    const string usuario = "remote_user";
    const string baseDatos = "alumno_11";

    string? clave = Environment.GetEnvironmentVariable("@1234");

    if (string.IsNullOrWhiteSpace(clave))
    {
        Console.WriteLine("La contraseña del entorno es incorrecta");
        return;
    }

    var configuracion = new MySqlConnectionStringBuilder
    {
        Server = servidor,
        Port = 3306,
        UserID = usuario,
        Password = clave,
        Database = baseDatos
    };


while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Crear tablas");
    Console.WriteLine("2. Comprobar tablas");
    Console.WriteLine("3. Reiniciar estructura y datos");
    Console.WriteLine("4. Importar marcas");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");

    string? opcion = Console.ReadLine();

    if (opcion == "0")
        break;

    if (opcion != "1" && opcion != "2" && opcion != "3" && opcion != "4")
    {
        Console.WriteLine("Opción no válida");
        continue;
    }

    try
    {
        using var conexion =
            new MySqlConnection(configuracion.ConnectionString);

        conexion.Open();

        switch (opcion)
        {
            case "1":
                EstructuraDB.Crear(conexion);
                EstructuraDB.Comprobar(conexion);
                break;

            case "2":
                EstructuraDB.Comprobar(conexion);
                break;

            case "3":
                EstructuraDB.Reiniciar(conexion);
                break;

            case "4":
                Importador.ImportarMarcas(conexion);
                break;
        }
    }
    catch (MySqlException ex)
    {
        Console.WriteLine($"Error de base de datos {ex.Number}: {ex.Message}");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"JSON incorrecto: {ex.Message}");
    }
}