--CREATE TABLE Startup (
--Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
--Name NVARCHAR(80),
--Theme NVARCHAR(120),
--TechArea NVARCHAR(120),
--Summary	NVARCHAR(200),
--Country NVARCHAR(50),
--City NVARCHAR(50),
--DateAdded DATE,
--Comments NVARCHAR(200),
--InterestedPartners NVARCHAR(120),
--DateRemoved DATE,
--[Status] NVARCHAR(15) 
--);

--GO

--CREATE TABLE Favorites(
--Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
--StartupId INT FOREIGN KEY REFERENCES Startup(Id),
--UserId NVARCHAR(450) FOREIGN KEY REFERENCES dbo.AspNetUsers(Id)
--);


