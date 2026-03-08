# RQL Grammar Specification

This document contains the formal grammar and structural definitions used by the
Rule Query Language (RQL). It is primarily intended for implementers and
advanced users that need a precise description of the language syntax.

## Technical Specification

### Statements

An RQL script consists of one or more statements, each terminated by a semicolon (`;`).

```
<statement> ::= <expression_statement> ;
```

### Expression Statement

An expression statement is an expression followed by a semicolon.

```
<expression_statement> ::= <expression> ;
```

### Expressions

Expressions are the building blocks of RQL and can be of several types.

```
<expression> ::=
    <match_expression>
    | <search_expression>
    | <assignment_expression>
    | <new_object_expression>
    | <new_array_expression>
    | <binary_expression>
    | <unary_expression>
    | <literal_expression>
    | <identifier_expression>
    | <placeholder_expression>
```

### Literal Expressions

Literal values represent fixed data in the language. Supported literals include:

* **String** – quoted with double quotes (`"text"`).
* **Integer** – whole numbers (`123`).
* **Decimal** – floating‑point numbers (`1.23`).
* **Boolean** – `TRUE` or `FALSE`.
* **Date/Time** – enclosed in dollar signs (`$2024-01-01T12:00:00Z$`).
* **`NOTHING`** – the absence of a value.

The grammar for a literal is:

```
<literal_expression> ::=
    <string_literal>
    | <integer_literal>
    | <decimal_literal>
    | <boolean_literal>
    | <date_literal>
    | NOTHING
```

### `MATCH` Expression

The `MATCH` expression is used to retrieve rules.

**Syntax**

```rql
MATCH ( ONE RULE | ALL RULES )
FOR <ruleset_name>
ON <date>
[ WHEN { <conditions> } ]
```
> **Note:** the `ON <date>` clause is required and specifies the evaluation date.

**Arguments**

*   `ONE RULE` | `ALL RULES` (required)
    Specifies the cardinality of the match.

*   `FOR <ruleset_name>` (required)
    Specifies the ruleset to match rules from. `<ruleset_name>` is a string literal.

*   `ON <date>` (required)
    Specifies the evaluation date. `<date>` is a date/time literal.

*   `WHEN { <conditions> }` (optional)
    A set of conditions to filter the rules. See [Conditions](#conditions) for more details.

### `SEARCH` Expression

The `SEARCH` expression is used to search for rules.

**Syntax**

```rql
SEARCH RULES
FOR <ruleset_name>
SINCE <date>
UNTIL <date>
[ WHEN { <conditions> } ]
```
> **Note:** both `SINCE` and `UNTIL` clauses are required to define the evaluation range.

**Arguments**

*   `FOR <ruleset_name>` (required)
    Specifies the ruleset to search in. `<ruleset_name>` is a string literal.

*   `SINCE <date>` (required)
    The start of the date range for the search.

*   `UNTIL <date>` (required)
    The end of the date range for the search.

*   `WHEN { <conditions> }` (optional)
    A set of conditions to filter the rules. See [Conditions](#conditions) for more details.

### `OBJECT` Expression

The `OBJECT` expression is used to create a new object.

**Syntax**

```rql
OBJECT [ { <property_assignments> } ]
```

**Arguments**

*   `<property_assignments>` (optional)
    A comma-separated list of property assignments. Each assignment has the following syntax:
    `<property_name> = <value>`
    *   `<property_name>`: A string literal.
    *   `<value>`: Any valid expression.

### `ARRAY` Expression

The `ARRAY` expression is used to create a new array.

**Syntax**

```rql
ARRAY [ <size> ] | ARRAY { <elements> }
```

**Arguments**

*   `[ <size> ]`
    Creates an array of a specific size, initialized with `NOTHING` values. `<size>` is an integer literal.

*   `{ <elements> }`
    Creates an array with a list of initial elements. `<elements>` is a comma-separated list of expressions.

### Conditions

Conditions are used in the `WHEN` clause of a `MATCH` expression and a
`SEARCH` expression.

**Syntax**

```
<condition> [, <condition>...]
```

Each condition has the following syntax:

```rql
@<condition_name> IS <value>
```

*   `@<condition_name>`
    The name of the condition, prefixed with `@`.

*   `<value>`
    A literal value or an expression.
