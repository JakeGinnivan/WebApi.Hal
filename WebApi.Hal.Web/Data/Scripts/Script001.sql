create table BeerStyles
(
	Id int identity PRIMARY KEY,
	Name nvarchar(100)
)
go

create table Breweries
(
	Id int identity PRIMARY KEY,
	Name nvarchar(100)
)
go

create table Beers
(
	Id int identity PRIMARY KEY,
	Name nvarchar(100),
	StyleId int FOREIGN KEY REFERENCES BeerStyles(Id),
	BreweryId int FOREIGN KEY REFERENCES Breweries(Id)
)
go