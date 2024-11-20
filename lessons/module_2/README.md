# Módulo 2

CQRS e Event Sourcing

## Desafio do módulo : Plataforma de Gestão de Finanças pessoais


Implemente uma aplicação que seja responsável pelo gerenciamento de finanças pessoais. Essa aplicação deve ter as seguintes funcionalidades:

- **Criação de uma conta para lançamentos de receitas e despesas**  
  Essa conta tem como campo obrigatório o nome. Além disso, é possível definir um valor máximo de despesas (opcional) e uma quantia máxima de lançamentos (opcional).

- **Atualização dos dados da conta**  
  Permite alterar o nome, o número máximo de transações e o valor máximo de despesas.


- **Lançamento de despesas**  
  Permite registrar uma despesa informando o valor e a categoria.

- **Lançamento de receitas**  
  Permite registrar uma receita informando o valor e a categoria.

- **Consulta de todas as contas**  
  Retorna apenas o ID de todas as contas cadastradas.

- **Consulta individual da conta**  
  Retorna o nome da conta, o valor total de receitas e despesas lançados.

- **Consulta das somas das despesas por categoria**  
  Retorna a soma das despesas agrupadas por categoria.

- **Consulta do histórico da conta**  
  Retorna todos os lançamentos (receitas e despesas) da conta.