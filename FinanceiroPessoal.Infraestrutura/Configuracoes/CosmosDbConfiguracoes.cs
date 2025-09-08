using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceiroPessoal.Infraestrutura.Configuracoes
{
    public class CosmosDbConfiguracoes
    {
        public string Endpoint { get; set; }
        public string Chave { get; set; }
        public string NomeDatabase { get; set; }
        public string ContainerDespesas { get; set; }
        public string ContainerReceitas { get; set; }
        public string ContainerCategorias { get; set; }
    }
}
