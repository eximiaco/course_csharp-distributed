# Módulo 1

Fundamentos sobre sistemas distríbuidos.

## Desafio do módulo : Plataforma de Gestão de Eventos - EventHub

Você foi contratado pela EventHub, uma startup que deseja criar uma plataforma inovadora para gestão de eventos. A ideia é fornecer um sistema que atenda tanto aos organizadores de eventos quanto aos participantes, garantindo uma experiência prática, eficiente e moderna. O sistema deve permitir que organizadores cadastrem eventos como palestras, workshops, shows e conferências, configurando detalhes como data, horário, local, capacidade de público e tipos de ingressos disponíveis. Os ingressos podem variar entre entrada geral, VIP, meia-entrada ou outras categorias criadas pelos organizadores.

Para eventos presenciais, os organizadores podem habilitar a funcionalidade de lugares marcados, onde os participantes escolhem seus assentos no momento da compra. Já em eventos online, o sistema deve gerar links exclusivos para cada participante. Além disso, os organizadores podem configurar promoções e cupons de desconto para atrair mais público. Os cupons podem ser configurados para oferecer um desconto fixo ou percentual sobre o valor do ingresso, e podem ser aplicados a todos os ingressos ou apenas a tipos específicos. Por exemplo, um cupom “DESCONTO10” pode dar 10% de desconto em ingressos gerais, enquanto um cupom “VIP50” pode oferecer um desconto de R$50 para ingressos VIP. Os organizadores também podem definir condições para o uso dos cupons, como limite de uso por participante ou data de validade.

O processo de compra de ingressos deve ser simples e direto. Após selecionar o evento e o ingresso desejado, o participante pode aplicar um cupom de desconto antes de finalizar a compra. Uma vez concluído o pagamento, o sistema gera um ingresso digital com um QR Code, que será utilizado para validar sua entrada no evento. Além disso, o sistema deve notificar os participantes por e-mail ou SMS com informações importantes, como confirmação de compra, mudanças no evento, lembretes próximos à data ou avisos sobre novas promoções.

Os organizadores precisam de acesso a relatórios detalhados sobre o andamento das vendas e o uso de cupons de desconto, incluindo dados sobre quais cupons foram mais utilizados, qual foi o impacto das promoções nas vendas e qual a receita gerada com cada tipo de ingresso. Eles também desejam acompanhar a lotação do evento em tempo real e receber insights sobre o desempenho das estratégias promocionais configuradas.

Como a EventHub espera crescer rapidamente, o sistema deve ser projetado para suportar eventos de diferentes tamanhos, desde pequenas reuniões até grandes shows com milhares de participantes. Além disso, o sistema precisa ser capaz de lidar com situações críticas, como dois participantes tentando comprar o mesmo lugar ao mesmo tempo, ou interrupções em serviços externos, como o processamento de pagamentos.

Com essa plataforma, a EventHub busca criar uma solução confiável, escalável e atrativa, que não apenas facilite a gestão dos eventos, mas também conquiste a fidelidade dos organizadores e participantes, oferecendo uma experiência memorável do começo ao fim.

### Desafio

Você deverá entregar os seguintes artefatos para o sistema **EventHub**:
- **Identificação de contextos**: Analise e identifique os diferentes contextos necessários para o sistema funcionar.
- **Escolha de padrões de comunicação**: Decida entre comunicação síncrona ou assíncrona para interações entre os contextos.
- **Defina a arquiteura de dados** das informações e seus golden sources.
- **Implemente um projeto referência** para exemplificar a comunicação entre os contextos (Não precisa de regras de negócio ou persistência)