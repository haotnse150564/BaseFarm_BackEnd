using System;
using System.Collections.Generic;

namespace Application.Commons
{
    public class Pagination<T>
    {
        public int TotalItemCount { get; set; }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        private const int MaxPageSize = 100;

        public int TotalPagesCount => (int)Math.Ceiling((double)TotalItemCount / PageSize);

        private int _pageIndex = 1; // Đặt mặc định trang đầu tiên là 1

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (value < 1)
                    _pageIndex = 1; // Không cho phép nhỏ hơn 1
                else if (value > TotalPagesCount)
                    _pageIndex = TotalPagesCount; // Không cho phép vượt quá số trang
                else
                    _pageIndex = value;
            }
        }

        public bool Next => PageIndex < TotalPagesCount;
        public bool Previous => PageIndex > 1;

        public ICollection<T> Items { get; set; }
    }
}
