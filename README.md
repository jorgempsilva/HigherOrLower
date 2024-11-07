# HigherOrLower

## This is a .NET Core project called HigherOrLower, which implements a card-based guessing game where players attempt to guess if the next card in the deck will be higher or lower than the current one. The project is organized with a layered architecture, following best practices for separation of concerns.

# Project Overview

## HigherOrLower is a simple card game where players compete to guess whether the next card revealed will have a higher or lower value than the current card. The system manages the game state, including the deck, players, and scores. This project uses ASP.NET Core for the API and Entity Framework Core (EF Core) for database interaction.

# Project Structure

## The project is organized into four main layers, with a well-defined folder structure:


- **HigherOrLower**: Contains the main API project and controllers to expose endpoints.

    - ***Controllers***: Defines API endpoints, e.g., GameController.cs, which manages endpoints such as game creation and guessing.

- **Domain**: Defines the core business logic and domain models.

    - **Entities**: Contains classes representing core entities, like Game, Card, and Player.
    - ***Enums***: Defines enumerated types for card values and suits (CardSuit and CardValue).
    - ***Dto***: Data Transfer Objects (DTOs) for sending and receiving data via the API, such as CreateGameDto.
    - ***Interfaces***: Interface for repositories, e.g., IGameRepository.
    - ***Services***: Contains the main business logic, such as GameService, where game operations are handled.
    - ***Validators***: Defines validators to ensure the data received by the API is correct, e.g., CreateGameDtoValidator.

- **Infrastructure**: Manages persistence and database configurations.

    - ***Contexts***: Defines the database context and configures EF Core.
    - ***Mappings***: Configures how entities are mapped to the database.
    - ***Repositories***: Repository implementations for database interaction.

- **Tests**: Contains unit tests for the project.

    - ***Unit/Api/Controllers***: Contains unit tests for controllers, e.g., GameControllerTest.

# Environment Setup

## Prerequisites

- .NET 8 SDK
- SQL Server (or Docker to run SQL Server in a container)
- dotnet CLI tool

# Setup Steps

1. **Clone the Repository**

```
git clone https://github.com/yourusername/HigherOrLower.git
cd HigherOrLower
```

2. **Docker Compose Setup**

The application is configured to run with Docker Compose, which includes a container for SQL Server and automatically applies migrations on startup.

Make sure Docker and Docker Compose are installed on your machine.
In the root directory of the project, run the following command to start the containers:

```
docker-compose up --d
```

This command will:

- Start the SQL Server container, configured to listen on the default port 1433.
- Start the HigherOrLower API container and automatically run the database migrations through a script included in the Dockerfile or as an additional command in the docker-compose.yml.

Docker Compose will use the connection settings from appsettings.json, which should already be configured to connect to the SQL Server running in the container.

3. **Verify the Application**

Once the containers are running, the API should be accessible at http://localhost:5002, and the database will be automatically initialized and updated with migrations.

If you need to check or modify the database, SQL Server can be accessed locally on port 1433 on the host machine.

# Endpoints

## Main Endpoints

- **POST /api/game/newGame** - Creates a new game.
- **GET /api/game/{gameId}** - Retrieves the current state of a game.
- **GET /api/game** - Retrieves all games.
- **POST /api/game/{gameId}/guess** - Makes a guess for higher or lower.
- **GET /api/game/{gameId}/score** - Retrieves the final score of the players.

# Testing

## The project includes unit tests in the Tests layer. To run the tests, use the command:

```
dotnet test
```

This will execute all tests and display results in the console.