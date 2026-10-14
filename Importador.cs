using System.Text.Json;
using MySqlConnector;


namespace ZapateriaLosPasos;

public class Importador
{
    public static void ImportarMarcas(MySqlConnection conector)
    {
        string ruta = Path.Combine(
            AppContext.BaseDirectory, "zapateria.json");

        string texto = File.ReadAllText(ruta);

        using JsonDocument documento = JsonDocument.Parse(texto);

        JsonElement productos = documento.RootElement.GetProperty("productos");

        int insertadas = 0;
        int existentes = 0;
        int rechazadas = 0;

        using var transaccion = conector.BeginTransaction();

        try
        {
            foreach (JsonElement producto in productos.EnumerateArray())
            {
                string codigo = producto.GetProperty("codigo").GetString() ?? "(sin codigo)";

                if (!producto.TryGetProperty("marca", out JsonElement marca)
                    || marca.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine($"{codigo}: marca ausente o inválida,");
                    rechazadas++;
                    continue;
                }
                if (!marca.TryGetProperty("nombre", out JsonElement campoNombre)
                    || campoNombre.ValueKind != JsonValueKind.String
                    || string.IsNullOrWhiteSpace(campoNombre.GetString()))
                {
                    Console.WriteLine($"{codigo}: marca sin nombre válido.");
                    rechazadas++;
                    continue;
                }
                string nombre = campoNombre.GetString()!.Trim();
                string? pais = null;

                if (marca.TryGetProperty("pais", out JsonElement campoPais)
                    && campoPais.ValueKind == JsonValueKind.String)
                {
                    pais = campoPais.GetString();
                }

                const string buscarSql = """
                                         SELECT id_marca
                                         FROM Marca
                                         WHERE nombre = @nombre;
                                         """;

                using var buscar = new MySqlCommand(
                    buscarSql, conector, transaccion);

                buscar.Parameters.AddWithValue("@nombre", nombre);

                object? idExistente = buscar.ExecuteScalar();

                if (idExistente != null && idExistente != DBNull.Value)
                {
                    existentes++;
                    continue;
                }

                const string insertarSql = """
                                           INSERT INTO Marca (nombre, pais)
                                           VALUES (@nombre, @pais);
                                           """;

                using var insertar = new MySqlCommand(
                    insertarSql, conector, transaccion);

                insertar.Parameters.AddWithValue("@nombre", nombre);
                insertar.Parameters.AddWithValue(
                    "@pais", (object?)pais ?? DBNull.Value);

                insertadas += insertar.ExecuteNonQuery();
            }

            transaccion.Commit();

            Console.WriteLine("Importación de marcas terminada.");
            Console.WriteLine($"Marcas insertadas: {insertadas}");
            Console.WriteLine($"Referencias a marcas existentes: {existentes}");
            Console.WriteLine($"Referencias rechazadas: {rechazadas}");
        }
        catch
        {
            transaccion.Rollback();
            Console.WriteLine("Se han deshecho las inserciones de marcas.");
            throw;
        }
    }

    public static void ImportarCategorias(MySqlConnection conexion)
    {
        string ruta = Path.Combine(AppContext.BaseDirectory, "zapateria.json");

        string texto = File.ReadAllText(ruta);

        using JsonDocument documento = JsonDocument.Parse(texto);

        JsonElement productos = documento.RootElement.GetProperty("productos");

        int insertadas = 0;
        int existentes = 0;
        int rechazadas = 0;

        using var transaccion = conexion.BeginTransaction();

        try
        {
            foreach (JsonElement producto in productos.EnumerateArray())
        {
            string codigo = producto.GetProperty("codigo").GetString()
                ?? "(sin código)";

            if (!producto.TryGetProperty("categoria", out JsonElement campo)
                || campo.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(campo.GetString()))
            {
                Console.WriteLine($"{codigo}: categoría ausente o inválida.");
                rechazadas++;
                continue;
            }

            string nombre = campo.GetString()!.Trim();

            const string buscarSql = """
                SELECT id_categoria
                FROM Categoria
                WHERE nombre = @nombre;
                """;

            using var buscar = new MySqlCommand(
                buscarSql, conexion, transaccion);

            buscar.Parameters.AddWithValue("@nombre", nombre);

            object? idExistente = buscar.ExecuteScalar();

            if (idExistente != null && idExistente != DBNull.Value)
            {
                existentes++;
                continue;
            }

            const string insertarSql = """
                INSERT INTO Categoria (nombre)
                VALUES (@nombre);
                """;

            using var insertar = new MySqlCommand(
                insertarSql, conexion, transaccion);

            insertar.Parameters.AddWithValue("@nombre", nombre);

            insertadas += insertar.ExecuteNonQuery();
        }

        transaccion.Commit();

        Console.WriteLine("Importación de categorías terminada.");
        Console.WriteLine($"Categorías insertadas: {insertadas}");
        Console.WriteLine($"Referencias a categorías existentes: {existentes}");
        Console.WriteLine($"Referencias rechazadas: {rechazadas}");
        }
        catch(Exception ex)
        {
            transaccion.Rollback();
            Console.WriteLine("Se han deshecho las inserciones de categorías.");
            throw;
        }
    } 
}
    
