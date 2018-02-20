--
-- Script
--

ALTER TABLE Offers.Offer
  ADD BasePrice VARCHAR(255)
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName])
VALUES ('2018-02-20_19-12-45_add_baseprice_to_offers');