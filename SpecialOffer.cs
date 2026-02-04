//==========================================================
// Student Number : S10274934
// Student Name   : [Jovan Yeo]
// Partner Name   : [Leroy Loh]
// Class   By     : [Leroy Loh]
//==========================================================

using System;

public class SpecialOffer
{
    private string _offerCode;
    private string _offerDesc;
    private double _discount;       

    public SpecialOffer(string offerCode, string offerDesc, double discount)
    {
        _offerCode = offerCode;
        _offerDesc = offerDesc;
        _discount  = discount;
    }

    public string OfferCode
    {
        get { return _offerCode; }
        set { _offerCode = value; }
    }

    public string OfferDesc
    {
        get { return _offerDesc; }
        set { _offerDesc = value; }
    }

    public double Discount
    {
        get { return _discount; }
        set { _discount = value; }
    }

    public override string ToString()
    {
        if (_discount > 0)
            return $"[{_offerCode}] {_offerDesc} — {_discount}% off";
        else
            return $"[{_offerCode}] {_offerDesc}";
    }
}
