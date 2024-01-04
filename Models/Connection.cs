using Microsoft.Data.SqlClient;

namespace iakademi47_proje.Models
{
    public class Connection
    {

        public static SqlConnection ServerConnect
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection("Server=DESKTOP-K4CLE4P;Database=iakademi47Core_Proje;trusted_connection=True;TrustServerCertificate=True;");
                return sqlConnection;
            }
        }

    }
}
