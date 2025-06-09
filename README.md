# AbrigueSe

## Visão Geral

AbrigueSe é uma API RESTful desenvolvida em ASP.NET Core para gerenciar abrigos, pessoas, recursos e outras entidades relacionadas. A API visa fornecer uma plataforma robusta e escalável para o gerenciamento de informações críticas em situações de necessidade, especialmente no contexto de eventos extremos.

## Proposta da Solução: Sistema Integrado de Gerenciamento de Abrigos e Evacuação em Eventos Extremos

Nossa solução propõe uma plataforma tecnológica completa para gestão inteligente de abrigos emergenciais, voltada para situações de desastres naturais como enchentes, deslizamentos, queimadas e outros eventos extremos. A proposta integra tecnologia, inovação e acessibilidade para proteger vidas e otimizar o uso de recursos em momentos críticos.

A plataforma, suportada por esta API, conta com as seguintes funcionalidades principais:

*   **📍 Mapeamento em tempo real:** Exibição da situação regional, abrigos próximos e rotas seguras para evacuação.
*   **🧍‍♀ Cadastro de Pessoas nos Abrigos:** Registro de dados individuais e condição médica para priorização de atendimento.
*   **📊 Dashboard para Autoridades:** Apresentação de gráficos, mapas e indicadores atualizados sobre a ocupação dos abrigos e status dos recursos.
*   **🏠 Painel para os Abrigos:** Permite o controle da lotação, atualização da disponibilidade de suprimentos (água, alimentos, remédios) e comunicação de necessidades.
*   **🆘 Busca por Desaparecidos:** Integração das informações de todos os abrigos cadastrados na rede.

Estas funcionalidades são disponibilizadas através de um aplicativo mobile acessível à população e um sistema web para gestão e tomada de decisão por autoridades e responsáveis pelos abrigos.

O objetivo é fortalecer a resiliência das comunidades, salvar vidas e garantir uma resposta rápida e eficiente em momentos de crise.

### Estrutura Funcional da Aplicação (Telas/Painéis)

*   **[T1] Tela Inicial (Aplicativo Mobile/Web)**
    *   Acesso às principais funcionalidades da plataforma.
*   **[T2] Destinação de Pessoas (Aplicativo Mobile/Web)**
    *   Visualização da Situação da Região (mapas, alertas).
    *   Lista de Abrigos Próximos (disponibilidade, localização).
    *   Rotas Seguras para evacuação.
*   **[T3] Tela de Reports para Autoridades (Sistema Web)**
    *   Visão Geral dos Abrigos (status, capacidade, ocupação).
    *   Gráficos (ocupação por perfil, evolução temporal, recursos disponíveis/necessários).
    *   Mapa Geral (status dos abrigos, regiões críticas, alertas).
*   **[T4] Tela de Gestão de Abrigo (Sistema Web/Mobile para Responsáveis)**
    *   Cadastro de Pessoas no Abrigo (check-in, informações detalhadas).
    *   Status de Recursos (input manual do abrigo sobre níveis de água, alimentos, remédios, etc.).
    *   Situação da Lotação (atualização em tempo real).
    *   Busca por Desaparecidos (consulta à base de dados consolidada de pessoas abrigadas na rede).

## Funcionalidades Principais da API

*   Gerenciamento de Abrigos (CRUD, capacidade, lotação, endereço)
*   Gerenciamento de Pessoas (CRUD, dados pessoais, informações médicas)
*   Gerenciamento de Recursos (CRUD, tipos de suprimentos)
*   Gerenciamento de Endereços, Cidades, Estados e Países
*   Sistema de Check-in/Check-out de Pessoas em Abrigos
*   Controle de Estoque de Recursos por Abrigo
*   Autenticação de Usuários via Google (para acesso a painéis de gestão)

## Tecnologias Utilizadas

*   ASP.NET Core 9 (ou a versão mais recente utilizada)
*   Entity Framework Core (com Oracle Database)
*   AutoMapper
*   Swagger/OpenAPI para documentação da API
*   AspNetCoreRateLimit para limitação de taxa
*   Autenticação Google OAuth 2.0

## Design da API RESTful

A API AbrigueSe segue as melhores práticas de design RESTful para garantir uma interface consistente e fácil de usar.

### HATEOAS (Hypermedia as the Engine of Application State)

Para melhorar a descoberta e navegabilidade da API, implementamos HATEOAS. Isso significa que as respostas da API incluem links que o cliente pode usar para navegar para recursos relacionados ou executar ações permitidas.

*   **`ResourceBaseDto`**: Uma classe base foi introduzida para todos os DTOs (Data Transfer Objects) de resposta. Esta classe contém uma propriedade `Links`, que é uma lista de objetos `LinkDto`.
*   **`LinkDto`**: Cada objeto `LinkDto` representa um link hypermedia e possui as seguintes propriedades:
    *   `Href`: A URL completa do recurso ou ação.
    *   `Rel`: A relação do link com o recurso atual (por exemplo, "self", "update_usuario", "delete_abrigo").
    *   `Method`: O método HTTP recomendado para interagir com o `Href` (por exemplo, "GET", "PUT", "DELETE").

*   **Geração de Links nos Controllers**: Os controllers são responsáveis por popular a lista `Links` nos DTOs de resposta. Eles utilizam o `UrlHelper` do ASP.NET Core para gerar URLs corretas para as ações e recursos relacionados.

**Exemplo de Resposta com Links HATEOAS (JSON):**