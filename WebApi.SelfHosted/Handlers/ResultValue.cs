namespace WebApi.SelfHosted.Handlers
{
    public class ResultValue<T>
    {
        public int Count { get; set; }
        public T[] Results { get; set; }

        //public void SetResults(object[] results)
        //{
        //    Results = results.Select(x => (T)x).ToArray();
        //}
    }
}
