use master
go

if db_id('Sales') is not null
begin
    drop database [Sales] 
end

create database [Sales]
go

use [Sales]
go

create  table [Buyers]
(
    [Id] int not null primary key identity,
    [Name] nvarchar(256) not null check( [Name] <> '') ,
    [Surname] nvarchar(256) not null check( [Surname] <> '')
);

create  table [Sellers]
(
    [Id] int not null primary key identity,
    [Name] nvarchar(256) not null check( [Name] <> '') ,
    [Surname] nvarchar(256) not null check( [Surname] <> '')
);

create  table [Sales]
(
    [Id] int not null primary key identity,
    [SellerId] int not null references [Sellers](Id)  on delete cascade,
    [BuyerId] int not null references [Buyers](Id) on delete cascade,
    [SaleAmount] money not null check( [SaleAmount] >= 0) default(0),
    [SaleDate] date not null check ([SaleDate] <= getdate())
);

