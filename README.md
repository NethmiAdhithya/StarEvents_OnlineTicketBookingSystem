# StarEvents: Online Event Ticketing Web Application

## 🌟 Project Overview
StarEvents is a comprehensive multi-role e-commerce platform designed to automate the end-to-end event management lifecycle. The application bridges the gap between event organizers and attendees, providing a seamless experience for creating, managing, and purchasing tickets for events.



[Image of Web application MVC architecture diagram]


## 🛠 Tech Stack
* **Framework:** ASP.NET Core MVC
* **Language:** C#
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core (EF Core)
* **Security:** ASP.NET Identity (RBAC)
* **Querying:** LINQ (Language Integrated Query)
* **Frontend:** Bootstrap, HTML5, CSS3, JavaScript

## ✨ Key Features
* **Role-Based Access Control (RBAC):** Tailored dashboards and permissions for **Admins**, **Organizers**, and **Customers**.
* **Event Lifecycle Management:** Organizers can create, update, and track events, while Admins manage platform-wide approval and categories.
* **Atomic Ticket Inventory System:** A robust backend algorithm using EF Core transactions to handle high-concurrency ticket sales and prevent overselling.
* **Automated E-Ticketing:** Generates unique tickets for customers upon successful purchase, featuring QR code integration for entry validation.
* **Business Intelligence Dashboard:** Real-time data visualization for organizers to track sales volume, revenue, and attendee demographics.

## ⚙️ Technical Highlights
* **Layered Architecture:** Followed the Model-View-Controller (MVC) pattern to ensure a clean separation of concerns and maintainability.
* **Data Integrity:** Implemented ACID-compliant transactions to manage ticket availability during peak traffic.
* **Secure Authentication:** Utilized ASP.NET Identity for encrypted user data storage and secure session management.

---

## 🚀 Getting Started

### Prerequisites
* Visual Studio 2022 or VS Code
* .NET SDK (6.0 or later)
* SQL Server (LocalDB or Express)

### Installation
1. **Clone the repository:**
2. **Navigate to the project directory:cd StarEvents**
3. **Update the Database: Ensure your connection string in appsettings.json is correct, then run:dotnet ef database update**
4. **Run the application:dotnet run**
