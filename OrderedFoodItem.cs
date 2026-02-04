//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Jovan Yeo]
//==========================================================

using System;

public class OrderedFoodItem
{
    private FoodItem _foodItem;         
    private int      _qtyOrdered;
    private double   _subTotal;

    public OrderedFoodItem(FoodItem foodItem, int qtyOrdered)
    {
        _foodItem   = foodItem;
        _qtyOrdered = qtyOrdered;
        _subTotal   = CalculateSubtotal();  
    }

    public FoodItem FoodItem
    {
        get { return _foodItem; }
        set { _foodItem = value; }
    }

    public int QtyOrdered
    {
        get { return _qtyOrdered; }
        set
        {
            _qtyOrdered = value;
            _subTotal   = CalculateSubtotal();  // recalculate when qty changes
        }
    }

    public double SubTotal
    {
        get { return _subTotal; }
    }

    /// returns price * quantity for this line item
    public double CalculateSubtotal()
    {
        _subTotal = _foodItem.ItemPrice * _qtyOrdered;
        return _subTotal;
    }

    public override string ToString()
    {
        return $"{_foodItem.ItemName} x{_qtyOrdered} - ${_subTotal:F2}";
    }
}
