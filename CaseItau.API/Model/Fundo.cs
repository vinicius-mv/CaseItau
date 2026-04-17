using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseItau.API.Model
{
    public class Fundo
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public int CodigoTipo { get; set; }
        public string NomeTipo { get; set; }
        public decimal? Patrimonio { get; set; }
    }
}
