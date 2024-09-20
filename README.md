# SurveyBasket.API

## Features
- **User Authentication and Authorization**: Secure user authentication using JWT tokens and role-based access control.
- **Survey Management**: Create, update, delete, and publish surveys.
- **Question Management**: Add, update, and remove questions within surveys.
- **Response Collection**: Collect and store survey responses.
- **Result Analysis**: Analyze survey results with detailed reports.
- **Email Notifications**: Send email notifications for survey invitations and confirmations.
- **Rate Limiting**: Protect the API from abuse with IP-based rate limiting.
- **Health Checks**: Monitor the health of the API with integrated health checks.
- **Background Jobs**: Manage background tasks using Hangfire.
- **Swagger Integration**: Interactive API documentation with Swagger.

## Technologies Used
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- Microsoft Identity
- FluentValidation
- Hangfire
- Serilog
- Swagger
- Mapster

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server

### Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/SurveyBasket.git
    cd SurveyBasket
    ```
2. Apply migrations to set up the database:
    ```bash
    dotnet ef database update
    ```

### Usage
- Access the Swagger UI at `http://localhost:5000/swagger` to explore and test the API endpoints.

## Contributing
Contributions are welcome! Please read the [CONTRIBUTING.md](CONTRIBUTING.md) for more information.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact
For any inquiries or feedback, please contact [yourname@yourdomain.com](mailto:yourname@yourdomain.com).
