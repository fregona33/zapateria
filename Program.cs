using MySqlConnector;

try
{
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
}
catch(Exception ex)
{
    Console.WriteLine($"Fallo en la entrada a la base de datos. {ex.Message}");
}

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1. Crear tablas");
    Console.WriteLine("2. Comprobar tablas");
    Console.WriteLine("3. Reiniciar estructura y datos");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");

    string? opcion = Console.ReadLine();

    if (opcion == 0)
        break;
}