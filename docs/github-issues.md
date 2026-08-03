# Danh sách Issue — Edu_Group_2 (Blogging Platform)

13 Issue chia thành **4 khu chức năng**, mỗi thành viên phụ trách trọn 1 khu.
Tài liệu tham chiếu: [ERD](./blog-platform-erd.md) · [Use Case](./blog-platform-usecase.md) · [Quy tắc nghiệp vụ](./business-rules.md) · [Dàn ý tầng nghiệp vụ](./service-layer-outline.md)

---

## Cách nhận việc

1. Chọn khu mình phụ trách, comment vào Issue trên GitHub để nhận
2. Tạo branch riêng: `git checkout -b feature/<tên-ngắn>` (ví dụ `feature/comment`)
3. Code xong tạo Pull Request, ghi `Closes #<số issue>` trong phần mô tả

> **Quy tắc:** không code trực tiếp lên branch `main`.

---

## Tổng quan 4 khu

| Khu | Thành viên | Issue | Điểm |
|-----|-----------|-------|------|
| **A — Tài khoản & Quản trị** | Người 1 | #1, #3, #13 | 6 |
| **B — Giao diện & Trải nghiệm đọc** | Người 2 | #2, #4, #10, #11 | 8 |
| **C — Quản lý nội dung** | Người 3 | #5, #6, #12 | 9 |
| **D — Tương tác người dùng** | Người 4 | #7, #8, #9 | 6 |

Điểm tính theo độ khó: 🟢 = 1, 🟡 = 2, 🔴 = 3.

**Ngày đầu ai làm gì:**

```
Người 1 → #1 Session & Phân quyền   (không chờ ai — cả nhóm đang đợi)
Người 2 → #2 Layout & Theme          (không chờ ai)
Người 3 → chờ #1; trong lúc đó nghiên cứu rich text editor
Người 4 → chờ #1 và #4; trong lúc đó thiết kế cây bình luận
```

---

# KHU A — Tài khoản & Quản trị (Người 1)

## Issue #1 — Đăng nhập bằng Session và phân quyền

**Độ khó:** 🟡 · **Ưu tiên:** Cao nhất · **Phụ thuộc:** Không

Dự án **không dùng** ASP.NET Core Identity, phải tự viết cơ chế lưu phiên và chặn quyền.
Cả nhóm đang chờ Issue này — làm trước tiên.

**File cần code**
- `Filters/SessionAuthorizeAttribute.cs`
- `Services/PasswordService.cs` + `IPasswordService.cs`
- `Helpers/SessionKeys.cs` (đã có sẵn hằng số)

**Checklist**
- [ ] `PasswordService`: hàm băm mật khẩu và hàm kiểm tra mật khẩu
- [ ] `SessionAuthorizeAttribute` override `OnActionExecuting`, xử lý 3 trường hợp:
  - Chưa đăng nhập → redirect `/User/Account/Login` kèm `returnUrl`
  - Sai role → trả về 403 Forbidden
  - Hợp lệ → cho request đi tiếp
- [x] ~~Bỏ comment 10 dòng đăng ký service trong `Program.cs`~~ — **đã làm sẵn khi dựng
      sườn**, Khu A không phải mở file này nữa. Dòng thứ 11 (`ITaxonomyService`) vẫn để
      comment, chờ nhóm duyệt [dàn ý §3 mục 4](./service-layer-outline.md)
- [ ] Test: vào `/Author/Post/Index` khi chưa đăng nhập phải bị đẩy về trang login

**Cảnh báo bảo mật:** không dùng MD5 hay SHA-1 để băm mật khẩu.

---

## Issue #3 — Đăng ký, đăng nhập, hồ sơ cá nhân

**Use case:** UC07, UC08, UC09 · **Độ khó:** 🟡 · **Phụ thuộc:** #1

**File cần code**
- `Areas/User/Controllers/AccountController.cs`
- `Services/AccountService.cs` + `IAccountService.cs`
- `ViewModel/LoginViewModel.cs`, `RegisterViewModel.cs`
- `Areas/User/Views/Account/{Login,Register,Profile}.cshtml`

**Checklist**
- [ ] Form đăng ký: username, email, mật khẩu, xác nhận mật khẩu
- [ ] Kiểm tra trùng username và email trước khi tạo
- [ ] Tài khoản mới mặc định role `Reader`
- [ ] Đăng nhập: kiểm tra mật khẩu, chặn tài khoản có `IsLocked = true`
- [ ] Lưu Session: `UserId`, `UserName`, `DisplayName`, `RoleName`
- [ ] Đăng nhập xong quay lại đúng `returnUrl`
- [ ] Đăng xuất: xoá Session
- [ ] Trang hồ sơ: sửa `DisplayName`, `Bio`, `AvatarUrl`
- [ ] Validation hiển thị lỗi ngay dưới từng field
- [ ] Viết `Views/Shared/_AccountMenu.cshtml` — **Khu A sở hữu file này**, Khu B không sửa

**Dữ liệu để đăng nhập thử:** đã có sẵn, không phải tạo tay.
Chạy `Database/SeedData.sql` trong SSMS là có 3 vai trò và 4 tài khoản mẫu.

| Tài khoản | Mật khẩu | Vai trò |
|-----------|----------|---------|
| `admin` | `Admin@123` | Admin |
| `minh` | `Admin@123` | Author |
| `lan` | `Admin@123` | Author |
| `hoa` | `Admin@123` | Reader |

Chuỗi `PasswordHash` trong file SQL sinh bằng đúng thư viện `BCrypt.Net-Next 4.2.0`,
nên `BCrypt.Verify()` trong `PasswordService` kiểm tra sẽ khớp.

---

## Issue #13 — Khu vực quản trị

**Use case:** UC25, UC26, UC27, UC28 · **Độ khó:** 🟡 · **Phụ thuộc:** #3, #5, #7

**File cần code**
- `Areas/Admin/Controllers/{Dashboard,UserManagement,Taxonomy,Moderation}Controller.cs`
- 4 view tương ứng trong `Areas/Admin/Views/`

**Checklist**
- [ ] Dashboard: tổng số user, bài viết, bình luận, lượt xem toàn hệ thống
- [ ] Quản lý user: danh sách, đổi role, khoá/mở tài khoản (`IsLocked`)
- [ ] Quản lý chuyên mục và thẻ: thêm, sửa, xoá
- [ ] Gỡ nội dung vi phạm: xoá bài, xoá bình luận trên toàn hệ thống
- [ ] Admin không tự khoá tài khoản của chính mình

**Lưu ý:** xoá chuyên mục thì bài viết vẫn còn (quy tắc `SetNull` trong ERD) — không được để mất bài.

---

# KHU B — Giao diện & Trải nghiệm đọc (Người 2)

## Issue #2 — Layout, CSS nền và 4 theme preset

**Độ khó:** 🟡 · **Ưu tiên:** Cao · **Phụ thuộc:** Không

Dựng khung giao diện chung để các thành viên khác có nền mà gắn nội dung vào.

**File cần code**
- `Views/Shared/_Layout.cshtml`
- `wwwroot/css/site.css`
- `wwwroot/themes/{light,dark,serif,minimal}.css`
- 3 file `Areas/*/Views/_ViewStart.cshtml` (thêm dòng khai báo `Layout`)

**Checklist**
- [ ] Navbar: logo, ô tìm kiếm, menu tài khoản (đổi theo trạng thái đăng nhập)
- [ ] Footer
- [ ] Định nghĩa CSS variables: `--primary`, `--font`, `--bg`, `--text`
- [ ] Viết 4 file theme preset
- [ ] Responsive: chạy được trên mobile, không bị cuộn ngang
- [ ] Thêm `Layout = "~/Views/Shared/_Layout.cshtml";` vào `_ViewStart` của 3 Area

**Yêu cầu bắt buộc từ đề:** contrast chữ/nền ≥ 4.5:1, vùng bấm ≥ 44×44px, input ≥ 16px trên mobile.

---

## Issue #4 — Trang chủ, chi tiết bài viết, trang tác giả

**Use case:** UC01, UC02, UC05 · **Độ khó:** 🟡 · **Phụ thuộc:** #2

Đây là phần **khách chưa đăng nhập** nhìn thấy — bộ mặt của website.

**File cần code**
- `Areas/User/Controllers/BlogController.cs`
- `ViewModel/PostListViewModel.cs`, `PostDetailViewModel.cs`, `AuthorProfileViewModel.cs`
- `Areas/User/Views/Blog/{Index,Detail}.cshtml`, `Areas/User/Views/Profile/Author.cshtml`

**Checklist**
- [ ] Trang chủ: liệt kê bài `Published`, mới nhất trước, có phân trang
- [ ] Chỉ hiện bài `Status = Published` — tuyệt đối không lộ bài nháp
- [ ] Trang chi tiết: tra theo `Slug`, không phải theo `Id`
- [ ] Trang tác giả `/author/{username}`: thông tin tác giả + bài viết của họ
- [ ] Thêm action `Error()` — `UseExceptionHandler` đang trỏ tới `/User/Blog/Error`
- [ ] ⚠️ **Đóng băng `PostDetailViewModel`** — khai báo đủ property cho cả 3 khu rồi không sửa nữa
- [ ] ⚠️ `Detail.cshtml` chỉ dựng khung + gọi 2 partial của Khu D, không viết nội dung vào đó
- [ ] Thêm sẵn dòng `await analyticsService.RecordViewAsync(post.Id, HttpContext);` cho Khu C

**Khu B là nút thắt:** 3 khu còn lại chờ Issue này. Làm ngay sau #2.

**Lưu ý hiệu năng:** dùng `Include` để lấy tác giả và chuyên mục trong 1 query, tránh N+1.

---

## Issue #10 — Tìm kiếm, lọc và sắp xếp

**Use case:** UC03, UC04 · **Độ khó:** 🟡 · **Phụ thuộc:** #4

**File cần code**
- `Services/SearchService.cs` + `ISearchService.cs`
- `ViewModel/SearchViewModel.cs`
- `Areas/User/Views/Blog/Search.cshtml`

**Checklist**
- [ ] Tìm theo từ khoá trong tiêu đề và nội dung
- [ ] Lọc theo chuyên mục, thẻ, tác giả
- [ ] Sắp xếp: mới nhất / xem nhiều nhất / nhiều lượt thích nhất
- [ ] Phân trang kết quả
- [ ] Giữ nguyên bộ lọc khi chuyển trang
- [ ] Trạng thái rỗng: "Không tìm thấy bài viết nào"

**Lưu ý hiệu năng:** chỉ `Select` các cột cần hiển thị, đừng lấy cột `Content`
(kiểu `nvarchar(max)`) trong danh sách kết quả.

---

## Issue #11 — Tuỳ biến giao diện blog

**Use case:** UC24 · **Độ khó:** 🟡 · **Phụ thuộc:** #2, #3

Chức năng đặc trưng của đề nhóm 2 — cần làm cho ra tấm ra món.

**File cần code**
- `Areas/Author/Controllers/BlogSettingController.cs`
- `Services/BlogSettingService.cs` + `IBlogSettingService.cs`
- `ViewModel/BlogSettingViewModel.cs`
- `Areas/Author/Views/BlogSetting/Index.cshtml`

**Checklist**
- [ ] Form chọn theme trong 4 preset
- [ ] Chọn màu chủ đạo (color picker), chọn font
- [ ] Upload logo, nhập tagline
- [ ] Xem trước trực tiếp khi đổi, chưa cần lưu
- [ ] Trang `/author/{username}` áp đúng theme của tác giả đó
- [ ] Tác giả chưa cấu hình gì thì dùng giá trị mặc định

**Cách áp theme:** đọc `BlogSetting` của tác giả rồi đổ ra CSS variables trong `_Layout`
— xem mục 2.2 tài liệu ERD.

---

# KHU C — Quản lý nội dung (Người 3)

## Issue #5 — Quản lý bài viết (CRUD + publish)

**Use case:** UC15–UC19, UC21 · **Độ khó:** 🔴 · **Phụ thuộc:** #1

**File cần code**
- `Areas/Author/Controllers/PostController.cs`
- `Services/PostService.cs` + `IPostService.cs`
- `ViewModel/PostEditViewModel.cs`
- `Areas/Author/Views/Post/{Index,Create,Edit}.cshtml`

**Checklist**
- [ ] Danh sách bài của tôi, lọc theo trạng thái
- [ ] Tạo bài: tiêu đề, nội dung, tóm tắt, chuyên mục, thẻ, ảnh đại diện
- [ ] Sinh `Slug` từ tiêu đề, bỏ dấu tiếng Việt, trùng thì thêm hậu tố số
- [ ] Sửa bài, xoá bài
- [ ] Publish / Unpublish, cập nhật `PublishedAt`
- [ ] Gắn thẻ: nhập thẻ mới thì tự tạo trong bảng `Tags`
- [ ] ⚠️ **Kiểm tra quyền sở hữu** ở mọi action sửa/xoá: so `post.AuthorId` với UserId trong Session

**Cảnh báo bảo mật (lỗi IDOR):** thiếu bước kiểm tra quyền sở hữu thì tác giả A sửa URL
là sửa được bài của tác giả B. Đây là lỗi đề bài chấm.

---

## Issue #6 — Upload ảnh và làm sạch HTML

**Use case:** UC20 · **Độ khó:** 🔴 · **Phụ thuộc:** #5

Phần bảo mật nặng nhất của dự án.

**File cần code**
- `Services/MediaService.cs` + `IMediaService.cs`
- `Services/HtmlSanitizerService.cs` + `IHtmlSanitizerService.cs`
- `Areas/Author/Controllers/MediaController.cs`

**Checklist**
- [ ] Tích hợp rich text editor vào form tạo/sửa bài (TinyMCE hoặc CKEditor)
- [ ] Upload ảnh: whitelist đuôi file, kiểm tra content-type, giới hạn 5MB
- [ ] Đổi tên file thành GUID trước khi lưu vào `wwwroot/uploads/`
- [ ] Lưu thông tin file vào bảng `MediaFiles`
- [ ] `HtmlSanitizerService` dùng package `HtmlSanitizer` (đã cài sẵn)
- [ ] Sanitize nội dung bài **trước khi lưu DB**
- [ ] Test XSS: nhập `<script>alert(1)</script>` vào bài, lưu, mở lại — không được chạy

**Cảnh báo:** không dùng `@Html.Raw()` với dữ liệu chưa sanitize.

---

## Issue #12 — Ghi nhận lượt xem và thống kê

**Use case:** UC06, UC23 · **Độ khó:** 🔴 · **Phụ thuộc:** #4, #5

**File cần code**
- `Services/AnalyticsService.cs` + `IAnalyticsService.cs`
- `Areas/Author/Controllers/AnalyticsController.cs`
- `ViewModel/AnalyticsViewModel.cs`
- `Areas/Author/Views/Analytics/Index.cshtml`

**Checklist**
- [ ] Ghi 1 dòng `PostView` mỗi lượt xem, lưu `IpHash` (SHA-256 của IP)
- [ ] Chống đếm trùng: cùng `(PostId, IpHash)` trong 30 phút chỉ tính 1 lần
- [ ] Cập nhật `Post.ViewCount` trong cùng transaction
- [ ] Trang thống kê: tổng lượt xem, lượt thích, số bình luận theo từng bài
- [ ] Biểu đồ lượt xem theo ngày (Chart.js)
- [ ] Chỉ xem được thống kê bài của chính mình
- [ ] ⚠️ **Không mở `BlogController.cs`** — Khu B đã gọi sẵn `RecordViewAsync`,
      chỉ cần viết phần thân hàm trong `AnalyticsService.cs`

**Vì sao lưu `IpHash` chứ không lưu IP thật:** vẫn chặn được spam F5 nhưng không giữ
dữ liệu cá nhân — ăn điểm mục Security của đề.

---

# KHU D — Tương tác người dùng (Người 4)

## Issue #7 — Bình luận và trả lời lồng nhau

**Use case:** UC10, UC11 · **Độ khó:** 🔴 · **Phụ thuộc:** #3, #4

**File cần code**
- `Areas/User/Controllers/CommentController.cs`
- `Services/CommentService.cs` + `ICommentService.cs`
- `ViewModel/CommentViewModel.cs`
- Partial view hiển thị cây bình luận

**Checklist**
- [ ] Form gửi bình luận dưới bài viết (bắt đăng nhập)
- [ ] Bình luận mới mặc định `Status = Pending`
- [ ] Trả lời bình luận khác (`ParentCommentId`)
- [ ] Giới hạn độ sâu tối đa **3 cấp**
- [ ] Chỉ hiển thị bình luận `Approved` cho người đọc
- [ ] Sanitize nội dung bình luận
- [ ] Render cây bình luận trong `_CommentTree.cshtml` và `_CommentItem.cshtml`
- [ ] ⚠️ **Không mở `Detail.cshtml`** — Khu B đã gọi sẵn partial của mình

**Gợi ý:** lấy toàn bộ bình luận của bài trong **1 query**, rồi dựng cây trong bộ nhớ
— đừng query lặp theo từng cấp.

---

## Issue #8 — Kiểm duyệt bình luận

**Use case:** UC22 · **Độ khó:** 🟡 · **Phụ thuộc:** #7

**File cần code**
- `Areas/Author/Controllers/CommentModerationController.cs`
- `Areas/Author/Views/CommentModeration/Index.cshtml`

**Checklist**
- [ ] Danh sách bình luận trên bài của tôi, lọc theo trạng thái
- [ ] Hành động: Duyệt / Từ chối / Gắn cờ
- [ ] Duyệt xong thì tăng `Post.CommentCount`
- [ ] ⚠️ Kiểm tra quyền sở hữu bài trước khi cho thao tác
- [ ] Hiển thị số bình luận đang chờ duyệt

---

## Issue #9 — Thích, lưu bài, chia sẻ

**Use case:** UC12, UC13, UC14 · **Độ khó:** 🟢 · **Phụ thuộc:** #3, #4

**File cần code**
- `Areas/User/Controllers/InteractionController.cs`
- `Services/InteractionService.cs` + `IInteractionService.cs`
- `Areas/User/Views/Interaction/Bookmarks.cshtml`

**Checklist**
- [ ] Nút thích / bỏ thích, cập nhật `Post.LikeCount`
- [ ] Nút lưu / bỏ lưu bài
- [ ] Trang "Bài đã lưu" của tôi
- [ ] Nút chia sẻ (copy link, Facebook)
- [ ] Bấm thích khi chưa đăng nhập → đẩy về trang login
- [ ] Xử lý bằng AJAX để không phải tải lại trang
- [ ] ⚠️ Nút bấm viết trong `_LikeBar.cshtml` — **không mở `Detail.cshtml`**

**Gợi ý:** hai bảng `PostLikes` và `Bookmarks` đã có khoá chính ghép nên database
tự chặn trùng, không cần kiểm tra thủ công.

---

# Thư viện dùng trong dự án

## Package NuGet — đã cài sẵn, chỉ cần restore

Bốn package dưới đã khai báo trong `BlogPlatform.csproj`. Clone repo về chỉ cần chạy:

```bash
dotnet restore
```

**Công cụ cần cài 1 lần trên máy** (nếu chưa có):

```bash
dotnet tool install --global dotnet-ef
```

| Package | Phiên bản | Dùng cho | Issue |
|---------|-----------|----------|-------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.10 | Kết nối SQL Server | Mọi Issue |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.10 | Lệnh `dotnet ef migrations` | Mọi Issue |
| `HtmlSanitizer` | 9.1.974 | Làm sạch HTML chống XSS | #6, #7 |
| `BCrypt.Net-Next` | 4.2.0 | Băm mật khẩu | #1 |

> **Không ai được tự thêm package vào `BlogPlatform.csproj`.** Nếu 2 người cùng thêm ở
> 2 branch khác nhau sẽ conflict. Cần package mới thì báo Người 1 thêm một lần.


## Thư viện client — dùng CDN, không phải tải file

Thư mục `wwwroot/lib/` cố tình để trống. Tất cả thư viện JS dùng CDN cho gọn.

| Thư viện | Đã gắn sẵn ở đâu | Issue |
|----------|------------------|-------|
| jQuery 3.7.1 | `Views/Shared/_Layout.cshtml` | #3, #9 |
| jQuery Validation 1.21.0 | `Views/Shared/_ValidationScriptsPartial.cshtml` | #3 |
| jQuery Validation Unobtrusive 4.0.0 | như trên | #3 |

**Cách bật kiểm tra dữ liệu form phía client** — thêm vào cuối view có form:

```razor
@section Scripts {
    @await Html.PartialAsync("_ValidationScriptsPartial")
}
```

## Thư viện cần tự thêm khi làm Issue

Ba thư viện này **chưa gắn** vì chỉ vài Issue dùng tới. Ai làm Issue nào thì tự chèn
thẻ `<script>` vào view của mình, **không sửa `_Layout.cshtml`**.

### Issue #6 — Rich text editor

Chọn 1 trong 2, dán vào `Areas/Author/Views/Post/Create.cshtml` và `Edit.cshtml`:

```html
<!-- TinyMCE — dễ dùng hơn, cần đăng ký lấy API key miễn phí tại tiny.cloud -->
<script src="https://cdn.tiny.cloud/1/API_KEY_CUA_BAN/tinymce/7/tinymce.min.js"></script>

<!-- Hoặc CKEditor 5 — không cần API key -->
<script src="https://cdn.ckeditor.com/ckeditor5/43.3.1/classic/ckeditor.js"></script>
```

> **Khuyến nghị dùng CKEditor** vì không phải đăng ký tài khoản.

### Issue #12 — Biểu đồ thống kê

Dán vào `Areas/Author/Views/Analytics/Index.cshtml`:

```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.6/dist/chart.umd.min.js"></script>
```

### Issue #2, #11 — Font chữ

Dán vào `<head>` của `_Layout.cshtml` (Khu B làm):

```html
<link href="https://fonts.googleapis.com/css2?family=Be+Vietnam+Pro:wght@400;600;700&display=swap" rel="stylesheet">
```

> **Tránh dùng** Inter, Roboto, Arial — nhìn generic. Font gợi ý cho tiếng Việt:
> Be Vietnam Pro, Plus Jakarta Sans, Outfit.

## Không cần thư viện cho những việc sau

| Việc | Cách làm không cần thư viện |
|------|----------------------------|
| Chọn màu (Issue #11) | Thẻ HTML sẵn có: `<input type="color">` |
| Gọi AJAX (Issue #9) | `fetch()` của trình duyệt, hoặc `$.ajax` của jQuery đã có |
| Băm IP thành `IpHash` (Issue #12) | `System.Security.Cryptography.SHA256` có sẵn trong .NET |
| Bỏ dấu tiếng Việt khi sinh slug (Issue #5) | `string.Normalize(NormalizationForm.FormD)` có sẵn trong .NET |

---

# Phụ lục

## Bảng tổng hợp 13 Issue

| # | Tên | Khu | Use case | Độ khó | Phụ thuộc |
|---|-----|-----|----------|--------|-----------|
| 1 | Session & Phân quyền | A | — | 🟡 | — |
| 2 | Layout & Theme | B | — | 🟡 | — |
| 3 | Tài khoản | A | UC07–09 | 🟡 | #1 |
| 4 | Trang đọc | B | UC01,02,05 | 🟡 | #2 |
| 5 | Quản lý bài viết | C | UC15–19,21 | 🔴 | #1 |
| 6 | Upload & Sanitize | C | UC20 | 🔴 | #5 |
| 7 | Bình luận | D | UC10,11 | 🔴 | #3,#4 |
| 8 | Kiểm duyệt bình luận | D | UC22 | 🟡 | #7 |
| 9 | Like / Bookmark | D | UC12–14 | 🟢 | #3,#4 |
| 10 | Tìm kiếm & Lọc | B | UC03,04 | 🟡 | #4 |
| 11 | Tuỳ biến giao diện | B | UC24 | 🟡 | #2,#3 |
| 12 | Thống kê | C | UC06,23 | 🔴 | #4,#5 |
| 13 | Quản trị | A | UC25–28 | 🟡 | #3,#5,#7 |

## ⚠️ Quy ước tránh đụng file — ĐỌC TRƯỚC KHI CODE

Trang chi tiết bài viết là chỗ 3 khu cùng cần chạm vào. Nếu không có quy ước,
3 người sẽ sửa cùng `Detail.cshtml`, `BlogController.cs`, `PostDetailViewModel.cs`
ở 3 branch khác nhau → **chắc chắn conflict khi merge**.

Bốn quy ước dưới đây đã được áp sẵn vào code trong repo:

### 1. `PostDetailViewModel` là contract chung — chỉ Khu B được sửa

File `ViewModel/PostDetailViewModel.cs` đã khai báo **đầy đủ** property cho cả 3 khu,
kể cả property chưa dùng tới. Khu C và Khu D chỉ **đọc**, thiếu gì báo Khu B thêm.

### 2. Trang Detail chỉ là khung, nội dung nằm trong partial

`Areas/User/Views/Blog/Detail.cshtml` (Khu B) chỉ gọi partial:

```razor
@await Html.PartialAsync("_LikeBar", Model)
@await Html.PartialAsync("_CommentTree", Model)
```

### 3. Menu tài khoản tách khỏi `_Layout`

`Views/Shared/_Layout.cshtml` (Khu B) gọi `@await Html.PartialAsync("_AccountMenu")`.

### 4. Ghi nhận lượt xem: Khu B gọi hộ, Khu C chỉ viết service

Khu B thêm sẵn 1 dòng trong action `Detail` ngay từ đầu:

```csharp
await analyticsService.RecordViewAsync(post.Id, HttpContext);
```

Khu C viết phần thân hàm trong `AnalyticsService.cs`, **không mở `BlogController.cs`**.

---

## Bảng chủ sở hữu file — mỗi file đúng 1 khu

| File | Chủ sở hữu | Khu khác được làm gì |
|------|-----------|----------------------|
| `ViewModel/PostDetailViewModel.cs` | **Khu B** | Chỉ đọc |
| `Areas/User/Views/Blog/Detail.cshtml` | **Khu B** | Không mở |
| `Areas/User/Controllers/BlogController.cs` | **Khu B** | Không mở |
| `Views/Shared/_Layout.cshtml` | **Khu B** | Không mở |
| `Areas/*/Views/_ViewStart.cshtml` | **Khu B** | Không mở |
| `Areas/User/Views/Shared/_LikeBar.cshtml` | **Khu D** | Không mở |
| `Areas/User/Views/Shared/_CommentTree.cshtml` | **Khu D** | Không mở |
| `Areas/User/Views/Shared/_CommentItem.cshtml` | **Khu D** | Không mở |
| `ViewModel/CommentViewModel.cs` | **Khu D** | Chỉ đọc |
| `Views/Shared/_AccountMenu.cshtml` | **Khu A** | Không mở |
| `Program.cs` | **Khu A** | Không mở (xem ghi chú dưới) |

> **`Program.cs`:** 10 dòng đăng ký service **đã bỏ comment sẵn** khi dựng sườn, cùng với
> 4 route URL dạng slug (`/post/{slug}`, `/author/{username}`, `/category/{slug}`, `/tag/{slug}`).
> Cả 4 khu đều không cần mở file này nữa. Trường hợp duy nhất phải mở: sau khi nhóm duyệt
> việc bổ sung `ITaxonomyService` thì Khu A bỏ comment nốt dòng thứ 11.

## Khu B là nút thắt — cần biết trước

Ba khu còn lại đều chờ Khu B đóng băng `PostDetailViewModel` và `Detail.cshtml`.
Nên Khu B phải làm Issue #4 sớm, ngay sau Issue #2.

Đổi lại, chỉ mình Khu B phải chạy trước — thay vì cả nhóm chờ lẫn nhau.

## Tạo database — làm 1 lần trên máy mỗi người

**Bước 1** — tạo file `appsettings.Development.json` (file này không có trên git),
copy mẫu connection string trong `appsettings.json` rồi sửa cho khớp máy mình.

**Bước 2** — tạo bảng và dữ liệu mẫu. Chọn **1 trong 2 cách**, kết quả như nhau:

| | Cách A — file SQL | Cách B — Code First |
|---|---|---|
| Làm gì | Mở `Database/SeedData.sql` trong SSMS, bấm F5 | Chạy `dotnet ef database update`, rồi chạy riêng phần 3 của `SeedData.sql` nếu muốn dữ liệu mẫu |
| Được gì | Tạo database + 12 bảng + dữ liệu mẫu trong 1 lần | 12 bảng trống, sinh từ Entity class trong `Models/` |
| Hợp với | Ai chỉ cần DB chạy được ngay, không rành lệnh `dotnet ef` | Ai cần sửa Entity rồi sinh migration |

Cách A đã ghi sẵn lịch sử migration vào bảng `__EFMigrationsHistory`,
nên **không cần** chạy `dotnet ef database update` sau đó.

> Dự án **không** có seeder tự chạy lúc khởi động — `dotnet run` không tự chèn dữ liệu.
> Muốn thêm dữ liệu mẫu thì sửa phần 3 của `Database/SeedData.sql`.

Dữ liệu mẫu gồm:

| Bảng | Số dòng |
|------|---------|
| Roles | 3 (Admin, Author, Reader) |
| Users | 4 tài khoản mẫu, mật khẩu đều `Admin@123` |
| Categories | 5 |
| Tags | 8 |
| Posts | 4 (3 bài đã đăng, 1 bài nháp) |
| Comments | 4 (có 1 comment trả lời comment khác) |
| PostLikes / Bookmarks | 5 / 2 |
| BlogSettings | 2 tác giả có theme riêng |

### Khi cần đổi cấu trúc bảng giữa chừng

Chỉ sửa **một nơi duy nhất** là Entity class, rồi để EF Core lo phần còn lại:

```bash
# 1. Sửa file trong Models/, ví dụ thêm cột IsFeatured vào Post.cs
# 2. Sinh migration mô tả thay đổi
dotnet ef migrations add ThemCotIsFeatured
# 3. Áp lên database
dotnet ef database update
```

**Quy tắc cho cả nhóm:**

- Báo trước khi đổi bảng — thay đổi ảnh hưởng nhiều khu
- Commit file migration cùng với thay đổi Entity class, **không bỏ sót**
- Người khác `git pull` xong chỉ cần chạy `dotnet ef database update` là DB khớp lại
- Đặt tên migration bằng tiếng Việt không dấu, mô tả rõ việc: `ThemCotIsFeatured`,
  `DoiKieuSlug` — không đặt `Update1`, `Fix2`
- Sửa xong nhớ cập nhật `docs/blog-platform-erd.md` cho khớp
