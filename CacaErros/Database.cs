using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.IO;
using System.IO.Compression;


namespace CacaErros
{
    public class Database
    {
        //Produção
        static string serverName = "postgresql02.agilus.com.br";
        static string port = "5432";
        static string userName = "agilus12";
        static string password = "post168421";
        static string databaseName = "agilus12";
        public string connectionString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", serverName, port, userName, password, databaseName);

        public string VerificarConsultasErroDesconhecidos(int idSolicitacao)
        {
            string idsConsultaErros = String.Empty, resultado = String.Empty;
            using (NpgsqlConnection conexao = new NpgsqlConnection(this.connectionString))
            {
                string consultarErros = @"select resultado, id_consulta
                                            from resultado as r
                                            inner join consulta as c on r.id_consulta = c.id
                                            where c.id_solicitacao = @idSolicitacao";
                NpgsqlCommand cmd = new NpgsqlCommand(consultarErros, conexao);
                cmd.Parameters.AddWithValue("@idSolicitacao", idSolicitacao);
                cmd.CommandTimeout = 120000;

                conexao.Open();
                NpgsqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    resultado = DescompactarResultado(dr[0].ToString());

                    if ( !resultado.Contains("<html><head><title>SIW") && (!(resultado.Contains("RV2") || resultado.Contains("CONBAS") || resultado.Contains("TITULAr"))))
                    {
                        idsConsultaErros += dr[1].ToString() + ", ";
                    }
                    resultado = String.Empty;
                }
            }
            
            if (idsConsultaErros.Length > 2)
            {
                idsConsultaErros = idsConsultaErros.Substring(0, idsConsultaErros.Length - 2);
            }

            return idsConsultaErros;
        }

        private static string DescompactarResultado(string resultadoCompactado)
        {
            byte[] gzBuffer = Convert.FromBase64String(resultadoCompactado);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
