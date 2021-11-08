create table Persons
	(
		ID int primary key,
		FirstName varchar(50),
		LastName varchar(50),
		Age int not null,
		Sex varchar(20) not null, --male, female -- dropdown
		Relation varchar(50) null, -- dropdown
		Occasion varchar(100) null, -- dropdown
		Interests varchar(255) null, --seperated by comma -- dropdown
		PriceLevel varchar(50) null, --High, medium, low -- dropdown
		PsycoType varchar(255) null, --introvert, extravert -- dropdown
		Present varchar(100) null -- predicted present
	)