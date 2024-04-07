namespace DTS.Common.DataTables;

public class DataTablesParameters
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DataTablesColumn[] Columns { get; set; }
    public DataTablesOrder[] Order { get; set; }
    public DataTablesSearch Search { get; set; }
}