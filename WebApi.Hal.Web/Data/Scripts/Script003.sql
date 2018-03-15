create table Reviews
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	Beer_Id int NULL,
	[Title] ntext NULL,
	[Content] ntext NULL,
	FOREIGN KEY (Beer_Id) REFERENCES Beers(Id)
)
go

INSERT INTO Reviews (Beer_Id, [Title], [Content])
VALUES
	(1, 'Nasty', 'Tastes like horse piss'),
	(1, 'Awful', 'Tastes like sheep piss'),
	(2, 'Gross', 'Tastes like cat stew'),
	(3, 'Yuck', 'Tastes like buzzard puke'),
	(4, 'Phhhttt', 'Two words: dis gusting')
GO
