Create database meal_order;

Use meal_order;

CREATE LOGIN db_user WITH PASSWORD = 'user_password_000';
CREATE USER db_user FOR LOGIN db_user;
GRANT SELECT, INSERT, UPDATE ON SCHEMA::dbo TO db_user;

Create table myUser
(
    Id    NVARCHAR(50) PRIMARY KEY,
    Email NVARCHAR(100),
)

Create table myOrder
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    UserId       NVARCHAR(50) NOT NULL,
    RestaurantId NVARCHAR(50) NOT NULL,
    Status       INT              NOT NULL,
    OrderDate    DATETIME         NOT NULL,
    CreateTime   DATETIME         NOT NULL,
    MealType     int              NOT NULL
    --CONSTRAINT FK_order_user FOREIGN KEY (UserId) REFERENCES [user] (Id),
    --CONSTRAINT FK_order_restaurant FOREIGN KEY (RestaurantId) REFERENCES [user] (Id)
)

create table foodItem
(
    OrderId              uniqueidentifier NOT NULL,
    Snapshot_Index       int              NOT NULL,
    Snapshot_Description nvarchar(200),
    Snapshot_Name        nvarchar(50)     NOT NULL,
    Snapshot_Price       int              NOT NULL,
    Snapshot_Count       int              NOT NULL,
    Snapshot_CountLimit  int              NOT NULL,
    Snapshot_ImageUrl    nvarchar(200),
    Snapshot_Tags        nvarchar(200),
    Count                int              NOT NULL,
    Description          nvarchar(200),
    CONSTRAINT FK_foodItem_order FOREIGN KEY (OrderId) REFERENCES [order] (Id)
)

