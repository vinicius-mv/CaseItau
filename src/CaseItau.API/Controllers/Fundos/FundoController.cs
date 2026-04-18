using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.ListarFundos;
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
        public async Task<ActionResult<IReadOnlyList<FundoResponse>>> Get()
        {
            var query = new ListarFundosQuery();
            var result = await _sender.Send(query);

            return Ok(result.Value);
        }

        // GET: api/Fundo/ITAUTESTE01
        [HttpGet("{codigo}")]
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
