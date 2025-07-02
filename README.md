Source: https://github.com/tquanglinh18/mail-marketing-api.git


# Chạy Project Local - Test
1. Khởi động SQL Server
2. Chạy lệnh tạo Migrations AppDbContext:
   dotnet ef migrations add AddDetailedSeedData --context AppDbContext
3. Update Migrations AppDbContext nếu thay đổi Models:
   dotnet ef database update --context AppDbContext
4. Chạy Project:
   dotnet watch --project mail-marketing-api
