use master
go
 Create Database CarCareDB
go
use CarCareDB

Create TABLE Customer(
	CustomerID int primary key IDENTITY,
	Name varchar(50) NOT NULL,	
	Phone nvarchar(20) NOT NULL,
	CarNumber varchar(20) NOT NULL
	)
go

use CarCareDB
CREATE TABLE ServiceDetails	(
	ServiceDetailsID int IDENTITY NOT NULL,
	CustomerID int references Customer(CustomerID),
	ServiceName varchar(50) NOT NULL,
	ServiceDate datetime NOT NULL,
	Price decimal(10, 2) NOT NULL,
	ImagePath NVARCHAR(MAX)  -- Path to product image file
	)
go

-- Create Service table
CREATE TABLE Service (
    ServiceID INT PRIMARY KEY,
    Name NVARCHAR(100),
    Price DECIMAL(10, 2)
);

INSERT INTO Service (ServiceID, Name, Price)
VALUES
    (1, 'Oil Change', 50.00),
    (2, 'Tire Rotation',30.00),
    (3, 'Brake Inspection', 40.00),
    (4, 'Engine Tune-Up',  100.00),
    (5, 'Wheel Alignment', 60.00),
	(6, 'Wash', 60.00);

	go

select * from Customer
select * from ServiceDetails
select * from Service
--Truncate table Customer
--Drop table ServiceDetails
--ALTER TABLE ServiceDetails ADD Image VARBINARY(MAX);