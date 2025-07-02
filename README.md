Source: https://github.com/tquanglinh18/mail-marketing-api.git


# Chạy Project Local - Test
1. Khởi động SQL Server
2. Thay các thông tin cần thiết trong chuỗi ConnectString tại appsetting.json
3. Chạy Project. Thành công thì làm tiếp bước sau
4. Chạy lệnh tạo Migrations AppDbContext:
   dotnet ef migrations add AddDetailedSeedData --context AppDbContext
5. Update Migrations AppDbContext nếu thay đổi Models:
   dotnet ef database update --context AppDbContext

   
