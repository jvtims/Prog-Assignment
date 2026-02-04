//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
//==========================================================

using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static List<Order> allOrders = new List<Order>();

    static string restaurantFile = "restaurants.csv";
    static string foodItemFile = "fooditems.csv";
    static string customerFile = "customers.csv";
    static string orderFile = "orders.csv";

    //  main
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
        Console.WriteLine();

        // feature 1, load restaurants and food items
        LoadRestaurants();
        LoadFoodItems();

        // feature 2, load customers and orders
        LoadCustomers();
        LoadOrders();

        // Display what was loaded
        Console.WriteLine(restaurants.Count + " restaurants loaded!");
        Console.WriteLine(GetTotalFoodItemCount() + " food items loaded!");
        Console.WriteLine(customers.Count + " customers loaded!");
        Console.WriteLine(allOrders.Count + " orders loaded!");
        Console.WriteLine();

        // main menu loop
        bool running = true;
        while (running)
        {
            DisplayMainMenu();
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                // feature 3 list all the restaurants and menu items
                ListAllRestaurantsAndMenuItems();
            }
            else if (choice == "2")
            {
                // feature 4 list all the orders
                ListAllOrders();
            }
            else if (choice == "0")
            {
                Console.WriteLine("Exiting Gruberoo. Goodbye!");
                running = false;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }

            Console.WriteLine();
        }
    }

    //  display main menu
    static void DisplayMainMenu()
    {
        Console.WriteLine("===== Gruberoo Food Delivery System =====");
        Console.WriteLine("1. List all restaurants and menu items");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    // load the restaurants.csv and create restaurant objects for feature 1
    static void LoadRestaurants()
    {
        string[] lines = File.ReadAllLines(restaurantFile);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');

            Restaurant r = new Restaurant(parts[0].Trim(),
                                          parts[1].Trim(),
                                          parts[2].Trim());

            Menu m = new Menu(parts[0].Trim() + "_M1",
                              parts[1].Trim() + " Menu");

            r.AddMenu(m);
            restaurants.Add(r);
        }
    }

    // load the food items from fooditems.csv and adds them to the correct restaurant menu
    static void LoadFoodItems()
    {
        string[] lines = File.ReadAllLines(foodItemFile);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = ParseCSVLine(lines[i]);

            Restaurant r = FindRestaurantById(parts[0].Trim());
            if (r == null) continue;

            double price;
            if (!double.TryParse(parts[3].Trim(), out price)) continue;

            string desc = parts[2].Trim();
            if (desc.StartsWith("\"") && desc.EndsWith("\""))
                desc = desc.Substring(1, desc.Length - 2);

            FoodItem fi = new FoodItem(parts[1].Trim(), desc, price);
            r.Menus[0].AddFoodItem(fi);
        }
    }

    // load the customers from customers.csv and creates
    static void LoadCustomers()
    {
        string[] lines = File.ReadAllLines(customerFile);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            customers.Add(new Customer(parts[0].Trim(), parts[1].Trim()));
        }
    }


    // load all the orders from orders.csv and creates Order objects.
    static void LoadOrders()
    {
        string[] lines = File.ReadAllLines(orderFile);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = ParseCSVLine(lines[i]);

            int orderId;
            double total;
            if (!int.TryParse(parts[0], out orderId)) continue;
            if (!double.TryParse(parts[7], out total)) continue;

            DateTime deliveryDT, orderDT;
            if (!DateTime.TryParseExact(parts[3] + " " + parts[4],
                "dd/MM/yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None,
                out deliveryDT)) continue;

            if (!DateTime.TryParseExact(parts[6],
                "dd/MM/yyyy HH:mm", null,
                System.Globalization.DateTimeStyles.None,
                out orderDT)) continue;

            Order o = new Order(orderId, orderDT, deliveryDT,
                                parts[5].Trim(), "CC", true, parts[8].Trim());

            o.OrderTotal = total;
            allOrders.Add(o);

            Customer c = FindCustomerByEmail(parts[1].Trim());
            if (c != null) c.AddOrder(o);

            Restaurant r = FindRestaurantById(parts[2].Trim());
            if (r != null) r.AddOrder(o);
        }
    }

    // display all restaurants and their menu items

    static void ListAllRestaurantsAndMenuItems()
    {
        Console.WriteLine();
        Console.WriteLine("All Restaurants and Menu Items");
        Console.WriteLine("==============================");

        foreach (Restaurant r in restaurants)
        {
            Console.WriteLine("Restaurant: " + r.RestaurantName +
                              " (" + r.RestaurantId + ")");

            foreach (FoodItem fi in r.Menus[0].FoodItems)
            {
                Console.WriteLine(" - " + fi.ItemName + ": " +
                                  fi.ItemDesc + " - $" +
                                  fi.ItemPrice.ToString("F2"));
            }
            Console.WriteLine();
        }
    }

    // display all orders in a table format
    static void ListAllOrders()
    {
        Console.WriteLine();
        Console.WriteLine("All Orders");
        Console.WriteLine("==========");

        Console.WriteLine("Order ID".PadRight(10) +
                          "Customer".PadRight(15) +
                          "Restaurant".PadRight(18) +
                          "Delivery Date/Time".PadRight(22) +
                          "Amount".PadRight(10) +
                          "Status");

        Console.WriteLine("--------".PadRight(10) +
                          "----------".PadRight(15) +
                          "-------------".PadRight(18) +
                          "------------------".PadRight(22) +
                          "------".PadRight(10) +
                          "---------");

        foreach (Order o in allOrders)
        {
            string custName = "Unknown";
            foreach (Customer c in customers)
                foreach (Order co in c.OrderList)
                    if (co.OrderId == o.OrderId)
                        custName = c.CustomerName;

            string restName = "Unknown";
            foreach (Restaurant r in restaurants)
                foreach (Order ro in r.OrderQueue)
                    if (ro.OrderId == o.OrderId)
                        restName = r.RestaurantName;

            Console.WriteLine(
                o.OrderId.ToString().PadRight(10) +
                custName.PadRight(15) +
                restName.PadRight(18) +
                o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm").PadRight(22) +
                ("$" + o.OrderTotal.ToString("F2")).PadRight(10) +
                o.OrderStatus
            );
        }
    }


    // find and returns a restaurant based on its restaurant ID return a null if no matching restaurant is found.
    static Restaurant FindRestaurantById(string id)
    {
        foreach (Restaurant r in restaurants)
            if (r.RestaurantId.ToLower() == id.ToLower())
                return r;
        return null;
    }

    // find and returns a customer based on email address. returns a null if no matching customer is found.
    static Customer FindCustomerByEmail(string email)
    {
        foreach (Customer c in customers)
            if (c.EmailAddress.ToLower() == email.ToLower())
                return c;
        return null;
    }

    // calculates and returns the total number of food items across all restaurants and menus
    static int GetTotalFoodItemCount()
    {
        int count = 0;
        foreach (Restaurant r in restaurants)
            count += r.Menus[0].FoodItems.Count;
        return count;
    }

    static string[] ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        string current = "";
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current);
                current = "";
            }
            else current += c;
        }

        fields.Add(current);
        return fields.ToArray();
    }
}