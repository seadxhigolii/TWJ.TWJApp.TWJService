namespace TWJ.TWJApp.TWJService.Application.Dto.Commands.Base
{
    public class FilterRequest
    {
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }
}
