//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Jovan Yeo]
//==========================================================

using System;
using System.Collections.Generic;

public class Order
{
    public const double DELIVERY_FEE = 5.00;

    private int              _orderId;
    private DateTime         _orderDateTime;         
    private double           _orderTotal;            
    private string           _orderStatus;           
    private DateTime         _deliveryDateTime;      
    private string           _deliveryAddress;
    private string           _orderPaymentMethod;    
    private bool             _orderPaid;
    private List<OrderedFoodItem> _orderedItems;     


    public Order(int orderId, DateTime orderDateTime, DateTime deliveryDateTime,
                 string deliveryAddress, string orderPaymentMethod, bool orderPaid, string orderStatus)
    {
        _orderId            = orderId;
        _orderDateTime      = orderDateTime;
        _deliveryDateTime   = deliveryDateTime;
        _deliveryAddress    = deliveryAddress;
        _orderPaymentMethod = orderPaymentMethod;
        _orderPaid          = orderPaid;
        _orderStatus        = orderStatus;
        _orderedItems       = new List<OrderedFoodItem>();
        _orderTotal         = 0.0;
    }

    public int OrderId
    {
        get { return _orderId; }
        set { _orderId = value; }
    }

    public DateTime OrderDateTime
    {
        get { return _orderDateTime; }
        set { _orderDateTime = value; }
    }

    public double OrderTotal
    {
        get { return _orderTotal; }
        set { _orderTotal = value; }
    }

    public string OrderStatus
    {
        get { return _orderStatus; }
        set { _orderStatus = value; }
    }

    public DateTime DeliveryDateTime
    {
        get { return _deliveryDateTime; }
        set { _deliveryDateTime = value; }
    }

    public string DeliveryAddress
    {
        get { return _deliveryAddress; }
        set { _deliveryAddress = value; }
    }

    public string OrderPaymentMethod
    {
        get { return _orderPaymentMethod; }
        set { _orderPaymentMethod = value; }
    }

    public bool OrderPaid
    {
        get { return _orderPaid; }
        set { _orderPaid = value; }
    }

    public List<OrderedFoodItem> OrderedItems
    {
        get { return _orderedItems; }
    }



    /// add an OrderedFoodItem to this order.

    public void AddOrderedFoodItem(OrderedFoodItem item)
    {
        _orderedItems.Add(item);
        _orderTotal = CalculateOrderTotal();
    }

    /// removes an OrderedFoodItem from this order. Returns true if found and removed
    public bool RemoveOrderedFoodItem(OrderedFoodItem item)
    {
        if (_orderedItems.Remove(item))
        {
            _orderTotal = CalculateOrderTotal();
            return true;
        }
        return false;
    }

    /// sum up all line subtotals and adds the delivery fee
    
    public double CalculateOrderTotal()
    {
        double subtotal = 0.0;
        foreach (OrderedFoodItem item in _orderedItems)
        {
            subtotal += item.CalculateSubtotal();
        }
        _orderTotal = subtotal + DELIVERY_FEE;
        return _orderTotal;
    }

    /// prints each ordered item (name and quantity)

    public void DisplayOrderedFoodItems()
    {
        for (int i = 0; i < _orderedItems.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {_orderedItems[i].FoodItem.ItemName} - {_orderedItems[i].QtyOrdered}");
        }
    }

    public override string ToString()
    {
        string result = $"Order {_orderId} | Status: {_orderStatus} | " +
                        $"Delivery: {_deliveryDateTime:dd/MM/yyyy HH:mm} | " +
                        $"Total: ${_orderTotal:F2}";
        return result;
    }
}
