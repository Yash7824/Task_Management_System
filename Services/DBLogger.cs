using Dapper;
using Npgsql;
using System;
using Task_Management_System.Constants;

namespace Task_Managament_System.Services
{
    public class DBLogger
    {
        public static async Task InsertLog(string? methodName, string? message, string? stackTrace, string? request, string? response, string? corelationID, string? userID)
        {
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "INSERT INTO DBLOGGER (METHOD_NAME, MESSAGE, STACK_TRACE, REQUEST, RESPONSE, CORELATION_ID, USER_ID, DATETIME) VALUES (@METHODNAME, @MESSAGE, @STACKTRACE, @REQUEST, @RESPONSE, @CORELATIONID, @USERID, @DATETIME)";
                    var parameters = new
                    {
                        METHODNAME = methodName,
                        MESSAGE = message,
                        STACKTRACE = stackTrace,
                        REQUEST = request,
                        RESPONSE = response,
                        CORELATIONID = corelationID,
                        USERID = userID,
                        DATETIME = DateTime.UtcNow
                    };
                    await dbConn.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred in DBLoggerDL.InsertLog(): {ex.Message}", ex);
            }
        }
    }
}
