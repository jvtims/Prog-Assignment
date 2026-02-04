//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Jovan Yeo]
//==========================================================

using System;

public class FoodItem
{
    private string _itemName;
    private string _itemDesc;
    private double _itemPrice;
    private string _customise;          

    public FoodItem(string itemName, string itemDesc, double itemPrice)
    {
        _itemName   = itemName;
        _itemDesc   = itemDesc;
        _itemPrice  = itemPrice;
        _customise  = "";               
    }

    public string ItemName
    {
        get { return _itemName; }
        set { _itemName = value; }
    }

    public string ItemDesc
    {
        get { return _itemDesc; }
        set { _itemDesc = value; }
    }

    public double ItemPrice
    {
        get { return _itemPrice; }
        set { _itemPrice = value; }
    }

    public string Customise
    {
        get { return _customise; }
        set { _customise = value; }
    }

    public override string ToString()
    {
        string result = $"{_itemName}: {_itemDesc} - ${_itemPrice:F2}";
        if (!string.IsNullOrWhiteSpace(_customise))
            result += $" [Customisation: {_customise}]";
        return result;
    }
}
