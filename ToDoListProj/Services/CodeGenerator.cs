namespace ToDoListProj.Services
{
    public class CodeGenerator
    {
        private static Random _random = new Random();

       
        public string GenerateCode()
        {
            return _random.Next(10000, 99999).ToString();
        }
    }

}
