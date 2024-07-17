CREATE DATABASE [ds-db];
GO
USE [ds-db]

CREATE TABLE [dbo].[Supplier] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);
GO
CREATE TABLE [dbo].[Item] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL
);
GO
CREATE TABLE [dbo].[PurchaseOrder] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ReferenceId NVARCHAR(255) NOT NULL,
    OrderNumber NVARCHAR(255) NOT NULL,
    SupplierId INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    ExpectedDate DATETIME NOT NULL,
    Remark NVARCHAR(MAX),
	IsDelete [bit] NOT NULL,
    FOREIGN KEY (SupplierId) REFERENCES [dbo].[Supplier](Id)
);
GO
CREATE TABLE [dbo].[OrderDetail] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ItemId INT NOT NULL,
    Quantity INT NOT NULL,
    Rate DECIMAL(18,2) NOT NULL,
	IsDelete [bit] NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES [dbo].[PurchaseOrder](Id),
    FOREIGN KEY (ItemId) REFERENCES [dbo].[Item](Id)
);
GO
CREATE TYPE dbo.OrderDetailType AS TABLE
(
	Id INT,
    ItemId INT,
    Quantity INT,
    Rate DECIMAL(18,2)
);
GO

-- Inserting dummy data into Supplier table
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Acme Supplies');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Global Distributors');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Techno Goods');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Prime Vendors');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Innovative Imports');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Superior Supplies');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Universal Providers');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Alpha Distributions');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Elite Supply Co.');
INSERT INTO [dbo].[Supplier] (Name) VALUES ('Trusted Traders');

-- Inserting dummy data into Item table
INSERT INTO [dbo].[Item] (Name) VALUES ('Laptop');
INSERT INTO [dbo].[Item] (Name) VALUES ('Smartphone');
INSERT INTO [dbo].[Item] (Name) VALUES ('Desk Chair');
INSERT INTO [dbo].[Item] (Name) VALUES ('Monitor');
INSERT INTO [dbo].[Item] (Name) VALUES ('Keyboard');
INSERT INTO [dbo].[Item] (Name) VALUES ('Mouse');
INSERT INTO [dbo].[Item] (Name) VALUES ('Printer');
INSERT INTO [dbo].[Item] (Name) VALUES ('Scanner');
INSERT INTO [dbo].[Item] (Name) VALUES ('Desk Lamp');
INSERT INTO [dbo].[Item] (Name) VALUES ('USB Drive');

GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetItems]
    @searchKeyword NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Item
    WHERE (@searchKeyword IS NULL OR Name LIKE '%' + @searchKeyword + '%')
    ORDER BY Name;
END
GO

CREATE PROCEDURE [dbo].[GetSuppliers]
    @searchKeyword NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Supplier
    WHERE (@searchKeyword IS NULL OR Name LIKE '%' + @searchKeyword + '%')
    ORDER BY Name;
END
GO

CREATE PROCEDURE [dbo].[GetPagedPurchaseOrders]
    @PageIndex INT,
    @PageSize INT,
    @SearchTerm NVARCHAR(100),
    @SortColumn NVARCHAR(50),
    @SortDirection NVARCHAR(4)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageIndex - 1) * @PageSize;

    -- Temp table to hold the results
    CREATE TABLE #TempResults
    (
        Id INT,
        ReferenceId NVARCHAR(50),
        OrderNumber NVARCHAR(50),
        SupplierId INT,
        OrderDate DATETIME,
        ExpectedDate DATETIME,
        Remark NVARCHAR(255)
    );

    -- Insert paged data into temp table
    INSERT INTO #TempResults
    SELECT Id, ReferenceId, OrderNumber, SupplierId, OrderDate, ExpectedDate, Remark
    FROM PurchaseOrder
    WHERE (@SearchTerm IS NULL OR ReferenceId LIKE '%' + @SearchTerm + '%' OR OrderNumber LIKE '%' + @SearchTerm + '%')
	AND IsDelete <> 1
    ORDER BY 
        CASE WHEN @SortDirection = 'asc' THEN 
            CASE @SortColumn
                WHEN 'ReferenceId' THEN ReferenceId
                WHEN 'OrderNumber' THEN OrderNumber
                WHEN 'SupplierId' THEN SupplierId
                WHEN 'OrderDate' THEN OrderDate
                WHEN 'ExpectedDate' THEN ExpectedDate
                WHEN 'Remark' THEN Remark
            END
        END ASC,
        CASE WHEN @SortDirection = 'desc' THEN 
            CASE @SortColumn
                WHEN 'ReferenceId' THEN ReferenceId
                WHEN 'OrderNumber' THEN OrderNumber
                WHEN 'SupplierId' THEN SupplierId
                WHEN 'OrderDate' THEN OrderDate
                WHEN 'ExpectedDate' THEN ExpectedDate
                WHEN 'Remark' THEN Remark
            END
        END DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    -- Select paged data
    SELECT * FROM #TempResults;

    -- Select total record count
    SELECT COUNT(*)
    FROM PurchaseOrder
    WHERE (@SearchTerm IS NULL OR ReferenceId LIKE '%' + @SearchTerm + '%' OR OrderNumber LIKE '%' + @SearchTerm + '%')
	AND IsDelete <> 1;
    DROP TABLE #TempResults;
END

GO

CREATE PROCEDURE [dbo].[GetPurchaseOrderById]
    @PurchaseOrderId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Retrieve PurchaseOrder details
    SELECT
        po.Id AS Id,
        po.ReferenceId,
        po.OrderNumber,
        po.SupplierId,
		s.[Name] SupplierName,
        po.OrderDate,
        po.ExpectedDate,
        po.Remark,
        (SELECT
             od.Id AS Id,
             od.ItemId,
			 i.Name ItemName,
             od.Quantity,
             od.Rate
         FROM OrderDetail od
		 JOIN Item i ON od.ItemId = i.Id
         WHERE od.OrderId = po.Id
         FOR JSON PATH) AS OrderDetails
    FROM PurchaseOrder po
	JOIN Supplier s ON po.SupplierId = s.Id
    WHERE po.Id = @PurchaseOrderId AND po.IsDelete <> 1
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
END;

GO

CREATE PROCEDURE [dbo].[InsertOrUpdatePurchaseOrderAndDetails]
    @PurchaseOrderId INT,
    @ReferenceId NVARCHAR(255),
    @OrderNumber NVARCHAR(255),
    @SupplierId INT,
    @OrderDate DATETIME,
    @ExpectedDate DATETIME,
    @Remark NVARCHAR(MAX),
    @OrderDetailsType dbo.OrderDetailType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM [dbo].[PurchaseOrder] WHERE ReferenceId = @ReferenceId)
    BEGIN
        RAISERROR ('ReferenceId already exists.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM [dbo].[PurchaseOrder] WHERE Id = @PurchaseOrderId)
    BEGIN
        UPDATE [dbo].[PurchaseOrder]
        SET ReferenceId = @ReferenceId,
            OrderNumber = @OrderNumber,
            SupplierId = @SupplierId,
            OrderDate = @OrderDate,
            ExpectedDate = @ExpectedDate,
            Remark = @Remark
        WHERE Id = @PurchaseOrderId;
    END
    ELSE
    BEGIN
        -- Insert new PurchaseOrder
        INSERT INTO [dbo].[PurchaseOrder] (ReferenceId, OrderNumber, SupplierId, OrderDate, ExpectedDate, Remark, IsDelete)
        VALUES (@ReferenceId, @OrderNumber, @SupplierId, @OrderDate, @ExpectedDate, @Remark, 0);

        SET @PurchaseOrderId = SCOPE_IDENTITY(); 
    END

    -- Insert or update OrderDetails
    MERGE [dbo].[OrderDetail] AS target
    USING @OrderDetailsType AS source
    ON target.Id = source.Id
    WHEN MATCHED THEN 
        UPDATE SET Quantity = source.Quantity, Rate = source.Rate, ItemId = source.ItemId
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (OrderId, ItemId, Quantity, Rate, IsDelete) 
        VALUES (@PurchaseOrderId, source.ItemId, source.Quantity, source.Rate, 0)
    WHEN NOT MATCHED BY SOURCE AND target.OrderId = @PurchaseOrderId THEN 
        DELETE;
END
GO

CREATE PROCEDURE [dbo].[DeletePurchaseOrder]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PurchaseOrder SET IsDelete = 1 WHERE Id = @Id;
	UPDATE OrderDetail SET IsDelete = 1 WHERE OrderId = @Id;
END
GO