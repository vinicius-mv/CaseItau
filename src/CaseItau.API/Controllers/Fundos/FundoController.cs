using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.ObterFundo;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

namespace CaseItau.API.Controllers.Fundos
{
    [Route("api/[controller]")]
    [ApiController]
    public class FundoController : ControllerBase
    {
        private readonly ISender _sender;

        public FundoController(ISender sender)
        {
            _sender = sender;
        }

        // GET: api/Fundo
        [HttpGet]
        public IEnumerable<FundoResponse> Get()
        {
            var lista = new List<FundoResponse>();
            var con = new SQLiteConnection("Data Source=dbCaseItau.s3db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT F.*, T.NOME AS NOME_TIPO FROM FUNDO F INNER JOIN TIPO_FUNDO T ON T.CODIGO = F.CODIGO_TIPO";
            cmd.CommandType = System.Data.CommandType.Text;
            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                var f = new FundoResponse();
                f.Codigo = reader[0].ToString();
                f.Nome = reader[1].ToString();
                f.Cnpj = reader[2].ToString();
                f.CodigoTipo = int.Parse(reader[3].ToString());
                var patrimonioRaw = reader[4].ToString();
                if (decimal.TryParse(patrimonioRaw, out decimal patrimonio))
                {
                    f.Patrimonio = patrimonio;
                }
                f.NomeTipo = reader[5].ToString();                
                lista.Add(f);
            }
            return lista;
        }

        // GET: api/Fundo/ITAUTESTE01
        [HttpGet("{codigo}", Name = "Get")]
        public async Task<ActionResult<FundoResponse>> Get(string codigo, CancellationToken cancellationToken)
        {
            var query = new ObterFundoQuery(codigo);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }

        // POST: api/Fundo
        [HttpPost]
        public void Post([FromBody] FundoResponse value)
        {
            var con = new SQLiteConnection("Data Source=dbCaseItau.s3db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO FUNDO VALUES('" + value.Codigo + "','" + value.Nome + "','" + value.Cnpj + "',"+value.CodigoTipo.ToString() + ",NULL)";
            cmd.CommandType = System.Data.CommandType.Text;
            var resultado = cmd.ExecuteNonQuery();
        }

        // PUT: api/Fundo/ITAUTESTE01
        [HttpPut("{codigo}")]
        public void Put(string codigo, [FromBody] FundoResponse value)
        {
            var con = new SQLiteConnection("Data Source=dbCaseItau.s3db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE FUNDO SET Nome = '" + value.Nome + "', CNPJ = '" + value.Cnpj + "', CODIGO_TIPO = " + value.CodigoTipo + " WHERE CODIGO = '" + codigo + "'";
            cmd.CommandType = System.Data.CommandType.Text;
            var resultado = cmd.ExecuteNonQuery();
        }

        // DELETE: api/Fundo/ITAUTESTE01
        [HttpDelete("{codigo}")]
        public void Delete(string codigo)
        {
            var con = new SQLiteConnection("Data Source=dbCaseItau.s3db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "DELETE FROM FUNDO WHERE CODIGO = '" + codigo + "'";
            cmd.CommandType = System.Data.CommandType.Text;
            var resultado = cmd.ExecuteNonQuery();
        }

        [HttpPut("{codigo}/patrimonio")]
        public void MovimentarPatrimonio(string codigo, [FromBody] decimal value)
        {
            var con = new SQLiteConnection("Data Source=dbCaseItau.s3db");
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE FUNDO SET PATRIMONIO = IFNULL(PATRIMONIO,0) + " + value.ToString() + " WHERE CODIGO = '" + codigo + "'";
            cmd.CommandType = System.Data.CommandType.Text;
            var resultado = cmd.ExecuteNonQuery();
        }
    }
}
