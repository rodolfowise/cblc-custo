# Task 7: Validação de Dados com Pydantic e Request/Response Schemas

**Ordem:** 7/10  
**Duração Estimada:** 1-2 horas  
**Depende de:** Task 2, Task 6 ✅

---

## Nome da Task

**Implementar Schemas Pydantic para Validação de API e Documentação Automática**

---

## Objetivo

Criar schemas Pydantic específicos para API REST (diferente de DTOs da aplicação) que validam requisições, definem respostas, e geram documentação automática no Swagger.

---

## Principais Entregas

- ✅ Request Schemas (para input da API)
- ✅ Response Schemas (para output da API)
- ✅ Validadores customizados (CPF, Email, Telefone)
- ✅ Exemplos de payloads para Swagger
- ✅ Error response schema padrão
- ✅ Paginação schema
- ✅ Enums serialáveis
- ✅ Config Pydantic com ajustes (forbid extra fields, json_schema_extra)
- ✅ Conversão entre Request Schema → Input DTO
- ✅ Conversão entre Output DTO → Response Schema

---

## Prompt de Execução

```
Como especialista em FastAPI e validação Pydantic v2, implemente schemas para API:

## Request Schemas (src/presentation/api/v1/schemas/)

### CreateFuncionarioRequest
```python
class CreateFuncionarioRequest(BaseModel):
    nome: str = Field(..., min_length=2, max_length=100, description="Nome completo")
    cpf: str = Field(..., pattern=r"^\d{11}$", description="11 dígitos sem pontuação")
    email: str = Field(..., description="Email único")
    telefone: Optional[str] = Field(None, pattern=r"^\(\d{2}\)\s\d{4,5}-\d{4}$")
    data_nascimento: date = Field(..., description="Formato: YYYY-MM-DD")
    cargo: Cargo = Field(..., description="Enum: GERENTE, ANALISTA, ...")
    departamento: Departamento = Field(..., description="Enum: TI, RH, ...")
    endereco: EnderecoRequest = Field(...)
    salario: Decimal = Field(..., gt=0, decimal_places=2)
    
    model_config = ConfigDict(
        json_schema_extra={
            "example": {
                "nome": "João Silva",
                "cpf": "12345678910",
                "email": "joao@empresa.com",
                "telefone": "(11) 98765-4321",
                "data_nascimento": "1990-05-15",
                "cargo": "analista",
                "departamento": "ti",
                "endereco": {...},
                "salario": "5000.00"
            }
        }
    )
    
    @field_validator('email')
    @classmethod
    def validate_email(cls, v):
        if '@' not in v:
            raise ValueError('Email inválido')
        return v.lower()
```

### UpdateFuncionarioRequest
```python
class UpdateFuncionarioRequest(BaseModel):
    nome: Optional[str] = Field(None, min_length=2, max_length=100)
    email: Optional[str] = Field(None)
    telefone: Optional[str] = Field(None)
    endereco: Optional[EnderecoRequest] = None
    salario: Optional[Decimal] = Field(None, gt=0)
    
    model_config = ConfigDict(extra='forbid')
```

### ListFuncionariosRequest (Query Parameters)
```python
class ListFuncionariosRequest(BaseModel):
    departamento: Optional[str] = Query(None)
    apenas_ativos: bool = Query(True)
    skip: int = Query(0, ge=0)
    limit: int = Query(100, ge=1, le=1000)
    ordenar_por: str = Query("criado_em")
```

### EnderecoRequest
```python
class EnderecoRequest(BaseModel):
    rua: str = Field(..., min_length=1)
    numero: str = Field(...)
    complemento: Optional[str] = None
    bairro: str = Field(...)
    cidade: str = Field(...)
    estado: str = Field(..., pattern=r"^[A-Z]{2}$")
    cep: str = Field(..., pattern=r"^\d{8}$")
```

## Response Schemas

### FuncionarioResponse
```python
class FuncionarioResponse(BaseModel):
    id: str
    nome: str
    cpf: str  # Formatado XXX.XXX.XXX-XX
    email: str
    telefone: Optional[str] = None
    cargo: Cargo
    departamento: Departamento
    ativo: bool
    criado_em: datetime
    atualizado_em: datetime
    
    model_config = ConfigDict(from_attributes=True)
```

### ListFuncionariosResponse
```python
class ListFuncionariosResponse(BaseModel):
    funcionarios: List[FuncionarioResponse]
    total: int
    skip: int
    limit: int
    has_more: bool
```

### ErrorResponse
```python
class ErrorResponse(BaseModel):
    status_code: int = Field(..., description="HTTP status code")
    message: str = Field(..., description="Mensagem de erro")
    detail: Optional[str] = Field(None, description="Detalhes técnicos")
    timestamp: datetime = Field(default_factory=datetime.now)
    path: Optional[str] = Field(None, description="Path da requisição")
    request_id: Optional[str] = Field(None, description="Correlation ID")
```

### PaginatedResponse (genérico)
```python
class PaginatedResponse(BaseModel, Generic[T]):
    data: List[T]
    pagination: PaginationMeta

class PaginationMeta(BaseModel):
    skip: int
    limit: int
    total: int
    has_more: bool
```

## Conversores

```python
class SchemaConverter:
    @staticmethod
    def request_to_input_dto(request: CreateFuncionarioRequest) -> CreateFuncionarioInputDTO:
        return CreateFuncionarioInputDTO(
            nome=request.nome,
            cpf=request.cpf,
            email=request.email,
            telefone=request.telefone,
            data_nascimento=request.data_nascimento,
            cargo=request.cargo,
            departamento=request.departamento,
            endereco=EnderecoDTO(**request.endereco.model_dump()),
            salario=request.salario
        )
    
    @staticmethod
    def output_dto_to_response(dto: FuncionarioOutputDTO) -> FuncionarioResponse:
        return FuncionarioResponse(
            id=dto.id,
            nome=dto.nome,
            cpf=dto.cpf,  # Já formatado
            email=dto.email,
            # ...
        )
```

## Enums Serializáveis

```python
class Cargo(str, Enum):
    GERENTE = "gerente"
    ANALISTA = "analista"
    # FastAPI serializa automaticamente
```

## Validadores Customizados

```python
def validate_cpf(cpf: str) -> str:
    cpf_obj = CPF(cpf)  # Value Object
    return cpf_obj.valor

def validate_email(email: str) -> str:
    email_obj = Email(email)
    return email_obj.valor

def validate_telefone(telefone: str) -> str:
    if telefone:
        telefone_obj = Telefone(telefone)
        return telefone_obj.formatado()
    return None
```

## Arquivos a Gerar

1. src/presentation/api/v1/schemas/funcionario_schema.py
2. src/presentation/api/v1/schemas/endereco_schema.py
3. src/presentation/api/v1/schemas/error_schema.py
4. src/presentation/api/v1/schemas/pagination_schema.py
5. src/presentation/api/converters.py (Request ↔ DTO)

## Boas Práticas

1. **Validação:**
   - Usar Field(...) para validações
   - @field_validator para lógica complexa
   - Mensagens claras

2. **Exemplos:**
   - json_schema_extra com exemplos reais
   - Facilita testes via Swagger UI

3. **Type Hints:**
   - Generic[T] para responses genéricas
   - Optional[] quando apropriado

4. **Config:**
   - from_attributes=True (para ORM)
   - extra='forbid' (rejeita campos extras)
   - json_schema_extra para docs

## Resultado Esperado

Após esta task:
1. Schemas validam input/output
2. Swagger documentação automática e clara
3. Conversores entre camadas funcionando
4. Type hints completos
5. Exemplos nos endpoints
```

---

## Dependências Futuras

- Task 8 usará schemas em Controllers
- Testes E2E usarão para validar responses

---

## Referência Técnica

- [Pydantic v2 Field Validation](https://docs.pydantic.dev/latest/concepts/fields/)
- [FastAPI Request Body](https://fastapi.tiangolo.com/tutorial/body/)
- [FastAPI Responses](https://fastapi.tiangolo.com/tutorial/response-model/)
