# RQL - Rule Query Language

RQL (Rule Query Language) is a simple yet powerful language for matching and searching for rules. It also supports creating objects and arrays, and assigning them to variables.

## Getting Started

This section will guide you through the basic features of RQL.

### Statements

RQL scripts are made of one or more statements, each terminated by a semicolon (`;`).

```rql
// This is a single statement
MATCH ONE RULE FOR "MyRuleset" ON $2024-01-01Z$;
```

### Comments

RQL supports single-line comments using `//`.

```rql
// This is a comment
```

### Literals

RQL supports a variety of literal values:

*   **String:** Enclosed in double quotes (e.g., `"hello world"`).
*   **Integer:** e.g., `123`, `456`.
*   **Decimal:** e.g., `1.23`, `4.56`.
*   **Boolean:** `TRUE` or `FALSE`.
*   **Date/Time:** Enclosed in dollar signs (e.g., `$2024-01-01T12:00:00Z$`).
*   **`NOTHING`:** Represents the absence of a value.

### Matching Rules

The `MATCH` expression is used to retrieve rules.

#### Syntax

```rql
MATCH (ONE RULE | ALL RULES) FOR <ruleset_name> ON <date> [WHEN { <conditions> }];
```

*   `ONE` or `ALL`: Specifies the cardinality. Use `RULE` for `ONE` and `RULES` for `ALL`.
*   `<ruleset_name>`: The name of the ruleset to match rules from (a string literal).
*   `<date>`: A date to match rules at a specific point in time. This evaluation date is **required** and tells the engine when to evaluate the rules.
*   `<conditions>` (optional): A set of conditions to filter the rules.

#### Examples

Match one rule from the "MyRuleset" ruleset on a given evaluation date:

```rql
MATCH ONE RULE FOR "MyRuleset" ON $2024-01-01Z$;
```

Match all rules from the "MyRuleset" ruleset on a given evaluation date:

```rql
MATCH ALL RULES FOR "MyRuleset" ON $2024-01-01Z$;
```

Match one rule from "MyRuleset" on a given evaluation date with a condition:

```rql
MATCH ONE RULE FOR "MyRuleset" ON $2024-01-01Z$ WHEN { @IsVip is TRUE };
```

### Searching Rules

The `SEARCH` expression is used to search for rules within a date range and with specific conditions.

#### Syntax

```rql
SEARCH RULES FOR <ruleset_name> SINCE <date> UNTIL <date> [WHEN { <conditions> }];
```

*   `<ruleset_name>`: The name of the ruleset to search in (a string literal).
*   `SINCE <date>`: The start of the date range. Required.
*   `UNTIL <date>`: The end of the date range. Required.
*   `<conditions>` (optional): A set of conditions to filter the rules.

#### Examples

Search for all rules in the "MyRuleset" ruleset within a defined range:

```rql
SEARCH RULES FOR "MyRuleset" SINCE $2023-01-01Z$ UNTIL $2024-01-01Z$;
```

Search for rules with specific conditions in that range:

```rql
SEARCH RULES FOR "MyRuleset" SINCE $2023-01-01Z$ UNTIL $2024-01-01Z$ WHEN { @Age > 30, @Country is "USA" };
```

### Conditions

Conditions allow you to filter which rules are returned by `MATCH` or `SEARCH` expressions.  A condition is written as:

```rql
@<condition_name> IS <value>
```

Multiple conditions are separated by commas and enclosed in `{ }`.

Use `#!rql WHEN { ... }` after a `#!rql MATCH` or after a `#!rql SEARCH` to apply filters.

### Object Creation

You can create objects using the `#!rql OBJECT` keyword. Objects are created via prototyping, which allows you to define them with as much properties as you like.

#### Syntax

```rql
OBJECT {
    <property1> = <value1>,
    <property2> = <value2>
}
```

#### Example

```rql
OBJECT {
    "Name" = "John Doe",
    "Age" = 30,
    "IsStudent" = FALSE
}
```

### Array Creation

RQL provides the ability to create arrays, which fixed size structures that hold multiple values. RQL arrays allow you to hold on them any value, which means that you place inside the same array integers, decimals, strings, bools, objects, and even other arrays.

There are two ways to create arrays.

#### With a specific size

The `#!rql ARRAY[<size>]` syntax creates an array with a given size, initialized with `#!rql NOTHING` values.

```rql
// Creates an array of size 5
ARRAY[5];
```

#### With initial elements

The `#!rql ARRAY { <elements> }` syntax creates an array with the specified elements.

```rql
// Creates an array with three elements
ARRAY { 1, "two", TRUE };
```

### Variables and Assignments

You can declare variables and assign values to them. Variable names are case-sensitive.

```rql
// Declare a variable 'myVar' and assign an array to it
myVar = ARRAY { 1, 2, 3 };

// Declare another variable and assign an object
person = OBJECT {
    "Name" = "Jane Doe"
};
```

For a complete formal grammar of the language, see the separate document: [RQL Grammar Specification](rql-grammar.md).


