--
-- Script
--

ALTER TABLE Offers.Offer
  ADD SizeInfo VARCHAR(255)
GO

UPDATE Offers.Offer
SET Offer.SizeInfo = (SELECT Product.SizeInfo
                      FROM Offers.Product
                      WHERE Product.Id = Offer.ProductId)
GO

ALTER TABLE Offers.Offer
  ALTER COLUMN SizeInfo VARCHAR(255) NOT NULL
GO

ALTER TABLE Offers.Product
  DROP COLUMN SizeInfo;
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2018-02-11_13-02-26_move_sizeinfo_to_offer');