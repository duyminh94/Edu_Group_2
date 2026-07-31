# Use Case Diagram — Blogging Platform (Mini Project 2, Nhóm 2)

> Đi kèm tài liệu [blog-platform-erd.md](./blog-platform-erd.md).
> Kiến trúc: 1 website chung, mỗi content creator có trang cá nhân `/author/{username}` tuỳ biến được theme.

---

## 1. Danh sách Actor

| Actor | Mô tả | Kế thừa từ |
|-------|-------|------------|
| **Guest** | Khách vãng lai, chưa đăng nhập. Chỉ đọc nội dung công khai | — |
| **Reader** | Người dùng đã đăng nhập. Tương tác được với bài viết | Guest |
| **Author** | Content creator. Sở hữu và quản lý bài viết của mình | Reader |
| **Admin** | Quản trị viên toàn hệ thống | Author |

> **Quan hệ generalization (mũi tên rỗng):** Admin làm được mọi thứ Author làm,
> Author làm được mọi thứ Reader làm, Reader làm được mọi thứ Guest làm.
> Nhờ vậy không phải vẽ lại use case trùng cho từng actor.

---

## 2. Sơ đồ Use Case (PlantUML — dùng để nộp bài)

> Dán code dưới vào [plantuml.com/plantuml](https://www.plantuml.com/plantuml/uml/) để xuất ảnh PNG.

```plantuml
@startuml Blogging Platform - Use Case Diagram
left to right direction
skinparam packageStyle rectangle
skinparam actorStyle awesome

actor Guest as G
actor Reader as R
actor Author as A
actor Admin as AD

R --|> G
A --|> R
AD --|> A

rectangle "Blogging Platform" {

  package "Duyệt & Tìm kiếm" {
    usecase "UC01 - Xem danh sách bài viết" as UC01
    usecase "UC02 - Xem chi tiết bài viết" as UC02
    usecase "UC03 - Tìm kiếm bài viết" as UC03
    usecase "UC04 - Lọc & sắp xếp kết quả" as UC04
    usecase "UC05 - Xem trang cá nhân tác giả" as UC05
    usecase "UC06 - Ghi nhận lượt xem" as UC06
  }

  package "Tài khoản" {
    usecase "UC07 - Đăng ký tài khoản" as UC07
    usecase "UC08 - Đăng nhập / Đăng xuất" as UC08
    usecase "UC09 - Quản lý hồ sơ cá nhân" as UC09
  }

  package "Tương tác" {
    usecase "UC10 - Bình luận bài viết" as UC10
    usecase "UC11 - Trả lời bình luận" as UC11
    usecase "UC12 - Thích bài viết" as UC12
    usecase "UC13 - Lưu bài viết (Bookmark)" as UC13
    usecase "UC14 - Chia sẻ bài viết" as UC14
  }

  package "Quản lý nội dung" {
    usecase "UC15 - Tạo bài viết" as UC15
    usecase "UC16 - Sửa bài viết" as UC16
    usecase "UC17 - Xoá bài viết" as UC17
    usecase "UC18 - Publish / Unpublish" as UC18
    usecase "UC19 - Gắn chuyên mục & thẻ" as UC19
    usecase "UC20 - Upload ảnh / media" as UC20
    usecase "UC21 - Kiểm tra quyền sở hữu" as UC21
  }

  package "Kiểm duyệt & Thống kê" {
    usecase "UC22 - Kiểm duyệt bình luận" as UC22
    usecase "UC23 - Xem thống kê bài viết" as UC23
    usecase "UC24 - Tuỳ biến giao diện blog" as UC24
  }

  package "Quản trị hệ thống" {
    usecase "UC25 - Quản lý người dùng & phân quyền" as UC25
    usecase "UC26 - Quản lý chuyên mục & thẻ" as UC26
    usecase "UC27 - Gỡ nội dung vi phạm" as UC27
    usecase "UC28 - Xem thống kê toàn hệ thống" as UC28
  }
}

' ===== Guest =====
G --> UC01
G --> UC02
G --> UC03
G --> UC05
G --> UC07

' ===== Reader =====
R --> UC08
R --> UC09
R --> UC10
R --> UC12
R --> UC13
R --> UC14

' ===== Author =====
A --> UC15
A --> UC16
A --> UC17
A --> UC18
A --> UC22
A --> UC23
A --> UC24

' ===== Admin =====
AD --> UC25
AD --> UC26
AD --> UC27
AD --> UC28

' ===== include (bắt buộc xảy ra) =====
UC02 ..> UC06 : <<include>>
UC15 ..> UC19 : <<include>>
UC16 ..> UC21 : <<include>>
UC17 ..> UC21 : <<include>>
UC18 ..> UC21 : <<include>>
UC22 ..> UC21 : <<include>>

' ===== extend (tuỳ chọn) =====
UC04 ..> UC03 : <<extend>>
UC20 ..> UC15 : <<extend>>
UC20 ..> UC16 : <<extend>>
UC11 ..> UC10 : <<extend>>

@enduml
```

---

## 3. Sơ đồ rút gọn (Mermaid — xem nhanh ngay trong Markdown)

> UML chuẩn không có sơ đồ use case trong Mermaid, đây là bản mô phỏng để anh xem nhanh
> khi chưa muốn mở PlantUML.

```mermaid
flowchart LR
    G([Guest]) --> R([Reader]) --> A([Author]) --> AD([Admin])

    G --- UC01[UC01 Xem danh sách bài]
    G --- UC02[UC02 Xem chi tiết bài]
    G --- UC03[UC03 Tìm kiếm]
    G --- UC05[UC05 Xem trang tác giả]
    G --- UC07[UC07 Đăng ký]

    R --- UC08[UC08 Đăng nhập/xuất]
    R --- UC09[UC09 Hồ sơ cá nhân]
    R --- UC10[UC10 Bình luận]
    R --- UC12[UC12 Thích bài]
    R --- UC13[UC13 Bookmark]
    R --- UC14[UC14 Chia sẻ]

    A --- UC15[UC15 Tạo bài]
    A --- UC16[UC16 Sửa bài]
    A --- UC17[UC17 Xoá bài]
    A --- UC18[UC18 Publish/Unpublish]
    A --- UC22[UC22 Kiểm duyệt bình luận]
    A --- UC23[UC23 Thống kê bài viết]
    A --- UC24[UC24 Tuỳ biến giao diện]

    AD --- UC25[UC25 Quản lý người dùng]
    AD --- UC26[UC26 Quản lý chuyên mục/thẻ]
    AD --- UC27[UC27 Gỡ nội dung vi phạm]
    AD --- UC28[UC28 Thống kê hệ thống]

    UC02 -.include.-> UC06[UC06 Ghi nhận lượt xem]
    UC15 -.include.-> UC19[UC19 Gắn chuyên mục & thẻ]
    UC16 -.include.-> UC21[UC21 Kiểm tra quyền sở hữu]
    UC17 -.include.-> UC21
    UC20[UC20 Upload media] -.extend.-> UC15
    UC11[UC11 Trả lời bình luận] -.extend.-> UC10
    UC04[UC04 Lọc & sắp xếp] -.extend.-> UC03
```

---

## 4. Giải thích `<<include>>` và `<<extend>>`

Đây là 2 ký hiệu hay bị nhầm nhất khi vẽ use case:

| | `<<include>>` | `<<extend>>` |
|---|---|---|
| Ý nghĩa | **Luôn luôn** xảy ra | **Có thể** xảy ra, tuỳ điều kiện |
| Hướng mũi tên | Use case chính → use case con | Use case phụ → use case chính |
| Ví dụ trong bài | Xem chi tiết bài **luôn** ghi nhận lượt xem | Tạo bài viết **có thể** upload ảnh, cũng có thể không |

**Các quan hệ trong sơ đồ:**

| Quan hệ | Loại | Vì sao |
|---------|------|--------|
| UC02 → UC06 Ghi nhận lượt xem | include | Mở bài nào cũng phải đếm view |
| UC15 → UC19 Gắn chuyên mục & thẻ | include | Bài viết bắt buộc phải có phân loại |
| UC16/17/18/22 → UC21 Kiểm tra quyền sở hữu | include | ⚠️ **Bắt buộc** — chống lỗi IDOR (xem mục 6) |
| UC20 Upload media → UC15/UC16 | extend | Ảnh là tuỳ chọn |
| UC11 Trả lời bình luận → UC10 | extend | Có thể comment gốc, có thể trả lời |
| UC04 Lọc & sắp xếp → UC03 | extend | Tìm xong mới lọc thêm nếu muốn |

---

## 5. Đặc tả chi tiết 2 use case quan trọng

### UC15 — Tạo bài viết

| Mục | Nội dung |
|-----|----------|
| **Actor** | Author |
| **Tiền điều kiện** | Đã đăng nhập, có role `Author` hoặc `Admin` |
| **Hậu điều kiện** | Bản ghi mới trong bảng `Post` với `Status = 0 (Draft)` |
| **Luồng chính** | 1. Author chọn "Viết bài mới"<br>2. Hệ thống hiển thị form (tiêu đề, nội dung rich text, chuyên mục, thẻ, ảnh đại diện)<br>3. Author nhập nội dung<br>4. *(extend UC20)* Author upload ảnh nếu cần<br>5. *(include UC19)* Author chọn chuyên mục và gắn thẻ<br>6. Author bấm "Lưu nháp"<br>7. Hệ thống **sanitize HTML** trong nội dung<br>8. Hệ thống sinh `Slug` từ tiêu đề, kiểm tra trùng<br>9. Hệ thống lưu bài, chuyển về danh sách bài của tôi |
| **Luồng phụ** | 6a. Tiêu đề để trống → báo lỗi tại field, giữ nguyên dữ liệu đã nhập<br>8a. `Slug` bị trùng → tự thêm hậu tố số: `bai-viet-cua-toi-2` |
| **Ngoại lệ** | 4a. File upload sai định dạng hoặc quá 5MB → từ chối, hiện thông báo |

### UC22 — Kiểm duyệt bình luận

| Mục | Nội dung |
|-----|----------|
| **Actor** | Author (trên bài của mình), Admin (trên mọi bài) |
| **Tiền điều kiện** | Đã đăng nhập, có bình luận ở trạng thái `Pending` |
| **Hậu điều kiện** | `Comment.Status` được cập nhật; nếu duyệt thì `Post.CommentCount` tăng |
| **Luồng chính** | 1. Author mở trang "Quản lý bình luận"<br>2. Hệ thống liệt kê bình luận theo trạng thái<br>3. *(include UC21)* Hệ thống kiểm tra Author có sở hữu bài chứa bình luận đó không<br>4. Author chọn hành động: Duyệt / Từ chối / Gắn cờ<br>5. Hệ thống cập nhật `Status` và bộ đếm |
| **Luồng phụ** | 3a. Không sở hữu bài → trả về HTTP 403 Forbidden |
| **Ghi chú** | Bình luận mới mặc định `Status = 0 (Pending)`, chưa hiện với người đọc |

---

## 6. Vì sao có UC21 "Kiểm tra quyền sở hữu"?

Đây không phải chức năng người dùng nhìn thấy, nhưng **phải vẽ ra sơ đồ** vì đề chấm mục Security.

**Vấn đề (lỗi IDOR — Insecure Direct Object Reference):**
Author A đăng nhập, sửa URL từ `/Post/Edit/5` (bài của mình) thành `/Post/Edit/9` (bài của Author B).
Nếu code chỉ kiểm tra "đã đăng nhập chưa" mà không kiểm tra "có phải chủ bài không"
→ A sửa/xoá được bài của B.

**Cách chặn:** trong mọi action sửa/xoá/publish, so sánh `post.AuthorId` với ID người đang đăng nhập
trước khi cho thao tác.

---

## 7. Bảng tra: yêu cầu đề bài → Use Case

| Yêu cầu trong đề | Use Case đáp ứng |
|------------------|------------------|
| 2. User Registration and Authentication | UC07, UC08, UC09 |
| 3. Blog Post Management | UC15, UC16, UC17, UC18, UC19, UC20 |
| 4. Comment Management (moderation, threaded) | UC10, UC11, UC22 |
| 5. Customization and Theming | UC24 |
| 6. User Interaction (like, share, bookmark) | UC12, UC13, UC14 |
| 6. Analytics và statistics | UC06, UC23, UC28 |
| 7. Search Functionality (keyword, category, tag, author) | UC03, UC04 |

→ **28 use case phủ đủ 7 nhóm yêu cầu chức năng của đề.**

---

## 8. Việc cần làm tiếp

1. Xuất sơ đồ PlantUML ra ảnh PNG để chèn vào báo cáo
2. Dựng project ASP.NET Core MVC + cấu hình Session (tự quản đăng nhập)
3. Viết Entity classes theo [ERD](./blog-platform-erd.md) → `dotnet ef migrations add InitialCreate`
4. Seed 3 role (`Admin`, `Author`, `Reader`) + 1 tài khoản admin
