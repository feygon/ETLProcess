---
title: "ETLProcess — Secrets Remediation"
created: 2026-08-09
status: action-required
category: security
tags: [secrets, credentials, remediation, env]
---

# 🔑 ETLProcess — Secrets Remediation

**Status:** 🟢 Working tree cleaned · **history rewritten and force-pushed 2026-08-09**

## TLDR

Live credentials were hardcoded in this public repository. They have been removed from the
working tree, replaced with fail-closed environment reads, and **purged from every commit in
history** via `git filter-branch` across all branches and tags.

Verified from a fresh clone: zero occurrences of the SMTP key, the SQL credentials, the internal
server names, the UNC path, any former-employer email address, the former employer's name, or the
former client's name — across all 39 commits on every branch and tag.

**One thing still needs a human at a console:**

1. Create a local `.env` from `.env.example` — the code will not run without it.

**One thing is outside our control:** see [After the rewrite](#after-the-rewrite).

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
| Mandrill SMTP API key (22-char, redacted) | `codemap/General/IO/Email.cs`, `codemap/BasicPreprocess/General/IO/Email.cs` | 🔴 Live third-party service credential |
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
- Processor-side addresses replaced with `Sample@ETLP.sample.com`; the external/client
  placeholder is `Sample@GPHealth.sample.com`. Both belong in `.env`, not in source.
- The former employer's name was replaced throughout with `ETLP`, in XML doc comments and in the
  email class, which is now named `ETLPEmail`.
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

## After the rewrite

**Done, 2026-08-09.** History was rewritten with `git filter-branch --tree-filter` across all
refs and force-pushed. `master`, `Library`, and both tags (`v1.0.1alpha`, `v1.1.1alphaStable`)
were rewritten. A backup bundle of the pre-rewrite state is at
`D:\Repos\ETLProcess-BACKUP-2026-08-09.bundle` — **that bundle still contains every secret**, so
it is local-only and must never be published.

`git-filter-repo` was not used: it is not installed, and adding a dependency mid-remediation
would have violated this project's own supply-chain rule. `filter-branch` is built into git and
is entirely adequate for 39 commits.

Two caveats that a rewrite does not fix:

1. **GitHub retains unreferenced objects.** After a force-push, old commits can remain reachable
   by direct SHA URL until GitHub garbage-collects, and forks or clones taken before the rewrite
   are unaffected. To force server-side cleanup, open a GitHub Support request asking them to run
   `gc` on the repository.
2. **The credentials should still be treated as compromised.** They were public for years. The
   Mandrill key and the SQL account belong to a former employer, so the responsible step —
   independent of this repo — is to notify them so they can rotate. "Probably long dead" is not
   "verified dead."

### Second pass — employer name removed

A follow-up rewrite (same day) removed the former employer's name entirely, at the owner's
direction. All spelling variants — spaced, unspaced, lowercase, and the `…Email` class name —
now read `ETLP` / `ETLPEmail` throughout code and history.

Note for anyone editing this file: it is itself subject to the scrub. Earlier passes rewrote
this document's own prose into nonsense ("X became X"), because the filter cannot distinguish a
secret from a description of that secret. Describe what was removed in the abstract; do not
quote the removed strings.

**Deliberately kept:** `DocGen` (the commercial statement-processing platform), including
`DocGenConnection` and the `ProprietaryStack.dbo` schema references. It is a third-party product name rather
than a client or employer identifier, and it appears on the owner's public résumé. Removing it
would break the semantics of the connection property for no confidentiality gain. Revisit if the
combination of product, industry, and region is judged too identifying.

The first pass had deliberately left the company name in place on the reasoning that it is
provenance rather than a secret, and appears on the owner's public résumé. The owner's call
overrode that, and it is the more defensible position: a portfolio artifact does not need to
name the client whose production system it describes, and the code reads no worse for it.

## Still outstanding

- `packages/EntityFramework.6.4.4/` is vendored in full — DLLs, `ef6.exe`, PDBs, and
  `install.ps1` / `init.ps1`. Not a secret, but a committed third-party binary tree with install
  scripts, which sits badly beside a supply-chain-hardening claim. `.gitignore` has the entry
  commented out; untracking it requires `git rm -r --cached packages/`.
- `codemap/obj/Debug/codemap.exe` and `.pdb` are committed build output.
- The README states the library "generates a decoupled skeleton." It does not generate code;
  interface contracts drive compile-time enforcement of the shape an implementor must provide.
  The claim needs rewording to describe type-enforced contracts rather than codegen.
