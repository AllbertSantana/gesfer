namespace backend.Models
{
    public class QueryResults<T>
    {
        public uint PageNumber { get; set; }
        public uint PageSize { get; set; }
        public uint PageCount { get; set; }
        public uint RowCount { get; set; }
        public IEnumerable<T>? Results { get; set; }
    }

    public class QueryArguments<T>
    {
        public uint PageNumber { get; set; }
        public uint PageSize { get; set; }
        public T Arguments { get; set; } = default!;
        public string? OrderBy { get; set; }
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
