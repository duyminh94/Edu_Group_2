# Dàn ý tầng nghiệp vụ — BlogPlatform

> **Tài liệu này là bản dàn ý để nhóm duyệt, chưa phải code.**
> Sau khi 4 thành viên thống nhất, mỗi người tự gõ phần khu mình vào file `.cs` tương ứng.
>
> Đọc cùng: [Quy tắc nghiệp vụ](./business-rules.md) · [ERD](./blog-platform-erd.md) · [Use Case](./blog-platform-usecase.md) · [Phân công Issue](./github-issues.md)

---

## 0. Vì sao cần tài liệu này

`business-rules.md` đã định nghĩa **quy tắc** nghiệp vụ (bài mới luôn Draft, comment mới luôn Pending…),
nhưng phần "Chữ ký hàm đề xuất" trong đó mới là bản nháp: còn thiếu 2 service, còn 4 chỗ tự mâu thuẫn,
và chưa thống nhất kiểu trả về. Bốn người cứ thế code song song sẽ khai báo lệch nhau, tới lúc merge mới vỡ.

Tài liệu này chốt đúng ba thứ:

1. **Quy ước dùng chung** — kiểu trả về, cách báo lỗi, cách viết truy vấn
2. **Danh sách đầy đủ 11 service** và chữ ký từng hàm
3. **Danh sách ViewModel** kèm ai sở hữu file nào

---

## 1. Kiến trúc đã chốt

```
Controller  →  Service  →  BlogDbContext  →  SQL Server
   mỏng         dày          truy vấn
```

Dự án đi theo pattern **CoreDay04 / CoreDay05**: Service gọi thẳng `DbContext`, **không có tầng Repository**
(lý do đã ghi ở `business-rules.md` §0.1).

Ba kỹ thuật lấy từ bài học vẫn giữ nguyên:

| Kỹ thuật | Học ở đâu | Áp dụng vào đâu |
|----------|-----------|-----------------|
| `IQueryable` + nối `.Where()` rồi mới `.ToListAsync()` (deferred execution) | CoreFirstDay03 — `StudentService.GetFilteredScoresAsync` | `SearchService`, mọi hàm có bộ lọc |
| `Select()` chiếu Entity sang ViewModel ngay trong câu SQL (projection) | CoreFirstDay03 | Mọi hàm trả danh sách |
| Load entity → chỉ gán field được phép sửa → `SaveChanges` (chống over-posting) | CoreFirstDay03 — `UpdateScoreAsync` | `PostService.UpdateAsync`, `AccountService.UpdateProfileAsync` |
| Tách việc phụ xuống hàm `private` trong cùng Service | CoreDay04 — `SaveImageAsync`, `DeleteImage` | `MediaService`, mọi Service dài |
| `BeginTransactionAsync` khi ghi dữ liệu gốc + cập nhật bộ đếm | CoreDay05 — `EventService.RegisterEventAsync` | `InteractionService`, `AnalyticsService`, `CommentService` |
| Service trả `string` trạng thái thay vì `throw` | CoreDay05 | Mọi hàm ghi (Create / Update / Delete) |

Một thứ **cố ý không mang sang**: `Task<object>` + `dynamic ViewBag.Stats` của `CoreDay05.EventService.GetStatisticsAsync`.
Kiểu `dynamic` mất hết gợi ý của trình biên dịch, gõ sai tên property tới lúc chạy mới biết.
Thay bằng `AnalyticsViewModel` có kiểu rõ ràng.

---

## 2. Quy ước dùng chung — mọi khu phải theo

### 2.1. Kiểu trả về

| Loại hàm | Trả về | Vì sao |
|----------|--------|--------|
| **Ghi** (Create / Update / Delete / Publish / Approve…) | `Task<string>` | `"SUCCESS"` hoặc câu thông báo lỗi tiếng Việt. Thống nhất theo `business-rules.md` §0 |
| **Đọc danh sách** | `Task<List<XxxListItemViewModel>>` | Projection, **không kéo cột `Content`** kiểu `nvarchar(max)` |
| **Đọc 1 bản ghi để hiển thị chi tiết hoặc đổ lên form sửa** | `Task<Post?>`, `Task<User?>` (Entity) | Cần đủ field, và luồng sửa cần chính entity đó |
| **Đọc cả màn hình** (nhiều mảnh dữ liệu) | `Task<XxxViewModel>` | Một View chỉ nhận được 1 `@model` |
| **Kiểm tra đúng/sai** | `Task<bool>` | `IsOwnerAsync`, `IsLikedAsync` |
| **Toggle** (Like / Bookmark) | `Task<ToggleResultViewModel>` | Xem §3, mục 4 |

> **Quy tắc nhớ nhanh:** danh sách → ViewModel, chi tiết → Entity, ghi → `string`.

### 2.2. Ba quy tắc xuyên suốt (nhắc lại từ `business-rules.md` §8)

| # | Quy tắc |
|---|---------|
| 8.1 | Mọi hàm sửa/xoá nhận `currentUserId` và kiểm tra quyền sở hữu **trước** khi thao tác — chống IDOR |
| 8.2 | Ghi dữ liệu gốc + cập nhật bộ đếm (`ViewCount`, `LikeCount`, `CommentCount`) phải nằm trong **cùng 1 transaction** |
| 8.3 | Mọi truy vấn chỉ đọc phải có `AsNoTracking()` |

### 2.3. Quy ước sở hữu file

Mỗi file `.cs` có **đúng một khu sở hữu**. Khu khác cần thêm property thì báo chủ file, không tự sửa —
đây là nguyên nhân merge conflict phổ biến nhất khi 4 người làm song song.

---

## 3. Bốn điểm cần sửa so với `business-rules.md`

Bốn chỗ dưới đây là mâu thuẫn nội tại của bản nháp chữ ký. Nhóm duyệt xong thì sửa luôn vào `business-rules.md`.

### 1. `RecordViewAsync` không được nhận `HttpContext`

`business-rules.md` §0 ghi Service không đụng `HttpContext`, nhưng chữ ký đề xuất lại là
`RecordViewAsync(int postId, HttpContext httpContext)`.

| | |
|---|---|
| **Bỏ** | `Task RecordViewAsync(int postId, HttpContext httpContext)` |
| **Dùng** | `Task RecordViewAsync(int postId, string ipHash, int? currentUserId)` |
| **Vì sao** | Controller lấy IP và băm SHA-256 (quy tắc 4.3) rồi truyền xuống. Service chỉ nhận dữ liệu thuần → giữ đúng ranh giới tầng, và không tốn thêm công gì |

### 2. `ToggleLikeAsync` trả `string` là thiếu dữ liệu

Bấm Like xong View cần biết **đã like chưa** và **`LikeCount` mới**. Trả `"SUCCESS"` thì Controller
buộc phải query lại — thừa một vòng gọi DB trên hành động được bấm nhiều nhất hệ thống.

| | |
|---|---|
| **Bỏ** | `Task<string> ToggleLikeAsync(int postId, int userId)` |
| **Dùng** | `Task<ToggleResultViewModel> ToggleLikeAsync(int postId, int userId)` |
| **`ToggleResultViewModel` gồm** | `bool IsSuccess` · `string Message` · `bool IsActive` (sau khi toggle) · `int NewCount` |

### 3. Hàm trả danh sách không được trả `List<Post>`

`business-rules.md` §6 ghi rõ *"danh sách kết quả không `Select` cột `Content`"*, nhưng chữ ký đề xuất
lại là `Task<List<Post>> GetPublishedAsync(...)` — trả `Post` là kéo luôn `Content`.

| | |
|---|---|
| **Bỏ** | `Task<List<Post>> GetPublishedAsync(int page, int pageSize)` |
| **Dùng** | `Task<List<PostListItemViewModel>> GetPublishedAsync(int page, int pageSize)` |

### 4. Thiếu hẳn service quản lý chuyên mục và thẻ

Bảng §7 của `business-rules.md` liệt kê 10 service, không service nào sở hữu CRUD `Category` / `Tag` —
nhưng Issue #13 yêu cầu ("Quản lý chuyên mục và thẻ: thêm, sửa, xoá") và `Areas/Admin/Controllers/TaxonomyController.cs`
đã tồn tại sẵn trong project. Ngoài ra form tạo bài (Issue #5) cũng cần lấy danh sách chuyên mục đổ vào dropdown.

→ **Bổ sung service thứ 11: `ITaxonomyService`** (xem §5.11).

---

## 4. Dàn ý ViewModel

### 4.1. File đã có nội dung — giữ nguyên

| File | Khu sở hữu | Ghi chú |
|------|:---:|---|
| `PostDetailViewModel.cs` | B | Contract dùng chung 3 khu, đã ghi rõ quy ước trong file |
| `CommentViewModel.cs` | D | Một bình luận + cây `Replies`, tối đa 3 cấp |

### 4.2. File đang rỗng — cần điền

| File | Khu | Property chính |
|------|:---:|---|
| `LoginViewModel.cs` | A | `UserName`, `Password`, `ReturnUrl` |
| `RegisterViewModel.cs` | A | `UserName`, `Email`, `DisplayName`, `Password`, `ConfirmPassword` |
| `PostEditViewModel.cs` | C | `Id`, `Title`, `Summary`, `Content`, `FeaturedImageUrl`, `CategoryId`, `TagNames`, `Categories` (SelectList), `Status` (chỉ đọc) |
| `PostListViewModel.cs` | B | `Posts`, `Page`, `TotalPages`, `PageTitle`, `Categories`, `Tags` |
| `SearchViewModel.cs` | B | `Keyword`, `CategorySlug`, `TagSlug`, `AuthorUserName`, `SortBy`, `Page`, `Results`, `TotalCount` |
| `AuthorProfileViewModel.cs` | B | `Author` (User), `Setting` (BlogSetting), `Posts`, `TotalPosts`, `IsOwnProfile` |
| `BlogSettingViewModel.cs` | B | `ThemeName`, `PrimaryColor`, `FontFamily`, `LogoUrl`, `Tagline`, `AvailableThemes`, `AvailableFonts` |
| `AnalyticsViewModel.cs` | C | `TotalPosts`, `TotalViews`, `TotalLikes`, `TotalComments`, `PostRows`, `ViewsByDay` |

### 4.3. File cần tạo mới

| File | Khu | Vì sao cần |
|------|:---:|---|
| `PostListItemViewModel.cs` | B (C dùng chung) | Kiểu trả về của mọi hàm đọc danh sách bài. **Không có property `Content`** — đây là điểm mấu chốt của quy tắc §6 |
| `UserListItemViewModel.cs` | A | Bảng quản lý user của Admin (Issue #13). Không có `PasswordHash` |
| `CommentListItemViewModel.cs` | D | Hàng chờ kiểm duyệt (Issue #8) — phẳng, kèm tên bài viết. Khác `CommentViewModel` vốn là cây cho người đọc |
| `ToggleResultViewModel.cs` | D | Kết quả Like / Bookmark (xem §3 mục 2) |
| `ViewsByDayViewModel.cs` | C | Thay cho `List<(DateTime, int)>` — tuple trong interface public khó đọc và khó bind trong Razor |

**Tổng: 10 file điền nội dung + 5 file tạo mới = 15 ViewModel.**

---

## 5. Dàn ý 11 Service

### 5.1. `IPasswordService` — Khu A · Issue #1 · quy tắc 3.4

```csharp
string Hash(string plainPassword);
bool Verify(string plainPassword, string hash);
```

- Dùng thư viện `BCrypt.Net-Next` (đã có trong `.csproj`). **Không** MD5, **không** SHA-1.
- Đây là service duy nhất không async và không đụng DB → đăng ký `AddSingleton`, không phải `AddScoped`.

### 5.2. `IHtmlSanitizerService` — Khu C · Issue #6 · quy tắc 2.7

```csharp
// Cho nội dung bài viết — cho phép nhiều thẻ định dạng
string SanitizePostContent(string html);

// Cho bình luận — chỉ cho <b> <i> <a> <br> (quy tắc 2.7)
string SanitizeCommentContent(string html);
```

- Dùng thư viện `HtmlSanitizer` (đã có trong `.csproj`).
- Hai hàm riêng vì hai mức độ cho phép khác nhau — dùng chung một hàm sẽ phải truyền cờ, rối hơn.
- Không đụng DB → `AddSingleton`.

### 5.3. `IAccountService` — Khu A · Issue #3, #13 · quy tắc 3.1–3.15

```csharp
// Đăng ký / đăng nhập
Task<string> RegisterAsync(RegisterViewModel model);          // 3.1, 3.2, 3.3, 3.5
Task<User?> ValidateLoginAsync(string userName, string password);  // null = thất bại (3.6, 3.7)
Task<User?> GetByUserNameAsync(string userName);
Task<string> UpdateProfileAsync(int userId, string displayName, string? bio, string? avatarUrl);

// Quản trị — Issue #13
Task<List<UserListItemViewModel>> GetAllAsync();
Task<string> ChangeRoleAsync(int userId, int newRoleId, int currentAdminId);   // 3.12, 3.15
Task<string> ToggleLockAsync(int userId, int currentAdminId);                  // 3.11, 3.13
```

- `ValidateLoginAsync` trả `User?` chứ không phải `string`: Controller cần chính đối tượng user
  để đổ vào Session (`UserId`, `UserName`, `DisplayName`, `RoleName` — quy tắc 3.8).
  Trả `null` cho **mọi** trường hợp thất bại, Controller hiện chung câu *"Sai tài khoản hoặc mật khẩu"* (quy tắc 3.6).
- `ChangeRoleAsync` phải chặn hạ role của Admin cuối cùng (3.12); `ToggleLockAsync` phải chặn Admin tự khoá mình (3.11)
  → đó là lý do cả hai nhận thêm `currentAdminId`.

### 5.4. `ITaxonomyService` — Khu A · Issue #13 · **bổ sung mới**

```csharp
// Đọc — Khu C gọi để đổ dropdown ở form tạo/sửa bài
Task<List<Category>> GetAllCategoriesAsync();
Task<List<Tag>> GetAllTagsAsync();
Task<Category?> GetCategoryBySlugAsync(string slug);
Task<Tag?> GetTagBySlugAsync(string slug);

// Ghi — Admin quản lý
Task<string> CreateCategoryAsync(string name, string? description);
Task<string> UpdateCategoryAsync(int id, string name, string? description);
Task<string> DeleteCategoryAsync(int id);      // SetNull — bài viết vẫn còn
Task<string> CreateTagAsync(string name);
Task<string> DeleteTagAsync(int id);           // Cascade — gỡ tag khỏi mọi bài

// Gắn tag khi lưu bài — Khu C gọi (UC19)
Task<List<int>> EnsureTagsAsync(List<string> tagNames);   // chưa có thì tạo, trả về danh sách TagId
```

- `Category` và `Tag` chỉ có 3–4 cột và không có nghiệp vụ phức tạp → gộp chung 1 service, không tách 2.
- Slug của Category/Tag sinh bằng cùng thuật toán slug của bài viết → dùng lại `IPostService.GenerateSlugAsync`.

### 5.5. `IPostService` — Khu C · Issue #5 · quy tắc 1.1–1.14

```csharp
// Đọc
Task<Post?> GetBySlugAsync(string slug);
Task<Post?> GetByIdAsync(int id);                        // đổ lên form sửa
Task<List<PostListItemViewModel>> GetPublishedAsync(int page, int pageSize);
Task<List<PostListItemViewModel>> GetByAuthorAsync(int authorId, PostStatus? status);

// Ghi
Task<string> CreateAsync(PostEditViewModel model, int authorId);   // 1.1, 1.10–1.14
Task<string> UpdateAsync(PostEditViewModel model, int currentUserId);  // 1.5, 1.6, 1.13
Task<string> DeleteAsync(int postId, int currentUserId);           // 1.8
Task<string> PublishAsync(int postId, int currentUserId);          // 1.2, 1.3, 1.4
Task<string> UnpublishAsync(int postId, int currentUserId);        // 1.7

// Dùng chung
Task<string> GenerateSlugAsync(string title);                      // 1.10–1.14
Task<bool> IsOwnerAsync(int postId, int userId);                   // 1.9
```

**Ba chỗ dễ sai nhất:**

| Chỗ | Sai thường gặp | Đúng |
|-----|----------------|------|
| Quy tắc 1.3 | Publish lại là gán `PublishedAt = DateTime.Now` | `PublishedAt ??= DateTime.Now` — chỉ gán khi đang `null` |
| Quy tắc 1.13 | Sửa tiêu đề thì sinh slug mới | Bài đã publish thì **giữ nguyên slug**, không thì hỏng link người khác đã chia sẻ |
| Quy tắc 1.9 | Chỉ kiểm tra `post.AuthorId == currentUserId` | Phải cho Admin qua luôn — nếu không Admin không gỡ được nội dung vi phạm (UC27) |

### 5.6. `IMediaService` — Khu C · Issue #6 · **bổ sung chữ ký mới**

```csharp
Task<string> UploadAsync(IFormFile file, int uploadedById, int? postId);  // trả URL hoặc câu lỗi
Task<List<MediaFile>> GetByUploaderAsync(int uploaderId);
Task<string> DeleteAsync(int mediaId, int currentUserId);
Task<string> AttachToPostAsync(int mediaId, int postId, int currentUserId);
```

- `business-rules.md` §7 có nhắc `IMediaService` nhưng không đưa chữ ký nào — phần này là bổ sung.
- Toàn bộ logic lấy từ `CoreDay04.ProductService`: kiểm tra đuôi file, kiểm tra dung lượng,
  đổi tên thành GUID, `Path.Combine` với `IWebHostEnvironment.WebRootPath`.
- Khác CoreDay04 ở hai điểm: giới hạn **5MB** (UC15 ngoại lệ 4a, không phải 2MB), và phải
  ghi thêm 1 dòng vào bảng `MediaFile` chứ không chỉ lưu đường dẫn vào cột của bài viết.

### 5.7. `ICommentService` — Khu D · Issue #7, #8 · quy tắc 2.1–2.11

```csharp
// Hiển thị cho người đọc
Task<List<CommentViewModel>> GetTreeByPostAsync(int postId, int? currentUserId);  // 2.3, 2.4, 2.5, 2.6
Task<string> CreateAsync(int postId, int userId, int? parentCommentId, string content);  // 2.1, 2.2, 2.6, 2.7, 2.11

// Kiểm duyệt — Issue #8
Task<List<CommentListItemViewModel>> GetPendingByAuthorAsync(int authorId);
Task<string> ApproveAsync(int commentId, int currentUserId);   // 2.8, 2.9
Task<string> RejectAsync(int commentId, int currentUserId);    // 2.8, 2.9
Task<string> FlagAsync(int commentId, int currentUserId);
Task<string> DeleteAsync(int commentId, int currentUserId);    // 2.10
Task<int> CountPendingAsync(int authorId);                     // hiện badge số trên menu
```

- `GetTreeByPostAsync` nhận `currentUserId` vì quy tắc 2.4: người gửi phải thấy comment `Pending`
  **của chính mình** kèm nhãn "Chờ duyệt", người khác thì không.
- Dựng cây bằng **1 query duy nhất** rồi ghép trong bộ nhớ (`business-rules.md` §Luồng 2) —
  không query lặp theo từng cấp.
- `ApproveAsync` / `RejectAsync` đụng `Post.CommentCount` → bắt buộc transaction (quy tắc 8.2).

### 5.8. `IInteractionService` — Khu D · Issue #9 · quy tắc 4.7–4.13

```csharp
Task<ToggleResultViewModel> ToggleLikeAsync(int postId, int userId);      // 4.7–4.11
Task<ToggleResultViewModel> ToggleBookmarkAsync(int postId, int userId);  // 4.11
Task<bool> IsLikedAsync(int postId, int userId);
Task<bool> IsBookmarkedAsync(int postId, int userId);
Task<List<PostListItemViewModel>> GetBookmarkedPostsAsync(int userId);
```

- Khoá chính ghép `(PostId, UserId)` trong ERD đã tự chặn like trùng (quy tắc 4.7) — **không cần** viết `if` kiểm tra.
- Cả hai hàm toggle đều sửa bộ đếm → transaction (quy tắc 4.9, 8.2).

### 5.9. `IAnalyticsService` — Khu C · Issue #12 · quy tắc 4.1–4.6

```csharp
// Khu B gọi từ BlogController.Detail, Khu C viết thân hàm
Task RecordViewAsync(int postId, string ipHash, int? currentUserId);   // 4.1–4.6

Task<AnalyticsViewModel> GetByAuthorAsync(int authorId);        // UC23
Task<AnalyticsViewModel> GetSystemWideAsync();                  // UC28, Issue #13
Task<List<ViewsByDayViewModel>> GetViewsByDayAsync(int postId, int soNgay);
```

- Chữ ký `RecordViewAsync` đã sửa theo §3 mục 1 — nhận `ipHash` thay vì `HttpContext`.
- Quy tắc 4.6 (*ghi view lỗi thì không chặn hiển thị bài*) → hàm này trả `Task` trần, nuốt lỗi
  bên trong và ghi log. Đây là **ngoại lệ duy nhất** được phép không trả `string`.
- `GetByAuthorAsync` và `GetSystemWideAsync` cùng trả `AnalyticsViewModel`, khác nhau ở phạm vi lọc.

### 5.10. `IBlogSettingService` — Khu B · Issue #11 · quy tắc 5.1–5.7

```csharp
Task<BlogSetting> GetByUserIdAsync(int userId);        // 5.2 — chưa có thì trả bản mặc định
Task<BlogSetting> GetByUserNameAsync(string userName); // dùng khi render /author/{username}
Task<string> UpdateAsync(int userId, BlogSettingViewModel model);   // 5.3, 5.7
List<string> GetAvailableFonts();                      // 5.5
bool IsValidHexColor(string color);                    // 5.4
```

- Hai hàm cuối không async vì không đụng DB — danh sách font là hằng số trong code.
- ⚠️ `PrimaryColor` và `FontFamily` được chèn thẳng vào thẻ `<style>` của `_Layout`.
  **Bắt buộc** `IsValidHexColor` và đối chiếu `GetAvailableFonts` trước khi lưu, nếu không là
  lỗ hổng chèn CSS tuỳ ý (cảnh báo ở `business-rules.md` §Luồng 5).

### 5.11. `ISearchService` — Khu B · Issue #10 · quy tắc 6.1–6.10

```csharp
Task<SearchViewModel> SearchAsync(SearchViewModel filter);   // 6.1–6.10
Task<List<PostListItemViewModel>> GetByCategoryAsync(string categorySlug, int page);
Task<List<PostListItemViewModel>> GetByTagAsync(string tagSlug, int page);
Task<List<PostListItemViewModel>> GetRelatedAsync(int postId, int soLuong);
```

- `SearchAsync` nhận và trả **cùng một kiểu**: `SearchViewModel` chứa cả điều kiện lọc lẫn kết quả.
  Nhờ vậy sau khi tìm, ô Search và các dropdown vẫn giữ nguyên giá trị cũ (quy tắc 6.7) —
  đúng cách `StudentViewModel` giữ lại `Filter` ở CoreFirstDay03.
- Đây là chỗ dùng kỹ thuật deferred execution rõ nhất: lấy `IQueryable` gốc → nối `.Where()` cho từng
  điều kiện có nhập → `Select()` sang `PostListItemViewModel` → `ToListAsync()`. Toàn bộ vẫn chỉ 1 câu SQL.
- Quy tắc 6.3 (tìm không dấu) là phần tốn công nhất. `business-rules.md` §9 đã ghi: thiếu thời gian thì bỏ quy tắc này trước tiên.

---

## 6. Đăng ký DI trong `Program.cs`

10 dòng đang bị comment ở `Program.cs` cần bổ sung thêm `ITaxonomyService`, thành 11 dòng:

| Service | Vòng đời | Vì sao |
|---------|----------|--------|
| `IPasswordService`, `IHtmlSanitizerService` | `AddSingleton` | Không đụng DB, không giữ trạng thái riêng theo request |
| 9 service còn lại | `AddScoped` | Có nhận `BlogDbContext` — mà `DbContext` là Scoped, service giữ nó không được sống lâu hơn |

> Theo Issue #1, **Khu A bỏ comment cả 11 dòng trong một lần** để 3 khu còn lại không phải mở file này nữa.

---

## 7. Thứ tự dựng

Có ràng buộc phụ thuộc thật sự: không định nghĩa ViewModel thì không khai báo được chữ ký interface.

```
Bước 1 — ViewModel   (15 file)   ← chặn tất cả
Bước 2 — Interface   (11 file)   ← chặn phần implement
Bước 3 — Class rỗng implement interface
Bước 4 — Program.cs: bỏ comment 11 dòng DI
Bước 5 — Mỗi khu code thân hàm phần mình
```

⚠️ **Lưu ý kỹ thuật ở bước 3:** khi interface đã có method mà class để trống thì **build sẽ lỗi**
(`CS0535: does not implement interface member`). Cách xử lý chuẩn cho scaffold là để mỗi method
`throw new NotImplementedException();` — build vẫn chạy, thành viên chỉ việc thay thân hàm.
Nhóm cần biết trước điều này để không tưởng là lỗi.

---

## 8. Việc cần nhóm xác nhận trước khi code

| # | Nội dung | Đề xuất |
|---|----------|---------|
| 1 | Bốn điểm sửa ở §3 | Duyệt rồi cập nhật lại `business-rules.md` cho khớp |
| 2 | Bổ sung `ITaxonomyService` cho Khu A | Khu A đang giữ 3 issue, nhẹ nhất trong 4 khu — thêm service này là hợp lý |
| 3 | Quy tắc 6.3 (tìm không dấu) | Chốt làm hay bỏ **trước** khi Khu B bắt đầu Issue #10, vì ảnh hưởng cả collation database |
| 4 | 7 quy tắc còn để ngỏ ở `business-rules.md` §9 | Duyệt một lượt trong buổi họp nhóm gần nhất |

## 9. Các chỗ lệch trong tài liệu

### Đã xử lý

| Chỗ | Vấn đề | Đã làm |
|-----|--------|--------|
| `github-issues.md` | Mục "Phụ lục" bị **lặp 2 lần**, bản thứ hai là bản cũ — còn ghi *"không dùng EF Core Migrations"* dù repo đã có thư mục `Migrations/` | Cắt bỏ bản trùng (127 dòng) |
| `github-issues.md` Issue #3 | Ghi *"Chạy `Database/BlogPlatform.sql`"* | Sửa thành `Database/SeedData.sql` |
| `github-issues.md` Issue #1 | Giao Khu A bỏ comment 10 dòng DI | Đã làm sẵn khi dựng sườn, cập nhật lại checklist |
| `README.md` | Ghi *"Services/ — 8 cặp interface"* | Sửa thành 10 cặp, thêm số ViewModel |
| `business-rules.md` | Không có chỗ nào trỏ tới bản chữ ký đã chốt | Thêm dòng cảnh báo ở đầu file |

### Đã chốt

**`BlogPlatform/docs/` là bản chuẩn duy nhất.** Thư mục `Edu/` bên ngoài repo chỉ là tài liệu
tham khảo lúc phân tích đề, **không đồng bộ ngược lại**. Sửa tài liệu thì sửa trong `docs/`,
đừng sửa bên `Edu/` rồi copy qua — hai bên đã lệch nhau và sẽ còn lệch tiếp.
