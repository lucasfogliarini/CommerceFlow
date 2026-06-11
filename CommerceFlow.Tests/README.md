# CommerceFlow.Tests

## Fluxo Principal

| Número | Comando/Evento Recebido               | Cenário                          | Evento Gerado                              |
| ----- | ------------------------------------- | -------------------------------- | ------------------------------------------ |
| 1     | CriarPedido                           | Criar Pedido                     | PedidoCriado                                |
| 2     | PedidoCriado                          | Reservar Estoque                 | EstoqueReservado                             |
| 3     | EstoqueReservado                      | Aguardar Pagamento               | PedidoAguardandoPagamento                    |
| 4     | AprovarPagamento						| Aprovar Pagamento				   | PagamentoAprovado                             |
| 5     | PagamentoAprovado                      | Iniciar Entrega                  | EntregaIniciada                              |
| 6     | EntregaIniciada                       | Despachar Entrega                | EntregaDespachada                             |
| 7     | EntregaDespachada                     | Concluir Entrega                 | EntregaConcluida                              |


## Cenários Gherkin

### Criar Pedido (Cenário 1)

```gherkin
Dado que o "Cliente" adicionou um item ao "Pedido"

Quando o comando "CriarPedido" é enviado

Então o "Pedido" deve ser criado e o evento "OrderCreated" deve ser emitido
```

### Reservar Estoque (Cenário 2)

```gherkin
Dado que o "Pedido" foi criado com um item

Quando o processo "ReservarEstoque" tenta reservar uma quantidade específica para o pedido

Então a "Reserva de Estoque" deve ocorrer, reduzindo a quantidade disponível e emitindo "InventoryReserved"
```

### Aguardar Pagamento (Cenário 3)

```gherkin
Dado que a "Reserva de Estoque" foi realizada para o pedido

Quando o processo "AguardarPagamento" é executado para esse pedido

Então o "Pedido" deve ficar no status "WaitingForPayment" e emitir "OrderWaitingForPayment"
```

### Aprovar Pagamento (Cenário 4)

```gherkin
Dado que foi criado um "Payment" para o pedido

Quando o comando "ApprovePayment" é executado

Então o pagamento deve ficar no status "Approved" e emitir "PaymentApproved"
```

### Iniciar Entrega (Cenário 5)

```gherkin
Dado que o "Pedido" foi aprovado para pagamento

Quando o processo "IniciarEntrega" é executado para o pedido

Então a "Entrega" deve ser iniciada (status Pending) e emitir "ShipmentStarted"
```

### Despachar Entrega (Cenário 6)

```gherkin
Dado que a "Entrega" está no status Pending

Quando o processo "DespacharEntrega" é executado com um código de rastreamento

Então a "Entrega" deve ficar no status Shipped, registrar o código de rastreamento e emitir "OrderShipped"
```

### Concluir Entrega (Cenário 7)

```gherkin
Dado que a "Entrega" foi despachada (status Shipped)

Quando o processo "ConcluirEntrega" é executado

Então a "Entrega" deve ser marcada como Delivered e emitir "OrderDelivered"
```
