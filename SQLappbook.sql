CREATE DATABASE appbook;
GO

USE appbook;
GO
CREATE TABLE Publishers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL
);
GO
CREATE TABLE Authors
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL
);
GO
CREATE TABLE Books
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    IsRead BIT NOT NULL,
    DateRead DATETIME NULL,
    Rate INT NULL,
    Genre NVARCHAR(100) NULL,
    CoverUrl NVARCHAR(500) NULL,
    DateAdded DATETIME NOT NULL,
    PublisherId INT NOT NULL,

    CONSTRAINT FK_Books_Publishers
        FOREIGN KEY (PublisherId)
        REFERENCES Publishers(Id)
);
GO
CREATE TABLE Books_Authors
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookId INT NOT NULL,
    AuthorId INT NOT NULL,

    CONSTRAINT FK_BookAuthor_Book
        FOREIGN KEY (BookId)
        REFERENCES Books(Id),

    CONSTRAINT FK_BookAuthor_Author
        FOREIGN KEY (AuthorId)
        REFERENCES Authors(Id)
);
GO
INSERT INTO Publishers (Name)
VALUES
(N'Nhà xuất bản Kim Đồng'),
(N'Nhà xuất bản Giáo dục');

INSERT INTO Authors (FullName)
VALUES
(N'Nguyễn Nhật Ánh'),
(N'Nam Cao'),
(N'Nguyễn Du');
USE appbook;
GO

SELECT * FROM Publishers;
SELECT * FROM Authors;
SELECT * FROM Books;
SELECT * FROM Books_Authors;
SELECT * FROM Books;
SELECT * FROM Books_Authors;