# ETLProcess

A C# .NET library that generates decoupled ETL programs from interface selection, so each client's
business rules stay separated from the logic that moves the data.

## TLDR

A data-to-document company had many near-identical ETL programs — one per client, each slightly
different, each siloed to whichever employee wrote it. ETLProcess replaces that with one library: a
programmer picks the input and output interfaces, and the library generates a decoupled skeleton
from those interfaces' promises. Business rules live in the implementation module; the plumbing
never changes.

**Inputs:** SQL query, XML, CSV, position-delimited flat files.
**Outputs:** XML serialization (for a document mapper) and SQL bulk copy (for reporting).

## Contents

- [TLDR](#tldr)
- [The problem](#the-problem)
- [The solution](#the-solution)
- [How it works](#how-it-works)
- [Repository layout](#repository-layout)
- [Stack and techniques](#stack-and-techniques)
- [Glossary](#glossary)
- [Sample data](#sample-data)
- [Building](#building)
- [Status](#status)

## The problem

The client served data-to-document customers: take a customer's data, produce hardcopy documents
from it. The XML output fed a third-party document mapping and break-pack program that generated
the physical mailings.

Every customer needed the same shape of work, and none of it the same way:

- **Varied inputs** — CSV, XML, SQL query results, and position-delimited flat files, often several
  at once, needing to be assembled into a single dataset.
- **Varied rules** — each customer filtered or calculated on that data differently to produce the
  values printed on their documents.
- **Varied outputs** — an XML tree for the document mapper, plus SQL for reporting.

The result was many disparate programs, each siloed to a past employee. What they wanted was a
library that could generate replacements, accommodate a decoupled implementation per released
solution, and **sequester each customer's business rules away from the logical programming**.

## The solution

ETLProcess provides inversion of control around an implementation-specific code module.

A programmer selects the input and output interfaces. From the promises those interfaces make, the
library generates a decoupled skeleton. The programmer fills in the business rules — and only the
business rules.

The library then drives that skeleton to populate the entity relationship model, and serializes
output according to a serializable profile singleton.

## How it works

```
   CSV ─┐
   XML ─┤
   SQL ─┼─► LINQ to DataSet ─► entity relationship model ─► XmlSerializer ─► document mapper
  flat ─┘     (extract + transform)          │
                                             └────────► SqlBulkCopy ─────► reporting
```

Control is inverted: the library owns the sequence, the implementation module owns the rules.
Onboarding a new customer means writing a module, not a program.

## Repository layout

| Path | What lives there |
|---|---|
| `ETLProcess/Specific/` | The per-client seam. `ClientBusinessRules.cs` and `ClientSpecificSQL.cs` hold what changes between customers; `Boilerplate/ClientETLProcess.cs` is the generated skeleton. |
| `ETLProcessFactory/Containers/` | The type system — abstract bases, database and file-record containers, header sources, key strings, member records, and foreign-key constraint elements. |
| `ETLProcessFactory/Algorithms/` | CSV and general parsing routines. |
| `ETLProcessFactory/ExtendLinQ/` | LINQ extensions, including a `DataRelationKeyColumnComparer` for joining across relations. |
| `ETLProcessFactory/BasicPreprocess/` | Pre-transform normalization stage. |
| `ETLProcess/IOFiles/` | Synthetic sample inputs — see [Sample data](#sample-data). |
| `ETLProcess/CodeMaps/` | Visual Studio DGML dependency maps plus a rendered flow diagram. |

## Stack and techniques

| | |
|---|---|
| **Language** | C# .NET Framework |
| **Design patterns** | Singleton, component model, inversion of control |
| **Data** | LINQ to DataSet, XmlSerializer, SqlBulkCopy, CSV parsing |
| **Language features** | Interfaces, delegates and generics, dependency injection, extension methods |

## Glossary

| Term | Meaning |
|---|---|
| **ETL** | Extract, Transform, Load — pull data from sources, reshape it, write it somewhere useful. |
| **Break-pack** | Splitting a bulk print run into individual customer mailings. The third-party mapper did this from the XML output. |
| **Position-delimited** | A flat file where each field is identified by its column position rather than by a separator. Common in legacy mainframe exports. |
| **LINQ to DataSet** | Querying in-memory .NET `DataSet` tables with LINQ. Used here for the transform stage. |
| **SqlBulkCopy** | .NET's high-throughput bulk insert — far faster than row-by-row writes for reporting loads. |
| **Inversion of control** | The library calls your code rather than your code calling the library. This is what keeps business rules out of the plumbing. |
| **Serializable profile singleton** | One configuration object, itself serializable, that decides how a given implementation's output is written. |
| **DGML** | Directed Graph Markup Language — the format Visual Studio uses for the dependency maps in `CodeMaps/`. |

## Sample data

`ETLProcess/IOFiles/` contains **synthetic** examples only — `John J. Smith` and `Jane J. Jones` at
"1234 Crabapple Way, Nowhere, CA 11111", sequential `ID0000001` identifiers, and dates in 1900.
No customer data of any kind is present in this repository.

## Building

Open `ETLProcess.sln` in Visual Studio and build. The solution predates `PackageReference`, so its
EntityFramework dependency is vendored under `packages/` in the old NuGet `packages.config` style
rather than restored at build time.

## Status

Portfolio case study, not maintained. Two directions were designed and never built:

- **Alias file input** — a mapping file allowing new CSV and flat-file implementations with no
  programming at all.
- **A modelling UI** — model the intended input documents, output documents, and queries, then
  generate boilerplate into reiterable modular release packages.
