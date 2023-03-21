# Housing application

- [x] The application consists of 2 parts: Background service and REST API

- [x] Background service is resposible for scraping the Partner API (http://partnerapi.funda.nl) and store the objects(houses) in the database 

- [x] REST API is responsible for providing the data that is being scraped from the Partner API

- [x] REST API is implemented using clean architecture (onion style), with CQRS & Mediator pattern.

- [x] The application uses an InMemory database.

- [x] The application is developed using .NET 7, C# and is covered (not 100%) with unit tests using xUnit and Moq.

- [x] The application is containerized and can run using docker compose.

------------------------------------------------------------

## To run the application

1. Clone the repository

2. In the command prompt/terminal/powershell go to the root folder of the application (/Housing)

3. Run this command -> docker compose up

4. Browse to http://localhost:5000/swagger