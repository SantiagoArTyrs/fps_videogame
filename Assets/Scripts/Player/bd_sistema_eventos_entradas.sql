-- ============================================
--  CREAR BASE DE DATOS
-- ============================================
DROP DATABASE IF EXISTS gestion_eventos;
CREATE DATABASE gestion_eventos;
USE gestion_eventos;

-- ============================================
--  TABLA: USUARIO
-- ============================================
CREATE TABLE usuario (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(120) NOT NULL UNIQUE,
    fecha_registro DATE NOT NULL
);

-- ============================================
--  TABLA: TIPO_EVENTO
-- ============================================
CREATE TABLE tipo_evento (
    id_tipo_evento INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(100) NOT NULL
);

-- ============================================
--  TABLA: LUGAR
-- ============================================
CREATE TABLE lugar (
    id_lugar INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(150),
    capacidad INT NOT NULL CHECK (capacidad > 0)
);

-- ============================================
--  TABLA: ORGANIZADOR
-- ============================================
CREATE TABLE organizador (
    id_organizador INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(120) NOT NULL UNIQUE,
    telefono VARCHAR(20)
);

-- ============================================
--  TABLA: EVENTO
-- ============================================
CREATE TABLE evento (
    id_evento INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(120) NOT NULL,
    fecha DATE NOT NULL,
    id_tipo_evento INT,
    id_lugar INT,
    id_organizador INT,
    precio DECIMAL(10,2) NOT NULL CHECK (precio >= 0),
    aforo_maximo INT NOT NULL CHECK (aforo_maximo > 0),
    estado ENUM('activo','finalizado','cancelado') DEFAULT 'activo',
    FOREIGN KEY (id_tipo_evento) REFERENCES tipo_evento(id_tipo_evento),
    FOREIGN KEY (id_lugar) REFERENCES lugar(id_lugar),
    FOREIGN KEY (id_organizador) REFERENCES organizador(id_organizador)
);

-- ============================================
--  TABLA: PAGO
-- ============================================
CREATE TABLE pago (
    id_pago INT AUTO_INCREMENT PRIMARY KEY,
    fecha_pago DATE NOT NULL,
    monto DECIMAL(10,2) NOT NULL CHECK (monto >= 0),
    metodo VARCHAR(50),
    estado VARCHAR(20) DEFAULT 'completado'
);

-- ============================================
--  TABLA: ENTRADA
-- ============================================
CREATE TABLE entrada (
    id_entrada INT AUTO_INCREMENT PRIMARY KEY,
    id_usuario INT,
    id_evento INT,
    fecha_compra DATE NOT NULL,
    precio_pagado DECIMAL(10,2) NOT NULL CHECK (precio_pagado >= 0),
    id_pago INT,
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
    FOREIGN KEY (id_evento) REFERENCES evento(id_evento),
    FOREIGN KEY (id_pago) REFERENCES pago(id_pago)
);

-- ============================================
-- TRIGGER: Evita superar el aforo del evento
-- ============================================
DELIMITER //

CREATE TRIGGER validar_aforo
BEFORE INSERT ON entrada
FOR EACH ROW
BEGIN
    DECLARE cantidad_entradas INT;
    DECLARE limite INT;

    SELECT COUNT(*) INTO cantidad_entradas
    FROM entrada
    WHERE id_evento = NEW.id_evento;

    SELECT aforo_maximo INTO limite
    FROM evento
    WHERE id_evento = NEW.id_evento;

    IF cantidad_entradas >= limite THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No se pueden vender más entradas: aforo completo.';
    END IF;
END //

DELIMITER ;

-- ============================================
-- INSERTS DE PRUEBA (realistas)
-- ============================================

INSERT INTO usuario (nombre, email, fecha_registro) VALUES
('Carlos Pérez','carlos@gmail.com','2024-09-10'),
('María Lopez','maria@gmail.com','2024-10-12'),
('Juan Torres','juan@hotmail.com','2024-08-01');

INSERT INTO tipo_evento (descripcion) VALUES
('Concierto'),
('Conferencia'),
('Taller'),
('Deporte');

INSERT INTO lugar (nombre, direccion, capacidad) VALUES
('Coliseo Norte','Calle 23 #45-10', 5000),
('Auditorio Central','Cra 7 #12-30', 800),
('Teatro Municipal','Av. Siempre Viva #22', 1200);

INSERT INTO organizador (nombre, email, telefono) VALUES
('LiveEvents CO','eventos@live.com','3001234567'),
('ProConferencias','info@proconf.com','3129988221');

INSERT INTO evento (nombre, fecha, id_tipo_evento, id_lugar, id_organizador, precio, aforo_maximo, estado) VALUES
('Bad Bunny Tour','2025-03-15',1,1,1,250000,5000,'activo'),
('Conferencia IA 2025','2025-04-10',2,2,2,150000,800,'activo'),
('Taller de Programación','2025-02-20',3,3,2,50000,200,'activo');

INSERT INTO pago (fecha_pago, monto, metodo, estado) VALUES
('2025-01-10', 250000, 'Tarjeta', 'completado'),
('2025-01-12', 150000, 'Nequi', 'completado'),
('2025-01-15', 50000, 'Efectivo', 'completado');

INSERT INTO entrada (id_usuario, id_evento, fecha_compra, precio_pagado, id_pago) VALUES
(1,1,'2025-01-10',250000,1),
(2,2,'2025-01-12',150000,2),
(3,3,'2025-01-15',50000,3);

-- ============================================
-- CONSULTAS AVANZADAS PEDIDAS
-- ============================================

-- 1. Eventos con más asistentes
SELECT e.nombre, COUNT(en.id_entrada) AS asistentes
FROM evento e
LEFT JOIN entrada en ON e.id_evento = en.id_evento
GROUP BY e.id_evento
ORDER BY asistentes DESC;

-- 2. Ingresos por evento
SELECT e.nombre, SUM(en.precio_pagado) AS ingresos
FROM evento e
JOIN entrada en ON e.id_evento = en.id_evento
GROUP BY e.id_evento;

-- 3. Usuarios que han comprado más de 3 entradas en el mes
SELECT u.nombre, COUNT(*) AS total_compras
FROM entrada en
JOIN usuario u ON u.id_usuario = en.id_usuario
WHERE MONTH(en.fecha_compra) = 1
GROUP BY u.id_usuario
HAVING total_compras > 3;

-- 4. Estadísticas por tipo de evento
SELECT t.descripcion AS tipo_evento, COUNT(e.id_evento) AS total_eventos
FROM tipo_evento t
LEFT JOIN evento e ON e.id_tipo_evento = t.id_tipo_evento
GROUP BY t.id_tipo_evento;
