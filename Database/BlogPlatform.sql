-- =========================================================
-- Blogging Platform - Edu_Group_2
-- Tao database, 12 bang va du lieu mau
-- Mo file trong SSMS, ket noi SQL Server roi bam Execute (F5)
--
-- Luu y: neu da chay 'dotnet ef database update' thi khong chay file nay nua
--        (chon 1 trong 2 cach, khong lam ca hai)
-- =========================================================

create database BlogPlatformDb
go

use BlogPlatformDb
go


-- =========================================================
-- 1. TAO BANG
-- =========================================================

create table [Roles]
(
    Id int primary key identity,
    Name nvarchar(50) not null unique,
    Description nvarchar(200) null
)

create table [Users]
(
    Id int primary key identity,
    UserName nvarchar(50) not null unique,
    Email nvarchar(256) not null unique,
    PasswordHash nvarchar(255) not null,
    DisplayName nvarchar(100) not null,
    AvatarUrl nvarchar(500) null,
    Bio nvarchar(500) null,
    RoleId int not null,
    IsLocked bit not null default 0,
    CreatedAt datetime not null default GetDate(),
    constraint fkUserRole foreign key (RoleId) references [Roles](Id)
)

create table [BlogSettings]
(
    UserId int primary key,
    ThemeName nvarchar(50) not null default 'light',
    PrimaryColor nvarchar(7) not null default '#2563eb',
    FontFamily nvarchar(100) not null default 'Be Vietnam Pro',
    LogoUrl nvarchar(500) null,
    Tagline nvarchar(200) null,
    UpdatedAt datetime not null default GetDate(),
    constraint fkSettingUser foreign key (UserId) references [Users](Id) on delete cascade
)

create table [Categories]
(
    Id int primary key identity,
    Name nvarchar(100) not null unique,
    Slug nvarchar(120) not null unique,
    Description nvarchar(300) null
)

create table [Tags]
(
    Id int primary key identity,
    Name nvarchar(50) not null unique,
    Slug nvarchar(60) not null unique
)

create table [Posts]
(
    Id int primary key identity,
    Title nvarchar(200) not null,
    Slug nvarchar(220) not null unique,
    Summary nvarchar(500) null,
    Content nvarchar(max) not null,
    FeaturedImageUrl nvarchar(500) null,
    CategoryId int null,
    AuthorId int not null,
    Status tinyint not null default 0,
    PublishedAt datetime null,
    ViewCount int not null default 0,
    LikeCount int not null default 0,
    CommentCount int not null default 0,
    CreatedAt datetime not null default GetDate(),
    UpdatedAt datetime not null default GetDate(),
    constraint fkPostCategory foreign key (CategoryId) references [Categories](Id) on delete set null,
    constraint fkPostAuthor foreign key (AuthorId) references [Users](Id)
)

create table [PostTags]
(
    PostId int not null,
    TagId int not null,
    constraint pkPostTag primary key (PostId, TagId),
    constraint fkPostTagPost foreign key (PostId) references [Posts](Id) on delete cascade,
    constraint fkPostTagTag foreign key (TagId) references [Tags](Id) on delete cascade
)

create table [Comments]
(
    Id int primary key identity,
    PostId int not null,
    UserId int not null,
    ParentCommentId int null,
    Content nvarchar(2000) not null,
    Status tinyint not null default 0,
    CreatedAt datetime not null default GetDate(),
    UpdatedAt datetime not null default GetDate(),
    constraint fkCommentPost foreign key (PostId) references [Posts](Id) on delete cascade,
    constraint fkCommentUser foreign key (UserId) references [Users](Id),
    constraint fkCommentParent foreign key (ParentCommentId) references [Comments](Id)
)

create table [PostLikes]
(
    PostId int not null,
    UserId int not null,
    CreatedAt datetime not null default GetDate(),
    constraint pkPostLike primary key (PostId, UserId),
    constraint fkLikePost foreign key (PostId) references [Posts](Id) on delete cascade,
    constraint fkLikeUser foreign key (UserId) references [Users](Id)
)

create table [Bookmarks]
(
    PostId int not null,
    UserId int not null,
    CreatedAt datetime not null default GetDate(),
    constraint pkBookmark primary key (PostId, UserId),
    constraint fkBookmarkPost foreign key (PostId) references [Posts](Id) on delete cascade,
    constraint fkBookmarkUser foreign key (UserId) references [Users](Id)
)

create table [PostViews]
(
    Id bigint primary key identity,
    PostId int not null,
    UserId int null,
    IpHash nvarchar(64) not null,
    ViewedAt datetime not null default GetDate(),
    constraint fkViewPost foreign key (PostId) references [Posts](Id) on delete cascade,
    constraint fkViewUser foreign key (UserId) references [Users](Id)
)

create table [MediaFiles]
(
    Id int primary key identity,
    OriginalFileName nvarchar(255) not null,
    StoredFileName nvarchar(100) not null unique,
    ContentType nvarchar(100) not null,
    SizeBytes bigint not null,
    PostId int null,
    UploadedById int not null,
    UploadedAt datetime not null default GetDate(),
    constraint fkMediaPost foreign key (PostId) references [Posts](Id) on delete cascade,
    constraint fkMediaUser foreign key (UploadedById) references [Users](Id)
)


-- =========================================================
-- 2. TAO INDEX cho cac cot hay dung trong WHERE va ORDER BY
-- =========================================================

create index ixPostStatusPublished on [Posts](Status, PublishedAt)
create index ixPostAuthorStatus on [Posts](AuthorId, Status)
create index ixCommentPostStatus on [Comments](PostId, Status)
create index ixViewPostTime on [PostViews](PostId, ViewedAt)


-- =========================================================
-- 3. DU LIEU MAU
-- =========================================================

-- 3 vai tro, bat buoc phai co moi dang ky tai khoan duoc
insert into [Roles] (Name, Description) values
(N'Admin', N'Quan tri toan he thong'),
(N'Author', N'Viet bai, duyet binh luan tren bai cua minh'),
(N'Reader', N'Doc bai, binh luan, thich va luu bai');

-- Tai khoan mau. Mat khau ca 3 tai khoan deu la: Admin@123
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
 1, 2, 1, '2026-7-10', 120, 8, 2),

(N'Hieu ve Entity Framework Core', N'hieu-ve-entity-framework-core',
 N'Code First, migration va cach EF Core sinh bang tu class C#',
 N'<p>EF Core cho phep viet class C# roi tu sinh ra bang trong database.</p>',
 1, 2, 1, '2026-7-15', 85, 5, 1),

(N'Chon mau va font cho website', N'chon-mau-va-font-cho-website',
 N'Vai nguyen tac chon bang mau va font chu de trang web de nhin',
 N'<p>Do tuong phan giua chu va nen nen dat toi thieu 4.5:1.</p>',
 3, 3, 1, '2026-7-20', 64, 12, 0),

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


-- =========================================================
-- 4. KIEM TRA KET QUA
-- =========================================================

select * from [Roles]
select * from [Users]
select * from [Categories]
select * from [Tags]
select * from [Posts]
select * from [Comments]
