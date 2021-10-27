using System;
using System.Collections.Generic;

namespace Generics.Tables
{
    public class Table<TRow, TColumn, TCell>
    {
        public HashSet<TRow> Rows { get; }
        public HashSet<TColumn> Columns { get; }
        public Dictionary<Cell<TRow, TColumn>, TCell> Cells { get; }
        public Open<TRow, TColumn, TCell> Open { get; }
        public Existed<TRow, TColumn, TCell> Existed { get; }

        public Table()
        {
            Rows = new HashSet<TRow>();
            Columns = new HashSet<TColumn>();
            Cells = new Dictionary<Cell<TRow, TColumn>, TCell>();
            Open = new Open<TRow, TColumn, TCell>(this);
            Existed = new Existed<TRow, TColumn, TCell>(this);
        }

        public void AddRow(TRow row)
        {
            Rows.Add(row);
        }

        public void AddColumn(TColumn column)
        {
            Columns.Add(column);
        }
    }

    public struct Cell<TRow, TColumn>
    {
        public TRow Row;
        public TColumn Column;
        public Cell(TRow row, TColumn column)
        {
            Row = row;
            Column = column;
        }
    }

    public class Open<TRow, TColumn, TCell>
    {
        public Table<TRow, TColumn, TCell> Table { get; }

        public Open(Table<TRow, TColumn, TCell> table)
        {
            Table = table;
        }

        public TCell this[TRow row, TColumn column]
        {
            get
            {
                var cell = new Cell<TRow, TColumn>(row, column);
                return Table.Cells.ContainsKey(cell)
                    ? Table.Cells[cell]
                    : default;
            }
            set
            {
                Table.Rows.Add(row);
                Table.Columns.Add(column);
                Table.Cells.Add(new Cell<TRow, TColumn>(row, column), value);
            }
        }
    }

    public class Existed<TRow, TColumn, TCell>
    {
        public Table<TRow, TColumn, TCell> Table { get; }

        public Existed(Table<TRow, TColumn, TCell> table)
        {
            Table = table;
        }

        public TCell this[TRow row, TColumn column]
        {
            get
            {
                if (!Table.Rows.Contains(row) || !Table.Columns.Contains(column))
                    throw new ArgumentException();

                var cell = new Cell<TRow, TColumn>(row, column);
                return Table.Cells.ContainsKey(cell)
                    ? Table.Cells[cell]
                    : default;
            }
            set
            {
                if (!Table.Rows.Contains(row) || !Table.Columns.Contains(column))
                    throw new ArgumentException();

                Table.Cells.Add(new Cell<TRow, TColumn>(row, column), value);
            }
        }
    }
}
