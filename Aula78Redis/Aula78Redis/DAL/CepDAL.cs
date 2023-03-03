using Aula78Redis.Models;
using System.Data.SqlClient;

namespace Aula78Redis.DAL
{
    public class CepDAL
    {
        public static SqlConnection Conectar()
        {
            string stringConexao = "Data Source=localhost,1433;User ID=sa;Password=senha@1234;Initial Catalog=DBCep;";
            SqlConnection conectar = new SqlConnection(stringConexao);
            return conectar;
        }

        public async Task<CepDTO> ObterCEPBancoDeDadosSQl(string dadoCep)
        {
            var cep = new CepDTO();

            SqlConnection connection = Conectar();
            string query = "SELECT * FROM Cep WHERE cep = @CEP;";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            command.Parameters.AddWithValue("@CEP", dadoCep);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (!reader["CEP"].Equals(DBNull.Value)) cep.CEP = (String)reader["CEP"];
                if (!reader["CIDADE"].Equals(DBNull.Value)) cep.CIDADE = (String)reader["CIDADE"];
                if (!reader["ESTADO"].Equals(DBNull.Value)) cep.ESTADO = (String)reader["ESTADO"];
                if (!reader["BAIRRO"].Equals(DBNull.Value)) cep.BAIRRO = (String)reader["BAIRRO"];
                if (!reader["LOGRADOURO"].Equals(DBNull.Value)) cep.LOGRADOURO = (String)reader["LOGRADOURO"];
            }

            reader.Close();

            return cep;
        }

        public async Task<List<CepDTO>> ObterListaCEPBancoDeDadosSQl()
        {
            var ceps = new List<CepDTO>();
            CepDTO cep;

            SqlConnection connection = Conectar();
            string query = "SELECT * FROM Cep;";
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                cep = new CepDTO();
                if (!reader["CEP"].Equals(DBNull.Value)) cep.CEP = (String)reader["CEP"];
                if (!reader["CIDADE"].Equals(DBNull.Value)) cep.CIDADE = (String)reader["CIDADE"];
                if (!reader["ESTADO"].Equals(DBNull.Value)) cep.ESTADO = (String)reader["ESTADO"];
                if (!reader["BAIRRO"].Equals(DBNull.Value)) cep.BAIRRO = (String)reader["BAIRRO"];
                if (!reader["LOGRADOURO"].Equals(DBNull.Value)) cep.LOGRADOURO = (String)reader["LOGRADOURO"];

                ceps.Add(cep);
            }

            reader.Close();

            return ceps;
        }
        public async Task AlterarCEPBancoDeDadosSql(CepDTO cep)
        {
            SqlConnection connection = Conectar();
            string query = "UPDATE CEP SET CIDADE = @CIDADE, ESTADO = @ESTADO, BAIRRO = @BAIRRO, LOGRADOURO = @LOGRADOURO WHERE CEP = @CEP;";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CEP", cep.CEP);
            command.Parameters.AddWithValue("@CIDADE", cep.CIDADE);
            command.Parameters.AddWithValue("@ESTADO", cep.ESTADO);
            command.Parameters.AddWithValue("@BAIRRO", cep.BAIRRO);
            command.Parameters.AddWithValue("@LOGRADOURO", cep.LOGRADOURO);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
