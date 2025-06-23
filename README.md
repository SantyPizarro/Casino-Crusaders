# Casino-Crusaders

ES IMPORTANTE TENER CORRIENDO LA API EN LOCALHOST:7000 ADEMAS DE LA BASE DE DATOS CARGADA PARA EL FUNCIONAMIENTO DEL JUEGO

Link GitHub Web + API: https://github.com/MartuPe/CasinoCrusaders

Creacion Base de datos:
CREATE DATABASE [CasinoCrusaders]
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CasinoCrusaders].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [CasinoCrusaders] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET ARITHABORT OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [CasinoCrusaders] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [CasinoCrusaders] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET  DISABLE_BROKER 
GO

ALTER DATABASE [CasinoCrusaders] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [CasinoCrusaders] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [CasinoCrusaders] SET  MULTI_USER 
GO

ALTER DATABASE [CasinoCrusaders] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [CasinoCrusaders] SET DB_CHAINING OFF 
GO

ALTER DATABASE [CasinoCrusaders] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [CasinoCrusaders] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [CasinoCrusaders] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [CasinoCrusaders] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [CasinoCrusaders] SET QUERY_STORE = ON
GO

ALTER DATABASE [CasinoCrusaders] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO

ALTER DATABASE [CasinoCrusaders] SET  READ_WRITE 
GO

USE [CasinoCrusaders]
GO
/****** Object:  Table [dbo].[Enemigo]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enemigo](
	[Id_enemigo] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [text] NULL,
	[Vida] [int] NULL,
	[Ataque] [int] NULL,
	[Defensa] [int] NULL,
	[Descripcion] [nchar](200) NULL,
	[Imagen] [nchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_enemigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Nivel]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Nivel](
	[Id_nivel] [int] IDENTITY(1,1) NOT NULL,
	[Id_enemigo] [int] NULL,
	[Nombre] [text] NULL,
	[Id_tipo_casillero] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_nivel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Objeto]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Objeto](
	[IdObjeto] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Estadistica] [int] NOT NULL,
	[Precio] [int] NOT NULL,
 CONSTRAINT [PK_Objeto] PRIMARY KEY CLUSTERED 
(
	[IdObjeto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personaje]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personaje](
	[Id_personaje] [int] IDENTITY(1,1) NOT NULL,
	[Vida_maxima] [int] NULL,
	[Vida_actual] [int] NULL,
	[Dano_ataque] [int] NULL,
	[Defensa] [int] NULL,
	[Monedas] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_personaje] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Progreso]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Progreso](
	[Id_progreso] [int] IDENTITY(1,1) NOT NULL,
	[Id_nivel] [int] NOT NULL,
	[Id_personaje] [int] NOT NULL,
	[Fecha_creacion] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_progreso] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tipo_casillero]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tipo_casillero](
	[Id_tipo_casillero] [int] IDENTITY(1,1) NOT NULL,
	[descripcion] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_tipo_casillero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 22/6/2025 17:38:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[Id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[Gmail] [varchar](50) NOT NULL,
	[Contraseña] [varchar](150) NOT NULL,
	[Nombre_usuario] [varchar](50) NOT NULL,
	[Tipo_usuario] [varchar](30) NULL,
	[Id_personaje] [int] NULL,
	[EmailVerificacionToken] [varchar](40) NULL,
	[ExpiracionToken] [datetime] NULL,
	[EmailVerificado] [bit] NOT NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[Id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Personaje] ADD  CONSTRAINT [DF_Personaje_Monedas]  DEFAULT ((0)) FOR [Monedas]
GO
ALTER TABLE [dbo].[Usuario] ADD  DEFAULT ((0)) FOR [EmailVerificado]
GO
ALTER TABLE [dbo].[Nivel]  WITH CHECK ADD FOREIGN KEY([Id_enemigo])
REFERENCES [dbo].[Enemigo] ([Id_enemigo])
GO
ALTER TABLE [dbo].[Nivel]  WITH CHECK ADD FOREIGN KEY([Id_tipo_casillero])
REFERENCES [dbo].[Tipo_casillero] ([Id_tipo_casillero])
GO
ALTER TABLE [dbo].[Progreso]  WITH CHECK ADD FOREIGN KEY([Id_nivel])
REFERENCES [dbo].[Nivel] ([Id_nivel])
GO
ALTER TABLE [dbo].[Progreso]  WITH CHECK ADD FOREIGN KEY([Id_personaje])
REFERENCES [dbo].[Personaje] ([Id_personaje])
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD  CONSTRAINT [FK_Usuario_Personaje] FOREIGN KEY([Id_personaje])
REFERENCES [dbo].[Personaje] ([Id_personaje])
GO
ALTER TABLE [dbo].[Usuario] CHECK CONSTRAINT [FK_Usuario_Personaje]
GO

INSERT INTO [dbo].[Tipo_casillero]
           ([descripcion])
     VALUES
           ('Combate'), ('Tienda'), ('Evento') 
GO

INSERT INTO [dbo].[Enemigo]
           ([Nombre]
           ,[Vida]
           ,[Ataque]
           ,[Defensa]
           ,[Descripcion]
           ,[Imagen])
     VALUES
           ('Carta', 100, 15, 7, 'Carta','https://i.postimg.cc/2yGS1Gkn/enemigo-Carta.png'), 
           ('Ficha', 100, 17, 5, 'Ficha','https://i.postimg.cc/T2zmSKy0/enemigo-Ficha.png '),
           ('Slot', 120, 17, 15, 'Slot','https://i.postimg.cc/NfNtFH17/enemigo-Slot-removebg-preview.png '),
           ('Dragon', 200, 20, 25, 'Dragon','https://i.postimg.cc/RhbPzTWz/Whats-App-Image-2025-06-22-at-9-40-48-PM-removebg-preview.png')
GO

INSERT INTO [dbo].[Objeto]
           ([Nombre]
           ,[Estadistica]
           ,[Precio])
     VALUES
           ('VidaMax',5,5), ('Armadura',5,5), ('Pocion',25,5), ('Espada',5,5)
GO

INSERT INTO [dbo].[Nivel]
           ([Id_enemigo]
           ,[Nombre]
           ,[Id_tipo_casillero])
     VALUES
           (1,'Combate1',1), (2,'Combate2',1), (3,'Combate3',1),(4,'Combate4',1)
GO
