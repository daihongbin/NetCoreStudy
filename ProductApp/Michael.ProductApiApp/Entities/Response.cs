namespace Michael.ProductApiApp.Entities
{
    public class Response<T>
    {
        public string Code { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }
}
