# Roohollah Jafari — ASP.NET Core Razor Pages + Docker + PostgreSQL

نسخه کامل ASP.NET Core Razor Pages قالب هتل **Luxivo** با برند **Roohollah Jafari** —
شامل دیتابیس، فرم‌های واقعی، REST API، پنل ادمین و اجرای داکر.

## اجرا با Docker (پیشنهادی)

```bash
cd RoohollahJafari.Web
docker compose up -d --build
```

| سرویس | آدرس | توضیح |
|---|---|---|
| وب‌سایت | http://localhost:5088 | اپلیکیشن ASP.NET Core (کانتینر `roohollah-web`) |
| پنل ادمین | http://localhost:5088/admin/messages | نمایش داده‌های دیتابیس |
| سلامت API | http://localhost:5088/api/health | وضعیت دیتابیس |
| PostgreSQL | localhost:5432 | دیتابیس (کانتینر `roohollah-db`) |

```bash
docker compose ps        # وضعیت سرویس‌ها
docker compose logs -f   # لاگ‌ها
docker compose down      # توقف (با -v دیتابیس هم پاک می‌شه)
```

اجرای بدون داکر (نیاز به PostgreSQL/SQL Server محلی):

```bash
dotnet run --project RoohollahJafari.Web
```

## معماری

```
RoohollahJafari.Web/
├── Dockerfile                 # multi-stage build (sdk:10.0 → aspnet:10.0)
├── docker-compose.yml         # web + postgres17 + healthcheck + volume
├── Program.cs                 # مسیریابی، DI دیتابیس، REST API، seed
├── Models/Entities.cs         # ContactMessage, BookingRequest, BlogComment, Subscriber
├── Data/AppDbContext.cs       # EF Core + EnsureCreated + seed داده نمونه
└── Pages/
    ├── _Layout.cshtml         # هدر/فوتر/منو/اسکریپت مشترک
    ├── Shared/                # _Header + سه فوتر (v1/v2/v3)
    ├── Admin/Messages.cshtml  # داشبورد ادمین (جدول‌های دیتابیس)
    └── *.cshtml + *.cshtml.cs #21 صفحه سایت + PageModel ها
```

### اتصال دیتابیس

از طریق تنظیمات (docker-compose یا appsettings):

```
Database__Provider              = Postgres | SqlServer
ConnectionStrings__Default      = رشته اتصال
```

ساختار جدول‌ها با `EnsureCreated` خودکار ساخته می‌شه و داده نمونه seed می‌شه.
نمونه SQL Server در `docker-compose.yml` به صورت کامنت موجوده.

### فرم‌های متصل به دیتابیس (همه تست شده)

| فرم | صفحه | جدول |
|---|---|---|
| تماس | Contact، Team Details | `ContactMessages` |
| رزرو | Home One (فرم اصلی + سرچ)، Home Three، About، Room Details | `BookingRequests` |
| کامنت | Blog Details | `BlogComments` |
| خبرنامه (فوتر همه صفحات) | هر صفحه | `Subscribers` |

فرم سرچ هدر، اتاق‌ها را با تاریخ ریدایرکت می‌کنه و بنر نتیجه نشون میده.
فرم جستجوی بلاگ، پست‌ها را فیلتر می‌کنه.
فرم‌ها از antiforgery هم محافظت می‌شن.

### REST API

| متد | مسیر | توضیح |
|---|---|---|
| GET | `/api/health` | سلامت سیستم + دیتابیس |
| GET | `/api/contact` | لیست پیام‌ها |
| POST | `/api/contact` | ثبت پیام جدید (JSON) |
| GET | `/api/contact/{id}` | یک پیام |
| GET | `/api/bookings` | لیست رزروها |
| GET | `/api/comments` | لیست کامنت‌ها |
| GET | `/api/subscribers` | لیست مشترکین |
| POST | `/subscribe` | عضویت خبرنامه (فرم) |

## اسکرین‌شات‌ها

پوشه [`../screenshots/`](../screenshots/) شامل ۱۹ اسکرین‌شات از بخش‌های مختلف
پروژه (خانه، اتاق‌ها، گالری، بلاگ، فرم تماس، 404، داشبورد ادمین و API)
برای استفاده در سایت و نمایش نمونه‌کار.

## تغییر برند

- عنوان صفحات، متا توضیحات، کپی‌رایت `© 2026 Roohollah Jafari`
- ایمیل‌ها: `info@roohollah-jafari.ir`
- لوگو: `wwwroot/assets/images/logo/logo-white.png` و `logo-main.png`
  (متنی «ROOHOLLAH JAFARI» — برای تعویض با لوگوی گرافیکی جایگزین کنید)
- همه عکس‌های placeholder قالب با عکس واقعی هتل جایگزین شدن
  (منبع: ریپوهای عمومی GitHub — برای استفاده تجاری عکس‌های خودتان را جایگزین کنید)

## نکات

- کلاس‌های CSS با `luxivo-` شروع می‌شن — تغییرشان ندهید
- آدرس صفحات `.html` حفظ شده (مانند قالب اصلی) — `/index.html` به `/` ریدایرکت می‌شه
- آدرس‌های ناموجود صفحه 404 با کد واقعی404 نشون میدن
- شماره تلفن و آدرس در هدر/فوتر هنوز placeholder است
