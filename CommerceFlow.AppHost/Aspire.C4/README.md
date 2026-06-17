# Aspire.C4

## Resumo

**Aspire.C4** é uma DSL em C# para modelar sistemas distribuídos utilizando conceitos do C4 Model sobre o builder do .NET Aspire.

A ideia é permitir que a arquitetura do sistema seja definida como código, de forma mais semântica, legível e organizada do que utilizando apenas o `IDistributedApplicationBuilder` diretamente.

Além da infraestrutura, o Aspire.C4 também busca representar:

* o contexto arquitetural do sistema;
* as relações entre serviços;
* a topologia da aplicação;
* a organização por domínios;
* a estrutura de DNS e subdomínios;
* e a geração de diagramas arquiteturais.

---

# Objetivo

O Aspire.C4 simplifica a definição de:

* sistemas;
* serviços;
* bancos de dados;
* containers;
* dependências;
* relações arquiteturais;
* domínios DNS;
* topologias distribuídas.

Tudo utilizando uma API fluente em C#.

---

# Baseado no `IDistributedApplicationBuilder`

O Aspire.C4 utiliza internamente o `IDistributedApplicationBuilder` do Aspire.

O builder do Aspire já permite criar aplicações distribuídas declarando vários recursos de infraestrutura (APIs, workers, bancos, containers, referências, variáveis de ambiente e dependências). Isso funciona bem para infraestrutura, mas conforme o sistema cresce, o código pode ficar muito técnico e pouco arquitetural.

---