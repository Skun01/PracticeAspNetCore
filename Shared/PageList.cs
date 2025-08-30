using System;
using Microsoft.EntityFrameworkCore;

namespace LearnWebApi.Shared;

public class PageList<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public bool HasNext => CurrentPage < TotalPages;
    public List<T> Items { get; private set; }
    public PageList(List<T> items, int pageNumber, int pageSize)
    {
        int count = items.Count;
        Items = items;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        CurrentPage = pageNumber;
    }
}
