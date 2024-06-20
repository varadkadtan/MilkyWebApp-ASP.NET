# ASP.NET Core Milk Distribution Website

This is a dynamic ASP.NET Core MVC website designed to streamline the distribution of milk products.

### Index Page

![Index Page](MilkyWeb/wwwroot/images/Screenshot_1025.png)

### Multi-step Registration is made possible using TempData.

![Multi-step registration](MilkyWeb/wwwroot/images/Screenshot_1010.png)

![Multi-step registration](MilkyWeb/wwwroot/images/Screenshot_1011.png)

### The "Buy" or "Add to Cart" button is only visible during farm operational hours.

![Operational Hours](MilkyWeb/wwwroot/images/Screenshot_1022.png)

### The website is crafted with Bootstrap 5 and JavaScript for seamless, real-time updates without page reloads.

![Responsive Design](MilkyWeb/wwwroot/images/Screenshot_1014.png)

### Email Confirmation
Users, excluding administrators, must confirm their email addresses before accessing the site. Otherwise, they will continuously encounter the "RegisterConfirmation" view.

![Email Confirmation](MilkyWeb/wwwroot/images/Screenshot_1015.png)

### Stripe is used for efficient and secure Payment transactions.

![Payment Processing](MilkyWeb/wwwroot/images/Screenshot_1016.png)

### Integrated PostMark API for email delivery

![Email Delivery](MilkyWeb/wwwroot/images/Screenshot_1017.png)

### Customer Identification
A unique 12-digit secret key is issued to each customer upon order placement for identification purposes.

![Customer Identification](MilkyWeb/wwwroot/images/Screenshot_1018.png)

### Order Management
Implemented separate order management for both customers and employees/Admins.

![Order Management](MilkyWeb/wwwroot/images/Screenshot_1019.png)

![Order Management](MilkyWeb/wwwroot/images/Screenshot_1020.png)

### Stock Management
Purchased items are decremented from the available total stock.

Shopping cart contents are reset if accessed outside operational hours or if items are out of stock.

The system automatically updates product status to out of stock if no item stock is detected after an order is placed.

### Security Measures
Security measures are implemented to prevent unauthorized access, including attempts to bypass the website's normal flow of operations.