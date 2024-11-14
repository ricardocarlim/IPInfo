
# Project Architecture, Design Patterns, and Best Practices

This section provides an in-depth analysis of the architecture, design patterns used, and the rationale behind their adoption. The aim is to ensure that the development practices align with technical requirements while promoting flexibility, scalability, and maintainability.

---

## 1. **Project Architecture Overview**

The project's architecture follows best practices of **Domain-Driven Design (DDD)**, with a clear separation of concerns across layers. Additionally, **Test-Driven Development (TDD)** was adopted to ensure that the code is reliable and easy to test.

The project structure is composed of several layers:

- **API (Presentation Layer)**
- **Domain Services (Application Layer)**
- **Repositories (Persistence Layer)**
- **Infrastructure**

Each layer is responsible for a specific part of the process, keeping the code decoupled and easily extensible.

---

## 2. **Design Patterns Used and Justifications**

### 2.1 **Unit of Work**
**Pattern Used**: `IUnitOfWork` and transaction implementation.

- **Where Used**: The **Unit of Work** pattern is implemented in the `UnitOfWork` class and used in services such as `CountryService` and `IPAddressService`, where transactions need to involve multiple repositories and ensure data consistency.

- **Why Used**: This pattern allows multiple operations involving different repositories to be treated as a single transaction. If any operation fails, all changes can be rolled back, ensuring database consistency. **Unit of Work** provides transactional control over repository interactions, as seen in the `SaveAsync` method in the `CountryService`.

```csharp
public async Task<Country> SaveAsync(Country country)
{
    using (var transaction = await _unitOfWork.BeginTransactionAsync())
    {
        // Repository operations...
        await _unitOfWork.CompleteAsync();
        await transaction.CommitAsync();
    }
}
```

### 2.2 **Repository**
**Pattern Used**: `IRepository` (e.g., `ICountryRepository`, `IIPAddressRepository`)

- **Where Used**: The **Repository** pattern is used in repositories like `CountryRepository` and `IPAddressRepository`.

- **Why Used**: The **Repository Pattern** abstracts the complexity of data access by providing a common interface for read and write operations. It allows the persistence layer to be changed without affecting the business logic. The business code interacts with repositories through interfaces like `ICountryRepository` for database operations.

```csharp
public async Task<Country> ListByNameAsync(string name)
{
    return await _context.Countries
                         .Where(p => p.Name == name)
                         .FirstOrDefaultAsync();
}
```

### 2.3 **Singleton**
**Pattern Used**: `ConnectionMultiplexer` (Redis) and `IP2CCacheService`

- **Where Used**: The **Singleton** pattern is applied to **Redis** (`IConnectionMultiplexer`) and **IP2CCacheService** instances.

- **Why Used**: The **Singleton** pattern ensures that only one instance of certain objects is created and used throughout the application. In the case of Redis, this is crucial as repeatedly creating connections could lead to performance issues and excessive resource consumption. The **Singleton** ensures that the `IConnectionMultiplexer` instance remains unique throughout the application's lifecycle, improving connection efficiency.

```csharp
public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
{
    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig["ConnectionString"]));
}
```

### 2.4 **Factory**
**Pattern Used**: Object creation for complex objects such as `HttpWebRequest`

- **Where Used**: The **Factory** pattern is used in the **IP2CService** to create `HttpWebRequest` instances.

- **Why Used**: The **Factory** pattern is applied to centralize the creation of complex objects. Creating an `HttpWebRequest` requires specific configurations (e.g., `CachePolicy`), and the factory helps encapsulate this process, allowing future modifications without altering the service code. This makes the code more flexible and maintainable.

```csharp
public static async Task<Domain.Models.Country> GetCountryInfoFromIPAsync(string ip)
{
    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    // Specific configurations...
}
```

### 2.5 **Strategy**
**Pattern Used**: Strategies for communication with external services (e.g., IP2C)

- **Where Used**: The **IP2CService** implements a strategy for communicating with the external service to retrieve country information based on the provided IP.

- **Why Used**: Although not explicitly formalized as a **Strategy** pattern, it can be seen in how different types of responses from the external IP service are handled. Depending on the response from the service, the system's behavior changes. This strategy allows the system to adapt flexibly to the type of response it receives.

```csharp
switch (result[0])
{
    case '0':
        return null;
    case '1':
        // Process the response
        break;
    case '2':
        return null;
    default:
        return null;
}
```

## 3. **Clean Code and TDD Practices**

### 3.1 **Clean Code**
- **Single Responsibility Principle**: Each class and method has a well-defined responsibility, making the code easier to understand and maintain. For example, domain services like `CountryService` have methods that do just one thing, such as saving an IP or fetching country information.
  
- **Meaningful Names**: Class, method, and variable names are self-explanatory, reducing the need for excessive comments and making the code more readable.

### 3.2 **Test-Driven Development (TDD)**
- The project follows **Test-Driven Development (TDD)** principles, where tests are written before production code. The modular structure and dependency injection allow classes to be easily tested in isolation.

- **Testability**: Clear separation of concerns and the use of interfaces make it easy to test classes with **mocks** and **stubs**, ensuring that business logic is validated under different scenarios.

---

## 4. **Infrastructure and Performance**

### 4.1 **Hangfire**
- **Hangfire** is used to manage background tasks, such as updating IP information asynchronously. This improves scalability by keeping the interface responsive while heavy tasks are handled in the background.

### 4.2 **Cache (Redis and Memory Cache)**
- **Redis** and **Memory Cache** are used to significantly improve performance by storing frequently accessed data, such as IP and country information, avoiding repeated database accesses and reducing latency.

---

## 5. **Unit Tests**

Unit tests are crucial to ensure the functionality of the application. Below are some examples of unit tests for repositories and services:

### 5.1 **IPAddressRepositoryTests**

Test for adding an IP address to the repository:

```csharp
[Fact]
public async Task AddAsync_ShouldAddIPAddressToDatabase()
{
    var ipAddress = new IPAddress
    {
        IP = "192.168.1.1",
        Country = new Country { Name = "Brazil", ThreeLetterCode = "BRA", TwoLetterCode = "BR" }
    };

    var repository = CreateRepository();
    var addedIPAddress = await repository.AddAsync(ipAddress);
    await _context.SaveChangesAsync();
    
    Assert.NotNull(addedIPAddress);
    Assert.Equal("192.168.1.1", addedIPAddress.IP);
    Assert.Equal("Brazil", addedIPAddress.Country.Name);
}

### 5.2 **CountryRepositoryTests**

Test for listing a country by name:

```csharp
[Fact]
public async Task ListByNameAsync_ShouldReturnCountries_WhenExists()
{
    var brazil = new Country { Name = "Brazil", ThreeLetterCode = "BRA", TwoLetterCode = "BR" };
    var argentina = new Country { Name = "Argentina", ThreeLetterCode = "ARG", TwoLetterCode = "AR" };

    _context.Countries.Add(brazil);
    _context.Countries.Add(argentina);
    await _context.SaveChangesAsync();

    var repository = CreateRepository();
    var countries = await repository.ListByNameAsync("Brazil");

    Assert.Single(countries);
    Assert.Equal("Brazil", countries.First().Name);
}

### 5.3 **IP2CServiceTests**

Test for IP-to-country lookup:

```csharp
[Fact]
public async Task GetCountryInfoFromIPAsync_ShouldReturnCountry_WhenValidIP()
{
    var ip = "8.8.8.8";
    var result = await _ip2CService.GetCountryInfoFromIPAsync(ip);

    Assert.NotNull(result);
    Assert.Equal("United States", result.Country.Name);
}

---

## 6. **Conclusion**

The architecture adopted for this project combines the best practices of **DDD**, **Clean Code**, **TDD**, and design patterns, resulting in a decoupled, flexible, and maintainable system. The use of patterns like **Unit of Work**, **Repository**, **Factory**, **Singleton**, and **Strategy** provides flexibility, scalability, and ease of maintenance. These patterns were selected to address specific problems, such as ensuring transactional consistency, abstracting data access, managing complex object creation, and allowing the system to adapt to different API responses.

Additionally, the caching infrastructure and use of **Hangfire** for background tasks contribute to the application's performance and scalability, ensuring that it can handle high data volumes efficiently. The code is designed to be easily extended and modified, ensuring the system's sustainability in the long term.

---

