using CaseItau.Domain.Abstractions;

namespace CaseItau.Domain.Fundos
{
    public class TipoFundo : Entity<int>
    {
        public int CodigoTipo { get; private set; }
        public string NomeTipo { get; private set; }

        public TipoFundo(int codigoTipo, string nomeTipo) 
            : base(codigoTipo)
        {
            CodigoTipo = codigoTipo;
            NomeTipo = nomeTipo;
        }

        // rehydration
        protected TipoFundo() { }
    }    
}
