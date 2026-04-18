using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.AdicionarFundo;
using CaseItau.Application.Fundos.AtualizarFundo;
using CaseItau.Application.Fundos.ListarFundos;
using CaseItau.Application.Fundos.MovimentarPatrimonioFundo;
using CaseItau.Application.Fundos.ObterFundo;
using CaseItau.Application.Fundos.RemoverFundo;
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

            if (result.IsFailure)
                return NotFound();

            return Ok(result.Value);
        }

        // POST: api/Fundo
        [HttpPost]
        public async Task<ActionResult> Post(AdicionarFundoRequest fundo, CancellationToken cancellationToken)
        {
            var command = new AdicionarFundoCommand(fundo.Codigo, fundo.Nome, fundo.Cnpj, fundo.CodigoTipo);
            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(
                actionName: nameof(Get),
                routeValues: new { codigo = result.Value },
                value: result.Value);
        }

        // PUT: api/Fundo/ITAUTESTE01
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, AtualizarFundoRequest request)
        {
            var command = new AtualizarFundoCommand(codigo, request.Nome, request.Cnpj, request.CodigoTipo);
            var result = await _sender.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        // DELETE: api/Fundo/ITAUTESTE01
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var command = new RemoverFundoCommand(codigo);
            var result = await _sender.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpPut("{codigo}/patrimonio")]
        public async Task<ActionResult> MovimentarPatrimonio(string codigo, [FromBody] decimal value, CancellationToken cancellationToken)
        {
            var command = new MovimentarPatrimonioFundoCommand(codigo, value);
            var result = await _sender.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }
    }
}
