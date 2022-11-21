namespace backend.Models
{
    public class QueryResults<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int RowCount { get; set; }
        public IEnumerable<T>? Results { get; set; }
    }

    public class QueryArguments<TArg, TCol>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public TArg Arguments { get; set; } = default!;
        public TCol OrderBy { get; set; } = default!;
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
