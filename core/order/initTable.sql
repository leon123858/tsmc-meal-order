Create database meal_order;

Use meal_order;

CREATE LOGIN db_user WITH PASSWORD = 'user_password_000';
CREATE USER db_user FOR LOGIN db_user;
GRANT SELECT, INSERT, UPDATE ON SCHEMA::dbo TO db_user;

Create table [user]
(
    Id    UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(100),
)

Create table [order]
(
    Id           UNIQUEIDENTIFIER PRIMARY KEY,
    UserId       UNIQUEIDENTIFIER NOT NULL,
    RestaurantId UNIQUEIDENTIFIER NOT NULL,
    Status       INT              NOT NULL,
    OrderDate    DATETIME         NOT NULL,
    CreateTime   DATETIME         NOT NULL,
    CONSTRAINT FK_order_user FOREIGN KEY (UserId) REFERENCES [user] (Id),
    CONSTRAINT FK_order_restaurant FOREIGN KEY (RestaurantId) REFERENCES [user] (Id)
)

Create table foodItem
(
    OrderId     UNIQUEIDENTIFIER NOT NULL,
    Description NVARCHAR(200),
    Name        NVARCHAR(50)     NOT NULL,
    Price       INT              NOT NULL,
    Count       INT              NOT NULL,
    ImageUrl    NVARCHAR(200),
    Tags        NVARCHAR(200),
    CONSTRAINT FK_foodItem_order FOREIGN KEY (OrderId) REFERENCES [order] (Id)
)