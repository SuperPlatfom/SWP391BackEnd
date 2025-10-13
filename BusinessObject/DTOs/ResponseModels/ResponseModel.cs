namespace BusinessObject.DTOs
{
    public class ResponseModel
    {
        public int StatusCode { get; set; }

        public string Title { get; set; }
        public object Data { get; set; }

        public ResponseModel(int statusCode, string title, object data)
        {
            StatusCode = statusCode;
            Title = title;
            Data = data;
        }
    }
}