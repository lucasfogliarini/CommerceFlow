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

# O que o Aspire.C4 adiciona

O Aspire.C4 adiciona uma camada mais próxima da arquitetura do sistema. Ao invés de focar apenas em recursos técnicos, a modelagem passa a representar:

* sistemas;
* containers;
* componentes;
* relações arquiteturais;
* dependências de negócio;
* organização de domínio.

---

# Relação com Diagramas Arquiteturais

O Aspire.C4 foi pensado para aproximar infraestrutura executável e documentação arquitetural.

A definição da aplicação pode servir como base para:

* diagramas C4;
* system landscape;
* system context;
* container diagrams;
* mapas de dependência;
* visualização de relações entre serviços.

Isso permite que o próprio código se torne uma fonte de verdade arquitetural.

---

# Relação com DNS e Domínio da Aplicação

O Aspire.C4 também busca representar a organização lógica da aplicação através de domínios e subdomínios DNS.

A modelagem de DNS e subdomínios permite associar serviços, gateways, APIs, frontends e bounded contexts com seus respectivos domínios públicos.

* serviços;
* gateways;
* APIs;
* frontends;
* bounded contexts;

com seus respectivos domínios públicos.

A modelagem de DNS ajuda a representar:

* fronteiras do sistema;
* contexto organizacional;
* exposição pública;
* estrutura multi-tenant;
* divisão entre ambientes;
* arquitetura orientada a subdomínios.

---

# Vantagens do Aspire.C4 sobre o `IDistributedApplicationBuilder` puro

## 1. Arquitetura mais legível

O código pode ficar mais próximo da linguagem arquitetural do sistema quando utilizamos abstrações do Aspire.C4 em vez de manipular apenas recursos técnicos.

---

## 2. Modelagem semântica

O Aspire puro modela infraestrutura.

O Aspire.C4 modela:

* sistemas;
* containers;
* componentes;
* relações arquiteturais;
* domínios DNS;
* topologia distribuída.

Isso aproxima o código do C4 Model e do DDD.

---

## 3. Melhor organização em sistemas grandes

Em aplicações maiores, o `Program.cs` do Aspire pode virar um grande grafo técnico.

O Aspire.C4 permite organizar:

* bounded contexts;
* módulos;
* subsistemas;
* stacks reutilizáveis;
* domínios de negócio.

---

## 4. Documentação viva

A arquitetura fica no próprio código.

Isso reduz divergência entre:

* diagramas;
* documentação;
* infraestrutura;
* implementação real.

---

## 5. Reutilização de padrões

É possível encapsular estruturas recorrentes:

É possível encapsular estruturas recorrentes (por exemplo: identidade, observability, mensageria) em helpers reutilizáveis.

---

## 6. Relação entre arquitetura e DNS

O Aspire.C4 permite aproximar:

* arquitetura lógica;
* exposição de serviços;
* estrutura de domínio;
* organização pública da plataforma.

Isso facilita visualizar:

* quais serviços pertencem a cada domínio;
* como os sistemas são publicados;
* relações entre APIs e frontends;
* composição de ambientes distribuídos.

---

# Aspire puro vs Aspire.C4

| Aspire puro          | Aspire.C4                     |
| -------------------- | ----------------------------- |
| Infraestrutura first | Arquitetura first             |
| Recursos técnicos    | Conceitos arquiteturais       |
| AddProject           | AddService                    |
| AddPostgres          | AddDatabase                   |
| WithReference        | Uses / Relationship           |
| Foco operacional     | Foco arquitetural             |
| Grafo técnico        | Modelo de sistema             |
| Infraestrutura       | Infraestrutura + documentação |
| Serviços isolados    | Topologia arquitetural        |

---

# Características

* DSL interna em C#
* API fluente
* Sem parser próprio
* Baseado em .NET Aspire
* Inspirado no C4 Model
* Compatível com arquitetura distribuída
* Orientado a documentação viva
* Modelagem de relações arquiteturais
* Integração conceitual com DNS e domínios

---

# Casos de Uso

O Aspire.C4 é útil para:

* microservices;
* plataformas distribuídas;
* sistemas DDD;
* arquitetura como código;
* documentação arquitetural viva;
* ambientes de desenvolvimento complexos;
* padronização arquitetural;
* geração de diagramas;
* organização multi-domínio.

---

# Posicionamento

O Aspire.C4 não substitui o Aspire.

Ele funciona como uma camada arquitetural acima do `IDistributedApplicationBuilder`, adicionando semântica, organização e modelagem baseada no C4 Model, incluindo relações arquiteturais, topologia distribuída e organização por domínios DNS.
