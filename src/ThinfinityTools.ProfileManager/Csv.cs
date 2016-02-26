namespace ThinfinityTools.ProfileManager
{
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;

    public class Csv
    {
        public Csv(string csvPath, bool hasHeaderLine = true)
        {
            _CsvPath = csvPath;
            _HasHeaderLine = hasHeaderLine;
        }

        public string CsvPath => _CsvPath;
        private string _CsvPath;
        private readonly bool _HasHeaderLine;


        public bool FileExists => File.Exists(CsvPath);

        public List<string> Headers => _Headers
                                    ?? (_Headers = _HasHeaderLine
                                                 ? Lines[0].Split(',').Select(s => s.Trim()).ToList()
                                                 : null);
        private List<string> _Headers;

        public List<string> Lines => _Lines ?? (_Lines = File.ReadAllLines(_CsvPath).ToList());
        private List<string> _Lines;

        public DataTable Table
        {
            get { return _Table ?? (_Table = ConvertLinesToDataTable()); }
            set { _Table = value; }
        }
        private DataTable _Table;

        public DataTable ConvertLinesToDataTable()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(Headers.Select(h => new DataColumn(h)).ToArray());
            foreach (var csvRow in Lines.Skip(1))
            {
                var row = dt.NewRow();
                row.ItemArray = csvRow.Split(',').Select(s => s.Trim()).ToArray<object>();
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
