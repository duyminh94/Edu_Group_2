-- =========================================================
-- Blogging Platform - Edu_Group_2
-- DU LIEU MAU
--
-- File nay CHI chen du lieu, KHONG tao bang.
-- Bang do EF Core Migrations tao ra.
--
-- Chay theo thu tu:
--   1. dotnet ef database update     (tao 12 bang)
--   2. Mo file nay trong SSMS, bam F5 (chen du lieu mau)
-- =========================================================

use BlogPlatformDb
go


-- =========================================================
-- 1. DU LIEU MAU
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
-- 2. KIEM TRA KET QUA
-- =========================================================

select * from [Roles]
select * from [Users]
select * from [Categories]
select * from [Tags]
select * from [Posts]
select * from [Comments]
