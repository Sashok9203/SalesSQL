use [Sales]
go

--2
select concat(sl.Name,' ',sl.Surname ) as [Seller], concat(b.Name,' ',b.Surname ) as [Buyer],s.SaleDate as [Sale Date],s.SaleAmount as [Sale Amount]
from Sales as s
join Sellers as sl on sl.Id = s.SellerId
join Buyers as  b on b.Id = s.BuyerId
where s.SaleDate between '2023/07/1' and '2023/07/18'


delete from Sales
where Sales.SaleAmount = 3500

--3
select top 1  concat(sl.Name,' ',sl.Surname)as [Seller], s.SaleDate as [Last Sale Date],s.SaleAmount as [Sale Amount]
from Sales as s
join Sellers as sl on sl.Id = s.SellerId
where  concat(sl.Name,' ',sl.Surname) = 'Beverley Casino'
order by s.SaleDate desc

--5
select top 1  concat(sl.Name,' ',sl.Surname) as [Seller],sum(s.SaleAmount) as [Sale Amount]
from Sales as s
join Sellers as sl on sl.Id = s.SellerId
group by sl.Name,sl.Surname
order by 'Sale Amount' desc
