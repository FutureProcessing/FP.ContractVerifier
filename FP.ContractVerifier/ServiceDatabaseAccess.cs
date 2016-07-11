using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using Dapper;

namespace ContractVerifier
{
    public static class ServiceDatabaseAccess
    {
        public static void ExecuteSqlQuery(string sqlQuery)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DatabaseContext"].ConnectionString))
            {
                connection.Execute(sqlQuery);
            }
        }

        public static void ExecuteSqlQueryInMasterContext(string sqlQuery)
        {
            // TODO: inject?
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MasterContext"].ConnectionString))
            {
                connection.Execute(sqlQuery);
            }
        }

        public static string CreateDatabaseBackup(string databaseName, string backupName)
        {
            var backupPath = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}.mdf", Directory.GetCurrentDirectory(), backupName);

            ExecuteSqlQueryInMasterContext(string.Format(
                CultureInfo.InvariantCulture,
                "IF EXISTS (select * from sys.databases where name='{0}') BACKUP DATABASE {0} TO DISK = '{1}' WITH FORMAT, NAME = '{2}';",
                databaseName,
                backupPath,
                backupName));

            return backupPath;
        }

        public static void RestoreDatabaseBackup(string dbName, string backupName, string backupPath)
        {
            if (string.IsNullOrEmpty(backupName))
            {
                return;
            }

            string restoreDatabaseFromBackup = @"ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
                                                 RESTORE DATABASE {0} FROM DISK = '{1}'  WITH REPLACE; 
                                                 ALTER DATABASE {0} SET MULTI_USER;";
            ExecuteSqlQueryInMasterContext(string.Format(CultureInfo.InvariantCulture, restoreDatabaseFromBackup, dbName, backupPath));
        }

        public static void DropDatabaseBackup(string backupPath)
        {
            File.Delete(backupPath);
        }
    }
}