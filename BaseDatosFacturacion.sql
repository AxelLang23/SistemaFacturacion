Create DataBase SistemaFacturacion

Use SistemaFacturacion


CREATE TABLE Usuario
(
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(50) UNIQUE NOT NULL,
    Contraseña NVARCHAR(30) NOT NULL,
	CondicionDeBaja BIT DEFAULT 0 --El campo CondicionDeBaja será de tipo BIT, donde 0 representará que el usuario está activo, y 1 
									--indicará que ha sido dado de baja. El valor predeterminado (DEFAULT 0) se asegura de que los
									--registros nuevos se creen con la condición de "activos" a menos que se indique lo contrario.	
);

INSERT INTO Usuario (NombreUsuario, Contraseña)
VALUES

('MiglioreLucas', 'lucas123'),
('LangAxel', 'axel123');
 

CREATE TABLE CodigoPostal
(
    IdCodigoPostal INT PRIMARY KEY IDENTITY(1,1),
	NroCodigoPostal NVARCHAR(20) UNIQUE NOT NULL 
);


INSERT INTO CodigoPostal(NroCodigoPostal) 
VALUES

   (2300),                      
   (5600),
   (8000);

CREATE TABLE Provincia
(
    IdProvincia INT PRIMARY KEY IDENTITY(1,1), 
    Descripcion NVARCHAR (50) UNIQUE NOT NULL
);

INSERT INTO Provincia(Descripcion) 
VALUES

    ('Santa Fe'),                      
    ('Mendoza'),
	('Buenos Aires');

CREATE TABLE Pais
(
    IdPais INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR (50) UNIQUE NOT NULL
);

INSERT INTO Pais(Descripcion) 
VALUES

    ('Argentina');

CREATE TABLE Localidad
(
    IdLocalidad INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(50) NOT NULL,
    IdCodigoPostal INT NOT NULL,
    IdProvincia INT NOT NULL,
    IdPais INT NOT NULL,

	 -- Claves Foráneas
    FOREIGN KEY (IdCodigoPostal) REFERENCES CodigoPostal(IdCodigoPostal),
    FOREIGN KEY (IdProvincia) REFERENCES Provincia(IdProvincia),
    FOREIGN KEY (IdPais) REFERENCES Pais(IdPais)
);

INSERT INTO Localidad(Descripcion,IdCodigoPostal,IdProvincia,IdPais)
VALUES

    ('Rafaela',1,1,1),
	('San Rafael',2,2,1),
	('Bahia Blanca',3,3,1);



CREATE TABLE CondicionIVA
(
    IdCondicionIVA INT IDENTITY(1,1) PRIMARY KEY,

    Descripcion NVARCHAR(50) NOT NULL -- Descripción de la condición IVA
    CHECK (Descripcion NOT LIKE '%[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]%')
);


INSERT INTO CondicionIVA (Descripcion)
VALUES

('Responsable Inscripto'),
('Monotributista'),
('Exento'),
('Consumidor Final'),
('No Responsable');


CREATE TABLE Empresa
(
    IdEmpresa INT PRIMARY KEY IDENTITY(1,1), 
    NombreEmpresa NVARCHAR(50) NOT NULL, 
    IdCondicionIVA INT NOT NULL DEFAULT 1, 
    Telefono NVARCHAR(20) UNIQUE NULL , 
    
    CUIT NVARCHAR(20) DEFAULT NULL,


    Email NVARCHAR(50) UNIQUE NOT NULL, 
    CHECK (Email LIKE '%@%._%'), 
    
    Direccion NVARCHAR(80) NOT NULL,
	IdLocalidad INT NOT NULL,

	FOREIGN KEY (IdCondicionIVA) REFERENCES CondicionIVA(IdCondicionIVA), 
	FOREIGN KEY (IdLocalidad) REFERENCES Localidad(IdLocalidad)
);



INSERT INTO Empresa (NombreEmpresa, Telefono, CUIT, Email, Direccion, IdLocalidad) 
VALUES

(
    'LAML',                      
    '3492-309867',
	'30-35261312-6',
    'LAMLempresa@gmail.com',
    'Av. Siempre Viva 651',       
     1                                   
);




--Los campos de esta tabla Clientes permite que los mismos sean tanto personas como empresas.
CREATE TABLE Cliente
(
    IdCliente INT PRIMARY KEY IDENTITY(1,1),
    NombreCompleto NVARCHAR(60) NOT NULL,
    DNI INT DEFAULT NULL,

    CUIT NVARCHAR(20) DEFAULT NULL,

    IdCondicionIVA INT NOT NULL, -- Relación con la tabla CondicionIVA     
	Direccion NVARCHAR(80) NOT NULL,     
    IdLocalidad INT NOT NULL,
	CondicionDeBaja BIT DEFAULT 0,
	
    FOREIGN KEY (IdLocalidad) REFERENCES Localidad(IdLocalidad),
    FOREIGN KEY (IdCondicionIVA) REFERENCES CondicionIVA(IdCondicionIVA) 
);

-- Crear índice único para DNI solo si el valor no es NULL
CREATE UNIQUE INDEX dni_indice_unico
ON Cliente (DNI)
WHERE DNI IS NOT NULL;

-- Crear índice único para CUIT solo si el valor no es NULL
CREATE UNIQUE INDEX cuit_indice_unico
ON Cliente (CUIT)
WHERE CUIT IS NOT NULL;


/*CREATE UNIQUE INDEX: Este comando crea un índice en el campo DNI y CUIT, y los hace único. Esto significa que no 
permitirá que haya dos filas con el mismo valor en esos campos.
dni_unique_idx/cuit_índice_único: Son los nombres de los índices. 
ON Clientes (DNI/CUIT): Indica que los índices se aplican a los campos DNI y Cuit de la tabla Clientes.
WHERE DNI/CUIT IS NOT NULL: Esta cláusula WHERE significa que los índices solo se aplicarán a los valores que no sean NULL. 
Los valores NULL no se verán afectados por la restricción de unicidad, por lo que se puede tener varios valores 
NULL en el campo DNI y CUIT sin generar errores. */

--Responsables Inscriptos, Exentos, No Responsables
INSERT INTO Cliente (NombreCompleto, DNI, CUIT, IdCondicionIVA, Direccion, IdLocalidad)
VALUES

('Mateo Gimenez', 43166971, '20-43166971-6', 1, 'Gabriel Maggi 589', 1),
('Nicolás Michelini', 44163881, '20-43163881-6', 2, 'Las Heras 618', 1),
('Agustín Gorosito', 44654321, '20-44654321-5', 2, 'Av. Suipacha 742',1),
('Pablo Clementin', 23556677, '20-23556677-9', 3, 'San Martín 456',1),
('Maitena Zanabria', 44566213, '27-44566213-7', 5, 'Bolivar 789',1);


--Consumidores Finales
INSERT INTO Cliente (NombreCompleto, DNI, IdCondicionIVA, Direccion, IdLocalidad)
VALUES
('Franco Madera', 43456991, 4, 'Falucho 789',1),
('Ivan Panighi', 44183441, 4, 'Av Santa Fe 1530',1);

--Empresas
INSERT INTO Cliente (NombreCompleto, CUIT, IdCondicionIVA, Direccion, IdLocalidad)
VALUES
('SoftwareGroup', '30-78934522-5', 1, 'Av Bartolomé Mitre 1800', 2),
('Globant', '30-85623411-5',1,'Luis María Drago 45', 3);

CREATE TABLE Alicuota
(
    IdAlicuota INT PRIMARY KEY IDENTITY(1,1),
    Porcentaje DECIMAL(5,2) NOT NULL
);

/*En la tabla Alicuotas se agregaron todos los porcentajes utilizados actualmente hasta fin de 2024,
los mismos fueron sacados de un pdf de la Agencia Tributaria que enuncia los tipos impositivos en el IVA 2024.
Si bien nuestra empresa comercia hardware, y por lo tanto los productos disponen del mismo tipo (Tipo General - 21%)
también se agregaron porcentajes que aplican a otras determinadas clasificaciones, que aunque no se utilizarán,
podrian servir ante una posible extensión ya sea de nuevos productos o servicios ofrecidos 
y comercializados por la empresa*/

-- Insertar los valores
INSERT INTO Alicuota (Porcentaje) VALUES 
(0),
(2),
(4),
(5),
(7.5),
(10),
(21);

CREATE TABLE Marca 
(
    IdMarca INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(50) NOT NULL UNIQUE,
	CondicionDeBaja BIT DEFAULT 0
);

INSERT INTO Marca (Descripcion)
VALUES 
('Samsung'),
('Logitech'),
('Genius'),
('Epson'),
('HP'),
('Sony'),
('Western Digital'),
('Kingston');


CREATE TABLE Articulo
(
    IdArticulo INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(50) NOT NULL UNIQUE, 
    IdMarca INT NOT NULL,
    PrecioUnitarioSinIVA DECIMAL(10, 2) NOT NULL,
    CantidadEnStock INT NOT NULL,
	CondicionDeBaja BIT DEFAULT 0,
    IdAlicuota INT NOT NULL	

	FOREIGN KEY (IdMarca) REFERENCES Marca(IdMarca),
    FOREIGN KEY (IdAlicuota) REFERENCES Alicuota(IdAlicuota)
);


INSERT INTO Articulo (Descripcion, IdMarca, PrecioUnitariosinIVA, CantidadEnStock, IdAlicuota)
VALUES

('Teclado Inalámbrico', 2, 40000.00, 100, 7),
('Mouse Inalámbrico', 3, 7000.00, 150, 7),
('Impresora Láser', 4, 100000.00, 20, 7),
('Notebook Core i7', 5, 3000000.00, 10, 7),
('Tablet 10"', 1, 250000.00, 15, 7),
('Auriculares Bluetooth', 6, 90000.00, 50, 7),
('Disco Duro Externo 1TB', 7, 96000.00, 30, 7),
('Pendrive 64GB', 8, 9500.00, 200, 7),
('Cámara Web Full HD', 2, 120000.00, 40, 7);


CREATE TABLE CondicionVenta
(
    IdCondicionVenta INT IDENTITY(1,1) PRIMARY KEY, -- Autoincrementa desde 1
    Descripcion NVARCHAR(50) NOT NULL,          -- Descripción de la condición de venta
	CondicionDeBaja BIT DEFAULT 0
);

INSERT INTO CondicionVenta (Descripcion)
VALUES

('Contado'),
('Tarjeta de Crédito'),
('Tarjeta de Débito'),
('Cheque'),
('Transferencia Bancaria');


CREATE TABLE Factura
(
    IdFactura INT IDENTITY(1,1) PRIMARY KEY,
    Tipo CHAR(1) NOT NULL
    CHECK (Tipo IN ('A', 'B')),
	NroFactura INT NOT NULL,
	PuntoDeVenta INT DEFAULT 1, 
    IdCondicionVenta INT NOT NULL,          -- Relación con la tabla CondicionVenta
    Fecha DATETIME NOT NULL,                  
    IdCliente INT NOT NULL,                 -- Identificación del cliente (se relaciona con la tabla Clientes)
    Subtotal DECIMAL(10, 2) DEFAULT NULL,      
    IVA DECIMAL(10, 2) DEFAULT NULL,            
    TotalNeto DECIMAL(10, 2) NOT NULL,   
	
    FOREIGN KEY (IdCliente) REFERENCES Cliente(IdCliente),
    FOREIGN KEY (IdCondicionVenta) REFERENCES CondicionVenta(IdCondicionVenta)  -- Relación con la tabla CondicionVenta

);

-- Creación del trigger para incrementar NroFactura
GO
CREATE TRIGGER IncrementarNroFactura
ON Factura
AFTER INSERT
AS
BEGIN
    DECLARE @NuevoNroFactura INT;

    -- Obtener el último valor de NroFactura
    SELECT @NuevoNroFactura = ISNULL(MAX(NroFactura), 0) + 1 FROM Factura;

    -- Actualizar la fila insertada con el nuevo número de factura
    UPDATE Factura
    SET NroFactura = @NuevoNroFactura
    WHERE Idfactura IN (SELECT Idfactura FROM inserted);
END;



CREATE TABLE FacturaDetalle
(
    IdFactura INT NOT NULL,                        -- Relación con la factura
    IdArticulo INT NOT NULL,                       -- Relación con la tabla Artículo
    Cantidad INT NOT NULL,                         -- Cantidad de productos
    PrecioUnitario DECIMAL(10, 2) NOT NULL,       -- Precio unitario del producto (viene de la tabla Artículo)
    PrecioTotal DECIMAL(10, 2) NOT NULL,          -- Precio total por cantidad (Cantidad * PrecioUnitario)

    FOREIGN KEY (IdFactura) REFERENCES Factura(IdFactura),  -- Relación con la tabla Factura
    FOREIGN KEY (IdArticulo) REFERENCES Articulo(IdArticulo) -- Relación con la tabla Artículo
);




select * from Factura
Select * from FacturaDetalle




-- 2. Borrar los datos de las tablas
DELETE FROM FacturaDetalle;
DELETE FROM Factura;

-- 3. Resetear el IDENTITY a 1
DBCC CHECKIDENT ('Factura', RESEED, 0);  -- El próximo ID será 1



