//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Jovan Yeo]
//==========================================================

using System;
using System.Collections.Generic;

public class Customer
{
    private string       _emailAddress;
    private string       _customerName;
    private List<Order>  _orderList;     

    public Customer(string customerName, string emailAddress)
    {
        _customerName  = customerName;
        _emailAddress  = emailAddress;
        _orderList     = new List<Order>();
    }

    public string EmailAddress
    {
        get { return _emailAddress; }
        set { _emailAddress = value; }
    }

    public string CustomerName
    {
        get { return _customerName; }
        set { _customerName = value; }
    }

    public List<Order> OrderList
    {
        get { return _orderList; }
    }


    /// add an order to this customer's order list
    public void AddOrder(Order order)
    {
        _orderList.Add(order);
    }

    /// removes an order from this customer's order list return true if successful
    public bool RemoveOrder(Order order)
    {
        return _orderList.Remove(order);
    }

    /// displays all orders belonging to the customer
    public void DisplayAllOrders()
    {
        if (_orderList.Count == 0)
        {
            Console.WriteLine("  No orders found.");
            return;
        }
        foreach (Order order in _orderList)
        {
            Console.WriteLine($"  {order.ToString()}");
        }
    }

    public override string ToString()
    {
        return $"{_customerName} ({_emailAddress})";
    }
}
