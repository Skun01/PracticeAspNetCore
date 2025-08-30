using System;

namespace LearnWebApi.DTOs;

public class ProductQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    public string? Name { set; get; }
    public decimal? MinPrice { set; get; }
    public decimal? MaxPrice { set; get; }
    public string? SortBy { set; get; }
    public int PageNumer { set; get; }
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
