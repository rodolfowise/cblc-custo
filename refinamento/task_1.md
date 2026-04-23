# Task 1: Setup do Projeto e Infraestrutura

**Ordem:** 1/10  
**Duração Estimada:** 1-2 horas  

---

## Nome da Task

**Setup Inicial do Projeto ms-cadastro-funcionario com Docker e Ambiente Clean**

---

## Objetivo

Estabelecer a fundação do projeto com estrutura de pastas, configuração de ambiente, containerização Docker e ferramentas de desenvolvimento, garantindo que qualquer desenvolvedor possa clonar o repositório e executar a aplicação localmente em minutos.

---

## Principais Entregas

- ✅ Estrutura de diretórios seguindo padrão DDD (src/domain, src/application, src/infrastructure, src/presentation)
- ✅ Arquivo `pyproject.toml` com todas as dependências organizadas (FastAPI, Pydantic, Motor, etc)
- ✅ Arquivo `.env.example` com variáveis de ambiente documentadas
- ✅ `docker-compose.yml` com serviços de API e MongoDB
- ✅ `Dockerfile` multi-stage para build e runtime otimizado
- ✅ `.gitignore` apropriado para projeto Python (.venv, __pycache__, .env, etc)
- ✅ `.editorconfig` para padronização de código
- ✅ `README.md` com instruções de setup local e via Docker
- ✅ Script `setup.sh` ou `.bat` para inicializar ambiente virtual e instalar dependências
- ✅ Arquivo `conftest.py` base para configuração de testes (sem implementação de testes ainda)
- ✅ Arquivo `__init__.py` em cada diretório de pacote Python

---

## Critério de Pronto

- [ ] Projeto pode ser clonado e `docker-compose up -d` executa sem erros
- [ ] API está acessível em `http://localhost:8000` (health check endpoint funciona)
- [ ] MongoDB está acessível via connection string em variável de ambiente
- [ ] `pip install -r requirements.txt` (ou via pyproject.toml) instala sem erros em venv local
- [ ] `.env.example` contém todas as variáveis necessárias documentadas
- [ ] Todos os arquivos `__init__.py` criados
- [ ] Estrutura de pastas reflete camadas DDD (não misturado)
- [ ] `.gitignore` está configurado corretamente (testa com `git status`)

---

## Prompt de Execução

```
Como especialista em arquitetura Python e DevOps, execute a seguinte tarefa:

## Contexto
Você está criando um microserviço de cadastro de funcionários chamado "ms-cadastro-funcionario" 
usando FastAPI (Python), Pydantic para validação de dados, Motor para acesso assíncrono ao MongoDB, 
e Docker + Docker Compose para containerização.

## Tecnologias
- Python 3.11+
- FastAPI 0.104+
- Pydantic v2
- Motor (async driver MongoDB)
- MongoDB 6.0+
- Docker e Docker Compose
- Desenvolvimento: pytest, httpx (para testes HTTP), faker (para fixtures)

## Objetivo
Criar a estrutura base do projeto com ambiente limpo, configurações externas (variáveis de ambiente),
containerização e documentação clara para onboarding de desenvolvedores.

## Estrutura de Diretórios Esperada
```
ms-cadastro-funcionario/
├── src/
│   ├── __init__.py
│   ├── domain/
│   │   ├── __init__.py
│   │   ├── entities/
│   │   │   └── __init__.py
│   │   ├── value_objects/
│   │   │   └── __init__.py
│   │   ├── repositories/
│   │   │   └── __init__.py
│   │   └── services/
│   │       └── __init__.py
│   │
│   ├── application/
│   │   ├── __init__.py
│   │   ├── dtos/
│   │   │   └── __init__.py
│   │   ├── use_cases/
│   │   │   └── __init__.py
│   │   ├── mappers/
│   │   │   └── __init__.py
│   │   └── services/
│   │       └── __init__.py
│   │
│   ├── infrastructure/
│   │   ├── __init__.py
│   │   ├── persistence/
│   │   │   ├── __init__.py
│   │   │   ├── repositories/
│   │   │   │   └── __init__.py
│   │   │   └── database.py
│   │   ├── config/
│   │   │   ├── __init__.py
│   │   │   ├── settings.py
│   │   │   └── logging_config.py
│   │   └── external/
│   │       └── __init__.py
│   │
│   └── presentation/
│       ├── __init__.py
│       ├── api/
│       │   ├── __init__.py
│       │   └── v1/
│       │       ├── __init__.py
│       │       ├── routes/
│       │       │   └── __init__.py
│       │       └── schemas/
│       │           └── __init__.py
│       └── middlewares/
│           └── __init__.py
│
├── tests/
│   ├── __init__.py
│   ├── conftest.py
│   ├── unit/
│   │   └── __init__.py
│   ├── integration/
│   │   └── __init__.py
│   └── fixtures/
│       └── __init__.py
│
├── docker-compose.yml
├── Dockerfile
├── pyproject.toml
├── requirements.txt
├── .env.example
├── .gitignore
├── .editorconfig
├── setup.sh (ou setup.bat para Windows)
├── README.md
└── .dockerignore
```

## Instruções Específicas

### 1. Arquivo `pyproject.toml`
Criar com:
- Seção [project]: nome, versão, descrição, autores
- Seção [project.optional-dependencies]: dev, test, prod
- Dependências principais:
  - fastapi==0.104.1
  - uvicorn[standard]==0.24.0
  - pydantic==2.5.0
  - pydantic-settings==2.1.0
  - motor==3.3.2 (async MongoDB driver)
  - pymongo==4.6.0
  - python-dotenv==1.0.0
  - pydantic-core==2.14.0

### 2. Arquivo `.env.example`
Incluir:
- ENVIRONMENT (development, staging, production)
- MONGODB_URL (mongo://localhost:27017)
- MONGODB_DB_NAME (ms_cadastro_funcionario)
- API_TITLE (ms-cadastro-funcionario)
- API_VERSION (1.0.0)
- LOG_LEVEL (INFO, DEBUG, WARNING)
- API_PORT (8000)
- API_HOST (0.0.0.0)

### 3. Arquivo `docker-compose.yml`
- Serviço: mongodb (imagem mongo:6.0, porta 27017, volume para /data/db)
- Serviço: api (build context ., porta 8000:8000, depends_on: mongodb)
- Network compartilhada
- Variáveis de ambiente mapeadas de .env (usar .env file)
- Healthcheck para ambos serviços

### 4. Arquivo `Dockerfile`
Multi-stage:
- Stage 1 (builder): Python 3.11 slim, copiar pyproject.toml, instalar dependências
- Stage 2 (runtime): Python 3.11 slim, copiar de builder, copiar src/, EXPOSE 8000
- ENTRYPOINT: uvicorn src.presentation.main:app --host 0.0.0.0 --port 8000 --reload

### 5. Arquivo `.gitignore`
Incluir:
- .venv/, venv/, env/
- __pycache__/, *.pyc, *.pyo, *.egg-info/
- .pytest_cache/, .coverage, htmlcov/
- .vscode/, .idea/, *.swp, *.swo
- .env (não o .env.example)
- *.log
- .DS_Store
- dist/, build/

### 6. Arquivo `.editorconfig`
Padrões:
- root = true
- [*.py]: indent_style = space, indent_size = 4, end_of_line = lf
- [*.{yml,yaml,json}]: indent_size = 2

### 7. Arquivo `setup.sh` (Linux/Mac)
#!/bin/bash
set -e
echo "Creating virtual environment..."
python3 -m venv .venv
source .venv/bin/activate
echo "Installing dependencies..."
pip install --upgrade pip setuptools
pip install -e ".[dev]"
echo "Setup complete! Run: docker-compose up -d"

### 8. Arquivo `setup.bat` (Windows)
@echo off
echo Creating virtual environment...
python -m venv .venv
call .venv\\Scripts\\activate.bat
echo Installing dependencies...
python -m pip install --upgrade pip setuptools
pip install -e ".[dev]"
echo Setup complete! Run: docker-compose up -d

### 9. Arquivo `README.md`
Seções:
- Descrição do projeto
- Pré-requisitos (Python 3.11+, Docker, Docker Compose)
- Setup Local (com venv)
- Setup com Docker
- Estrutura do projeto (breve explicação das camadas)
- Endpoints disponíveis (será expandido nas tasks futuras)
- Contribuição

### 10. Arquivo `conftest.py` (base para testes futuros)
Sem implementação de testes ainda, apenas:
- Import de pytest, async-related utilities (pytest-asyncio)
- Configurações de markers
- Comments indicando onde fixtures serão adicionadas
- Path setup (adicionar src/ ao sys.path)

### 11. Arquivo `src/presentation/main.py` (mínimo)
- Criar aplicação FastAPI
- Adicionar health check endpoint GET /health que retorna {"status": "ok"}
- Configurar CORS (permitir localhost:3000 para frontend futuro)
- Não adicionar rotas de negócio ainda (será feito na Task 8)

## Boas Práticas e Padrões

1. **Organização de Código:**
   - Separar por responsabilidade (camadas DDD)
   - Cada camada independente, sem dependências circulares
   - Namespaces refletem estrutura de pastas

2. **Variáveis de Ambiente:**
   - Usar Pydantic Settings para validação tipada
   - Nunca commitar .env, apenas .env.example
   - Suportar defaults sensatos para desenvolvimento local

3. **Docker:**
   - Multi-stage para reduzir tamanho de imagem
   - Healthcheck em cada serviço
   - Usar volumes para código em desenvolvimento (:w modo watch)
   - Não usar latest para imagens base, fixar versão

4. **Desacoplamento:**
   - Estrutura de pastas reflete independência de camadas
   - Não importar de presentation em domain/application
   - Infrastructure implementa interfaces definidas em domain

5. **Preparação para Testes:**
   - conftest.py base criado (será expandido com fixtures)
   - Estrutura de pastas tests/ reflete src/
   - Markers pytest definidos mas vazios

## Resultado Esperado

Após executar esta tarefa, deve-se conseguir:
1. Clone do repositório
2. `cd ms-cadastro-funcionario`
3. `./setup.sh` (ou setup.bat)
4. `source .venv/bin/activate` (ou activate.bat)
5. `docker-compose up -d`
6. `curl http://localhost:8000/health` → {"status": "ok"}

Ou diretamente:
1. Clone
2. `docker-compose up -d`
3. `curl http://localhost:8000/health` → {"status": "ok"}

## Arquivos a Gerar

1. pyproject.toml
2. requirements.txt (gerado de pyproject.toml)
3. .env.example
4. docker-compose.yml
5. Dockerfile
6. .gitignore
7. .editorconfig
8. setup.sh
9. setup.bat
10. README.md
11. conftest.py (em tests/)
12. src/presentation/main.py
13. Todos os __init__.py da estrutura de pastas

Gere cada arquivo com conteúdo completo, pronto para produção, sem comentários excessivos.
```

---

## Dependências Futuras

As tasks seguintes dependerão de:
- Variáveis de ambiente configuradas
- MongoDB rodando (via Docker)
- Estrutura de pastas estabelecida
- FastAPI application instanciada

---

## Referência Técnica

- [FastAPI Documentation](https://fastapi.tiangolo.com/)
- [Pydantic v2 Documentation](https://docs.pydantic.dev/)
- [Motor Documentation](https://motor.readthedocs.io/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
