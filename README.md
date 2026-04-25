# Case de Engenharia Itaú — .NET Core

> Solução desenvolvida como parte do processo seletivo de engenharia de software do Itaú Unibanco.  
> O projeto original (commit `88b7fb57`) foi completamente refatorado, corrigindo bugs críticos e elevando o código a padrões de qualidade de produção.
>
> O enunciado original do desafio está preservado em [`DESAFIO.md`](./DESAFIO.md).

---

## Índice

- [O Desafio Original](#o-desafio-original)
- [Problemas e Falhas Identificadas](#problemas-e-falhas-identificadas)
- [Solução Implementada](#solução-implementada)
- [Arquitetura](#arquitetura)
- [Stack Tecnológica](#stack-tecnológica)
- [Funcionalidades da API](#funcionalidades-da-api)
- [Qualidade e Testes](#qualidade-e-testes)
- [Observabilidade](#observabilidade)
- [Como Executar](#como-executar)

---

## O Desafio Original

O projeto de setup fornecia uma API de Fundos de Investimento em .NET com os seguintes requisitos:

| Endpoint | Descrição |
|---|---|
| `GET /api/Fundo` | Listar todos os fundos |
| `GET /api/Fundo/{codigo}` | Obter fundo pelo código |
| `POST /api/Fundo` | Cadastrar novo fundo |
| `PUT /api/Fundo/{codigo}` | Editar fundo existente |
| `DELETE /api/Fundo/{codigo}` | Excluir fundo |
| `PUT /api/Fundo/{codigo}/patrimonio` | Movimentar patrimônio |

O enunciado alertava que o código "faz mal uso dos objetos, não segue boas práticas e não possui qualidade", e pedia para **refatorar utilizando as melhores bibliotecas, práticas e patterns, garantindo a qualidade da aplicação**.

---

## Problemas e Falhas Identificadas

A análise do código original revelou uma série de problemas que vão desde bugs funcionais críticos até ausência completa de boas práticas de engenharia.

### Bug Crítico: GET retornava erro após POST

Este era o bug funcional descrito no enunciado. Ao inserir um novo fundo, o campo `PATRIMONIO` era salvo como `NULL`. Na leitura, o código tentava converter diretamente:

```csharp
// CÓDIGO ORIGINAL — BUGADO
f.Patrimonio = decimal.Parse(reader[4].ToString()); // NullReferenceException quando PATRIMONIO é NULL
```

Qualquer chamada ao `GET` após um `POST` resultava em uma exceção não tratada, derrubando toda a listagem de fundos.

### Injeção de SQL (Vulnerabilidade de Segurança)

Todo o acesso ao banco era feito via concatenação direta de strings nos comandos SQL, abrindo o sistema para ataques de SQL Injection:

```csharp
// CÓDIGO ORIGINAL — VULNERÁVEL
cmd.CommandText = "SELECT ... WHERE F.CODIGO = '" + codigo + "'";
cmd.CommandText = "INSERT INTO FUNDO VALUES('" + value.Codigo + "','" + value.Nome + "',...)";
cmd.CommandText = "UPDATE FUNDO SET Nome = '" + value.Nome + "'...";
cmd.CommandText = "DELETE FROM FUNDO WHERE CODIGO = '" + codigo + "'";
```

### Ausência de Arquitetura e Separação de Responsabilidades

Todo o código estava concentrado em um único `FundoController` com ~100 linhas. O controller acumulava responsabilidades que deveriam pertencer a camadas distintas:

- Abertura e gerenciamento de conexões com o banco
- Construção de queries SQL
- Mapeamento de resultados
- Regras de negócio

### Ausência de Tratamento de Erros

Nenhum método retornava código HTTP adequado para erros. Todos eram `void` ou retornavam `null`, sem validação, sem status code, sem mensagem de erro:

```csharp
// CÓDIGO ORIGINAL — SEM RETORNO, SEM ERRO
public void Post([FromBody] Fundo value) { ... }
public void Delete(string codigo) { ... }
public Fundo Get(string codigo) { return null; } // Retorna null silenciosamente se não encontrar
```

### Ausência de Validação de Entrada

Nenhum campo era validado antes de ser persistido: CNPJ poderia ser qualquer string, o código do fundo poderia ser vazio, datas inválidas, etc.

### Conexões sem Gerenciamento (Resource Leak)

Cada método criava uma `SQLiteConnection` sem nunca fechá-la ou descartá-la (`using`), causando vazamento de recursos:

```csharp
// CÓDIGO ORIGINAL — SEM DISPOSE
var con = new SQLiteConnection("Data Source=dbCaseItau.s3db");
con.Open();
// connection nunca é fechada
```

### Sem Injeção de Dependência

A string de conexão estava hardcoded dentro de cada método. Nenhuma abstração, nenhum `IConfiguration`, nenhum DI Container sendo utilizado corretamente.

### Sem Testes

Zero cobertura de testes — sem testes unitários, de integração ou funcionais.

### Sem Logging

Nenhum tipo de log estruturado ou rastreabilidade de requisições.

### Banco de Dados SQLite

O SQLite é impróprio para ambientes de produção ou para simular cargas reais em um sistema financeiro.

### Código Síncrono

Todos os métodos eram síncronos (`IEnumerable<Fundo>`, `void`), bloqueando threads do servidor sob carga.

---

## Solução Implementada

A solução foi construída do zero sobre os princípios de **Clean Architecture**, **DDD**, **CQRS** e cobertura completa de testes, organizando o código em camadas bem definidas e de baixo acoplamento.

### Estrutura da Solução

```
case_itau_net_core/
├── src/
│   ├── CaseItau.Domain/          # Entidades, Value Objects, Eventos, Erros, Interfaces
│   ├── CaseItau.Application/     # Casos de uso: Commands, Queries, Handlers, Validators
│   ├── CaseItau.Infra/           # EF Core, Dapper, PostgreSQL, Migrations, Seed
│   └── CaseItau.API/             # Controllers, Middlewares, Program.cs
├── test/
│   ├── CaseItau.Domain.UnitTests/
│   ├── CaseItau.Application.UnitTests/
│   ├── CaseItau.Application.IntegrationTests/
│   └── CaseItau.Api.FunctionalTests/
└── docker-compose.yml
```

### Fluxo de Dependências (Clean Architecture)

```
API  ──▶  Application  ──▶  Domain
 │                   ▲
 └──▶ Infrastructure ┘
```

O **Domain** não conhece nenhuma outra camada. A **Application** conhece apenas o Domain. A **Infrastructure** implementa as abstrações definidas pelo Domain e Application. A **API** orquestra tudo via Dependency Injection.

---

## Arquitetura

### Domain Layer

O núcleo do sistema. Contém regras de negócio puras, sem dependência de frameworks.

**Aggregate `Fundo`** com factory method e proteção de invariantes:
- `Fundo.Criar(...)` → `Result<Fundo>` — valida e instancia o aggregate
- `fundo.Atualizar(...)` → `Result` — aplica mudanças com validação
- `fundo.MovimentarPatrimonio(decimal)` — atualiza o patrimônio e dispara evento

**Value Objects** com validação embutida:
- `FundoCodigo` — máximo de 20 caracteres, não nulo/vazio
- `FundoNome` — máximo de 100 caracteres, não nulo/vazio
- `Cnpj` — validação completa com dígitos verificadores

**Domain Events** publicados pelo aggregate:
- `FundoCriadoDomainEvent`
- `FundoAtualizadoDomainEvent`
- `PatrimonioMovimentadoDomainEvent`

**Result Pattern** para erros previsíveis sem uso de exceções:
- `Result` / `Result<T>` — encapsula sucesso ou falha
- `Error` com `ErrorType` enum: `Validation`, `NotFound`, `Conflict`, `Failure`
- `FundoErrors` — catálogo centralizado de erros de domínio com mensagens em português

### Application Layer

Implementa os casos de uso usando **CQRS** via MediatR:

| Tipo | Nome | Responsabilidade |
|---|---|---|
| Command | `AdicionarFundoCommand` | Cria novo fundo |
| Command | `AtualizarFundoCommand` | Atualiza dados do fundo |
| Command | `RemoverFundoCommand` | Remove fundo |
| Command | `MovimentarPatrimonioFundoCommand` | Credita/debita patrimônio |
| Query | `ListarFundosQuery` | Lista todos os fundos |
| Query | `ObterFundoQuery` | Obtém fundo por código |

**Pipeline Behaviors** (cross-cutting concerns injetados automaticamente em todos os commands):
- `ValidationBehavior<,>` — executa FluentValidation antes do handler; lança `ValidationException` estruturada se inválido
- `LoggingBehavior<,>` — loga início, duração e resultado de cada command com contexto estruturado

**Domain Event Handler:**
- `VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler` — detecta movimentações de alto valor e aciona `INotificationService`

**Read/Write Separation:**
- Writes: `IFundoRepository` (EF Core, orientado a agregados)
- Reads: `IFundoReadRepository` (Dapper, orientado a projeções/DTOs)

### Infrastructure Layer

**EF Core** para persistência orientada ao domínio:
- `ApplicationDbContext` implementa `IUnitOfWork`
- Após `SaveChangesAsync`, publica domain events via MediatR
- Configurações fluentes mapeiam Value Objects como owned types e conversões

**Dapper** para leituras performáticas:
- `FundoReadRepository` com SQL otimizado para retornar `FundoResponse` diretamente

**PostgreSQL** como banco de dados relacional de produção:
- Migrations via EF Core (`dotnet ef migrations`)
- Tabelas: `TIPO_FUNDO` e `FUNDO` com índice único em `CNPJ`
- `DatabaseSeeder` popula tipos de fundo na primeira execução

**`SqlConnectionFactory`** para gerenciar conexões Dapper de forma centralizada e injetável.

### API Layer

**Controller limpo** — apenas orquestra, sem lógica de negócio:
- Recebe request DTO → monta Command/Query → envia via `ISender` → mapeia `Result` para HTTP response

**Global Exception Handler** (`IExceptionHandler` do ASP.NET Core 8):
- `ValidationException` → `400 Bad Request` com `ValidationProblemDetails` (RFC 7807)
- Exceções não previstas → `500 Internal Server Error` com `traceId`

**Middleware de Correlação:**
- `RequestContextLoggingMiddleware` injeta `CorrelationId` (do header `X-Correlation-Id` ou `TraceIdentifier`) no contexto do Serilog

**Swagger documentado:**
- Comentários XML em todos os endpoints
- `ProducesResponseType` declarado para cada status code possível

---

## Stack Tecnológica

| Componente | Tecnologia |
|---|---|
| Runtime | .NET 8 |
| Framework Web | ASP.NET Core 8 (Minimal Hosting) |
| ORM (escrita) | Entity Framework Core 8 + Npgsql |
| Micro-ORM (leitura) | Dapper |
| Banco de Dados | PostgreSQL 16 |
| Mediator / CQRS | MediatR 12 |
| Validação | FluentValidation 11 |
| Logging Estruturado | Serilog 4 |
| Log Aggregator | Seq |
| Documentação API | Swashbuckle (OpenAPI 3) |
| Containerização | Docker + Docker Compose |
| Testes Unitários | xUnit + FluentAssertions + NSubstitute |
| Testes de Integração | xUnit + Testcontainers (PostgreSQL) |
| Testes Funcionais | xUnit + WebApplicationFactory + Testcontainers |
| Cobertura | Coverlet |

---

## Funcionalidades da API

Base URL: `http://localhost:5000/api/Fundo`

| Método | Endpoint | Descrição | Sucesso | Erro |
|---|---|---|---|---|
| `GET` | `/` | Lista todos os fundos | `200 OK` | — |
| `GET` | `/{codigo}` | Obtém fundo por código | `200 OK` | `404 Not Found` |
| `POST` | `/` | Cadastra novo fundo | `201 Created` | `400 Bad Request` |
| `PUT` | `/{codigo}` | Atualiza fundo existente | `204 No Content` | `400 Bad Request` |
| `DELETE` | `/{codigo}` | Remove fundo | `204 No Content` | `400 Bad Request` |
| `PUT` | `/{codigo}/patrimonio` | Movimenta patrimônio | `204 No Content` | `400 Bad Request` |

### Exemplo: Cadastrar Fundo

```http
POST /api/Fundo
Content-Type: application/json

{
  "codigo": "ITAURF001",
  "nome": "Fundo Itaú Renda Fixa",
  "cnpj": "11222333000181",
  "codigoTipo": 1
}
```

**Resposta `201 Created`:**
```http
Location: /api/Fundo/ITAURF001

"ITAURF001"
```

### Exemplo: Movimentar Patrimônio

```http
PUT /api/Fundo/ITAURF001/patrimonio
Content-Type: application/json

150000.00
```

Valores positivos creditam; valores negativos debitam. Movimentações de alto valor disparam alertas de monitoramento automaticamente via domain event.

---

## Qualidade e Testes

A solução implementa **quatro camadas de teste** cobrindo o sistema de ponta a ponta:

### 1. Testes Unitários — Domain (`CaseItau.Domain.UnitTests`)

Validam o comportamento do aggregate `Fundo` e seus value objects em isolamento total:
- Criação com dados válidos e inválidos
- Regras de validação de CNPJ (dígitos verificadores)
- Disparo correto de domain events
- Atualização e movimentação de patrimônio

### 2. Testes Unitários — Application (`CaseItau.Application.UnitTests`)

Validam os handlers de commands em isolamento usando **NSubstitute** para mockar dependências:
- `AdicionarFundoCommandHandler` — sucesso, CNPJ duplicado, tipo inválido
- `AtualizarFundoCommandHandler` — sucesso, not found, CNPJ já em uso
- `MovimentarPatrimonioFundoCommandHandler` — sucesso, not found
- `VerificarMovimentacaoSuspeitaPatrimonioMovimentadoDomainEventHandler` — detecção de movimentação suspeita

### 3. Testes de Integração (`CaseItau.Application.IntegrationTests`)

Testam handlers contra **PostgreSQL real** provisionado via **Testcontainers**:
- `AdicionarFundo` — persistência e recuperação real
- `AtualizarFundo` — atualização e verificação de dados
- `RemoverFundo` — deleção e confirmação de ausência
- `MovimentarPatrimonioFundo` — movimentação e consistência
- `ListarFundos` / `ObterFundo` — queries via Dapper

Cada teste roda em um banco isolado, garantindo independência completa.

### 4. Testes Funcionais / E2E (`CaseItau.Api.FunctionalTests`)

Testam a API completa via **`HttpClient`** com `WebApplicationFactory`:
- Cobertura de todos os 6 endpoints
- Validação de status codes HTTP
- Validação de headers de resposta (ex: `Location` no POST)
- Cenários de erro (not found, dados inválidos, CNPJ duplicado)
- Banco PostgreSQL real via Testcontainers

### Executar os Testes

```bash
# Todos os testes (requer Docker para os de integração e funcionais)
dotnet test

# Apenas testes unitários (sem Docker)
dotnet test --filter "FullyQualifiedName!~IntegrationTests&FullyQualifiedName!~FunctionalTests"
```

---

## Observabilidade

### Logging Estruturado com Serilog

Cada requisição gera logs estruturados com:
- `CorrelationId` — rastreabilidade ponta a ponta (header `X-Correlation-Id`)
- `MachineName` e `ThreadId` — diagnóstico de infraestrutura
- Duração de cada command processado pelo pipeline MediatR
- Detalhes do erro em caso de falha (`ResultError`)

### Seq — Aggregator de Logs

Interface web em `http://localhost:8081` para busca e análise de logs em tempo real. Acessível ao subir o ambiente Docker.

### CorrelationId

Cada requisição recebe um `CorrelationId` rastreável do primeiro middleware ao último log:
1. `RequestContextLoggingMiddleware` extrai o ID do header `X-Correlation-Id`
2. Injeta no contexto do Serilog via `LogContext.PushProperty`
3. Todos os logs daquela requisição carregam o ID automaticamente

---

## Como Executar

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (apenas para desenvolvimento local)

### Subir o Ambiente Completo (Docker Compose)

```bash
docker compose up -d
```

Isso sobe três serviços:

| Serviço | URL | Descrição |
|---|---|---|
| `caseitau.api` | http://localhost:5000 | API REST |
| `caseitau.db` | localhost:5432 | PostgreSQL 16 |
| `caseitau.seq` | http://localhost:8081 | Seq Log Viewer |

A API aplica as migrations e popula os dados de seed automaticamente na primeira execução.

### Swagger UI

```
http://localhost:5000/swagger
```

### Rodar Localmente (sem Docker para a API)

```bash
# Subir apenas banco e Seq
docker compose up caseitau.db caseitau.seq -d

# Rodar a API
cd src/CaseItau.API
dotnet run
```

### Migrations (caso necessário)

```bash
cd src/CaseItau.API
dotnet ef database update --project ../CaseItau.Infra
```

---

## Resumo das Melhorias

| Categoria | Antes | Depois |
|---|---|---|
| **Arquitetura** | Monolítico em um controller | Clean Architecture com 4 camadas |
| **Padrão** | Nenhum | CQRS + MediatR + Repository + Result |
| **Banco de Dados** | SQLite (arquivo local) | PostgreSQL 16 (containerizado) |
| **Acesso a dados** | SQL concatenado no controller | EF Core (escrita) + Dapper (leitura) |
| **Segurança** | SQL Injection em todos os endpoints | Queries parametrizadas / ORM |
| **Validação** | Inexistente | FluentValidation com pipeline behavior |
| **Erros** | `void` / `null` sem status HTTP | Result Pattern + ProblemDetails (RFC 7807) |
| **Bug GET após POST** | Exceção ao ler PATRIMONIO NULL | `decimal?` + tratamento correto |
| **Conexões DB** | Criadas e jamais fechadas | Factory + `using` / DI gerenciado |
| **Async** | Síncrono bloqueante | Async/await em toda a stack |
| **Logging** | Nenhum | Serilog + Seq + CorrelationId |
| **Testes** | Zero | 4 projetos: Unit, Unit-App, Integration, Functional |
| **Containerização** | Nenhuma | Docker Compose com API + DB + Seq |
| **Documentação** | Nenhuma | Swagger UI com comentários XML |
| **Domain Events** | Inexistente | FundoCriadoDomainEvent, etc. + handlers |
| **Value Objects** | Strings primitivas | `FundoCodigo`, `FundoNome`, `Cnpj` validados |
| **CNPJ** | String sem validação | Validação completa com dígitos verificadores |
