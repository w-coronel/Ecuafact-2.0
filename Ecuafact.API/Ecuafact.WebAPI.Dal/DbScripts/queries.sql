Use EcuafactExpress
Go

select * from Issuer
select * from Product
select * from Contributor
select * from RequestSession

-- truncate table Product
-- truncate table DocumentSequential

select * from ICERate
select * from IVARate
select * from IdentificationType
select * from PaymentMethod
select * from ProductType
select * from ContributorType
select * from DocumentType

select * from Document
select * from InvoiceInfo
Select * from Payment
Select * from Tax
Select * from TotalTax
Select * from AdditionalField
select * from DocumentDetail
select * from DocumentSequential


EXEC [dbo].[SpSearchProducts] @searchTerm = N'SERV', @issuerId=1


