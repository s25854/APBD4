using System;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication3
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IConfiguration _configuration;

        public WarehouseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Product> GetProductByIdAsync(int requestIdProduct)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();

            using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM Product WHERE IdProduct = @IdProduct";
            cmd.Parameters.AddWithValue("@IdProduct", requestIdProduct);

            var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Product
                {
                    IdProduct = (int)reader["IdProduct"],
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Price = (decimal)reader["Price"]
                };
            }

            return null;
        }

        public async Task<Warehouse> GetWarehouseByIdAsync(int requestIdWarehouse)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();

            using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            cmd.Parameters.AddWithValue("@IdWarehouse", requestIdWarehouse);

            var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Warehouse
                {
                    IdWarehouse = (int)reader["IdWarehouse"],
                    Name = reader["Name"].ToString(),
                    Address = reader["Address"].ToString()
                };
            }

            return null;
        }

        public async Task<Order> GetOrderAsync(int requestIdProduct, int requestAmount, DateTime requestCreatedAt)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();

            using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT * FROM \"Order\" " +
                              "WHERE IdProduct = @IdProduct " +
                              "AND Amount = @Amount " +
                              "AND CreatedAt <= @CreatedAt " +
                              "ORDER BY CreatedAt DESC";
            cmd.Parameters.AddWithValue("@IdProduct", requestIdProduct);
            cmd.Parameters.AddWithValue("@Amount", requestAmount);
            cmd.Parameters.AddWithValue("@CreatedAt", requestCreatedAt);

            var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var orderAsync = new Order
                {
                    IdOrder = (int)reader["IdOrder"],
                    IdProduct = (int)reader["IdProduct"],
                    Amount = (int)reader["Amount"],
                    CreatedAt = (DateTime)reader["CreatedAt"]
                };

                var fullFilledAt = reader["FulfilledAt"];
                if (!Convert.IsDBNull(fullFilledAt))
                {
                    orderAsync.FulfilledAt = (DateTime)fullFilledAt;
                }

                return orderAsync;
            }

            return null;
        }


        public async Task<int> UpdateOrderAsync(Order order)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();

            using var cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE \"Order\" SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
            cmd.Parameters.AddWithValue("@FulfilledAt", order.FulfilledAt);
            cmd.Parameters.AddWithValue("@IdOrder", order.IdOrder);

            var affectedCount = await cmd.ExecuteNonQueryAsync();
            return affectedCount;
        }

        public async Task<int> AddProductToWarehouseAsync(ProductWarehouse productWarehouse)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            await con.OpenAsync();

            using var transaction = con.BeginTransaction();
            try
            {
                using var cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.Transaction = transaction;

                cmd.CommandText =
                    "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                    "VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);" +
                    "SELECT SCOPE_IDENTITY();";
                cmd.Parameters.AddWithValue("@IdWarehouse", productWarehouse.IdWarehouse);
                cmd.Parameters.AddWithValue("@IdProduct", productWarehouse.IdProduct);
                cmd.Parameters.AddWithValue("@IdOrder", productWarehouse.IdOrder);
                cmd.Parameters.AddWithValue("@Amount", productWarehouse.Amount);
                cmd.Parameters.AddWithValue("@Price", productWarehouse.Price);
                cmd.Parameters.AddWithValue("@CreatedAt", productWarehouse.CreatedAt);

                var generatedId = await cmd.ExecuteScalarAsync();

                transaction.Commit();

                return Convert.ToInt32(generatedId);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> AddProductToWarehouseByStoredProcedureAsync(int idProduct, int idWarehouse, int amount,
            DateTime createdAt)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            {
                using (SqlCommand command = new SqlCommand("AddProductToWarehouse", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IdProduct", idProduct);
                    command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@CreatedAt", createdAt);

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@GeneratedProductWarehouseId";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParameter);


                    await con.OpenAsync();

                    using var transaction = con.BeginTransaction();

                    try
                    {
                        command.Transaction = transaction;

                        await command.ExecuteNonQueryAsync();

                        int generatedProductId = Convert.ToInt32(outputParameter.Value);

                        transaction.Commit();

                        return generatedProductId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }

    public interface IWarehouseRepository
    {
        Task<Product> GetProductByIdAsync(int requestIdProduct);
        Task<Warehouse> GetWarehouseByIdAsync(int requestIdProduct);
        Task<Order> GetOrderAsync(int requestIdProduct, int requestAmount, DateTime requestCreatedAt);
        Task<int> UpdateOrderAsync(Order order);
        Task<int> AddProductToWarehouseAsync(ProductWarehouse productWarehouse);
       
