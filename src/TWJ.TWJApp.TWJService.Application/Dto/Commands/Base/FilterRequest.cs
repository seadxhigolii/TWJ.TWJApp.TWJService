using System;

namespace TWJ.TWJApp.TWJService.Application.Dto.Commands.Base
{
    public class FilterRequest
    {
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public int? TopRecords { get; set; }
        public string TagName { get; set; }
        public Guid? TagID { get; set; }
    }
}
