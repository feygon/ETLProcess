---
title: "ETLProcess — Secrets Remediation"
created: 2026-08-09
status: action-required
category: security
tags: [secrets, credentials, remediation, env]
---

# 🔑 ETLProcess — Secrets Remediation

**Status:** 🟡 Working tree cleaned · **git history still contains the secrets**

## TLDR

Live credentials were hardcoded in this public repository. They have been removed from the
working tree and replaced with fail-closed environment reads.

**Two things still need a human at a console:**

1. Create a local `.env` from `.env.example` (the code will not run without it).
2. Decide what to do about git history, which still contains every secret below.

## Contents

- [TLDR](#tldr)
- [What was exposed](#what-was-exposed)
- [What changed](#what-changed)
- [⚠️ Do this when you are next at the console](#️-do-this-when-you-are-next-at-the-console)
- [History is not clean](#history-is-not-clean)
- [Still outstanding](#still-outstanding)

## What was exposed

All of this was public, in a repository pinned to the owner's GitHub profile.

| Secret | Where | Severity |
|---|---|---|
| Mandrill SMTP API key `REDACTED_SMTP_KEY` | `codemap/General/IO/Email.cs`, `codemap/BasicPreprocess/General/IO/Email.cs` | 🔴 Live third-party service credential |
| SQL credentials `User id=REDACTED;Password=REDACTED` | `codemap/General/IO/SQL.cs`, `codemap/BasicPreprocess/General/IO/SQL.cs` | 🔴 Username and password, identical to each other |
| Server names `SERVER\INSTANCE`, `DATABASE` | same files | 🟠 Internal infrastructure disclosure |
| UNC path `\\SERVER\SUBMIT\prt\` | `codemap/Specific/Program.cs`, `codemap/BasicPreprocess/Specific/Program.cs` | 🟠 Internal infrastructure disclosure |
| Mailboxes `WebSupport@…`, `support@…` | Email.cs ×3, Program.cs ×2 | 🟠 Operational addresses |
| Real client name (104 occurrences) | 17 files across `codemap/` | 🟠 Client confidentiality |

**Note on scope.** `ETLProcessFactory/IO/Email.cs` had already been sanitized by hand — it
carried placeholder values and an explicit comment that hardcoding credentials is not
recommended. The `codemap/` tree is the unsanitized original and was missed in that pass.
That is the lesson worth keeping: sanitizing one copy of a duplicated tree is not sanitizing.

## What changed

- All four SMTP constants and both SQL connection strings now read from the environment via a
  private `Env(string key)` helper that **throws** when a key is absent. No defaults, no
  fallbacks — a missing key is a loud failure, not a silent bad connection.
- `MailAddress` literals now read `ETLP_SMTP_FROM`.
- ETLP addresses replaced with `Sample@ETLP.sample.com`; the external/client
  placeholder is `Sample@GPHealth.sample.com`. Both belong in `.env`, not in source.
- The former client name was replaced throughout with the neutral placeholder `GPHealth`.
- Added `.env.example` and a `.gitignore` that excludes `.env` while keeping `.env.example`.

## ⚠️ Do this when you are next at the console

```bash
cd D:\Repos\ETLProcess
copy .env.example .env      # PowerShell: Copy-Item .env.example .env
```

Then fill in `.env`:

- [ ] `ETLP_SMTP_HOST` / `ETLP_SMTP_PORT` / `ETLP_SMTP_USER` / `ETLP_SMTP_PASSWORD`
- [ ] `ETLP_SMTP_FROM`
- [ ] `ETLP_SUPPORT_EMAIL`, `ETLP_CLIENT_EMAIL`
- [ ] `ETLP_SQL_CONNECTION` — prefer `Integrated Security=True` over an embedded password
- [ ] Confirm `.env` does **not** appear in `git status`

⚠️ .NET Framework does not load `.env` files natively. Either set these as real environment
variables, or add a loader. **Do not** reintroduce a config file that gets committed.

## History is not clean

Removing a secret from the working tree does not remove it from git. Every credential above is
still retrievable from this repository's history by anyone who clones it.

Options, roughly in order of effort:

1. **Treat as compromised.** The Mandrill key and the SQL account belong to a former employer.
   The genuinely responsible move is to notify ETLP so they can rotate, regardless of
   what happens to this repo. These are likely long dead, but "likely" is not "verified."
2. **Rewrite history** with `git filter-repo` and force-push. Effective, but rewrites every
   commit hash in a public repo.
3. **Delete and recreate the repository** from the cleaned tree, losing history entirely. For a
   portfolio case study, the history has little value — this is the cheapest complete fix.

**Until one of these happens, this repo should probably not be pinned to a profile that
advertises supply-chain hardening.** The contradiction is the risk, more than the dead
credential.

## Still outstanding

- `packages/EntityFramework.6.4.4/` is vendored in full — DLLs, `ef6.exe`, PDBs, and
  `install.ps1` / `init.ps1`. Not a secret, but a committed third-party binary tree with install
  scripts, which sits badly beside a supply-chain-hardening claim. `.gitignore` has the entry
  commented out; untracking it requires `git rm -r --cached packages/`.
- `codemap/obj/Debug/codemap.exe` and `.pdb` are committed build output.
- The README states the library "generates a decoupled skeleton." It does not generate code;
  interface contracts drive compile-time enforcement of the shape an implementor must provide.
  The claim needs rewording to describe type-enforced contracts rather than codegen.
