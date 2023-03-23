# Craft.Backend
Architecture
The Craft application is built using .NET Core, with a  Onion layered architecture that separates concerns into distinct layers. The layers include:

Application Layer: This layer contains the business logic of the application. It is responsible for handling requests and commands from the presentation layer, as well as executing queries against the database.
Domain Layer: This layer contains the domain models and business rules of the application.
Infrastructure Layer: This layer contains the implementation details of the application, including the database, external API integrations, and other third-party libraries.
The application uses MediatR library for implementing the mediator pattern which allows for easy handling of commands and queries.
The application contains:
Authentication and authorization system to handle different types of users (Admin, Vendor, Customer).
API endpoints to manage CRUD operations for Countries, States, Schools, Businesses, Products, and Orders.
Database schema to store user data, business data, and order data.
Features
The Craft application is an e-commerce platform that allows users to create order and order products. The following are the features of the application:

Workflow:
User Registration
User visits the registration page and fills out the required information, including their account type (Customer, Vendor, or Admin).
If the user selects "Vendor" as their account type, they are prompted to provide additional information such as their business name, logo, and location.
The user submits the registration form, and their information is saved in the database.
A confirmation email is sent to the user's email address with a link to verify their account.

User Login
User enters their email address and password on the login page and submits the form.
The server verifies the user's credentials and logs them in if they are valid.
If the user is a vendor, they are redirected to the vendor dashboard. If they are an admin, they are redirected to the admin dashboard. Otherwise, they are redirected to the customer dashboard.

Admin Dashboard
The admin can view a list of all registered users and filter them by account type.
{The Admin has access to modify basically almost everything on the application execpt data that they have been restricted from like user data and co.}.

The admin can create, edit, and delete countries, states, and schools.
The admin can create, edit, and delete products on behalf of vendors.
The admin can view a list of all orders and filter them by status (e.g. paid, shipped, cancelled).

Vendor Dashboard
The vendor can view a list of their businesses and add new businesses if they want to.
The vendor can view a list of their products for each business and add new products.

Customer Dashboard
The customer can view a list of available businesses and products, filtered by category and some other parameters.
The customer can leave reviews and ratings for products they have purchased.
customer can cancel an order within an hour of placing it.
The application has a functionality to retrieve all orders, orders placed within an hour, and orders by their order number

Order Processing
The order processing starts when a user places an order. The order information is then captured and stored in the application's database.
The application has a functionality to cancel orders within one hour of placing the order.

Conclusion
The Craft application is a fully functional e-commerce platform with essential features for managing orders and customers. With the layered architecture and mediator pattern, the application is easily maintainable, scalable and has a clear separation of concerns.
It provides a seamless shopping experience for users with features such as product browsing, and searching. The platform has a modular architecture that can be customized to suit different business needs.
