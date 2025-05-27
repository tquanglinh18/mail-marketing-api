using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

public class CheckConnectDb
{
    private readonly string _connectionString;

    public CheckConnectDb(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public void TestConnection()
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            try
            {
                conn.Open();
                Console.WriteLine("✅ Kết nối thành công đến SQL Server!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi kết nối: " + ex.Message);
            }
        }
    }
}
