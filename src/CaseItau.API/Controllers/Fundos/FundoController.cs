using CaseItau.Application.Fundos;
using CaseItau.Application.Fundos.AdicionarFundo;
using CaseItau.Application.Fundos.AtualizarFundo;
using CaseItau.Application.Fundos.ListarFundos;
using CaseItau.Application.Fundos.MovimentarPatrimonioFundo;
using CaseItau.Application.Fundos.ObterFundo;
using CaseItau.Application.Fundos.RemoverFundo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaseItau.API.Controllers.Fundos
{
    /// <summary>
    /// Gerenciamento de fundos de investimento.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Fundo")]
    public class FundoController : ControllerBase
    {
        private readonly ISender _sender;

        public FundoController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Lista todos os fundos cadastrados.
        /// </summary>
        /// <returns>Lista de fundos.</returns>
        /// <response code="200">Retorna a lista de fundos.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<FundoResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<FundoResponse>>> Get()
        {
            var query = new ListarFundosQuery();
            var result = await _sender.Send(query);

            return Ok(result.Value);
        }

        /// <summary>
        /// Obtém um fundo pelo seu código.
        /// </summary>
        /// <param name="codigo">Código único do fundo (ex: ITAURF321).</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Dados do fundo encontrado.</returns>
        /// <response code="200">Fundo encontrado com sucesso.</response>
        /// <response code="404">Fundo não encontrado.</response>
        [HttpGet("{codigo}")]
        [ProducesResponseType(typeof(FundoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FundoResponse>> Get(string codigo, CancellationToken cancellationToken)
        {
            var query = new ObterFundoQuery(codigo);
            var result = await _sender.Send(query, cancellationToken);

            if (result.IsFailure)
                return NotFound();

            return Ok(result.Value);
        }

        /// <summary>
        /// Cadastra um novo fundo de investimento.
        /// </summary>
        /// <param name="fundo">Dados do fundo a ser criado.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Código do fundo criado.</returns>
        /// <response code="201">Fundo criado com sucesso. Retorna o código do fundo.</response>
        /// <response code="400">Dados inválidos ou fundo já existente.</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Atualiza os dados de um fundo existente.
        /// </summary>
        /// <param name="codigo">Código único do fundo a ser atualizado.</param>
        /// <param name="request">Novos dados do fundo.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Fundo atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos ou fundo não encontrado.</response>
        [HttpPut("{codigo}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(string codigo, AtualizarFundoRequest request)
        {
            var command = new AtualizarFundoCommand(codigo, request.Nome, request.Cnpj, request.CodigoTipo);
            var result = await _sender.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        /// <summary>
        /// Remove um fundo de investimento.
        /// </summary>
        /// <param name="codigo">Código único do fundo a ser removido.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Fundo removido com sucesso.</response>
        /// <response code="400">Fundo não encontrado ou não pode ser removido.</response>
        [HttpDelete("{codigo}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string codigo)
        {
            var command = new RemoverFundoCommand(codigo);
            var result = await _sender.Send(command);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        /// <summary>
        /// Movimenta o patrimônio de um fundo (crédito ou débito).
        /// </summary>
        /// <param name="codigo">Código único do fundo.</param>
        /// <param name="value">Valor a movimentar. Positivo para crédito, negativo para débito.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Sem conteúdo.</returns>
        /// <response code="204">Patrimônio movimentado com sucesso.</response>
        /// <response code="400">Valor inválido ou fundo não encontrado.</response>
        [HttpPut("{codigo}/patrimonio")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
