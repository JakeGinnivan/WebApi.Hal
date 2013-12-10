create table BeerStyles
(
	Id int identity PRIMARY KEY,
	Name nvarchar(100) NOT NULL,
	[Description] ntext NULL
)
go

create table Breweries
(
	Id int identity PRIMARY KEY,
	Name nvarchar(100) NOT NULL,
	[Address] nvarchar(255) NULL,
	City nvarchar(100) NULL,
	Country nvarchar(100) NULL,
	Phone nvarchar(100) NULL,
	Website nvarchar(100) NULL,
	Twitter nvarchar(100) NULL,
	Notes ntext NULL
)
go

create table Beers
(
	Id int identity PRIMARY KEY,
	Name nvarchar(100) NOT NULL,
	Style_Id int FOREIGN KEY REFERENCES BeerStyles(Id) NULL,
	Brewery_Id int FOREIGN KEY REFERENCES Breweries(Id) NULL,
	Abv decimal(3,2) NULL
)
go

create table Reviews
(
	Id int identity PRIMARY KEY,
	Beer_Id int FOREIGN KEY REFERENCES Beers(Id) NULL,
	[Title] ntext NULL,
	[Content] ntext NULL
)
go