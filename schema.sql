-- Tables disponibles pour le test
-- Utilisez ces définitions pour écrire vos requêtes ADO.NET.

CREATE TABLE [dbo].[Member] (
    MemberId           INT            NOT NULL PRIMARY KEY,
    FirstName          NVARCHAR(100)  NOT NULL,
    LastName           NVARCHAR(100)  NOT NULL,
    PhoneNumber        NVARCHAR(20)   NULL,
    BirthDate          DATETIME       NULL,
    City               NVARCHAR(100)  NULL,
    Country            NVARCHAR(100)  NULL,
    LoyaltyPoints      INT            NOT NULL DEFAULT 0,
    MainEmailAddressId INT            NULL,        -- FK vers MemberEmail
    EnrolledAt         DATETIME       NOT NULL
);

CREATE TABLE [dbo].[MemberEmail] (
    MemberEmailId INT            NOT NULL PRIMARY KEY,
    MemberId      INT            NOT NULL,
    EmailAddress  NVARCHAR(255)  NOT NULL
);
