//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo] Features [2,3,5,7]
// Partner Name   : [Leroy Loh] Features [1,4,6,8]
//==========================================================

using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static List<Order> allOrders = new List<Order>();
    static Stack<Order> refundStack = new Stack<Order>();

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
            else if (choice == "3")
            {
                CreateNewOrder();   // Feature 5
            }
            else if (choice == "5")
            {
                ModifyExistingOrder();  // Feature 7
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
        Console.WriteLine("3. Create a new order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    // load the restaurants.csv and create restaurant objects for feature 1 (FEATURE 1)
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

    // load the food items from fooditems.csv and adds them to the correct restaurant menu (FEATURE 1)
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

            FoodItem fi = new FoodItem(parts[1].Trim(), desc, price);
            r.Menus[0].AddFoodItem(fi);
        }
    }

    // load the customers from customers.csv and creates (FEATURE 2)
    static void LoadCustomers()
    {
        string[] lines = File.ReadAllLines(customerFile);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            customers.Add(new Customer(parts[0].Trim(), parts[1].Trim()));
        }
    }


    // load all the orders from orders.csv and creates Order objects. (FEATURE 2)
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

            string custEmail = parts[1].Trim();
            string restId = parts[2].Trim();

            Order o = new Order(orderId, custEmail, restId,
                                orderDT, deliveryDT,
                                parts[5].Trim(), "CC", true, parts[8].Trim());

            o.OrderTotal = total;
            allOrders.Add(o);

            Customer c = FindCustomerByEmail(parts[1].Trim());
            if (c != null) c.AddOrder(o);

            Restaurant r = FindRestaurantById(parts[2].Trim());
            if (r != null) r.AddOrder(o);
        }
    }

    // display all restaurants and their menu items (FEATURE 3)

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

    // display all orders in a table format (FEATURE 4)
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
            Customer c = FindCustomerByEmail(o.CustomerEmail);
            if (c != null) custName = c.CustomerName;

            string restName = "Unknown";
            Restaurant r = FindRestaurantById(o.RestaurantId);
            if (r != null) restName = r.RestaurantName;

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
    
    // creats new order (Feature 5)
    static void CreateNewOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Create New Order");
        Console.WriteLine("================");

        // 1) Customer Email
        Customer cust = null;
        while (cust == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            cust = FindCustomerByEmail(email);
            if (cust == null)
                Console.WriteLine("Customer not found. Try again.");
        }

        // 2) Restaurant ID
        Restaurant rest = null;
        while (rest == null)
        {
            Console.Write("Enter Restaurant ID: ");
            string restId = Console.ReadLine().Trim();
            rest = FindRestaurantById(restId);
            if (rest == null)
                Console.WriteLine("Restaurant not found. Try again.");
        }

        // 3) Delivery Date
        DateTime deliveryDate;
        while (true)
        {
            Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
            string d = Console.ReadLine().Trim();
            if (DateTime.TryParseExact(d, "dd/MM/yyyy", null,
                System.Globalization.DateTimeStyles.None, out deliveryDate))
                break;

            Console.WriteLine("Invalid date format. Try again.");
        }

        // 4) Delivery Time
        DateTime deliveryTimeOnly;
        while (true)
        {
            Console.Write("Enter Delivery Time (hh:mm): ");
            string t = Console.ReadLine().Trim();
            if (DateTime.TryParseExact(t, "HH:mm", null,
                System.Globalization.DateTimeStyles.None, out deliveryTimeOnly))
                break;

            Console.WriteLine("Invalid time format. Try again.");
        }

        DateTime deliveryDT = new DateTime(deliveryDate.Year, deliveryDate.Month, deliveryDate.Day,
                                          deliveryTimeOnly.Hour, deliveryTimeOnly.Minute, 0);

        // 5) Address
        Console.Write("Enter Delivery Address: ");
        string address = Console.ReadLine();

        // 6) Create empty Order first (we assign ID later)
        int newOrderId = GetNextOrderId();
        DateTime orderCreatedDT = DateTime.Now;

        Order newOrder = new Order(newOrderId, orderCreatedDT, deliveryDT, address, "", false, "Unpaid");

        // 7) Select food items
        Console.WriteLine();
        Console.WriteLine("Available Food Items:");
        List<FoodItem> menuItems = rest.Menus[0].FoodItems;

        for (int i = 0; i < menuItems.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + menuItems[i].ItemName + " - $" + menuItems[i].ItemPrice.ToString("F2"));
        }

        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            int itemNo;
            if (!int.TryParse(Console.ReadLine(), out itemNo))
            {
                Console.WriteLine("Invalid input. Enter a number.");
                continue;
            }

            if (itemNo == 0) break;

            if (itemNo < 1 || itemNo > menuItems.Count)
            {
                Console.WriteLine("Invalid item number.");
                continue;
            }

            Console.Write("Enter quantity: ");
            int qty;
            if (!int.TryParse(Console.ReadLine(), out qty) || qty <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                continue;
            }

            FoodItem chosen = menuItems[itemNo - 1];
            newOrder.AddOrderedFoodItem(new OrderedFoodItem(chosen, qty));
        }

        if (newOrder.OrderedItems.Count == 0)
        {
            Console.WriteLine("No items selected. Order not created.");
            return;
        }

        // 8) Special request (one request only)
        Console.Write("Add special request? [Y/N]: ");
        string sr = Console.ReadLine().Trim().ToUpper();

        if (sr == "Y")
        {
            Console.Write("Enter special request: ");
            newOrder.SpecialRequest = Console.ReadLine();
        }

        // 9) Show total
        double total = newOrder.CalculateOrderTotal();
        Console.WriteLine();
        Console.WriteLine("Order Total: $" + (total - Order.DELIVERY_FEE).ToString("F2") +
                          " + $5.00 (delivery) = $" + total.ToString("F2"));

        // 10) Payment?
        Console.Write("Proceed to payment? [Y/N]: ");
        string payChoice = Console.ReadLine().Trim().ToUpper();
        if (payChoice != "Y")
        {
            Console.WriteLine("Payment not made. Order not created.");
            return;
        }

        // 11) Payment method
        string method = "";
        while (true)
        {
            Console.Write("Payment method [CC/PP/CD]: ");
            method = Console.ReadLine().Trim().ToUpper();

            if (method == "CC" || method == "PP" || method == "CD")
                break;

            Console.WriteLine("Invalid method. Enter CC, PP or CD.");
        }

        newOrder.OrderPaymentMethod = method;
        newOrder.OrderPaid = true;
        newOrder.OrderStatus = "Pending";
        newOrder.OrderTotal = total;

        // 12) Save into system
        allOrders.Add(newOrder);
        cust.AddOrder(newOrder);
        rest.AddOrder(newOrder);

        // 13) Append to orders.csv (match your LoadOrders format)
        AppendOrderToFile(newOrder, cust.EmailAddress, rest.RestaurantId);

        Console.WriteLine();
        Console.WriteLine("Order " + newOrder.OrderId + " created successfully! Status: " + newOrder.OrderStatus);
    }

    // part of (Feature 5)
    static int GetNextOrderId()
    {
        int max = 1000;
        foreach (Order o in allOrders)
            if (o.OrderId > max) max = o.OrderId;
        return max + 1;
    }

    // modifies an existing order (Feature 7)
    static void ModifyExistingOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Modify Order");
        Console.WriteLine("===========");

        // 1) Customer Email
        Customer cust = null;
        while (cust == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            cust = FindCustomerByEmail(email);
            if (cust == null)
                Console.WriteLine("Customer not found. Try again.");
        }

        // 2) Show Pending orders
        List<Order> pending = new List<Order>();
        foreach (Order o in cust.OrderList)
            if (o.OrderStatus == "Pending")
                pending.Add(o);

        if (pending.Count == 0)
        {
            Console.WriteLine("No Pending orders found.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (Order o in pending)
            Console.WriteLine(o.OrderId);

        // 3) Choose Order ID
        Order chosenOrder = null;
        while (chosenOrder == null)
        {
            Console.Write("Enter Order ID: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid input. Enter a number.");
                continue;
            }

            foreach (Order o in pending)
                if (o.OrderId == id)
                    chosenOrder = o;

            if (chosenOrder == null)
                Console.WriteLine("Order not found in Pending list. Try again.");
        }

        // Find restaurant (simple scan)
        Restaurant rest = FindRestaurantForOrder(chosenOrder.OrderId);

        // 4) Show details
        Console.WriteLine();
        Console.WriteLine("Order Items:");
        chosenOrder.DisplayOrderedFoodItems();
        Console.WriteLine("Address:");
        Console.WriteLine(chosenOrder.DeliveryAddress);
        Console.WriteLine("Delivery Date/Time:");
        Console.WriteLine(chosenOrder.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"));

        // 5) Choose modification
        Console.WriteLine();
        Console.Write("Modify: [1] Items [2] Address [3] Delivery Time: ");
        string opt = Console.ReadLine().Trim();

        if (opt == "1")
        {
            if (rest == null)
            {
                Console.WriteLine("Restaurant not found for this order. Cannot modify items.");
                return;
            }

            // backup old items + total
            List<OrderedFoodItem> oldItems = new List<OrderedFoodItem>();
            foreach (OrderedFoodItem x in chosenOrder.OrderedItems)
                oldItems.Add(x);

            double oldTotal = chosenOrder.OrderTotal;

            // clear items
            chosenOrder.OrderedItems.Clear();

            // re-select items
            Console.WriteLine();
            Console.WriteLine("Available Food Items:");
            List<FoodItem> menuItems = rest.Menus[0].FoodItems;

            for (int i = 0; i < menuItems.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + menuItems[i].ItemName + " - $" + menuItems[i].ItemPrice.ToString("F2"));
            }

            while (true)
            {
                Console.Write("Enter item number (0 to finish): ");
                int itemNo;
                if (!int.TryParse(Console.ReadLine(), out itemNo))
                {
                    Console.WriteLine("Invalid input. Enter a number.");
                    continue;
                }

                if (itemNo == 0) break;

                if (itemNo < 1 || itemNo > menuItems.Count)
                {
                    Console.WriteLine("Invalid item number.");
                    continue;
                }

                Console.Write("Enter quantity: ");
                int qty;
                if (!int.TryParse(Console.ReadLine(), out qty) || qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                FoodItem chosen = menuItems[itemNo - 1];
                chosenOrder.AddOrderedFoodItem(new OrderedFoodItem(chosen, qty));
            }

            if (chosenOrder.OrderedItems.Count == 0)
            {
                // restore old if user removed everything
                chosenOrder.OrderedItems.Clear();
                foreach (OrderedFoodItem x in oldItems)
                    chosenOrder.OrderedItems.Add(x);

                chosenOrder.OrderTotal = oldTotal;
                Console.WriteLine("No items selected. Items not changed.");
                return;
            }

            double newTotal = chosenOrder.CalculateOrderTotal();

            // if increased, ask for payment
            if (newTotal > oldTotal)
            {
                Console.WriteLine("New Total: $" + newTotal.ToString("F2") + " (Old: $" + oldTotal.ToString("F2") + ")");
                Console.Write("Additional payment required. Proceed? [Y/N]: ");
                string pay = Console.ReadLine().Trim().ToUpper();

                if (pay != "Y")
                {
                    // revert
                    chosenOrder.OrderedItems.Clear();
                    foreach (OrderedFoodItem x in oldItems)
                        chosenOrder.OrderedItems.Add(x);

                    chosenOrder.OrderTotal = oldTotal;
                    Console.WriteLine("Payment declined. Changes reverted.");
                    return;
                }

                Console.WriteLine("Payment accepted.");
            }

            chosenOrder.OrderTotal = newTotal;
            Console.WriteLine("Order " + chosenOrder.OrderId + " updated. New Total: $" + chosenOrder.OrderTotal.ToString("F2"));
        }
        else if (opt == "2")
        {
            Console.Write("Enter new Address: ");
            chosenOrder.DeliveryAddress = Console.ReadLine();
            Console.WriteLine("Order " + chosenOrder.OrderId + " updated. New Address saved.");
        }
        else if (opt == "3")
        {
            DateTime newTime;
            while (true)
            {
                Console.Write("Enter new Delivery Time (hh:mm): ");
                string t = Console.ReadLine().Trim();

                if (DateTime.TryParseExact(t, "HH:mm", null,
                    System.Globalization.DateTimeStyles.None, out newTime))
                    break;

                Console.WriteLine("Invalid time format. Try again.");
            }

            DateTime dt = chosenOrder.DeliveryDateTime;
            chosenOrder.DeliveryDateTime = new DateTime(dt.Year, dt.Month, dt.Day, newTime.Hour, newTime.Minute, 0);

            Console.WriteLine("Order " + chosenOrder.OrderId + " updated. New Delivery Time: " +
                              chosenOrder.DeliveryDateTime.ToString("HH:mm"));
        }
        else
        {
            Console.WriteLine("Invalid option.");
        }
    }

    static Restaurant FindRestaurantForOrder(int orderId)
    {
        foreach (Restaurant r in restaurants)
        {
            foreach (Order o in r.OrderQueue)
            {
                if (o.OrderId == orderId)
                    return r;
            }
        }
        return null;
    }










    static void AppendOrderToFile(Order o, string custEmail, string restId)
    {
        // Format used by your LoadOrders:
        // [0]OrderId,[1]CustEmail,[2]RestId,[3]DelDate,[4]DelTime,[5]Address,[6]OrderDateTime,[7]Total,[8]Status

        string delDate = o.DeliveryDateTime.ToString("dd/MM/yyyy");
        string delTime = o.DeliveryDateTime.ToString("HH:mm");
        string orderDT = o.OrderDateTime.ToString("dd/MM/yyyy HH:mm");

        // simple: if address has comma, wrap with quotes
        string addr = o.DeliveryAddress;
        if (addr.Contains(",")) addr = "\"" + addr + "\"";

        string line = o.OrderId + "," +
                      custEmail + "," +
                      restId + "," +
                      delDate + "," +
                      delTime + "," +
                      addr + "," +
                      orderDT + "," +
                      o.OrderTotal.ToString("F2") + "," +
                      o.OrderStatus;

        File.AppendAllText(orderFile, Environment.NewLine + line);
    }



    // HELPERS BELOW //
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
        List<string> result = new List<string>();
        string field = "";
        bool inQuotes = false;

        // read the line character by character
        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];

            // if we see a quote, we toggle inQuotes
            if (ch == '"')
            {
                inQuotes = !inQuotes;
            }
            // comma only splits when we are NOT inside quotes
            else if (ch == ',' && inQuotes == false)
            {
                result.Add(field);
                field = "";
            }
            // normal character
            else
            {
                field += ch;
            }
        }

        // add the last field
        result.Add(field);

        return result.ToArray();
    }
}