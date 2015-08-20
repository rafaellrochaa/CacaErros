using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacaErros
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database();
            Console.Write("Digite o id da solicitação com erros, em seguida aperte enter: ");
            int solicitacao = Convert.ToInt16(Console.ReadLine());
            Console.Clear();
            Console.Write("Verificando consultas com erro, aguarde...");
            string consultasComErro = db.VerificarConsultasErroDesconhecidos(solicitacao);
            Console.Clear();
            string texto_final = consultasComErro.Length == 0 ? "Não há erros nesta consulta." : "Os id's de consultas com erros são: " + consultasComErro + '.' ;
            Console.WriteLine(texto_final);
            int espacos = 80;
            Console.WriteLine(new string('\n',21) + "Aperte Enter para finalizar...".PadLeft(espacos));
            Console.ReadKey();
            
        }
    }
}
