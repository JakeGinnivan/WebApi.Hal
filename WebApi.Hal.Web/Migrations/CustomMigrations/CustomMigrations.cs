using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Hal.Web.Migrations.CustomMigrations
{
    public static class CustomMigrations
    {
        public static void AddInitialData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"INSERT INTO Breweries (Id, [Name], City, Country, Website)
                VALUES (1, 'Little Creatures', 'Fremantle', 'Australia', 'https://littlecreatures.com.au/');
                INSERT INTO Breweries (Id, [Name], City, Country, Website)
                VALUES (2, 'BrewDog ', 'Fraserburgh', 'Scotland', 'http://www.brewdog.com/');
                INSERT INTO Breweries (Id, [Name], City, Country, Website) 
                VALUES (3, 'Nøgne Ø', 'Grimstad', 'Norway', 'http://www.nogne-o.com/');
                INSERT INTO Breweries (Id, [Name], City, Country, Website)
                VALUES (4, '8 Wired Brewing Co.', 'Blenheim', 'New Zealand', 'http://www.8wired.co.nz/');
                INSERT INTO Breweries (Id, [Name], City, Country, Website)
                VALUES (5, 'Sierra Nevada Brewing Co.', 'Chico', 'USA', 'http://www.sierra-nevada.com/');
                INSERT INTO Breweries (Id, [Name], City, Country, Website)
                VALUES (6, 'Rogue Ales', 'Newport', 'United States', 'http://www.rogue.com/');");

            migrationBuilder.Sql(
                @"INSERT INTO BeerStyles (Id, [Name]) VALUES (1, 'American Double / Imperial IPA');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (2, 'American Amber / Red Ale');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (3, 'Winter Warmer');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (4, 'American Pale Ale');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (5, 'English Pale Ale');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (6, 'English Porter');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (7, 'Tripel');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (8, 'American IPA');
                INSERT INTO BeerStyles (Id, [Name]) VALUES (9, 'Maibock / Helles Bock');");

            migrationBuilder.Sql(
                @"INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (1, 'Hopwired IPA', 1, 4, 7.3);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (2, 'Tall Poppy', 2, 4, 7.0);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (3, 'Day Of The Long Shadow', 3, 1, 8.9);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (4, 'Little Creatures Pale Ale', 4, 1, 5.2);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (5, 'Rogers', 5, 1, 3.80);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (6, 'Hardcore IPA', 1, 2, 9);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (7, 'God Jul', 6, 3, 8.5);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (8, 'Tiger Tripel', 7, 3, 9.0);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (9, 'Nøgne Ø India Pale Ale', 8, 3, 7.5);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (10, 'Sierra Nevada Celebration Ale', 8, 5, 6.8);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (11, 'Sierra Nevada Pale Ale', 4, 5, 5.6);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (12, 'Red Fox Amber Ale', 2, 6, 5.1);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (13, 'Dead Guy Ale', 9, 6, 6.5);
                INSERT INTO Beers (Id, [Name], Style_Id , Brewery_Id, Abv)
                VALUES (14, '5 A.M. Saint', 2, 2, 5.0);");

            migrationBuilder.Sql(
                @"INSERT INTO Reviews (Beer_Id, [Title], [Content])
                VALUES (1, 'Nasty', 'Tastes like horse piss');
                INSERT INTO Reviews (Beer_Id, [Title], [Content])
                VALUES (1, 'Awful', 'Tastes like sheep piss');
                INSERT INTO Reviews (Beer_Id, [Title], [Content])
                VALUES (2, 'Gross', 'Tastes like cat stew');
                INSERT INTO Reviews (Beer_Id, [Title], [Content])
                VALUES (3, 'Yuck', 'Tastes like buzzard puke');
                INSERT INTO Reviews (Beer_Id, [Title], [Content])
                VALUES (4, 'Phhhttt', 'Two words: dis gusting');");
        }
    }
}