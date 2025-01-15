using Application.Mapper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Models
{
    public class PaginatedList<T1, T2>
    {
        public PaginatedList(List<T2> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            CountData = count;
            Data = items;
        }
        public static async Task<PaginatedList<T1, T2>> CreateAsync(IQueryable<T1> source, IMapper mapper, int pageIndex = 1, int pageSize = 10)
        {
            IMapper _mapper = mapper;
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var data = _mapper.Map<List<T2>>(items);
            return new PaginatedList<T1, T2>(data, count, pageIndex, pageSize);
        }
        public static PaginatedList<T1, T2> Create(IQueryable<T1> source, IMapper mapper, int pageIndex = 1, int pageSize = 10)
        {
            IMapper _mapper = mapper;
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var data = _mapper.Map<List<T2>>(items);
            return new PaginatedList<T1, T2>(data, count, pageIndex, pageSize);
        }

        public static PaginatedList<T1, T2> Create(List<T1> source, IMapper mapper, int pageIndex = 1, int pageSize = 10)
        {
            IMapper _mapper = mapper;
            var count = source.Count;
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            var data = _mapper.Map<List<T2>>(items);
            return new PaginatedList<T1, T2>(data, count, pageIndex, pageSize);
        }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int CountData { get; private set; }
        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }
        public bool HasNextPage
        {
            get
            {
                return PageIndex < TotalPages;
            }
        }
        public List<T2> Data { get; set; }
    }
    public class PaginatedList<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int CountData { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public List<T> Data { get; set; }

        public PaginatedList(List<T> data, int pageIndex, int totalPages,int countData)
        {
            PageIndex = pageIndex;
            TotalPages = totalPages;
            CountData = countData;
            Data = data;
        }
    }
}
