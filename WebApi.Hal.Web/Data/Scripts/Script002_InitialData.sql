INSERT INTO Breweries ([Name], City, Country, Website)
VALUES
	('Little Creatures', 'Fremantle', 'Australia', 'https://littlecreatures.com.au/'),
	('BrewDog ', 'Fraserburgh', 'Scotland', 'http://www.brewdog.com/'),
	('Nøgne Ø', 'Grimstad', 'Norway', 'http://www.nogne-o.com/'),
	('8 Wired Brewing Co.', 'Blenheim', 'New Zealand', 'http://www.8wired.co.nz/'),
	('Sierra Nevada Brewing Co.', 'Chico', 'USA', 'http://www.sierra-nevada.com/'),
	('Rogue Ales', 'Newport', 'United States', 'http://www.rogue.com/')
GO

INSERT INTO BeerStyles ([Name])
VALUES
	('American Double / Imperial IPA'),
	('American Amber / Red Ale'),
	('Winter Warmer'),
	('American Pale Ale'),
	('English Pale Ale'),
	('English Porter'),
	('Tripel'),
	('American IPA'),
	('Maibock / Helles Bock')
GO

INSERT INTO Beers ([Name], Style_Id , Brewery_Id, Abv)
VALUES
	('Hopwired IPA', 1, 4, 7.3),
	('Tall Poppy', 2, 4, 7.0),
	('Day Of The Long Shadow', 3, 1, 8.9),
	('Little Creatures Pale Ale', 4, 1, 5.2),
	('Rogers', 5, 1, 3.80),
	('Hardcore IPA', 1, 2, 9),
	('God Jul', 6, 3, 8.5),
	('Tiger Tripel', 7, 3, 9.0),
	('Nøgne Ø India Pale Ale', 8, 3, 7.5),
	('Sierra Nevada Celebration Ale', 8, 5, 6.8),
	('Sierra Nevada Pale Ale', 4, 5, 5.6),
	('Red Fox Amber Ale', 2, 6, 5.1),
	('Dead Guy Ale', 9, 6, 6.5),
	('5 A.M. Saint', 2, 2, 5.0)
GO

INSERT INTO Reviews (Beer_Id, [Title], [Content])
VALUES
	(1, 'Nasty', 'Tastes like horse piss'),
	(1, 'Awful', 'Tastes like sheep piss'),
	(2, 'Gross', 'Tastes like cat stew'),
	(3, 'Yuck', 'Tastes like buzzard puke'),
	(4, 'Phhhttt', 'Two words: dis gusting')
GO
