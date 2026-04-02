namespace MovieTicketAPI.Application.Helpers
{
    public static class HallGridHelper
    {
        public const int MinRows = 1;
        public const int MaxRows = 26;
        public const int MinColumns = 1;
        public const int MaxColumns = 40;

        public static string? ValidateDimensions(int rowCount, int columnCount)
        {
            if (rowCount < MinRows || rowCount > MaxRows)
                return $"Satır sayısı {MinRows} ile {MaxRows} arasında olmalıdır (A–Z).";
            if (columnCount < MinColumns || columnCount > MaxColumns)
                return $"Sütun sayısı {MinColumns} ile {MaxColumns} arasında olmalıdır.";
            return null;
        }

        /// <summary>Tek harf satır (A=0) + sütun numarası (1 tabanlı), örn. A1, J12.</summary>
        public static bool IsValidSeatForGrid(string? seatRaw, int rowCount, int columnCount)
        {
            if (string.IsNullOrWhiteSpace(seatRaw)) return false;
            var s = seatRaw.Trim().ToUpperInvariant().Replace("-", "").Replace(" ", "");
            if (s.Length < 2) return false;
            var rowLetter = s[0];
            if (rowLetter < 'A' || rowLetter > 'Z') return false;
            var rowIndex = rowLetter - 'A';
            if (rowIndex < 0 || rowIndex >= rowCount) return false;
            if (!int.TryParse(s.AsSpan(1), out var col) || col < 1 || col > columnCount)
                return false;
            return true;
        }
    }
}
