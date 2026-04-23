# Task 10: Integração, Validação End-to-End e Preparação para Testes

**Ordem:** 10/10  
**Duração Estimada:** 2 horas  
**Depende de:** Task 1-9 ✅

---

## Nome da Task

**Integração Completa, Testes Manuais E2E e Preparação de Fixtures para Testes Automatizados**

---

## Objetivo

Validar que todas as camadas funcionam integradas, criar dados de teste, preparar fixtures para testes futuros e documentar como testar manualmente a aplicação completa.

---

## Principais Entregas

- ✅ Verificação de que todos endpoints funcionam
- ✅ Testes manuais via Swagger documentados
- ✅ Script de teste com curl/httpx
- ✅ Fixtures e seed data completas
- ✅ Arquivo conftest.py atualizado com fixtures
- ✅ Builders para criar entidades em testes
- ✅ Database fixtures (setup/teardown)
- ✅ Docker Compose validado (up/down)
- ✅ README atualizado com instruções
- ✅ Checklist de validação
- ✅ Arquivo de exemplo de fluxo completo

---

## Critério de Pronto

- [ ] `docker-compose up -d` executa sem erros
- [ ] Health checks respondem corretamente
- [ ] Todos 7+ endpoints testáveis via Swagger
- [ ] Criar funcionário → Listar → Buscar → Atualizar → Desativar funciona end-to-end
- [ ] Validações de CPF/Email funcionam (duplicatas rejeitadas)
- [ ] Seed data carregável
- [ ] Fixtures preparadas para testes futuros
- [ ] Conftest.py com setup/teardown
- [ ] Script de teste automatizado (curl ou similar)
- [ ] Documentação completa de como testar

---

## Prompt de Execução

```
Como QA e especialista em testes de integração, prepare validação E2E:

## 1. Checklist de Validação Manual

Arquivo: docs/VALIDATION_CHECKLIST.md

```
# Validação E2E - ms-cadastro-funcionario

## Setup
- [ ] Docker Desktop instalado
- [ ] Git clonado: git clone <repo>
- [ ] cd ms-cadastro-funcionario
- [ ] docker-compose up -d

## Health Checks
- [ ] GET http://localhost:8000/health → status ok
- [ ] GET http://localhost:8000/health/db → connected true
- [ ] GET http://localhost:8000/health/live → alive
- [ ] GET http://localhost:8000/health/ready → ready

## Documentação
- [ ] Swagger visível em http://localhost:8000/docs
- [ ] ReDoc visível em http://localhost:8000/redoc

## Fluxo Criar e Listar
- [ ] POST /api/v1/funcionarios com dados válidos → 201
- [ ] GET /api/v1/funcionarios → lista contém novo
- [ ] GET /api/v1/funcionarios?apenas_ativos=true → apenas ativos
- [ ] GET /api/v1/funcionarios?departamento=ti → apenas TI

## Fluxo Buscar por ID
- [ ] GET /api/v1/funcionarios/{id} com id válido → 200
- [ ] GET /api/v1/funcionarios/{id} com id inválido → 404

## Fluxo Validação
- [ ] POST com CPF duplicado → 409
- [ ] POST com Email duplicado → 409
- [ ] POST com dados inválidos (CPF formato errado) → 422
- [ ] POST com campos faltando → 422

## Fluxo Atualizar
- [ ] PUT /api/v1/funcionarios/{id} com dados → 200
- [ ] PUT com id inexistente → 404

## Fluxo Status
- [ ] PATCH /api/v1/funcionarios/{id}/desativar → 200
- [ ] GET /api/v1/funcionarios/{id} → ativo=false
- [ ] PATCH /api/v1/funcionarios/{id}/ativar → 200
- [ ] GET /api/v1/funcionarios/{id} → ativo=true

## Logs
- [ ] Log file criado em logs/ms-cadastro-funcionario.log
- [ ] Logs em JSON format
- [ ] Correlation ID presente em logs

## Performance
- [ ] Criar funcionário < 500ms
- [ ] Listar < 200ms
- [ ] Buscar por ID < 100ms

## Cleanup
- [ ] docker-compose down
- [ ] docker-compose ps → empty
```

## 2. Script de Teste (Python httpx)

Arquivo: tests/e2e/test_flow.py ou scripts/test_manual.py

```python
import httpx
import asyncio
from decimal import Decimal
from datetime import date

BASE_URL = "http://localhost:8000/api/v1"

async def test_complete_flow():
    async with httpx.AsyncClient() as client:
        # 1. Health Check
        print("🔍 Checking health...")
        response = await client.get(f"{BASE_URL.replace('/api/v1', '')}/health")
        assert response.status_code == 200
        print("✅ Health check OK")
        
        # 2. Criar Funcionário
        print("\n📝 Creating funcionario...")
        create_payload = {
            "nome": "João Silva",
            "cpf": "12345678910",
            "email": "joao@test.com",
            "telefone": "(11) 98765-4321",
            "data_nascimento": "1990-05-15",
            "cargo": "analista",
            "departamento": "ti",
            "endereco": {
                "rua": "Rua A",
                "numero": "123",
                "bairro": "Centro",
                "cidade": "São Paulo",
                "estado": "SP",
                "cep": "01234567"
            },
            "salario": "5000.00"
        }
        
        response = await client.post(
            f"{BASE_URL}/funcionarios",
            json=create_payload
        )
        assert response.status_code == 201, f"Expected 201, got {response.status_code}: {response.text}"
        created = response.json()
        funcionario_id = created["id"]
        print(f"✅ Created funcionario: {funcionario_id}")
        
        # 3. Listar Funcionários
        print("\n📋 Listing funcionarios...")
        response = await client.get(f"{BASE_URL}/funcionarios")
        assert response.status_code == 200
        data = response.json()
        assert len(data["funcionarios"]) > 0
        print(f"✅ Found {data['total']} funcionarios")
        
        # 4. Buscar por ID
        print(f"\n🔍 Getting funcionario {funcionario_id}...")
        response = await client.get(f"{BASE_URL}/funcionarios/{funcionario_id}")
        assert response.status_code == 200
        funcionario = response.json()
        assert funcionario["id"] == funcionario_id
        assert funcionario["ativo"] == True
        print(f"✅ Funcionario retrieved: {funcionario['nome']}")
        
        # 5. Atualizar
        print(f"\n✏️ Updating funcionario...")
        update_payload = {
            "nome": "João Silva Santos",
            "salario": "6000.00"
        }
        response = await client.put(
            f"{BASE_URL}/funcionarios/{funcionario_id}",
            json=update_payload
        )
        assert response.status_code == 200
        print("✅ Updated")
        
        # 6. Desativar
        print(f"\n🔴 Deactivating funcionario...")
        deactivate_payload = {"motivo": "Demissão"}
        response = await client.patch(
            f"{BASE_URL}/funcionarios/{funcionario_id}/desativar",
            json=deactivate_payload
        )
        assert response.status_code == 200
        funcionario = response.json()
        assert funcionario["ativo"] == False
        print("✅ Deactivated")
        
        # 7. Ativar
        print(f"\n🟢 Reactivating funcionario...")
        response = await client.patch(
            f"{BASE_URL}/funcionarios/{funcionario_id}/ativar"
        )
        assert response.status_code == 200
        funcionario = response.json()
        assert funcionario["ativo"] == True
        print("✅ Reactivated")
        
        # 8. Alterar Cargo
        print(f"\n🎯 Changing cargo...")
        cargo_payload = {"novo_cargo": "gerente"}
        response = await client.patch(
            f"{BASE_URL}/funcionarios/{funcionario_id}/cargo",
            json=cargo_payload
        )
        assert response.status_code == 200
        print("✅ Cargo changed")
        
        # 9. Listar por Departamento
        print(f"\n📊 Filtering by departamento...")
        response = await client.get(
            f"{BASE_URL}/funcionarios?departamento=ti&apenas_ativos=true"
        )
        assert response.status_code == 200
        data = response.json()
        assert all(f["departamento"] == "ti" for f in data["funcionarios"])
        assert all(f["ativo"] for f in data["funcionarios"])
        print(f"✅ Filtered: {len(data['funcionarios'])} TI employees")
        
        # 10. Validações
        print(f"\n❌ Testing validations...")
        
        # CPF Duplicado
        response = await client.post(
            f"{BASE_URL}/funcionarios",
            json=create_payload
        )
        assert response.status_code == 409
        print("✅ Duplicate CPF rejected")
        
        # Dados Inválidos
        invalid_payload = create_payload.copy()
        invalid_payload["cpf"] = "12345"  # Muito curto
        response = await client.post(
            f"{BASE_URL}/funcionarios",
            json=invalid_payload
        )
        assert response.status_code == 422
        print("✅ Invalid data rejected")
        
        print("\n🎉 All tests passed!")

if __name__ == "__main__":
    asyncio.run(test_complete_flow())
```

## 3. Conftest.py Atualizado

Arquivo: tests/conftest.py

```python
import pytest
import asyncio
from motor.motor_asyncio import AsyncIOMotorClient
from src.infrastructure.persistence.database import get_database
from src.infrastructure.persistence.seed_data import seed_funcionarios, clear_seed_data
from src.infrastructure.config.settings import settings

@pytest.fixture
def event_loop():
    loop = asyncio.get_event_loop_policy().new_event_loop()
    yield loop
    loop.close()

@pytest.fixture
async def db():
    # Conectar
    client = AsyncIOMotorClient(settings.mongodb_url)
    db = client[settings.mongodb_db_name]
    
    # Carregar seed data
    await seed_funcionarios(db)
    
    yield db
    
    # Cleanup
    await clear_seed_data(db)
    client.close()

@pytest.fixture
async def async_client(db):
    from fastapi.testclient import TestClient
    from src.presentation.main import app
    
    # Injetar db mockada
    async def override_get_db():
        return db
    
    app.dependency_overrides[get_database] = override_get_db
    
    with TestClient(app) as client:
        yield client
    
    app.dependency_overrides.clear()

@pytest.fixture
def funcionario_factory():
    """Factory para criar funcionários em testes"""
    def _create(
        nome="João Silva",
        cpf="12345678910",
        email="joao@test.com",
        **kwargs
    ):
        from datetime import date
        from decimal import Decimal
        from src.domain.entities.funcionario import Funcionario
        from src.domain.value_objects import CPF, Email, Endereco
        from src.domain.entities.enums import Cargo, Departamento
        
        return Funcionario.criar(
            nome=nome,
            cpf_string=cpf,
            email_string=email,
            data_nascimento=date(1990, 5, 15),
            cargo=kwargs.get("cargo", Cargo.ANALISTA),
            departamento=kwargs.get("departamento", Departamento.TI),
            endereco=kwargs.get("endereco", Endereco(
                rua="Rua A",
                numero="123",
                bairro="Centro",
                cidade="São Paulo",
                estado="SP",
                cep="01234567"
            )),
            salario=kwargs.get("salario", Decimal("5000.00")),
            telefone_string=kwargs.get("telefone")
        )
    
    return _create
```

## 4. README Atualizado

Adicionar seções em docs/README.md:

```markdown
## 🚀 Iniciando

### Com Docker Compose
\`\`\`bash
docker-compose up -d
curl http://localhost:8000/health
\`\`\`

### Localmente
\`\`\`bash
./setup.sh
source .venv/bin/activate
docker-compose up -d mongodb  # Apenas BD
python -m uvicorn src.presentation.main:app --reload
\`\`\`

## 📖 Swagger UI
[http://localhost:8000/docs](http://localhost:8000/docs)

## 🧪 Testes Manuais
\`\`\`bash
# Executar fluxo completo
python scripts/test_manual.py

# Ou via curl
curl -X POST http://localhost:8000/api/v1/funcionarios \
  -H "Content-Type: application/json" \
  -d @docs/examples/create_funcionario.json
\`\`\`

## 📊 Health Checks
\`\`\`bash
curl http://localhost:8000/health
curl http://localhost:8000/health/db
curl http://localhost:8000/health/live
curl http://localhost:8000/health/ready
\`\`\`

## 📝 Logs
\`\`\`bash
tail -f logs/ms-cadastro-funcionario.log | python -m json.tool
\`\`\`

## 🛑 Parar
\`\`\`bash
docker-compose down
\`\`\`
```

## 5. Exemplos de Payloads

Arquivo: docs/examples/create_funcionario.json

```json
{
  "nome": "João Silva",
  "cpf": "12345678910",
  "email": "joao@empresa.com",
  "telefone": "(11) 98765-4321",
  "data_nascimento": "1990-05-15",
  "cargo": "analista",
  "departamento": "ti",
  "endereco": {
    "rua": "Rua das Flores",
    "numero": "123",
    "complemento": "Apt 456",
    "bairro": "Vila Mariana",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01234567"
  },
  "salario": "5000.00"
}
```

## 6. Arquivo de Fluxo Completo

Arquivo: docs/COMPLETE_FLOW.md

```markdown
# Fluxo Completo - Criar, Listar, Atualizar, Desativar Funcionário

## 1. Criar Funcionário

POST /api/v1/funcionarios
```

## 7. Atualizar conftest.py com Mais Fixtures

```python
@pytest.fixture
def endereco_factory():
    from src.domain.value_objects import Endereco
    def _create(
        rua="Rua A",
        numero="123",
        cidade="São Paulo",
        estado="SP",
        cep="01234567",
        **kwargs
    ):
        return Endereco(
            rua=rua,
            numero=numero,
            complemento=kwargs.get("complemento"),
            bairro=kwargs.get("bairro", "Centro"),
            cidade=cidade,
            estado=estado,
            cep=cep
        )
    return _create

@pytest.fixture
def cpf_factory():
    from src.domain.value_objects import CPF
    def _create(cpf="12345678910"):
        return CPF(cpf)
    return _create

@pytest.fixture
def email_factory():
    from src.domain.value_objects import Email
    def _create(email="test@test.com"):
        return Email(email)
    return _create
```

## 8. Arquivos a Gerar

1. docs/VALIDATION_CHECKLIST.md
2. docs/COMPLETE_FLOW.md
3. scripts/test_manual.py (ou tests/e2e/test_flow.py)
4. docs/examples/create_funcionario.json
5. docs/examples/update_funcionario.json
6. docs/examples/deactivate_funcionario.json
7. Atualizar: tests/conftest.py
8. Atualizar: README.md

## Boas Práticas

1. **Testes E2E:**
   - Usar dados realistas
   - Limpar estado antes/depois
   - Documentar cada passo

2. **Fixtures:**
   - Reutilizáveis
   - Independentes
   - Cleanup automático

3. **Validação:**
   - Testar casos felizes
   - Testar casos de erro
   - Testar validações

4. **Documentação:**
   - Screenshots do Swagger
   - Exemplos de curl
   - Tudo documentado

## Resultado Esperado

Após esta task:
1. Aplicação totalmente integrada e funcional
2. Todos endpoints testáveis
3. Fixtures preparadas para testes futuros
4. Documentação completa
5. Pronta para desenvolvimento de testes automatizados
6. Pronta para produção (com pequenos ajustes de segurança)

## Checklist Final

- [ ] docker-compose up -d funciona
- [ ] Health checks respondem
- [ ] Swagger acessível
- [ ] Todos endpoints funcionam
- [ ] Validações funcionam
- [ ] Logs estruturados
- [ ] Seed data funciona
- [ ] Fixtures preparadas
- [ ] Conftest.py completo
- [ ] Documentação atualizada
- [ ] Exemplo de fluxo documentado

## Próximas Fases (Pós-Refinamento)

1. **Unit Tests** - Testar Domain, Application
2. **Integration Tests** - Testar com BD real (containerizado)
3. **E2E Tests** - Testar via API
4. **Performance Tests** - Benchmarks
5. **Security Tests** - Validação, autenticação
6. **CI/CD** - GitHub Actions / GitLab CI
7. **Monitoring** - Prometheus, Datadog
8. **API Versioning** - /api/v2/ para mudanças futuras
```

---

## Dependências Futuras

- Próxima fase de testes implementará Unit/Integration/E2E
- CI/CD usará fixtures

---

## Referência Técnica

- [pytest Documentation](https://docs.pytest.org/)
- [httpx AsyncClient](https://www.python-httpx.org/)
- [Factory Boy Alternative - Custom Factories](https://docs.pytest.org/en/stable/how-to/fixtures.html)
