//==========================================================
// Student Number : S10273755
// Student Name   : [Leroy Loh]
// Partner Name   : [Jovan Yeo]
//==========================================================
// Class   By     : [Leroy]

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

    

    
    public void AddFoodItem(FoodItem foodItem)
    {
        _foodItems.Add(foodItem);
    }

    
    public bool RemoveFoodItem(FoodItem foodItem)
    {
        return _foodItems.Remove(foodItem);
    }

    
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
