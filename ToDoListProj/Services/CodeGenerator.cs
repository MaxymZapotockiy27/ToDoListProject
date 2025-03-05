namespace ToDoListProj.Services
{
    public class CodeGenerator
    {
        private static Random _random = new Random();

        // Генерація випадкового 5-значного коду
        public string GenerateCode()
        {
            return _random.Next(10000, 99999).ToString();
        }
    }

}
