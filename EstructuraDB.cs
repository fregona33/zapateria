using MySqlConnector;
namespace ZapateriaLosPasos;

public static class EstructuraDB
{
    private static readonly string[] Tablas =
    [
        "Marca", "Categoria", "Proveedor", "Cliente", "Vendedor",
        "Producto", "ProductoProveedor", "StockTalla",
        "Ticket", "LineaTicket"
    ];

    private static readonly string[] Sentencias =
    [
        """
        CREATE TABLE IF NOT EXISTS Marca (
            id_marca INT AUTO_INCREMENT PRIMARY KEY,
            nombre VARCHAR(100) NOT NULL UNIQUE,
            pais VARCHAR(100)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS Categoria (
            id_categoria INT AUTO_INCREMENT PRIMARY KEY,
            nombre VARCHAR(100) NOT NULL UNIQUE
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS Proveedor (
            cif VARCHAR(20) PRIMARY KEY,
            nombre VARCHAR(150) NOT NULL,
            ciudad VARCHAR(100),
            telefono VARCHAR(30)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS Cliente (
            id_cliente VARCHAR(20) PRIMARY KEY,
            nombre VARCHAR(150) NOT NULL,
            ciudad VARCHAR(100)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS Vendedor (
            id_vendedor INT AUTO_INCREMENT PRIMARY KEY,
            nombre VARCHAR(100) NOT NULL UNIQUE
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS Producto (
            codigo VARCHAR(20) PRIMARY KEY,
            nombre VARCHAR(150) NOT NULL,
            id_marca INT NOT NULL,
            id_categoria INT NOT NULL,
            genero VARCHAR(10) NOT NULL,
            precio DECIMAL(10,2) NOT NULL,
            CHECK (precio >= 0),
            FOREIGN KEY (id_marca) REFERENCES Marca(id_marca),
            FOREIGN KEY (id_categoria) REFERENCES Categoria(id_categoria)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS ProductoProveedor (
            codigo_producto VARCHAR(20) NOT NULL,
            cif_proveedor VARCHAR(20) NOT NULL,
            PRIMARY KEY (codigo_producto, cif_proveedor),
            FOREIGN KEY (codigo_producto) REFERENCES Producto(codigo),
            FOREIGN KEY (cif_proveedor) REFERENCES Proveedor(cif)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS StockTalla (
            codigo_producto VARCHAR(20) NOT NULL,
            talla DECIMAL(4,1) NOT NULL,
            stock INT NOT NULL,
            PRIMARY KEY (codigo_producto, talla),
            CHECK (talla > 0),
            CHECK (stock >= 0),
            FOREIGN KEY (codigo_producto) REFERENCES Producto(codigo)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS Ticket (
            numero VARCHAR(30) PRIMARY KEY,
            fecha DATE NOT NULL,
            hora TIME NOT NULL,
            id_vendedor INT NOT NULL,
            forma_pago VARCHAR(30) NOT NULL,
            id_cliente VARCHAR(20) NULL,
            FOREIGN KEY (id_vendedor) REFERENCES Vendedor(id_vendedor),
            FOREIGN KEY (id_cliente) REFERENCES Cliente(id_cliente)
        ) ENGINE=InnoDB;
        """,

        """
        CREATE TABLE IF NOT EXISTS LineaTicket (
            numero_ticket VARCHAR(30) NOT NULL,
            numero_linea INT NOT NULL,
            codigo_producto VARCHAR(20) NOT NULL,
            talla DECIMAL(4,1) NOT NULL,
            unidades INT NOT NULL,
            precio_unitario DECIMAL(10,2) NOT NULL,
            descuento_pct DECIMAL(5,2) NOT NULL DEFAULT 0,
            PRIMARY KEY (numero_ticket, numero_linea),
            CHECK (numero_linea > 0),
            CHECK (talla > 0),
            CHECK (unidades > 0),
            CHECK (precio_unitario >= 0),
            CHECK (descuento_pct BETWEEN 0 AND 100),
            FOREIGN KEY (numero_ticket) REFERENCES Ticket(numero),
            FOREIGN KEY (codigo_producto) REFERENCES Producto(codigo)
        ) ENGINE=InnoDB;
        """
    ];

    public static void Crear(MySqlConnection conexion)
    {
        foreach (string sql in Sentencias)
        {
            using var comando = new MySqlCommand(sql, conexion);
            comando.ExecuteNonQuery();
        }

        Console.WriteLine("Creación de estructuras completada");
    }

    public static void Comprobar(MySqlConnection conexion)
    {
        foreach (string tabla in Tablas)
        {
            const string sql = """
                               SELECT COUNT(*)
                               FROM information_schema.tables
                               WHERE table_schema = DATABASE()
                               AND table_name = @tabla;
                               """;
            
            using var comando = new MySqlCommand(sql, conexion);
            comando.Parameters.AddWithValue("@tabla", tabla);

            long cantidad = Convert.ToInt64(comando.ExecuteScalar());

            Console.WriteLine(
                $"{tabla}: {(cantidad == 1 ? "EXISTE" : "NO EXISTE")}");
        }
    }
    
    public static void Reiniciar(MySqlConnection conexion)
    {
        Console.WriteLine(
            $"Se borrarán las tablas de la práctica y sus datos en {conexion.Database}.");

        Console.Write("Escribe REINICIAR para confirmar: ");

        if (Console.ReadLine() != "REINICIAR")
        {
            Console.WriteLine("Reinicio cancelado.");
            return;
        }
        
        foreach (string tabla in Tablas.Reverse())
        {
            using var comando = new MySqlCommand(
                $"DROP TABLE IF EXISTS `{tabla}`;", conexion);

            comando.ExecuteNonQuery();
        }

        Crear(conexion);
        Comprobar(conexion);
    }
}
