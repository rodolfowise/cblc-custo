# Instruções de Execução das Tasks com Copilot

**Data:** 2026-04-23  
**Objetivo:** Guia prático de como usar as Tasks com GitHub Copilot ou Claude

---

## 📋 Como Usar Este Documento

Cada arquivo task_X.md contém tudo necessário para que você (ou um AI Assistant) implemente a funcionalidade. O arquivo é estruturado para ser:

1. **Legível por humanos** - Entender o que precisa ser feito
2. **Executável por Copilot** - Prompt está pronto para usar
3. **Validável** - Critério de pronto é testável

---

## 🔄 Fluxo de Trabalho Recomendado

### Opção 1: Usando GitHub Copilot Chat (Recomendado)

```
1. Abra VS Code
2. Abra Chat do Copilot (Ctrl+I ou Cmd+I)
3. Copie TUDO o "Prompt de Execução" da task
4. Cole no Chat do Copilot
5. Pressione Enter
6. Revise o código gerado
7. Copie os arquivos para seu projeto
```

### Opção 2: Usando Claude API/Web

```
1. Acesse claude.ai ou use API
2. Cole o "Prompt de Execução" completo
3. Aguarde a resposta
4. Copie os arquivos para seu projeto
5. Ajuste paths conforme necessário
```

### Opção 3: Linha por Linha (Manual)

```
1. Leia a task
2. Entenda o objetivo
3. Implemente conforme a descrição
4. Use o "Prompt de Execução" como referência
```

---

## 📄 Estrutura de Cada Task

```
# Task X: Nome Descritivo

## Nome da Task
Titulo curto, objetivo claro

## Objetivo
O que será entregue nessa etapa

## Principais Entregas
✅ Lista de artefatos (arquivos, funcionalidades)

## Critério de Pronto
- [ ] Checklist testável
- [ ] Validação clara

## Prompt de Execução
🤖 >>> COPIE TUDO ISSO E COLE NO COPILOT <<<

(Muito longo, com contexto completo, exemplos de código, padrões)

## Boas Práticas
Orientações de qualidade

## Resultado Esperado
O que você deve ter após completar a task

## Referência Técnica
Links para documentação externa
```

---

## 🎯 Checklist: Antes de Cada Task

- [ ] Você leu a seção "Objetivo"?
- [ ] Você entendeu "Principais Entregas"?
- [ ] Você tem todas as dependências (tasks anteriores completas)?
- [ ] Você tem o "Prompt de Execução" pronto?
- [ ] Você sabe qual Copilot usará (Chat, API, Claude)?

---

## 💡 Dicas Práticas

### Para Copilot (GitHub)

1. **Use Chat (Ctrl+I)** em vez de Inline Edit
2. **Cole o prompt completo** - Copilot gera melhor com contexto
3. **Especifique estrutura** - "Generate file: src/domain/..."
4. **Valide com type checking** - `mypy src/` após gerar
5. **Revise imports** - Certifique-se que imports estão corretos

### Para Claude

1. **Use web ou API** - Contexto maior (100k+ tokens)
2. **Cole tudo junto** - Claude digere melhor contexto gigante
3. **Peça revisão** - "Review this code for DDD compliance"
4. **Itere incrementalmente** - Se arquivo fica grande, divida

### Commits Git

Após cada task:
```bash
git add .
git commit -m "feat(task-X): implementar [funcionalidade]"
git push
```

---

## 🚀 Exemplo: Executando Task 1 com Copilot

### Passo 1: Abra a Task
```
c:\temp\custoCblc\cblc-custo\refinamento\task_1.md
```

### Passo 2: Copie o Prompt Completo
(Seção "Prompt de Execução", tudo entre as linhas)

```
Como especialista em arquitetura Python e DevOps, execute a seguinte tarefa:

## Contexto
Você está criando um microserviço de cadastro de funcionários...
[TODO RESTO DO PROMPT]
```

### Passo 3: Cole no Copilot
```
Ctrl+I (abrir chat)
Colar com Ctrl+V
Pressionar Enter
```

### Passo 4: Aguarde Resposta
Copilot gerará:
- Estrutura de pastas
- Arquivo pyproject.toml
- docker-compose.yml
- Dockerfile
- .env.example
- setup.sh / setup.bat
- README.md
- conftest.py base
- main.py com health check
- Todos os __init__.py

### Passo 5: Revise e Integre
```bash
# Verificar sintaxe
python -m py_compile src/**/*.py

# Testar Docker
docker-compose build
docker-compose up -d
curl http://localhost:8000/health

# Se tudo OK
git add .
git commit -m "feat(task-1): setup inicial do projeto"
```

### Passo 6: Valide Critério de Pronto

Checklist do task_1.md:
- [ ] `docker-compose up -d` executa sem erros ✅
- [ ] API está acessível em `http://localhost:8000` ✅
- [ ] MongoDB está acessível ✅
- [ ] `.env.example` contém todas variáveis ✅
- [ ] Estrutura de pastas reflete camadas DDD ✅
- [ ] `.gitignore` configurado ✅

### Passo 7: Próxima Task
Abra `refinamento/task_2.md` e repita

---

## ⚠️ Possíveis Problemas e Soluções

### "Copilot gerou código com erros"

**Solução:**
1. Tente novamente com prompt mais claro
2. Divida o prompt em partes menores
3. Use Claude Web em vez (melhor reasoning)
4. Revise manualmente o código

### "Arquivo é muito grande"

**Solução:**
1. Peça ao Copilot para dividir em múltiplos arquivos
2. Gere um arquivo por vez
3. Use comments para marcar seções

### "Imports estão circulares"

**Solução:**
1. Verifique que seguiu a estrutura DDD
2. Releia a Task 2 (Injeção de Dependências)
3. Peça ao Copilot: "Fix circular imports in this code"

### "Docker não inicia"

**Solução:**
1. Verifique docker-compose.yml syntax
2. Confira ports em uso
3. Limpe: `docker-compose down -v`
4. Tente: `docker-compose up --build`

---

## 📊 Ganho de Produtividade por Task

| Task | Manual | Com Copilot | Ganho |
|------|--------|-----------|-------|
| 1 | 2h | 20min | 85% ↑ |
| 2 | 3h | 30min | 83% ↑ |
| 3 | 3h | 25min | 86% ↑ |
| 4 | 3h | 35min | 81% ↑ |
| 5 | 3h | 45min | 75% ↑ |
| 6 | 3h | 50min | 72% ↑ |
| 7 | 2h | 20min | 83% ↑ |
| 8 | 3h | 45min | 75% ↑ |
| 9 | 2h | 25min | 79% ↑ |
| 10 | 2h | 30min | 75% ↑ |
| **TOTAL** | **24h** | **4h40min** | **80% ↑** |

---

## 🎓 Aprendizado ao Usar Este Guia

Conforme implemente as tasks, você aprenderá:

- ✅ Domain-Driven Design (DDD)
- ✅ Clean Architecture
- ✅ Padrões: Repository, Unit of Work, Factory
- ✅ FastAPI em produção
- ✅ MongoDB async com Motor
- ✅ Injeção de Dependências
- ✅ Logging estruturado
- ✅ Testes com pytest/fixtures
- ✅ Docker Compose para desenvolvimento
- ✅ Observabilidade e Health Checks

---

## 🔍 Validação de Cada Task

### Ao terminar cada task:

1. **Compile Python**
   ```bash
   python -m py_compile src/**/*.py
   ```

2. **Type Check** (se instalado mypy)
   ```bash
   mypy src/
   ```

3. **Teste básico**
   ```bash
   docker-compose up -d
   curl http://localhost:8000/health
   docker-compose down
   ```

4. **Git Commit**
   ```bash
   git status  # Revise mudanças
   git add .
   git commit -m "feat(task-X): [descrição]"
   ```

---

## 📞 Suporte

Se travar em uma task:

1. **Releia o Objetivo**
2. **Revise Critério de Pronto**
3. **Consulte Boas Práticas**
4. **Copie prompt novamente para Copilot**
5. **Ajuste prompt** com mais detalhes do erro

---

## 🎯 Meta Final

Após completar todas as 10 tasks você terá:

✅ **Microserviço production-ready**
- Completo com CRUD, validações, API REST
- Documentação automática (Swagger)
- Logging estruturado
- Preparado para testes
- Containerizado e pronto para deploy
- Seguindo best practices de software engineering

---

## 📈 Próximos Passos Após as 10 Tasks

1. **Unit Tests** → Adicione testes para Domain e Application
2. **Integration Tests** → Teste Infrastructure com BD real
3. **E2E Tests** → Teste API completa com fixtures
4. **CI/CD** → Crie pipeline (GitHub Actions)
5. **Monitoring** → Integre com Prometheus/Datadog
6. **Authentication** → Adicione JWT/OAuth
7. **Authorization** → Implemente RBAC
8. **API Versioning** → Support /api/v2/
9. **Performance** → Profile e otimize
10. **Documentation** → Postman collection, ADR's

---

## 🚀 Comece Agora

### Opção 1: Rápido (Com Copilot)
1. Abra [Task 1](task_1.md)
2. Copie o "Prompt de Execução"
3. Cole no Copilot Chat
4. Integre os arquivos

### Opção 2: Entendimento (Manual)
1. Leia cada task
2. Implemente manualmente
3. Use prompts como referência
4. Aprenda padrões no processo

### Opção 3: Híbrido (Recomendado)
1. Leia objetivo da task
2. Use Copilot para gerar scaffold
3. Revise e entenda o código
4. Faça ajustes manuais se necessário

---

**Bom desenvolvimento! 🚀**

Próximo passo: [Task 1: Setup do Projeto](task_1.md)
