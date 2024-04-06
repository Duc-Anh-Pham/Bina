namespace Bina.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager(int totalItems, int? page, int pageSize = 10)
        {
            // Calculate total, start and end pages
            TotalItems = totalItems;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)TotalItems / (decimal)PageSize);
            CurrentPage = page ?? 1;
            StartPage = Math.Max(1, CurrentPage - 5);
            EndPage = Math.Min(TotalPages, CurrentPage + 4);

            // Adjust start and end pages based on current page and total pages
            if (TotalPages <= 9)
            {
                StartPage = 1;
                EndPage = TotalPages;
            }
            else
            {
                if (CurrentPage <= 5)
                {
                    EndPage = 9;
                }
                else if (CurrentPage + 4 >= TotalPages)
                {
                    StartPage = TotalPages - 8;
                    EndPage = TotalPages;
                }
            }
        }
    }
}
