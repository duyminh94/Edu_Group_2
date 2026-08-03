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
| [docs/service-layer-outline.md](docs/service-layer-outline.md) | **Dàn ý tầng nghiệp vụ** — chữ ký 11 service, quy ước kiểu trả về, ai sở hữu file nào |
| [docs/github-issues.md](docs/github-issues.md) | Phân công 4 khu + quy ước tránh đụng file |

> Thứ tự nên đọc: `github-issues` (biết mình làm khu nào) → `business-rules` (biết quy tắc)
> → `service-layer-outline` (biết viết hàm gì, trả kiểu gì).

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

### Bước 3 — Tạo 12 bảng + dữ liệu mẫu

Có **2 cách**, chọn 1 tuỳ công cụ bạn quen dùng. Kết quả database giống hệt nhau.

<details open>
<summary><b>Cách A — Chạy file SQL (nhanh nhất, chỉ cần SSMS)</b></summary>

Mở **`Database/SeedData.sql`** trong SSMS hoặc Azure Data Studio, bấm **F5**.

Một lần chạy làm hết: tạo database → tạo 12 bảng → chèn dữ liệu mẫu.
**Không cần** chạy `dotnet ef database update` nữa — file SQL đã ghi sẵn lịch sử
migration vào bảng `__EFMigrationsHistory` để EF Core biết là đã xong.

> ⚠️ File này **xoá sạch dữ liệu cũ** mỗi lần chạy. Chỉ dùng cho máy học/dev.

</details>

<details open>
<summary><b>Cách B — Code First (bảng sinh từ Entity class trong <code>Models/</code>)</b></summary>

**B1.** Tạo 12 bảng trống:

| Công cụ | Lệnh |
|---------|------|
| VS Code / Terminal | `dotnet ef database update` |
| Visual Studio 2022 | Mở **Tools → NuGet Package Manager → Package Manager Console**, chọn Default project là `BlogPlatform`, gõ `Update-Database` |

Dùng Terminal thì cài `dotnet-ef` trước, **1 lần duy nhất** trên máy:

```bash
dotnet tool install --global dotnet-ef
```

**B2.** Muốn có dữ liệu mẫu thì mở `Database/SeedData.sql`, chỉ chạy **phần 3**
(khối `insert`) — bỏ qua phần 0–2 vì bảng đã có rồi.

</details>

> Dự án **không** có seeder tự chạy lúc khởi động. Dữ liệu mẫu chỉ đến từ file SQL,
> nên `dotnet run` sẽ không tự chèn thêm gì.

Dữ liệu mẫu gồm:

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

### Bước 4 — Chạy dự án

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
├── Services/          Tầng nghiệp vụ — 10 cặp interface + implementation (còn rỗng)
├── ViewModel/         15 class dữ liệu truyền từ Controller sang View
├── Filters/           SessionAuthorizeAttribute — phân quyền
├── Helpers/           SessionKeys — tên khoá lưu trong Session
├── Database/          SeedData.sql — tạo bảng + dữ liệu mẫu, chạy thẳng trên SSMS
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
| `There is already an object named 'Roles'` | Đã tạo bảng bằng `SeedData.sql` rồi còn chạy `dotnet ef database update` | Không cần update nữa — xem lại Bước 3 Cách A |
| `Unable to create a 'DbContext'` | Đứng sai thư mục khi chạy lệnh | `cd` vào thư mục chứa file `.csproj` |
| `CS0535: 'PostService' does not implement interface member` | Đã khai báo hàm trong interface nhưng class chưa có hàm đó | Thêm hàm vào class. Chưa viết được thân hàm thì tạm để `throw new NotImplementedException();` cho build chạy |
| Vào `/post/ten-bai-viet` bị 404 | Chưa viết action `Detail` trong `BlogController` | Route đã cấu hình sẵn trong `Program.cs`, chỉ thiếu action — thuộc Issue #4 |
| `Violation of UNIQUE KEY constraint 'IX_Roles_Name'` | Chạy `SeedData.sql` 2 lần chồng nhau | Chạy lại nguyên file — phần 1 tự xoá bảng cũ trước |
| Đăng nhập báo sai mật khẩu | Chưa chèn dữ liệu mẫu | Làm lại Bước 3 |

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

### `Program.cs` đã cấu hình sẵn — không ai cần mở

| Đã có sẵn | Chi tiết |
|-----------|----------|
| 10 dòng đăng ký DI | 8 service `Scoped` + 2 service `Singleton` (`IPasswordService`, `IHtmlSanitizerService` không đụng DB) |
| Session 30 phút | Đúng quy tắc 3.9 |
| 4 route URL dạng slug | `/post/{slug}` · `/author/{username}` · `/category/{slug}` · `/tag/{slug}` — khai báo **trước** route mặc định vì route khớp theo đúng thứ tự |

Hai chỗ còn để comment sẵn, kèm ghi chú ngay trong file:

- `ITaxonomyService` — chờ nhóm duyệt [dàn ý §3 mục 4](docs/service-layer-outline.md)
- `UseStatusCodePagesWithReExecute` — Khu B bật sau khi viết xong action `BlogController.Error`,
  bật sớm sẽ biến lỗi 404 thành lỗi chồng lỗi
