# Regulae: a rules engine for .NET

Welcome to **Regulae**, a powerful and flexible rules engine designed specifically for .NET applications. Regulae empowers developers to define, manage, and evaluate business rules with built-in time-awareness, enabling dynamic decision-making based on conditions and temporal constraints.

## The Mission

Regulae simplifies rule-based logic by providing a robust framework that handles the complexities of time-sensitive rules. Whether you're building insurance systems, financial applications, or any domain requiring conditional logic tied to dates, Regulae ensures your rules are evaluated accurately and efficiently. The goal is to abstract away the boilerplate of rule management, allowing you to focus on business logic while maintaining high performance and extensibility.

## Core Features

### Rule Management
At the heart of Regulae is comprehensive rule lifecycle management. Define rules with start and end dates, priorities, and custom content. Easily add, update, activate, or deactivate rules as business needs evolve. This feature ensures rules are versioned and time-bound, preventing outdated logic from affecting decisions.

### Rule Evaluation
Evaluate rules dynamically using conditions and evaluation dates. Match single or multiple rules against input data, or search for rules within date ranges. Regulae's evaluation engine supports complex conditions with operators like equality, ranges, and sets, making it adaptable to diverse scenarios.

### RQL: Rule Query Language
Interact with rules using RQL, a simple yet expressive query language. Write statements to match or search rules. RQL bridges the gap between code and rule definitions, offering a declarative way to query your rule repository.

### Web UI for Rule Management
For teams preferring visual interfaces, Regulae includes a web-based UI for configuring and managing rules. Easily set up the UI in your application to provide non-technical users with intuitive tools for rule administration, reducing reliance on developers for routine updates.

### Extensible Data Source Providers
Regulae supports multiple storage backends through pluggable data source providers. Out-of-the-box support for in-memory storage and MongoDB, with a clear API for creating custom providers. This ensures Regulae integrates seamlessly with your existing infrastructure, whether it's databases, caches, or cloud services.

## Why Choose Regulae?

- **Time-Awareness**: Rules are inherently temporal, with automatic handling of date ranges and evaluation points.
- **Performance**: Optimized for high-throughput rule evaluation with minimal overhead.
- **Flexibility**: Extensible architecture supports custom conditions, content types, and storage solutions.
- **Developer-Friendly**: Clean APIs, comprehensive documentation, and strong typing in .NET.
- **Open Source**: Community-driven development with contributions welcome.

## Getting Started

Ready to integrate Regulae into your project? Start with our [Getting Started guide](getting-started.md) to set up your first rules engine instance. Explore the documentation for detailed API references, examples, and best practices.

- **Repository**: [GitHub](https://github.com/luispfgarces/regulae)
- **Documentation**: Navigate the sections on the left for in-depth guides.

Join the community and build smarter, rule-driven applications with Regulae!
