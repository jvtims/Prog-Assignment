//===================================================================================================
// Student Number : S10273755
// Student Name   : [Leroy Loh] Features [1,4,6,8] Advanced Feature (A) Bonus: Customer Notifications
// Partner Name   : [Jovan Yeo] Features [2,3,5,7] Advanced Feature (B) Bonus: Favourite Order
//===================================================================================================

using S10274934_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{

    static List<Restaurant> restaurants = new List<Restaurant>();
    static List<Customer> customers = new List<Customer>();
    static List<Order> allOrders = new List<Order>();
    static Stack<Order> refundStack = new Stack<Order>();
    static Dictionary<string, List<string>> notifications = new Dictionary<string, List<string>>();
    static List<FavouriteOrder> favouriteList = new List<FavouriteOrder>();


    static string restaurantFile = "restaurants.csv";
    static string foodItemFile = "fooditems.csv";
    static string customerFile = "customers.csv";
    static string orderFile = "orders.csv";
    static string favouriteFile = "favouriteorders.csv";

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
        LoadFavouriteOrders();

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
            else if (choice == "4")
            {
                ProcessOrder(); // Feature 6
            }
            else if (choice == "6")
            {
                DeleteExistingOrder(); // Feature 8
            }
            else if (choice == "7")
            {
                BulkProcessPendingOrdersForDay(); // ADVANCED FEATURE
            }
            else if (choice == "8")
            {
                ViewCustomerNotifications(); // Bonus Feature
            }
            else if (choice == "9") // ADVANCED FEATURE
            {
                DisplayTotalOrderAmountAndRefunds();
            }
            else if (choice == "10") // BONUS FEATURE
            {
                AddFavouriteOrder();
            }
            else if (choice == "11") // BONUS FEATURE
            {
                DeleteFavouriteOrder();
            }
            else if (choice == "12") // BONUS FEATURE
            {
                ViewFavouriteOrders();
            }
            else if (choice == "13") // BONUS FEATURE
            {
                CreateOrderFromFavourite();
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
        Console.WriteLine("4. Process an order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("6. Delete an existing order");
        Console.WriteLine("7. Bulk process pending orders (specific day)");
        Console.WriteLine("8. View customer notifications");
        Console.WriteLine("9. Display total order amount");
        Console.WriteLine("10. Add Favourite Order (Delivered only)");
        Console.WriteLine("11. Remove Favourite Order");
        Console.WriteLine("12. View Favourite Orders");
        Console.WriteLine("13. Create Order from Favourite");
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
    // UPDATED to read Items column (index 9) and populate OrderedItems
    static void LoadOrders()
    {
        string[] lines = File.ReadAllLines(orderFile);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = ParseCSVLine(lines[i]);

            // Must have Items column now
            if (parts.Length < 10) continue;

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

            //  Parse Items column and fill OrderedItems
            string itemsField = parts[9].Trim();
            AddItemsFromCsvToOrder(o, restId, itemsField);

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

    //Creates a new order using customer email under a restaraunt ID (FEATURE 5)
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

        Order newOrder = new Order(
            newOrderId,
            cust.EmailAddress,          // customer email
            rest.RestaurantId,          // restaurant ID
            orderCreatedDT,             // order creation date/time
            deliveryDT,                 // delivery date/time
            address,                    // delivery address
            "",                          // payment method (set later)
            false,                       // not paid yet
            "Unpaid"                    // order status
        );

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

        // 13) Append to orders.csv (NOW includes Items column)
        AppendOrderToFile(newOrder, cust.EmailAddress, rest.RestaurantId);

        Console.WriteLine();
        Console.WriteLine("Order " + newOrder.OrderId + " created successfully! Status: " + newOrder.OrderStatus);
        AddNotification(cust.EmailAddress,"Order " + newOrder.OrderId + " created successfully. Status: Pending."); // For Customer Notifications

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

    // HELPER FOR FEATURE 7
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

    // processes orders (FEATURE 6)
    static void ProcessOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Process Order");
        Console.WriteLine("=============");

        // Ask for restaurant ID
        Restaurant rest = null;
        while (rest == null)
        {
            Console.Write("Enter Restaurant ID: ");
            string restId = Console.ReadLine().Trim();
            rest = FindRestaurantById(restId);

            if (rest == null)
                Console.WriteLine("Restaurant not found. Try again.");
        }

        if (rest.OrderQueue.Count == 0)
        {
            Console.WriteLine("No orders found for this restaurant.");
            return;
        }

        // We will process each order currently in the queue once
        int originalCount = rest.OrderQueue.Count;

        for (int i = 0; i < originalCount; i++)
        {
            // Take the next order from the front
            Order o = rest.OrderQueue.Dequeue();

            Console.WriteLine();
            Console.WriteLine("Order " + o.OrderId + ":");
            Console.WriteLine("Customer: " + GetCustomerNameByOrderId(o.OrderId));

            Console.WriteLine("Ordered Items:");
            if (o.OrderedItems.Count == 0)
                Console.WriteLine("  (No ordered items recorded)");
            else
                o.DisplayOrderedFoodItems();

            Console.WriteLine("Delivery date/time: " + o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("Total Amount: $" + o.OrderTotal.ToString("F2"));
            Console.WriteLine("Order Status: " + o.OrderStatus);
            Console.WriteLine();

            Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
            string action = Console.ReadLine().Trim().ToUpper();

            if (action == "C")
            {
                if (o.OrderStatus == "Pending")
                {
                    o.OrderStatus = "Preparing";
                    Console.WriteLine("Order " + o.OrderId + " confirmed. Status: Preparing");
                    AddNotification(o.CustomerEmail,"Order " + o.OrderId + " confirmed. Status: Preparing.");

                }
                else
                {
                    Console.WriteLine("Cannot confirm. Order is not Pending.");
                }

                rest.OrderQueue.Enqueue(o);
            }
            else if (action == "R")
            {
                if (o.OrderStatus == "Pending")
                {
                    o.OrderStatus = "Rejected";
                    refundStack.Push(o);
                    Console.WriteLine("Order " + o.OrderId + " rejected. Refund of $" + o.OrderTotal.ToString("F2") + " processed.");
                    AddNotification(o.CustomerEmail, "Order " + o.OrderId + " rejected. Refund processed.");

                }
                else
                {
                    Console.WriteLine("Cannot reject. Order is not Pending.");
                }

                rest.OrderQueue.Enqueue(o);
            }
            else if (action == "S")
            {
                if (o.OrderStatus == "Cancelled")
                {
                    Console.WriteLine("Order " + o.OrderId + " skipped (Cancelled).");
                }
                else
                {
                    Console.WriteLine("Skip chosen. Moving on.");
                }

                rest.OrderQueue.Enqueue(o);
            }
            else if (action == "D")
            {
                if (o.OrderStatus == "Preparing")
                {
                    o.OrderStatus = "Delivered";
                    Console.WriteLine("Order " + o.OrderId + " delivered. Status: Delivered");
                    AddNotification(o.CustomerEmail,    "Order " + o.OrderId + " delivered successfully.");

                }
                else
                {
                    Console.WriteLine("Cannot deliver. Order is not Preparing.");
                }

                rest.OrderQueue.Enqueue(o);
            }
            else
            {
                Console.WriteLine("Invalid action. Moving to next order.");
                rest.OrderQueue.Enqueue(o);
            }
        }
    }

    // HELPER FOR FEATURE 6
    static string GetCustomerNameByOrderId(int orderId)
    {
        foreach (Customer c in customers)
        {
            foreach (Order o in c.OrderList)
            {
                if (o.OrderId == orderId)
                    return c.CustomerName;
            }
        }
        return "Unknown";
    }

    // finds and deletes a existing order (FEATURE 8)
 
    static void DeleteExistingOrder()
    {
        Console.WriteLine();
        Console.WriteLine("Delete Order");
        Console.WriteLine("============");

        // Ask customer email
        Customer cust = null;
        while (cust == null)
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();
            cust = FindCustomerByEmail(email);

            if (cust == null)
                Console.WriteLine("Customer not found. Try again.");
        }

        // Get pending orders for this customer
        List<Order> pendingOrders = new List<Order>();
        foreach (Order o in cust.OrderList)
        {
            if (o.OrderStatus == "Pending")
                pendingOrders.Add(o);
        }

        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No Pending orders found for this customer.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (Order o in pendingOrders)
            Console.WriteLine(o.OrderId);

        // Ask for order ID
        Order chosen = null;
        while (chosen == null)
        {
            Console.Write("Enter Order ID: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid input. Enter a number.");
                continue;
            }

            foreach (Order o in pendingOrders)
            {
                if (o.OrderId == id)
                {
                    chosen = o;
                    break;
                }
            }

            if (chosen == null)
                Console.WriteLine("Order ID not found in Pending list. Try again.");
        }

        // Display basic info
        Console.WriteLine();
        Console.WriteLine("Customer: " + cust.CustomerName);

        Console.WriteLine("Ordered Items:");
        if (chosen.OrderedItems.Count == 0)
            Console.WriteLine("  (No ordered items recorded)");
        else
            chosen.DisplayOrderedFoodItems();

        Console.WriteLine("Delivery date/time: " + chosen.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"));
        Console.WriteLine("Total Amount: $" + chosen.OrderTotal.ToString("F2"));
        Console.WriteLine("Order Status: " + chosen.OrderStatus);

        // Confirm deletion
        Console.Write("Confirm deletion? [Y/N]: ");
        string confirm = Console.ReadLine().Trim().ToUpper();

        if (confirm == "Y")
        {
            chosen.OrderStatus = "Cancelled";
            refundStack.Push(chosen);

            // persist to CSV
            SaveAllOrdersToFile();
            AddNotification(chosen.CustomerEmail,"Order " + chosen.OrderId + " cancelled. Refund processed.");


            Console.WriteLine("Order " + chosen.OrderId + " cancelled. Refund of $" + chosen.OrderTotal.ToString("F2") + " processed.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled. No changes made.");
        }
    }

    // HELPER to append changes/delete/create to csv files
    
    static void AppendOrderToFile(Order o, string custEmail, string restId)
    {
        string deliveryDate = o.DeliveryDateTime.ToString("dd/MM/yyyy");
        string deliveryTime = o.DeliveryDateTime.ToString("HH:mm");
        string orderDateTime = o.OrderDateTime.ToString("dd/MM/yyyy HH:mm");

        string address = o.DeliveryAddress ?? "";
        if (address.Contains(",")) address = "\"" + address + "\"";

        string items = "\"" + BuildItemsColumn(o) + "\"";

        // OrderId, CustomerEmail, RestaurantId, DeliveryDate, DeliveryTime,
        // Address, OrderDateTime, TotalAmount, Status, Items
        string line = o.OrderId + "," +
                      custEmail + "," +
                      restId + "," +
                      deliveryDate + "," +
                      deliveryTime + "," +
                      address + "," +
                      orderDateTime + "," +
                      o.OrderTotal.ToString("F2") + 
                      "," + o.OrderStatus + "," +
                      items;

        File.AppendAllText(orderFile, Environment.NewLine + line);
    }

    //  Writes the whole CSV back (keeps Items column)
    static void SaveAllOrdersToFile()
    {
        List<string> lines = new List<string>();
        lines.Add("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items");

        foreach (Order o in allOrders)
        {
            string deliveryDate = o.DeliveryDateTime.ToString("dd/MM/yyyy");
            string deliveryTime = o.DeliveryDateTime.ToString("HH:mm");
            string createdDT = o.OrderDateTime.ToString("dd/MM/yyyy HH:mm");

            string address = o.DeliveryAddress ?? "";
            if (address.Contains(",")) address = "\"" + address + "\"";

            string items = "\"" + BuildItemsColumn(o) + "\"";

            string line =
                o.OrderId + "," +
                o.CustomerEmail + "," +
                o.RestaurantId + "," +
                deliveryDate + "," +
                deliveryTime + "," +
                address + "," +
                createdDT + "," +
                o.OrderTotal.ToString("F2") + "," +
                o.OrderStatus + "," +
                items;

            lines.Add(line);
        }

        File.WriteAllLines(orderFile, lines);
    }

    // Convert OrderedItems back to Items string: ItemName,qty|ItemName,qty
    static string BuildItemsColumn(Order o)
    {
        if (o.OrderedItems == null || o.OrderedItems.Count == 0) return "";

        List<string> parts = new List<string>();
        foreach (OrderedFoodItem ofi in o.OrderedItems)
        {
            
            string name = ofi.FoodItem.ItemName;
            int qty = ofi.QtyOrdered;
            parts.Add(name + "," + qty);
        }
        return string.Join("|", parts);
    }

    //  Parse Items column from CSV into OrderedItems list
    static void AddItemsFromCsvToOrder(Order o, string restId, string itemsField)
    {
        if (string.IsNullOrWhiteSpace(itemsField)) return;

        itemsField = itemsField.Trim();
        if (itemsField.StartsWith("\"") && itemsField.EndsWith("\"") && itemsField.Length >= 2)
            itemsField = itemsField.Substring(1, itemsField.Length - 2);

        string[] items = itemsField.Split('|');

        foreach (string raw in items)
        {
            string token = raw.Trim();
            if (token.Length == 0) continue;

            string name;
            int qty = 1;

            int commaIndex = token.LastIndexOf(',');
            if (commaIndex >= 0)
            {
                name = token.Substring(0, commaIndex).Trim();
                string qtyStr = token.Substring(commaIndex + 1).Trim();

                int parsedQty;
                if (int.TryParse(qtyStr, out parsedQty) && parsedQty > 0)
                    qty = parsedQty;
            }
            else
            {
                name = token.Trim();
                qty = 1;
            }

            if (string.IsNullOrWhiteSpace(name)) continue;

            FoodItem fi = FindFoodItemByName(restId, name);
            if (fi == null)
                fi = new FoodItem(name, "Loaded from CSV", 0);

            o.AddOrderedFoodItem(new OrderedFoodItem(fi, qty));
        }
    }

    static FoodItem FindFoodItemByName(string restId, string itemName)
    {
        Restaurant r = FindRestaurantById(restId);
        if (r == null || r.Menus.Count == 0) return null;

        foreach (FoodItem fi in r.Menus[0].FoodItems)
        {
            if (fi.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                return fi;
        }
        return null;
    }

    // HELPERS BELOW //
    static Restaurant FindRestaurantById(string id)
    {
        foreach (Restaurant r in restaurants)
            if (r.RestaurantId.ToLower() == id.ToLower())
                return r;
        return null;
    }

    static Customer FindCustomerByEmail(string email)
    {
        foreach (Customer c in customers)
            if (c.EmailAddress.ToLower() == email.ToLower())
                return c;
        return null;
    }

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

        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];

            if (ch == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (ch == ',' && inQuotes == false)
            {
                result.Add(field);
                field = "";
            }
            else
            {
                field += ch;
            }
        }

        result.Add(field);

        return result.ToArray();
    }

    // ADVANCED/BONUS FEATURES //

    static void BulkProcessPendingOrdersForDay() // Leroy advanced feature
    {
        Console.WriteLine();
        Console.WriteLine("Bulk Process Pending Orders (Specific Day)");
        Console.WriteLine("==========================================");

        // Ask for date
        DateTime chosenDate;
        while (true)
        {
            Console.Write("Enter date (dd/MM/yyyy): ");
            string input = Console.ReadLine().Trim();

            if (DateTime.TryParseExact(input, "dd/MM/yyyy", null,
                System.Globalization.DateTimeStyles.None, out chosenDate))
                break;

            Console.WriteLine("Invalid date format. Try again.");
        }

        int totalPendingInQueues = 0;

        // First pass: count pending orders for that date in queues
        foreach (Restaurant r in restaurants)
        {
            foreach (Order o in r.OrderQueue)
            {
                if (o.OrderStatus == "Pending" &&
                    o.DeliveryDateTime.Date == chosenDate.Date)
                {
                    totalPendingInQueues++;
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine("Total Pending orders in ALL Restaurant queues for " +
                          chosenDate.ToString("dd/MM/yyyy") + ": " + totalPendingInQueues);

        if (totalPendingInQueues == 0)
            return;

        int processed = 0;
        int preparingCount = 0;
        int rejectedCount = 0;

        DateTime now = DateTime.Now;

        // Second pass: process orders (we must dequeue/enqueue to update queue safely)
        foreach (Restaurant r in restaurants)
        {
            int queueSize = r.OrderQueue.Count;

            for (int i = 0; i < queueSize; i++)
            {
                Order o = r.OrderQueue.Dequeue();

                // Only process pending orders for selected date
                if (o.OrderStatus == "Pending" && o.DeliveryDateTime.Date == chosenDate.Date)
                {
                    processed++;

                    // If delivery time is less than 1 hour from NOW -> reject
                    TimeSpan diff = o.DeliveryDateTime - now;

                    if (diff.TotalMinutes < 60)
                    {
                        o.OrderStatus = "Rejected";
                        refundStack.Push(o);
                        rejectedCount++;

                        AddNotification(o.CustomerEmail,
                            "Order " + o.OrderId + " auto-rejected (delivery time too soon). Refund processed.");
                    }
                    else
                    {
                        o.OrderStatus = "Preparing";
                        preparingCount++;

                        AddNotification(o.CustomerEmail,
                            "Order " + o.OrderId + " auto-processed. Status: Preparing.");
                    }
                }

                // Put order back into queue
                r.OrderQueue.Enqueue(o);
            }
        }

        double percentProcessed = 0;
        if (allOrders.Count > 0)
            percentProcessed = (processed * 100.0) / allOrders.Count;

        Console.WriteLine();
        Console.WriteLine("Summary Statistics");
        Console.WriteLine("------------------");
        Console.WriteLine("Orders processed: " + processed);
        Console.WriteLine("Preparing: " + preparingCount);
        Console.WriteLine("Rejected: " + rejectedCount);
        Console.WriteLine("Processed vs ALL orders: " + percentProcessed.ToString("F2") + "%");

        // Save updated statuses to CSV
        SaveAllOrdersToFile();
    }

    static void AddNotification(string customerEmail, string message)
    {
        if (customerEmail == null) return;

        string email = customerEmail.Trim().ToLower();
        if (email == "") return;

        if (!notifications.ContainsKey(email))
            notifications[email] = new List<string>();

        notifications[email].Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - " + message);
    }

    static void ViewCustomerNotifications() // Leroy bonus feature
    {
        Console.WriteLine();
        Console.WriteLine("View Customer Notifications");
        Console.WriteLine("===========================");

        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine().Trim().ToLower();

        if (email == "" || !notifications.ContainsKey(email) || notifications[email].Count == 0)
        {
            Console.WriteLine("No notifications found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Notifications for " + email + ":");
        Console.WriteLine("--------------------------------");

        foreach (string msg in notifications[email])
            Console.WriteLine("- " + msg);
    }

    static void DisplayTotalOrderAmountAndRefunds() // Jovan advanced Feature
    {
        const double COMMISSION_RATE = 0.30;

        double overallDeliveredFoodTotal = 0; // delivered less delivery fee (report requirement)
        double overallDeliveredFullTotal = 0; // delivered including delivery (for 30% earnings)
        double overallRefundTotal = 0;

        Console.WriteLine("\n=== Advanced Feature B: Revenue Summary ===");

        for (int i = 0; i < restaurants.Count; i++)
        {
            Restaurant r = restaurants[i];

            double deliveredFoodTotal = 0;
            double deliveredFullTotal = 0;
            double refundTotal = 0;

            for (int j = 0; j < allOrders.Count; j++)
            {
                Order o = allOrders[j];

                // match restaurant (change names if needed)
                if (o.RestaurantId != r.RestaurantId)
                    continue;

                if (o.OrderStatus == "Delivered")
                {
                    deliveredFoodTotal += (o.OrderTotal - Order.DELIVERY_FEE); // less delivery fee per order (WHAT DOES THIS MEAN)
                    deliveredFullTotal += o.OrderTotal;                        // includes delivery fee for gruberoo 30%
                }
                else if (o.OrderStatus == "Rejected" || o.OrderStatus == "Cancelled" || o.OrderStatus == "Refunded")
                {
                    refundTotal += o.OrderTotal; // refund includes delivery fee
                }
            }

            Console.WriteLine("\nRestaurant: " + r.RestaurantName + " (" + r.RestaurantId + ")");
            Console.WriteLine("Total Delivered Order Amount (less delivery): $" + deliveredFoodTotal.ToString("F2"));
            Console.WriteLine("Total Refunds (Cancelled): $" + refundTotal.ToString("F2"));

            overallDeliveredFoodTotal += deliveredFoodTotal;
            overallDeliveredFullTotal += deliveredFullTotal;
            overallRefundTotal += refundTotal;
        }

        double gruberooEarns = overallDeliveredFullTotal * COMMISSION_RATE;

        Console.WriteLine("\n=== Overall Totals ===");
        Console.WriteLine("Total Order Amount (less delivery): $" + overallDeliveredFoodTotal.ToString("F2"));
        Console.WriteLine("Total Refunds: $" + overallRefundTotal.ToString("F2"));
        Console.WriteLine("Final Amount Gruberoo Earns (30% of Delivered, incl. delivery): $" + gruberooEarns.ToString("F2"));
    }

    static void LoadFavouriteOrders() // Jovan bonus feature
    {
        // If file doesn't exist, create it with header only
        if (!File.Exists(favouriteFile))
        {
            File.WriteAllText(favouriteFile, "CustomerEmail,OrderID,FavouriteName\n");
            return;
        }

        string[] lines = File.ReadAllLines(favouriteFile);

        // Start from 1 to skip header
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Trim() == "")
                continue;

            // If your Program.cs already has ParseCSVLine, use it (recommended)
            string[] parts = ParseCSVLine(lines[i]);

            if (parts.Length < 3)
                continue;

            string email = parts[0].Trim();
            int orderId = Convert.ToInt32(parts[1].Trim());
            string favName = parts[2].Trim();

            favouriteList.Add(new FavouriteOrder(email, orderId, favName));
        }
    }

    static void SaveFavouriteOrders() // Jovan bonus feature
    {
        // Always rewrite the whole file (simpler, less bugs)
        List<string> lines = new List<string>();
        lines.Add("CustomerEmail,OrderID,FavouriteName");

        for (int i = 0; i < favouriteList.Count; i++)
        {
            FavouriteOrder f = favouriteList[i];

            string favName = f.FavouriteName;
            if (favName.Contains(","))
                favName = "\"" + favName + "\"";

            lines.Add(f.CustomerEmail + "," + f.OrderId + "," + favName);
        }

        File.WriteAllLines(favouriteFile, lines);
    }

    static void AddFavouriteOrder() // Jovan bonus feature
    {
        Console.WriteLine();
        Console.WriteLine("Add Favourite Order (Delivered only)");
        Console.WriteLine("===================================");

        // 1) Customer email (must exist)
        Customer cust = null;
        while (cust == null)
        {
            Console.Write("Enter Customer Email: ");
            string emailInput = Console.ReadLine().Trim();
            cust = FindCustomerByEmail(emailInput);
            if (cust == null)
                Console.WriteLine("Customer not found. Try again.");
        }

        string email = cust.EmailAddress;

        // 2) Collect Delivered orders for this customer
        List<Order> delivered = new List<Order>();
        for (int i = 0; i < allOrders.Count; i++)
        {
            Order o = allOrders[i];
            if (o.CustomerEmail == email && o.OrderStatus == "Delivered")
                delivered.Add(o);
        }

        if (delivered.Count == 0)
        {
            Console.WriteLine("No Delivered orders found. Only Delivered orders can be favourited.");
            return;
        }

        // 3) Show delivered orders and let user choose one
        Console.WriteLine();
        Console.WriteLine("Delivered Orders:");
        for (int i = 0; i < delivered.Count; i++)
        {
            Order o = delivered[i];
            Console.WriteLine((i + 1) + ". OrderId: " + o.OrderId + " | Restaurant: " + o.RestaurantId +
                              " | Total: $" + o.OrderTotal.ToString("F2"));
        }

        Console.Write("Choose an order number to favourite: ");
        int choice;
        if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > delivered.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        Order chosen = delivered[choice - 1];

        // 4) Favourite name
        Console.Write("Enter Favourite Name (e.g. MyUsual): ");
        string favName = Console.ReadLine().Trim();
        if (favName == "")
        {
            Console.WriteLine("Favourite name cannot be empty.");
            return;
        }

        // 5) Prevent duplicate favourite names for the same email (optional but neat)
        for (int i = 0; i < favouriteList.Count; i++)
        {
            if (favouriteList[i].CustomerEmail == email &&
                favouriteList[i].FavouriteName.Equals(favName, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("You already have a favourite with this name. Use a different name.");
                return;
            }
        }

        FavouriteOrder fav = new FavouriteOrder(email, chosen.OrderId, favName);

        favouriteList.Add(fav);
        SaveFavouriteOrders();

        Console.WriteLine("Favourite saved successfully!");
    }

    static void DeleteFavouriteOrder() // Jovan bonus feature
    {
        Console.Write("Enter customer email: ");
        string email = Console.ReadLine().Trim();

        // Collect favourites for this customer
        List<FavouriteOrder> userFavs = new List<FavouriteOrder>();

        for (int i = 0; i < favouriteList.Count; i++)
        {
            if (favouriteList[i].CustomerEmail == email)
                userFavs.Add(favouriteList[i]);
        }

        if (userFavs.Count == 0)
        {
            Console.WriteLine("No favourite orders found for this email.");
            return;
        }

        Console.WriteLine("\nFavourite Orders:");
        for (int i = 0; i < userFavs.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " +
                userFavs[i].FavouriteName +
                " (OrderID: " + userFavs[i].OrderId + ")");
        }

        Console.Write("Enter favourite number to delete: ");
        int choice = Convert.ToInt32(Console.ReadLine());

        if (choice < 1 || choice > userFavs.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        FavouriteOrder toRemove = userFavs[choice - 1];

        // Remove from main list
        favouriteList.Remove(toRemove);

        // Rewrite CSV to reflect deletion
        SaveFavouriteOrders();

        Console.WriteLine("Favourite order deleted successfully.");
    }

    static void ViewFavouriteOrders() // Jovan bonus feature
    {
        Console.Write("Enter customer email: ");
        string email = Console.ReadLine().Trim();

        bool found = false;

        Console.WriteLine("\nFavourite Orders:");
        for (int i = 0; i < favouriteList.Count; i++)
        {
            FavouriteOrder f = favouriteList[i];

            if (f.CustomerEmail == email)
            {
                found = true;
                Console.WriteLine((i + 1) + ". " + f.FavouriteName + " (OrderID: " + f.OrderId + ")");
            }
        }

        if (!found)
            Console.WriteLine("No favourites found for this email.");
    }

    static void CreateOrderFromFavourite() // Jovan bonus feature
    {
        Console.WriteLine();
        Console.WriteLine("Create Order from Favourite");
        Console.WriteLine("===========================");

        // 1) Customer email must exist
        Customer cust = null;
        while (cust == null)
        {
            Console.Write("Enter Customer Email: ");
            string emailInput = Console.ReadLine().Trim();
            cust = FindCustomerByEmail(emailInput);
            if (cust == null)
                Console.WriteLine("Customer not found. Try again.");
        }

        string email = cust.EmailAddress;

        // 2) Collect favourites for this customer
        List<FavouriteOrder> userFavs = new List<FavouriteOrder>();
        for (int i = 0; i < favouriteList.Count; i++)
        {
            if (favouriteList[i].CustomerEmail == email)
                userFavs.Add(favouriteList[i]);
        }

        if (userFavs.Count == 0)
        {
            Console.WriteLine("No favourites found. Please add one first.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Your Favourites:");
        for (int i = 0; i < userFavs.Count; i++)
        {
            Console.WriteLine((i + 1) + ". " + userFavs[i].FavouriteName + " (OrderId: " + userFavs[i].OrderId + ")");
        }

        Console.Write("Choose favourite number: ");
        int favChoice;
        if (!int.TryParse(Console.ReadLine(), out favChoice) || favChoice < 1 || favChoice > userFavs.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        FavouriteOrder selectedFav = userFavs[favChoice - 1];

        // 3) Find the original order by OrderId
        Order original = null;
        for (int i = 0; i < allOrders.Count; i++)
        {
            if (allOrders[i].OrderId == selectedFav.OrderId)
            {
                original = allOrders[i];
                break;
            }
        }

        if (original == null)
        {
            Console.WriteLine("Original order not found. (orders.csv may have been changed)");
            return;
        }

        // 4) Restaurant must exist
        Restaurant rest = FindRestaurantById(original.RestaurantId);
        if (rest == null)
        {
            Console.WriteLine("Restaurant not found for this favourite.");
            return;
        }

        // 5) Ask delivery date + time (same as CreateNewOrder, but items are copied)
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

        Console.Write("Enter Delivery Address: ");
        string address = Console.ReadLine();

        // 6) Create new order (same constructor as CreateNewOrder)
        int newOrderId = GetNextOrderId();
        DateTime orderCreatedDT = DateTime.Now;

        Order newOrder = new Order(
            newOrderId,
            email,
            rest.RestaurantId,
            orderCreatedDT,
            deliveryDT,
            address,
            "",
            false,
            "Unpaid"
        );

        // 7) Copy items from original order using Items string (re-use your existing helpers)
        string itemsField = BuildItemsColumn(original);
        AddItemsFromCsvToOrder(newOrder, rest.RestaurantId, itemsField);

        if (newOrder.OrderedItems.Count == 0)
        {
            Console.WriteLine("Favourite order has no items. Cannot create order.");
            return;
        }

        // 8) Special request (optional)
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

        // 13) Append to orders.csv (Items are included)
        AppendOrderToFile(newOrder, email, rest.RestaurantId);

        Console.WriteLine();
        Console.WriteLine("Order " + newOrder.OrderId + " created from favourite! Status: " + newOrder.OrderStatus);
        AddNotification(email, "Order " + newOrder.OrderId + " created from favourite. Status: Pending.");
    }
}

