---
name: azure-devops-cli
description: Manage Azure DevOps resources via CLI including projects, repos, pipelines, builds, pull requests, work items, artifacts, and service endpoints. Use when working with Azure DevOps CLI, CI/CD automation, pipelines or agents.
---

# Azure DevOps CLI

Use Azure CLI + `azure-devops` extension for Azure DevOps operations.

## Required pre-checks (always first)

Run these checks before any task. If one fails, stop and tell the user exactly how to fix it.

```bash
az --version
az extension show --name azure-devops --query name -o tsv
az account show --query user.name -o tsv
az devops configure --list
```

If missing:
- extension -> `az extension add --name azure-devops`
- login -> `az login`
- defaults -> `az devops configure --defaults organization=https://dev.azure.com/{org} project={project}`

## Operating rules

- Prefer `list/show` before `create/update/delete`.
- Never expose secret values in terminal output.
- For destructive actions, confirm target identifiers first.
- Use table output for human summaries; JSON only when parsing is required.

## References

Read only the minimal relevant reference file:

| File | Scope |
|---|---|
| `references/pipelines-and-builds.md` | Pipelines, runs, builds, releases, artifacts |
| `references/variables-and-agents.md` | Variables, variable groups, folders, pools, queues, agents |
