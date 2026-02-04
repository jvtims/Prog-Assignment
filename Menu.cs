//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Leroy Loh]
//==========================================================

using System;
using System.Collections.Generic;

public class Menu
{
    private string          _menuId;
    private string          _menuName;
    private List<FoodItem>  _foodItems;     

    public Menu(string menuId, string menuName)
    {
        _menuId    = menuId;
        _menuName  = menuName;
        _foodItems = new List<FoodItem>();
    }

    public string MenuId
    {
        get { return _menuId; }
        set { _menuId = value; }
    }

    public string MenuName
    {
        get { return _menuName; }
        set { _menuName = value; }
    }

    public List<FoodItem> FoodItems
    {
        get { return _foodItems; }
    }


    /// adds a FoodItem to this menu
    public void AddFoodItem(FoodItem foodItem)
    {
        _foodItems.Add(foodItem);
    }

    /// removes a FoodItem from this menu. Returns true if found and removed
    public bool RemoveFoodItem(FoodItem foodItem)
    {
        return _foodItems.Remove(foodItem);
    }

    /// prints all food items in this menu with index numbers
    public void DisplayFoodItems()
    {
        for (int i = 0; i < _foodItems.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {_foodItems[i].ToString()}");
        }
    }
    public override string ToString()
    {
        return $"Menu: {_menuName} (ID: {_menuId}) — {_foodItems.Count} item(s)";
    }
}
