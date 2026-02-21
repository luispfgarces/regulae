# Copilot Instructions

## General Guidelines
- Public classes, structures, interfaces, properties and methods must be documented. Private, protected and internal ones are excluded from this rule.
- Usings must be placed inside namespace. System usings always come first.
- Implicit variable declarations (using var) instead of explicit declarations should be preferred, to avoid redundancy and allow for more flexible and better readable code.
- Unit tests are separated per each class - suppose you are testing class Rule, you should place all tests for its' members under RuleTests.
- Use XUnit for unit testing.
- Unit tests naming must be done under the pattern MemberName_Conditions_ExpectedOutcome. See examples above for guidance.
- Unit tests body structure must follow the Arrange, Act & Assert pattern.
- Prefer Microsoft.CodeAnalysis.Testing XUnit packages version 1.1.2 for unit tests (use published packages on nuget.org).
- When adding a using in codefix, preserve the diagnostic source span and avoid altering original node locations. Prefer adding using via an editor.ReplaceNode of the compilation unit rather than changing the diagnostic node. Use leading trivia to ensure formatted fluent chain and preserve diagnostic source span when adding usings.
- Capture lambda condition bodies for translation into Regulae condition builder (capture ConditionLambda and translate method/enum names).
- Prefer iterative migrations with unit tests.