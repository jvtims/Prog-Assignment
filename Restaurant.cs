//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Leroy Loh]
//==========================================================

using System;
using System.Collections.Generic;

public class Restaurant
{
    private string              _restaurantId;
    private string              _restaurantName;
    private string              _restaurantEmail;
    private List<Menu>          _menus;
    private Queue<Order>        _orderQueue;
    private List<SpecialOffer>  _specialOffers;     

    public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
    {
        _restaurantId    = restaurantId;
        _restaurantName  = restaurantName;
        _restaurantEmail = restaurantEmail;
        _menus           = new List<Menu>();
        _orderQueue      = new Queue<Order>();
        _specialOffers   = new List<SpecialOffer>();
    }

    public string RestaurantId
    {
        get { return _restaurantId; }
        set { _restaurantId = value; }
    }

    public string RestaurantName
    {
        get { return _restaurantName; }
        set { _restaurantName = value; }
    }

    public string RestaurantEmail
    {
        get { return _restaurantEmail; }
        set { _restaurantEmail = value; }
    }

    public List<Menu> Menus
    {
        get { return _menus; }
    }

    public Queue<Order> OrderQueue
    {
        get { return _orderQueue; }
    }


    public List<SpecialOffer> SpecialOffers
    {
        get { return _specialOffers; }
    }


    /// add the menu to the restaurant
    public void AddMenu(Menu menu)
    {
        _menus.Add(menu);
    }

    /// removes a Menu from this restaurant. Returns true if successful(bool)
    public bool RemoveMenu(Menu menu)
    {
        return _menus.Remove(menu);
    }

    /// add an Order to this restaurant's order queue.
    public void AddOrder(Order order)
    {
        _orderQueue.Enqueue(order);
    }

    /// Adds a SpecialOffer to this restaurant.
    public void AddSpecialOffer(SpecialOffer offer)
    {
        _specialOffers.Add(offer);
    }

    /// Prints all orders currently in this restaurant's queue.
    public void DisplayOrders()
    {
        if (_orderQueue.Count == 0)
        {
            Console.WriteLine("  No orders in queue.");
            return;
        }
        foreach (Order order in _orderQueue)
        {
            Console.WriteLine($"  {order.ToString()}");
        }
    }

    /// Prints all special offers for this restaurant.
    public void DisplaySpecialOffers()
    {
        if (_specialOffers.Count == 0)
        {
            Console.WriteLine("  No special offers available.");
            return;
        }
        foreach (SpecialOffer offer in _specialOffers)
        {
            Console.WriteLine($"  {offer.ToString()}");
        }
    }

    public void DisplayMenu()
    {
        if (_menus.Count == 0)
        {
            Console.WriteLine("  No menus available.");
            return;
        }
        foreach (Menu menu in _menus)
        {
            Console.WriteLine($"  {menu.ToString()}");
            menu.DisplayFoodItems();
        }
    }

    public override string ToString()
    {
        return $"{_restaurantName} ({_restaurantId})";
    }
}
