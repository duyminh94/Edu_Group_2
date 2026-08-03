-- =========================================================
-- Blogging Platform - Edu_Group_2
-- TAO BANG + DU LIEU MAU (chay doc lap, khong can EF Core)
--
-- CACH DUNG - chay theo dung thu tu:
--   Buoc 1: mo file nay trong SSMS / Azure Data Studio, bam F5
--           (tao database -> xoa bang cu neu co -> tao 12 bang -> chen du lieu mau)
--   Buoc 2: ve terminal, chay:  dotnet build && dotnet run
--
-- KHONG can chay "dotnet ef database update" nua.
-- Phan 4 phia duoi da danh dau migration la da chay roi.
--
-- Luu y: script XOA HET du lieu cu moi lan chay. Chi dung cho moi truong hoc/dev.
-- =========================================================


-- =========================================================
-- 0. TAO DATABASE
-- =========================================================

if db_id('BlogPlatformDb') is null
    create database BlogPlatformDb;
go

use BlogPlatformDb;
go


-- =========================================================
-- 1. XOA BANG CU
-- Xoa theo thu tu nguoc voi khoa ngoai: bang con truoc, bang cha sau
-- =========================================================

drop table if exists [BlogSettings];
drop table if exists [Bookmarks];
drop table if exists [PostLikes];
drop table if exists [PostTags];
drop table if exists [PostViews];
drop table if exists [MediaFiles];
drop table if exists [Comments];
drop table if exists [Posts];
drop table if exists [Categories];
drop table if exists [Tags];
drop table if exists [Users];
drop table if exists [Roles];
go


-- =========================================================
-- 2. TAO BANG
-- =========================================================

-- Vai tro nguoi dung: Admin / Author / Reader
create table [Roles] (
    Id          int identity(1,1)   not null,
    Name        nvarchar(50)        not null,
    Description nvarchar(200)       null,
    constraint PK_Roles primary key (Id)
);
go

create unique index IX_Roles_Name on [Roles] (Name);
go


-- Danh muc bai viet
create table [Categories] (
    Id          int identity(1,1)   not null,
    Name        nvarchar(100)       not null,
    Slug        nvarchar(120)       not null,
    Description nvarchar(300)       null,
    constraint PK_Categories primary key (Id)
);
go

create unique index IX_Categories_Name on [Categories] (Name);
create unique index IX_Categories_Slug on [Categories] (Slug);
go


-- The (tag) gan cho bai viet
create table [Tags] (
    Id   int identity(1,1) not null,
    Name nvarchar(50)      not null,
    Slug nvarchar(60)      not null,
    constraint PK_Tags primary key (Id)
);
go

create unique index IX_Tags_Name on [Tags] (Name);
create unique index IX_Tags_Slug on [Tags] (Slug);
go


-- Tai khoan nguoi dung
create table [Users] (
    Id           int identity(1,1) not null,
    UserName     nvarchar(50)      not null,
    Email        nvarchar(256)     not null,
    PasswordHash nvarchar(255)     not null,
    DisplayName  nvarchar(100)     not null,
    AvatarUrl    nvarchar(500)     null,
    Bio          nvarchar(500)     null,
    RoleId       int               not null,
    IsLocked     bit               not null default 0,
    CreatedAt    datetime2         not null default sysutcdatetime(),
    constraint PK_Users primary key (Id),
    -- Restrict: khong cho xoa Role khi con user dang dung
    constraint FK_Users_Roles_RoleId foreign key (RoleId)
        references [Roles] (Id) on delete no action
);
go

create unique index IX_Users_Email    on [Users] (Email);
create unique index IX_Users_UserName on [Users] (UserName);
create index        IX_Users_RoleId   on [Users] (RoleId);
go


-- Cau hinh giao dien rieng cua tung tac gia (quan he 1-1 voi Users)
create table [BlogSettings] (
    UserId       int           not null,
    ThemeName    nvarchar(50)  not null,
    PrimaryColor nvarchar(7)   not null,
    FontFamily   nvarchar(100) not null,
    LogoUrl      nvarchar(500) null,
    Tagline      nvarchar(200) null,
    UpdatedAt    datetime2     not null default sysutcdatetime(),
    constraint PK_BlogSettings primary key (UserId),
    constraint FK_BlogSettings_Users_UserId foreign key (UserId)
        references [Users] (Id) on delete cascade
);
go


-- Bai viet. Status: 0 = Draft, 1 = Published, 2 = Unpublished
create table [Posts] (
    Id               int identity(1,1) not null,
    Title            nvarchar(200)     not null,
    Slug             nvarchar(220)     not null,
    Summary          nvarchar(500)     null,
    Content          nvarchar(max)     not null,
    FeaturedImageUrl nvarchar(500)     null,
    CategoryId       int               null,
    AuthorId         int               not null,
    Status           tinyint           not null default 0,
    PublishedAt      datetime2         null,
    ViewCount        int               not null default 0,
    LikeCount        int               not null default 0,
    CommentCount     int               not null default 0,
    CreatedAt        datetime2         not null default sysutcdatetime(),
    UpdatedAt        datetime2         not null default sysutcdatetime(),
    constraint PK_Posts primary key (Id),
    -- Xoa danh muc thi bai viet van con, chi bo trong CategoryId
    constraint FK_Posts_Categories_CategoryId foreign key (CategoryId)
        references [Categories] (Id) on delete set null,
    constraint FK_Posts_Users_AuthorId foreign key (AuthorId)
        references [Users] (Id) on delete no action
);
go

create unique index IX_Posts_Slug              on [Posts] (Slug);
create index        IX_Posts_CategoryId        on [Posts] (CategoryId);
create index        IX_Posts_AuthorId_Status   on [Posts] (AuthorId, Status);
create index        IX_Posts_Status_PublishedAt on [Posts] (Status, PublishedAt);
go


-- Binh luan. Status: 0 = Pending, 1 = Approved, 2 = Rejected, 3 = Flagged
-- ParentCommentId null = binh luan goc, co gia tri = tra loi binh luan khac
create table [Comments] (
    Id              int identity(1,1) not null,
    PostId          int               not null,
    UserId          int               not null,
    ParentCommentId int               null,
    Content         nvarchar(2000)    not null,
    Status          tinyint           not null default 0,
    CreatedAt       datetime2         not null default sysutcdatetime(),
    UpdatedAt       datetime2         not null default sysutcdatetime(),
    constraint PK_Comments primary key (Id),
    -- Tu tham chieu chinh no, phai dung no action de tranh vong lap cascade
    constraint FK_Comments_Comments_ParentCommentId foreign key (ParentCommentId)
        references [Comments] (Id) on delete no action,
    constraint FK_Comments_Posts_PostId foreign key (PostId)
        references [Posts] (Id) on delete cascade,
    constraint FK_Comments_Users_UserId foreign key (UserId)
        references [Users] (Id) on delete no action
);
go

create index IX_Comments_ParentCommentId on [Comments] (ParentCommentId);
create index IX_Comments_UserId          on [Comments] (UserId);
create index IX_Comments_PostId_Status   on [Comments] (PostId, Status);
go


-- Bang trung gian noi Posts voi Tags (quan he nhieu - nhieu)
create table [PostTags] (
    PostId int not null,
    TagId  int not null,
    constraint PK_PostTags primary key (PostId, TagId),
    constraint FK_PostTags_Posts_PostId foreign key (PostId)
        references [Posts] (Id) on delete cascade,
    constraint FK_PostTags_Tags_TagId foreign key (TagId)
        references [Tags] (Id) on delete cascade
);
go

create index IX_PostTags_TagId on [PostTags] (TagId);
go


-- Luot thich bai viet
create table [PostLikes] (
    PostId    int       not null,
    UserId    int       not null,
    CreatedAt datetime2 not null default sysutcdatetime(),
    constraint PK_PostLikes primary key (PostId, UserId),
    constraint FK_PostLikes_Posts_PostId foreign key (PostId)
        references [Posts] (Id) on delete cascade,
    constraint FK_PostLikes_Users_UserId foreign key (UserId)
        references [Users] (Id) on delete no action
);
go

create index IX_PostLikes_UserId on [PostLikes] (UserId);
go


-- Bai viet duoc luu lai de doc sau
create table [Bookmarks] (
    PostId    int       not null,
    UserId    int       not null,
    CreatedAt datetime2 not null default sysutcdatetime(),
    constraint PK_Bookmarks primary key (PostId, UserId),
    constraint FK_Bookmarks_Posts_PostId foreign key (PostId)
        references [Posts] (Id) on delete cascade,
    constraint FK_Bookmarks_Users_UserId foreign key (UserId)
        references [Users] (Id) on delete no action
);
go

create index IX_Bookmarks_UserId on [Bookmarks] (UserId);
go


-- File anh / tai lieu nguoi dung tai len
create table [MediaFiles] (
    Id               int identity(1,1) not null,
    OriginalFileName nvarchar(255)     not null,
    StoredFileName   nvarchar(100)     not null,
    ContentType      nvarchar(100)     not null,
    SizeBytes        bigint            not null,
    PostId           int               null,
    UploadedById     int               not null,
    UploadedAt       datetime2         not null default sysutcdatetime(),
    constraint PK_MediaFiles primary key (Id),
    constraint FK_MediaFiles_Posts_PostId foreign key (PostId)
        references [Posts] (Id) on delete cascade,
    constraint FK_MediaFiles_Users_UploadedById foreign key (UploadedById)
        references [Users] (Id) on delete no action
);
go

create unique index IX_MediaFiles_StoredFileName on [MediaFiles] (StoredFileName);
create index        IX_MediaFiles_PostId         on [MediaFiles] (PostId);
create index        IX_MediaFiles_UploadedById   on [MediaFiles] (UploadedById);
go


-- Lich su luot xem, dung de thong ke. UserId null = khach chua dang nhap
create table [PostViews] (
    Id       bigint identity(1,1) not null,
    PostId   int                  not null,
    UserId   int                  null,
    IpHash   nvarchar(64)         not null,
    ViewedAt datetime2            not null default sysutcdatetime(),
    constraint PK_PostViews primary key (Id),
    constraint FK_PostViews_Posts_PostId foreign key (PostId)
        references [Posts] (Id) on delete cascade,
    constraint FK_PostViews_Users_UserId foreign key (UserId)
        references [Users] (Id) on delete no action
);
go

create index IX_PostViews_PostId_ViewedAt on [PostViews] (PostId, ViewedAt);
create index IX_PostViews_UserId          on [PostViews] (UserId);
go


-- =========================================================
-- 3. DU LIEU MAU
-- =========================================================

-- 3 vai tro, bat buoc phai co moi dang ky tai khoan duoc
insert into [Roles] (Name, Description) values
(N'Admin', N'Quan tri toan he thong'),
(N'Author', N'Viet bai, duyet binh luan tren bai cua minh'),
(N'Reader', N'Doc bai, binh luan, thich va luu bai');

-- Tai khoan mau. Mat khau ca 4 tai khoan deu la: Admin@123
-- Chuoi PasswordHash duoi day sinh bang thu vien BCrypt.Net-Next 4.2.0
insert into [Users] (UserName, Email, PasswordHash, DisplayName, Bio, RoleId) values
(N'admin',  N'admin@blog.local',  N'$2a$11$fbtobL02kmcj1lbjczAyP.kSBK0gqaxfj0Cc39m9Amx8NEdOccQ8e', N'Quan tri vien', N'Tai khoan quan tri he thong', 1),
(N'minh',   N'minh@blog.local',   N'$2a$11$fbtobL02kmcj1lbjczAyP.kSBK0gqaxfj0Cc39m9Amx8NEdOccQ8e', N'Duy Minh',      N'Fresher developer, thich viet ve lap trinh', 2),
(N'lan',    N'lan@blog.local',    N'$2a$11$fbtobL02kmcj1lbjczAyP.kSBK0gqaxfj0Cc39m9Amx8NEdOccQ8e', N'Ngoc Lan',      N'Yeu thich thiet ke giao dien', 2),
(N'hoa',    N'hoa@blog.local',    N'$2a$11$fbtobL02kmcj1lbjczAyP.kSBK0gqaxfj0Cc39m9Amx8NEdOccQ8e', N'Thanh Hoa',     N'Doc gia thuong xuyen', 3);

-- Cau hinh giao dien rieng cua 2 tac gia
insert into [BlogSettings] (UserId, ThemeName, PrimaryColor, FontFamily, Tagline) values
(2, N'light',   N'#2563eb', N'Be Vietnam Pro', N'Ghi chep tren duong hoc lap trinh'),
(3, N'minimal', N'#0e8a16', N'Outfit',         N'Goc nho ve thiet ke');

insert into [Categories] (Name, Slug, Description) values
(N'Lap trinh',  N'lap-trinh',  N'Ngon ngu va ky thuat lap trinh'),
(N'Cong nghe',  N'cong-nghe',  N'Tin tuc va xu huong cong nghe'),
(N'Thiet ke',   N'thiet-ke',   N'Giao dien, trai nghiem nguoi dung'),
(N'Hoc tap',    N'hoc-tap',    N'Kinh nghiem va phuong phap hoc'),
(N'Doi song',   N'doi-song',   N'Chuyen doi thuong, ky nang song');

insert into [Tags] (Name, Slug) values
(N'ASP.NET Core',     N'aspnet-core'),
(N'C#',               N'csharp'),
(N'SQL Server',       N'sql-server'),
(N'Entity Framework', N'entity-framework'),
(N'JavaScript',       N'javascript'),
(N'CSS',              N'css'),
(N'Bao mat',          N'bao-mat'),
(N'Kinh nghiem',      N'kinh-nghiem');

-- Bai viet mau. Status: 0 = Draft, 1 = Published, 2 = Unpublished
insert into [Posts] (Title, Slug, Summary, Content, CategoryId, AuthorId, Status, PublishedAt, ViewCount, LikeCount, CommentCount) values
(N'Bat dau voi ASP.NET Core MVC', N'bat-dau-voi-aspnet-core-mvc',
 N'Huong dan dung project MVC dau tien tu con so khong',
 N'<p>ASP.NET Core MVC chia ung dung thanh ba phan: Model, View va Controller.</p>',
 1, 2, 1, '2026-07-10', 120, 8, 2),

(N'Hieu ve Entity Framework Core', N'hieu-ve-entity-framework-core',
 N'Code First, migration va cach EF Core sinh bang tu class C#',
 N'<p>EF Core cho phep viet class C# roi tu sinh ra bang trong database.</p>',
 1, 2, 1, '2026-07-15', 85, 5, 1),

(N'Chon mau va font cho website', N'chon-mau-va-font-cho-website',
 N'Vai nguyen tac chon bang mau va font chu de trang web de nhin',
 N'<p>Do tuong phan giua chu va nen nen dat toi thieu 4.5:1.</p>',
 3, 3, 1, '2026-07-20', 64, 12, 0),

(N'Ghi chu ve bao mat web', N'ghi-chu-ve-bao-mat-web',
 N'SQL injection, XSS va cach phong tranh',
 N'<p>Bai viet dang soan, chua dang.</p>',
 2, 2, 0, null, 0, 0, 0);

insert into [PostTags] (PostId, TagId) values
(1, 1), (1, 2),
(2, 1), (2, 4), (2, 3),
(3, 6),
(4, 7);

-- Binh luan. Status: 0 = Pending, 1 = Approved, 2 = Rejected, 3 = Flagged
-- ParentCommentId null nghia la binh luan goc, co gia tri nghia la tra loi
insert into [Comments] (PostId, UserId, ParentCommentId, Content, Status) values
(1, 4, null, N'Bai viet de hieu, cam on ban nhe', 1),
(1, 2, 1,    N'Cam on ban da doc', 1),
(2, 4, null, N'Phan migration minh van con hoi roi', 1),
(3, 4, null, N'Cho minh hoi font nay tai o dau vay', 0);

insert into [PostLikes] (PostId, UserId) values
(1, 3), (1, 4),
(2, 4),
(3, 2), (3, 4);

insert into [Bookmarks] (PostId, UserId) values
(1, 4),
(3, 4);
go


-- =========================================================
-- 4. DANH DAU MIGRATION DA CHAY
-- Ghi lai lich su migration de sau nay chay "dotnet ef database update"
-- EF Core khong tao lai 12 bang nay nua (tranh loi "There is already an object named...")
-- =========================================================

if object_id('[__EFMigrationsHistory]') is null
    create table [__EFMigrationsHistory] (
        MigrationId    nvarchar(150) not null,
        ProductVersion nvarchar(32)  not null,
        constraint PK___EFMigrationsHistory primary key (MigrationId)
    );
go

if not exists (select 1 from [__EFMigrationsHistory] where MigrationId = N'20260731012509_InitialCreate')
    insert into [__EFMigrationsHistory] (MigrationId, ProductVersion) values
    (N'20260731012509_InitialCreate', N'10.0.10');
go


-- =========================================================
-- 5. KIEM TRA KET QUA
-- =========================================================

select * from [Roles];
select * from [Users];
select * from [Categories];
select * from [Tags];
select * from [Posts];
select * from [Comments];
