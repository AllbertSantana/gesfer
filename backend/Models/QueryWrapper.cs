namespace backend.Models
{
    public class QueryResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int RowCount { get; set; }
        public ICollection<T>? Items { get; set; }
    }

    public class QueryFileResult
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int RowCount { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public byte[] Contents { get; set; } = null!;
    }

    public class QueryRequest<TFilter, TColumn>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public TFilter Filter { get; set; } = default!;
        public TColumn OrderBy { get; set; } = default!;
        public SortingOrder Order { get; set; }
    }
    /*
    public class Condition
    {
        public string Column { get; set; } = null!;
        public ComparisonOperator Operator { get; set; }
        public object Value { get; set; } = null!;
    }

    public class Sorting
    {
        public string Column { get; set; } = null!;
        public SortingOrder Order { get; set; }
    }

    public enum ComparisonOperator { Equal, Like, Between, In }
    public enum LogicalOperator { And, Or }
    */
    public enum SortingOrder { Asc, Desc }
}
