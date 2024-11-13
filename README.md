# Project Documentation

## Standards Used

### Repository Pattern
The **Repository Pattern** is used to manage database data. We define repository classes that encapsulate all the logic for handling data access. These classes expose methods to list, add, update, and delete objects of a given model, isolating database access from the rest of the application.

### Dependency Injection
**Dependency Injection** is used to avoid a high level of coupling in the code within the application. It facilitates maintenance and the implementation of new features, making the code more modular and flexible.

### Request-Response
The **Request-Response** pattern encapsulates the request and response parameters into classes. With this pattern, we can handle business logic and potential failures without interrupting the application process, promoting a more structured control flow.

### Unit of Work
The **Unit of Work** pattern is used to group related operations into a single transaction or logical unit. It also helps to isolate transactions, providing better control over data persistence operations.

## Libraries and Frameworks Used

### AutoMapper
**AutoMapper** automates the task of mapping data between objects of different types. With it, we can define mapping rules once and reuse them throughout the application, reducing the need to write manual mapping code. This improves readability, facilitates maintenance, and increases developer productivity.

### Entity Framework
**Entity Framework (EF)** is an Object-Relational Mapping (ORM) framework developed by Microsoft. It allows developers to work with data in a relational database using objects specific to the application's domain. EF eliminates the need to write repetitive and complex SQL code, providing a more productive and secure development experience.

## Running the Project

To run the project, simply open it with **Visual Studio** and run it.

## Logo Handling

The logo is always saved as **base64** in a text field in the database to avoid increasing its size. This decision was made due to the lack of a blob storage service that provides us with a URL for image uploads.

## Important Notes

- Always follow the **standards** and **frameworks** used in this project.
- Respect the **couplings** and **structure** of the codebase.

