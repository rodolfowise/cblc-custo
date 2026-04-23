# Task 3: Modelagem de Entidades e Value Objects

**Ordem:** 3/10  
**Duração Estimada:** 2-3 horas  
**Depende de:** Task 2 ✅

---

## Nome da Task

**Implementar Entidades de Domínio e Value Objects com Validações e Invariantes de Negócio**

---

## Objetivo

Definir as entidades principais do domínio (Funcionario) com seus atributos, métodos de negócio e invariantes, além de implementar Value Objects para dados estruturados (CPF, Email, Telefone) com validações encapsuladas. Garantir que as regras de negócio sejam respeitadas no nível de domínio.

---

## Principais Entregas

- ✅ Entidade `Funcionario` com atributos completos (nome, CPF, email, telefone, cargo, departamento, etc)
- ✅ Value Objects: `CPF`, `Email`, `Telefone`, `Endereco`, `DataNascimento`
- ✅ Métodos de negócio na entidade (ativar, desativar, alterar_cargo, etc)
- ✅ Invariantes de negócio respeitados (um CPF por funcionário, email único, etc)
- ✅ Validações encapsuladas nos Value Objects
- ✅ Exceções específicas de domínio para violações de invariantes
- ✅ Factory methods para criação segura de entidades
- ✅ Todos os atributos com type hints completos
- ✅ Docstrings descrevendo regras de negócio
- ✅ Métodos to_dict() e from_dict() para facilitar persistência

---

## Critério de Pronto

- [ ] Entidade `Funcionario` com todos os atributos esperados (mínimo 10 atributos)
- [ ] Cada Value Object implementado com validação no __init__
- [ ] Pelo menos 3 métodos de negócio na entidade (além de getters/setters)
- [ ] Exceções customizadas para cada tipo de violação (InvalidCPF, InvalidEmail, etc)
- [ ] Factory method `Funcionario.criar(...)` que retorna entidade validada
- [ ] `__eq__` e `__hash__` implementados baseado em id (para collections)
- [ ] `__repr__` implementado para debugging
- [ ] Método to_dict() que retorna dict com todos atributos
- [ ] Valor Objects são imutáveis (properties apenas leitura, sem setters)
- [ ] Type hints completos em todas as signatures
- [ ] Exemplo de uso documentado em docstring da entidade

---

## Prompt de Execução

```
Como especialista em Domain-Driven Design e modelagem de domínio, 
implemente as entidades e value objects do microserviço ms-cadastro-funcionario:

## Contexto
O microserviço gerencia cadastro de funcionários de uma empresa.
Um funcionário tem atributos como CPF, email, telefone, cargo, departamento, data de nascimento.
Alguns atributos têm regras de validação e de negócio específicas.

## Tecnologias
- Python 3.11+
- Typing para type hints
- Datetime para datas
- UUID para IDs únicos
- Enum para tipos (Status, Cargo, Departamento)

## Entidade Principal: Funcionario

Atributos obrigatórios:
- id: str (UUID4)
- nome: str (não vazio, 2-100 caracteres)
- cpf: CPF (Value Object, validado)
- email: Email (Value Object, validado)
- telefone: Telefone (Value Object, opcional)
- data_nascimento: DataNascimento (Value Object, validado)
- cargo: Cargo (Enum: GERENTE, ANALISTA, ASSISTENTE, etc)
- departamento: Departamento (Enum: TI, RH, FINANCEIRO, etc)
- data_admissao: datetime
- ativo: bool (default True)
- endereco: Endereco (Value Object, completo com rua, cidade, cep, etc)
- salario: decimal (positivo, até 2 casas decimais)
- criado_em: datetime (now quando criado)
- atualizado_em: datetime (now quando modificado)

## Especificações por Atributo

### 1. CPF (Value Object)
Validações:
- String de 11 dígitos (sem pontuação)
- Cálculo de dígitos verificadores válido (módulo 11)
- Não pode ser CPF conhecido inválido (111.111.111-11, etc)
- Imutável após criação

Métodos:
- __init__(cpf_string: str) com validação
- formatado() -> str (retorna XXX.XXX.XXX-XX)
- __str__() -> str formatado
- __eq__(other) -> bool

### 2. Email (Value Object)
Validações:
- Formato de email válido (regex simples ou validators)
- Não vazio
- Lowercase (normalizado)
- Imutável

Métodos:
- __init__(email_string: str) com validação
- dominio() -> str (parte após @)
- usuario() -> str (parte antes de @)
- __str__() -> str

### 3. Telefone (Value Object)
Validações:
- Formato brasileiro: (XX) 9XXXX-XXXX ou (XX) XXXX-XXXX
- Validação de DDD (11-99 range)
- Opcional (pode ser None)
- Imutável

Métodos:
- __init__(telefone_string: str | None) com validação
- ddd() -> str (primeiros 2 dígitos entre parênteses)
- numero() -> str (sem formatação)
- __str__() -> str formatado

### 4. DataNascimento (Value Object)
Validações:
- Formato YYYY-MM-DD
- Não pode ser data futura
- Idade mínima 18 anos (para trabalho)
- Idade máxima 120 anos (sanidade)
- Imutável

Métodos:
- __init__(data: date | str) com validação
- idade() -> int (calcula idade atual)
- eh_maior_idade() -> bool
- __str__() -> str formatado

### 5. Endereco (Value Object)
Atributos:
- rua: str
- numero: str
- complemento: str (opcional)
- bairro: str
- cidade: str
- estado: str (2 caracteres, UF)
- cep: str (8 dígitos, validado)
- Imutável

Validações:
- Nenhum campo obrigatório pode ser vazio (exceto complemento)
- CEP deve ter 8 dígitos
- Estado deve ser UF válida
- Método validar_cep()

### 6. Enums
```
class StatusFuncionario(str, Enum):
    ATIVO = "ativo"
    INATIVO = "inativo"
    AFASTADO = "afastado"
    FÉRIAS = "ferias"

class Cargo(str, Enum):
    GERENTE = "gerente"
    ANALISTA = "analista"
    ASSISTENTE = "assistente"
    ESTAGIARIO = "estagiario"
    DIRETOR = "diretor"

class Departamento(str, Enum):
    TI = "ti"
    RH = "rh"
    FINANCEIRO = "financeiro"
    OPERACOES = "operacoes"
    COMERCIAL = "comercial"
```

## Métodos de Negócio da Entidade Funcionario

### 1. Factory Method (criação segura)
```python
@staticmethod
def criar(
    nome: str,
    cpf_string: str,
    email_string: str,
    data_nascimento: date,
    cargo: Cargo,
    departamento: Departamento,
    endereco: Endereco,
    salario: Decimal,
    telefone_string: str | None = None,
) -> "Funcionario":
    # Valida todos os parâmetros
    # Cria Value Objects
    # Cria entidade com invariantes respeitados
    # Retorna entidade ou raises exceção
```

### 2. Alterar Cargo
```python
async def alterar_cargo(self, novo_cargo: Cargo, data_mudanca: datetime) -> None:
    if novo_cargo == self.cargo:
        raise SameCargo("Cargo é o mesmo atual")
    self.cargo = novo_cargo
    self.atualizado_em = data_mudanca
    # Publica DomainEvent (Task 6)
```

### 3. Ativar/Desativar
```python
async def ativar(self) -> None:
    if self.ativo:
        raise FuncionarioJaAtivo()
    self.ativo = True
    self.atualizado_em = datetime.now()

async def desativar(self, motivo: str) -> None:
    if not self.ativo:
        raise FuncionarioJaInativo()
    self.ativo = False
    self.atualizado_em = datetime.now()
    # Pode manter histórico de motivo
```

### 4. Ajustar Salário
```python
async def ajustar_salario(self, novo_salario: Decimal, data_ajuste: datetime) -> None:
    if novo_salario <= 0:
        raise SalarioInvalido("Salário deve ser positivo")
    if novo_salario == self.salario:
        raise SalarioIgual()
    self.salario = novo_salario
    self.atualizado_em = data_ajuste
    # Publica DomainEvent
```

### 5. Validar Invariantes
```python
def validar(self) -> None:
    # Valida que todos os invariantes de negócio são respeitados
    # Levanta exceções se houver violação
    # Chamado antes de persistência
    if not self.nome or len(self.nome) < 2:
        raise NomeInvalido()
    if not self.ativo and self.data_admissao > datetime.now():
        raise InvarianteViolado("Funcionário inativo não pode ter data de admissão futura")
```

## Exceções de Domínio

```
domain/exceptions.py:
- InvalidCPF(Exception)
- InvalidEmail(Exception)
- InvalidTelefone(Exception)
- InvalidDataNascimento(Exception)
- InvalidEndereco(Exception)
- InvalidNome(Exception)
- InvalidSalario(Exception)
- FuncionarioJaAtivo(Exception)
- FuncionarioJaInativo(Exception)
- SalarioInvalido(Exception)
- CargoInvalido(Exception)
- DomainException (base)
```

## Métodos da Entidade

Além de getters, implementar:
- to_dict() -> dict (serialização)
- from_dict(data: dict) -> Funcionario (desserialização)
- __eq__(other) -> bool (comparação por id)
- __hash__() -> int (para usar em sets/dicts)
- __repr__() -> str (para debugging)
- __str__() -> str (legível)
- validar() -> None (verifica invariantes)

## Arquivos a Gerar

**Domain - Entidades:**
- src/domain/entities/funcionario.py (entidade principal)
- src/domain/entities/enums.py (Cargo, Departamento, Status)

**Domain - Value Objects:**
- src/domain/value_objects/cpf.py (com validação de dígitos verificadores)
- src/domain/value_objects/email.py (com validação de formato)
- src/domain/value_objects/telefone.py (com validação de DDD)
- src/domain/value_objects/data_nascimento.py (com cálculo de idade)
- src/domain/value_objects/endereco.py (com validação de CEP)

**Domain - Exceções (atualizar):**
- src/domain/exceptions.py (adicionar todas as exceções listadas)

## Boas Práticas

1. **Imutabilidade de Value Objects:**
   - Usar @property apenas para leitura
   - Usar __slots__ para otimizar memória
   - Congelar com frozenset ou similar se necessário

2. **Validações:**
   - Todas as validações no __init__ (falha rápido)
   - Levanta exceção específica com mensagem clara
   - Não retorna True/False, usa exceções

3. **Type Hints:**
   - typing.Union, typing.Optional, typing.List
   - Usar Literal para Enums em alguns casos
   - Documentar exceções que pode lançar

4. **Métodos de Negócio:**
   - Refletem linguagem do domínio (ubiquitous language)
   - Encapsulam regras de negócio
   - Levantam exceções em violações

5. **Testabilidade:**
   - Valor Objects são fáceis de testar (sem dependências)
   - Entidades testáveis sem banco de dados
   - Factory methods facilitam criação em testes

## Exemplo de Uso (em docstring)

```python
# Criação segura
funcionario = Funcionario.criar(
    nome="João Silva",
    cpf_string="12345678910",
    email_string="joao@empresa.com",
    data_nascimento=date(1990, 5, 15),
    cargo=Cargo.ANALISTA,
    departamento=Departamento.TI,
    endereco=Endereco(
        rua="Rua A",
        numero="123",
        bairro="Centro",
        cidade="São Paulo",
        estado="SP",
        cep="01234567"
    ),
    salario=Decimal("5000.00"),
    telefone_string="(11) 98765-4321"
)

# Métodos de negócio
await funcionario.alterar_cargo(Cargo.GERENTE, datetime.now())
await funcionario.ajustar_salario(Decimal("6000.00"), datetime.now())
await funcionario.desativar("Demissão por justa causa")

# Acesso a dados
print(funcionario.cpf.formatado())  # 123.456.789-10
print(funcionario.email.dominio())  # empresa.com
print(funcionario.data_nascimento.idade())  # 34
```

## Resultado Esperado

Após esta task:
1. Entidade Funcionario completa com todos atributos
2. 5 Value Objects implementados com validações
3. Todos os métodos de negócio funcionando
4. Exceções específicas lançadas em violações
5. Código pronto para ser usado em testes (sem banco de dados)
6. Type hints completos permitindo type checking com mypy

## Validação

```bash
# Type check
mypy src/domain/

# Syntax check
python -m py_compile src/domain/**/*.py

# Teste manual de instanciação (em Python REPL)
from src.domain.entities.funcionario import Funcionario
funcionario = Funcionario.criar(...)  # sem exceções
```
```

---

## Dependências Futuras

As próximas tasks usarão:
- Entidade `Funcionario` para mapear do/para banco
- Value Objects para validação em DTOs
- Exceções para tratamento em Application e Presentation
- Métodos de negócio em Use Cases

---

## Referência Técnica

- [Pydantic Validators](https://docs.pydantic.dev/latest/api/validators/)
- [Python Dataclasses](https://docs.python.org/3/library/dataclasses.html)
- [Value Object Pattern](https://martinfowler.com/bliki/ValueObject.html)
- [CPF Validation Algorithm](https://en.wikipedia.org/wiki/CPF_(Brazil))
