//==========================================================
// Student Number : S10273755
// Student Name   : [Leroy Loh]
// Partner Name   : [Jovan Yeo]
//==========================================================
// Class   By     : [Leroy]

using System;
using System.Collections.Generic;

public class Restaurant
{
    
    private string              _restaurantId;
    private string              _restaurantName;
    private string              _restaurantEmail;
    private List<Menu>          _menus;             
    private List<Order>         _orderQueue;        
    private List<SpecialOffer>  _specialOffers;     

   
    public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
    {
        _restaurantId    = restaurantId;
        _restaurantName  = restaurantName;
        _restaurantEmail = restaurantEmail;
        _menus           = new List<Menu>();
        _orderQueue      = new List<Order>();
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

    public List<Order> OrderQueue
    {
        get { return _orderQueue; }
    }

    public List<SpecialOffer> SpecialOffers
    {
        get { return _specialOffers; }
    }

    
    public void AddMenu(Menu menu)
    {
        _menus.Add(menu);
    }

    
    public bool RemoveMenu(Menu menu)
    {
        return _menus.Remove(menu);
    }

    
    public void AddOrder(Order order)
    {
        _orderQueue.Add(order);
    }

    
    public void AddSpecialOffer(SpecialOffer offer)
    {
        _specialOffers.Add(offer);
    }

    
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
