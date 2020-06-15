create table ViewFilter (
	Id int primary key identity(1,1) not null,
	FilterName nvarchar(20) not null,
	FilterValue nvarchar(50) not null,
	ViewId int foreign key references UserView(Id) not null
)
create table UserView (
	Id int primary key identity(1,1) not null,
	UserId nvarchar(450) foreign key references AspNetUsers(Id) not null
)