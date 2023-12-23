Language: C# .Net Framework

Design Patterns: Singleton, Component model, Inversion of Control

Techniques: LinQ to DataSet, XMLSerializer, CSV Parsing algorithms, SQLBulkCopy, Interfaces, Extension methods, Decoupling, Dependency Injection


#ETLProcess
This code contains a decoupled library of classes designed for generic implementation of specific ETL business requirements, accepting SQL data, XML, CSV, and flat files as input.
It utilizes Linq to Dataset for extraction and transformation, and exports data by XML Serialization or SQL Bulk Copy for reporting.
Pending features include alias file input for programming-free input CSV/flat-file implementation.


Case Study:
A company required many similar program solutions for their ETL processes, serving data-to-document customers, but each with different business requirements. They needed to take various data tables in various input files types (CSV, XML, SQL query, position delimited), bring them together in a dataset, and output an XML tree and SQL reporting.

They used the XML output in a 3rd party document mapping break-pack program to generate hardcopy documents for their clients. Each client rewired this information to be filtered or calculated upon for the purpose of generating values in these documents.

Since they currently had many disparate solutions, siloed to past employees, they wanted a C# .Net Framework library that could generate replacement programs, accommodate or generate a decoupled specific implementation for each released solution, and sequester individual business rules separate from the logical programming. 

Solution:
I created a software library called ETLProcess that would provide inversion of control around an implementation-specific code module. A programmer has only to select the desired file types of input and output interfaces, to generate a decoupled skeleton of that code based on the promises if those interfaces.

The library uses this decoupled flow of control to populate the Entity Relationship Model (using LinQ to DataSet, XMLSerializer, csv parsing algorithms, etc.), then serialize output to XMLSerializer and SQLBulkCopy based upon a serializable profile singleton.

Upcoming:
It is intended to develop a user interface which will model intended input and output documents and queries, and generate boilerplate code for the user into reiterable modular release packages.
