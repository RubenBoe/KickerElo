
CREATE TABLE dbo.clients (
    Id int IDENTITY(1,1) PRIMARY KEY CLUSTERED,
    ClientName nvarchar(200) NOT NULL,
    CreationDate DATETIME NOT NULL,
    ClientToken NVARCHAR(200) NOT NULL
)

GO

CREATE TABLE dbo.seasons (
    SeasonID NVARCHAR(200) PRIMARY KEY CLUSTERED,
    ClientID INT NOT NULL
        REFERENCES dbo.clients(Id),
    SeasonNumber INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL
)

GO

CREATE TABLE dbo.players (
    PlayerID NVARCHAR(200) PRIMARY KEY CLUSTERED,
    ClientID INT NOT NULL
        REFERENCES dbo.clients(Id),
    Nickname NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(1000) NOT NULL
)

GO

CREATE TABLE dbo.playerElo (
    PlayerID NVARCHAR(200) NOT NULL
        REFERENCES dbo.players(PlayerID),
    SeasonID NVARCHAR(200) NOT NULL
        REFERENCES dbo.seasons(SeasonID),
    EloNumber int NOT NULL,
    LastUpdated DATETIME NOT NULL,
    CONSTRAINT UC_PlayerID_SeasonID UNIQUE (PlayerID, SeasonID)
)

GO

CREATE TABLE dbo.games (
    GameID NVARCHAR(200) PRIMARY KEY CLUSTERED,
    SeasonID NVARCHAR(200) NOT NULL 
        REFERENCES dbo.seasons(SeasonID),
    ClientID int NOT NULL
        REFERENCES dbo.clients(Id),
    [Date] DATETIME NOT NULL DEFAULT GETUTCDATE()
)

GO

CREATE TABLE dbo.playerGame (
    GameID NVARCHAR(200) NOT NULL
        REFERENCES dbo.games(GameID),
    PlayerID NVARCHAR(200) NOT NULL
        REFERENCES dbo.players(PlayerID),
    Team INT NOT NULL,
    Points INT NOT NULL,
    EloGain INT NOT NULL,
    CONSTRAINT UC_PlayerID_GameID UNIQUE (PlayerID, GameID)
)

GO

