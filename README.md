# Blogging Platform — Edu_Group_2

Mini Project 2: nền tảng viết blog cho phép đăng bài, bình luận nhiều cấp,
tìm kiếm và tuỳ biến giao diện riêng cho từng tác giả.

**Stack:** ASP.NET Core MVC (.NET 10) · EF Core Code First · SQL Server 2022

---

## Tài liệu

| File | Nội dung |
|------|----------|
| [docs/blog-platform-erd.md](docs/blog-platform-erd.md) | ERD 12 bảng, quy tắc xoá 17 khoá ngoại |
| [docs/blog-platform-usecase.md](docs/blog-platform-usecase.md) | 28 use case, 4 actor |
| [docs/business-rules.md](docs/business-rules.md) | **Quy tắc nghiệp vụ cho tầng Service** — đọc trước khi code |
| [docs/github-issues.md](docs/github-issues.md) | Phân công 4 khu + quy ước tránh đụng file |

---

## 1. Yêu cầu môi trường

| Phần mềm | Ghi chú |
|----------|---------|
| .NET SDK 10.0 | `dotnet --version` để kiểm tra |
| SQL Server 2022 | Bản Express cũng chạy được |
| SSMS | Để xem dữ liệu và chạy file seed |
| Visual Studio 2022 **hoặc** VS Code | Chọn 1 |

---

## 2. Cài đặt lần đầu

### Bước 1 — Clone và tải package

```bash
git clone https://github.com/duyminh94/Edu_Group_2.git
cd Edu_Group_2
dotnet restore
```

### Bước 2 — Tạo file cấu hình riêng của máy mình

Tạo file **`appsettings.Development.json`** ngay cạnh `appsettings.json`.
File này **không có trên git** (đã chặn trong `.gitignore`) vì mỗi máy cấu hình khác nhau.

Nội dung — chọn 1 trong 2 mẫu tuỳ cách SQL Server của bạn đăng nhập:

```jsonc
{
  "ConnectionStrings": {
    // Windows Authentication — thường dùng trên Windows
    "DefaultConnection": "Server=localhost;Database=BlogPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"

    // SQL Server Authentication — nếu đăng nhập bằng tài khoản sa
    // "DefaultConnection": "Server=localhost,1433;Database=BlogPlatformDb;User Id=sa;Password=MatKhauCuaBan;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

> Cài SQL Server dạng named instance thì đổi `localhost` thành `localhost\SQLEXPRESS`.

### Bước 3 — Tạo 12 bảng trong database

Dự án dùng **Code First**: bảng sinh ra từ Entity class trong `Models/`,
không viết `create table` bằng tay.

Chọn cách tương ứng với công cụ bạn dùng — **hai cách cho kết quả giống hệt nhau**:

<details open>
<summary><b>Cách A — VS Code (hoặc Terminal bất kỳ)</b></summary>

Cài công cụ `dotnet-ef`, chỉ cần làm **1 lần duy nhất** trên máy:

```bash
dotnet tool install --global dotnet-ef
```

Đứng trong thư mục chứa file `.csproj` rồi chạy:

```bash
dotnet ef database update
```

</details>

<details open>
<summary><b>Cách B — Visual Studio 2022</b></summary>

Không cần cài gì thêm, package `Microsoft.EntityFrameworkCore.Tools` đã có sẵn trong dự án.

1. Mở menu **Tools → NuGet Package Manager → Package Manager Console**
2. Ở ô **Default project** chọn `BlogPlatform`
3. Gõ lệnh:

```powershell
Update-Database
```

</details>

### Bước 4 — Chèn dữ liệu mẫu

Mở **`Database/SeedData.sql`** trong SSMS, bấm **F5**.

Dữ liệu tạo ra:

| Bảng | Số dòng |
|------|---------|
| Roles | 3 — Admin, Author, Reader |
| Users | 4 tài khoản mẫu |
| Categories | 5 |
| Tags | 8 |
| Posts | 4 — 3 bài đã đăng, 1 bài nháp |
| Comments | 4 — có 1 comment trả lời comment khác |
| PostLikes / Bookmarks | 5 / 2 |
| BlogSettings | 2 tác giả có theme riêng |

### Bước 5 — Chạy dự án

```bash
dotnet run
```

Hoặc bấm **F5** trong Visual Studio. Mở trình duyệt vào `http://localhost:5240`.

---

## 3. Tài khoản mẫu

Mật khẩu của **cả 4 tài khoản** đều là `Admin@123`.

| Tài khoản | Vai trò | Dùng để test |
|-----------|---------|--------------|
| `admin` | Admin | Khu vực quản trị |
| `minh` | Author | Viết bài, kiểm duyệt bình luận |
| `lan` | Author | Kiểm tra 2 tác giả có theme khác nhau |
| `hoa` | Reader | Bình luận, thích, lưu bài |

---

## 4. Bảng tra lệnh — VS Code ↔ Visual Studio

Hai cột dưới làm **cùng một việc**, chỉ khác công cụ:

| Việc cần làm | VS Code / Terminal | Visual Studio (Package Manager Console) |
|--------------|--------------------|------------------------------------------|
| Tạo hoặc cập nhật database | `dotnet ef database update` | `Update-Database` |
| Sinh migration mới | `dotnet ef migrations add TenThayDoi` | `Add-Migration TenThayDoi` |
| Xoá migration vừa tạo (chưa update) | `dotnet ef migrations remove` | `Remove-Migration` |
| Xem danh sách migration | `dotnet ef migrations list` | `Get-Migration` |
| Xoá sạch database làm lại | `dotnet ef database drop` | `Drop-Database` |
| Xuất ra file SQL để xem trước | `dotnet ef migrations script` | `Script-Migration` |
| Tải package | `dotnet restore` | Build → Restore NuGet Packages |
| Chạy dự án | `dotnet run` | F5 |

> **Lưu ý cho Visual Studio:** trước khi gõ lệnh nhớ chọn đúng **Default project**
> là `BlogPlatform` ở ô dropdown phía trên Package Manager Console.

---

## 5. Khi cần đổi cấu trúc bảng

Chỉ sửa **một nơi duy nhất** là Entity class, EF Core lo phần còn lại.

Ví dụ thêm cột `IsFeatured` vào bài viết:

```csharp
// 1. Sửa Models/Post.cs
public bool IsFeatured { get; set; } = false;
```

```bash
# 2. Sinh migration  (VS Code)
dotnet ef migrations add ThemCotIsFeatured
# 3. Áp lên database
dotnet ef database update
```

```powershell
# Hoặc trong Visual Studio
Add-Migration ThemCotIsFeatured
Update-Database
```

**Quy tắc cho cả nhóm:**

- Báo trước khi đổi bảng — thay đổi ảnh hưởng nhiều khu
- **Commit file migration cùng với Entity class**, không được bỏ sót
- Người khác `git pull` xong chỉ cần chạy `Update-Database` là database khớp lại
- Đặt tên migration mô tả rõ việc: `ThemCotIsFeatured`, `DoiKieuSlug`
  — tránh đặt `Update1`, `Fix2`
- Sửa xong nhớ cập nhật `docs/blog-platform-erd.md` cho khớp

---

## 6. Cấu trúc thư mục

```
BlogPlatform/
├── Areas/
│   ├── User/          Khách và người đọc: trang chủ, chi tiết bài, tài khoản
│   ├── Author/        Tác giả: quản lý bài viết, kiểm duyệt, thống kê
│   └── Admin/         Quản trị: người dùng, chuyên mục, gỡ nội dung
├── Models/            12 Entity class + 2 enum
├── Data/              BlogDbContext — cấu hình 17 khoá ngoại
├── Migrations/        Lịch sử thay đổi cấu trúc bảng (EF Core sinh)
├── Services/          Tầng nghiệp vụ — 8 cặp interface + implementation
├── ViewModel/         Dữ liệu truyền từ Controller sang View
├── Filters/           SessionAuthorizeAttribute — phân quyền
├── Helpers/           SessionKeys — tên khoá lưu trong Session
├── Database/          SeedData.sql — dữ liệu mẫu
├── docs/              ERD, Use Case, phân công
└── wwwroot/           css, js, 4 theme preset, thư mục uploads
```

---

## 7. Quy trình làm việc nhóm

1. Chọn Issue trong [danh sách Issue](https://github.com/duyminh94/Edu_Group_2/issues), comment để nhận
2. Tạo branch riêng: `git checkout -b feature/comment`
3. Code xong, commit và push branch của mình
4. Tạo Pull Request, ghi `Closes #<số issue>` trong phần mô tả
5. Chờ review rồi mới merge

> **Không code trực tiếp lên branch `main`.**
> **Không dùng `git push --force` lên branch chung.**

Mỗi thành viên phụ trách 1 khu chức năng. Xem bảng chủ sở hữu từng file trong
[docs/github-issues.md](docs/github-issues.md) — mục *Quy ước tránh đụng file* —
để không sửa trùng file của người khác.

---

## 8. Lỗi thường gặp

| Lỗi | Nguyên nhân | Cách sửa |
|-----|-------------|----------|
| `The ConnectionString property has not been initialized` | Chưa tạo `appsettings.Development.json` | Làm lại Bước 2 |
| `A network-related or instance-specific error` | Sai tên Server, hoặc SQL Server chưa chạy | Kiểm tra tên instance trong SSMS |
| `dotnet ef: command not found` | Chưa cài công cụ | `dotnet tool install --global dotnet-ef` |
| `Invalid column name 'X'` | Entity class có cột mà database chưa có | Chạy `dotnet ef database update` |
| `There is already an object named 'Roles'` | Database đã có bảng từ trước | `dotnet ef database drop` rồi update lại |
| `Unable to create a 'DbContext'` | Đứng sai thư mục khi chạy lệnh | `cd` vào thư mục chứa file `.csproj` |
| Đăng nhập báo sai mật khẩu | Chưa chạy `SeedData.sql` | Làm lại Bước 4 |

---

## 9. Ghi chú kỹ thuật

- **Không dùng ASP.NET Core Identity.** Dự án tự quản đăng nhập bằng Session,
  băm mật khẩu bằng `BCrypt.Net-Next`, phân quyền bằng filter `[SessionAuthorize]`.
- **Nội dung bài viết và bình luận phải sanitize** bằng `HtmlSanitizer` trước khi lưu
  — chống XSS, đây là mục đề bài có chấm.
- **Mọi action sửa/xoá bài phải kiểm tra quyền sở hữu** (`post.AuthorId` so với
  UserId trong Session) — chống lỗi IDOR.
- **Không tự thêm package vào `BlogPlatform.csproj`** — 2 người cùng thêm ở 2 branch
  sẽ conflict. Cần package mới thì báo người phụ trách Khu A thêm một lần.
