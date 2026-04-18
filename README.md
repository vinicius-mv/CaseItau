# Case de engenharia Itau - .Net

## Introdução

Neste projeto esta sendo utilizada a base de dados sqlite (arquivo dbcaseitau S3db) com as seguintes tabelas:

    Tabela: TIPO_FUNDO > "Tipos de fundos existentes"
    - CODIGO      - INT         NOT NULL - PRIMARY KEY
    - NOME        - VARCHAR(20) NOT NULL

    Tabela: FUNDO > "Registro relacionados ao cadastro de fundos"
    - CODIGO      - VARCHAR(20)  UNIQUE NOT NULL - PRIMARY KEY
    - NOME        - VARCHAR(100)        NOT NULL
    - CNPJ        - VARCHAR(14)  UNIQUE NOT NULL
    - CODIGO_TIPO - INT                 NOT NULL - FOREIGN KEY TIPO_FUNDO(CODIGO)
    - PATRIMONIO  - NUMERIC                 NULL

> Obs.: você pode fazer o uso do [sqliteadmin] para gerenciar a base de dados, visualizar as tabelas e seus respectivos dados

No projeto CaseItau.API foi disponibilizada uma API de Fundos com os metodos abaixo realizando acoes diretas na base de dados:

    GET                        - LISTAR TODOS OS FUNDOS CADASTRADOS
    GET    {CODIGO}            - RETORNAR OS DETALHES DE UM DETERMINADO FUNDO PELO CÓDIGO
    POST   {FUNDO}             - REALIZA O CADASTRO DE UM NOVO FUNDO
    PUT    {CODIGO}            - EDITA O CADASTRO DE UM FUNDO JÁ EXISTENTE
    DELETE {CODIGO}            - EXCLUI O CADASTRO DE UM FUNDO
    PUT    {CODIGO}/patrimonio - ADICIONA OU SUBTRAI DETERMINADO VALOR DO PATRIMONIO DE UM FUNDO

## Ações a serem realizadas

1. Faça o fork do projeto no seu github. Não realize commits na branch main e nem crie novas branchs.
2. O código da api de fundos faz mal uso dos objetos, não segue boas práticas e não possui qualidade. Refatore o codigo utilizando as melhores bibliotecas, praticas, patterns e garanta a qualidade da aplicação. Fique a vontade para usar outros componentes e base de dados.
3. Após a inclusão de um novo fundo via API, os metodos GET da API de Fundos estão retornando erro. Identifique e corrija o erro

4. Crie uma aplicação web (Angular ou ASP NET MVC) que consuma todos os metodos da API de fundos

Se a sua vaga for BACKEND não se preocupe em fazer a parte de FRONT.<br>
Após finalizar o case, envie o link do seu github com a solução final para o gestor que o solicitou.

[sqliteadmin]: http://sqliteadmin.orbmu2k.de

Observações:

A respeito do item 3, os métodos da API falham porque o POST insere um fundo sem patrimônio. Assim ao tentar ler os dados desse fundo, o método decimal.parser falha, pois não espera receber um valor nulo, para corrigir, basta verificar se o patrimônio possui valor ao parsear.

Outro ponto de atenção, as conexões de database não são corretamente gerenciadas, após a primeira operação de escrita, onde é obtido um lock no arquivo, este lock não é liberado, logo as tentativas de escritas subsequentes falharão, pois o arquivo do sqlite não foi desbloqueado corretamente após a operação.

As queries são montadas dinamicamente e recebem strings de complemento, isso deixa a API vulnerável a ataque de SQL injection.

O endpoint PUT {codigo}/patrimonio, usado para movimentar o patrimônio pode apresentar erros caso o valor não seja inteiro, caso a configuração de idioma do ambiente configurado para português e possivelmente outros idiomas. O número será escrito com vírgula no comando e causará erros no SQL interpreter.
