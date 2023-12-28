namespace bd_rgr
{
    public class View
    {
        public static void PrintDict(Dictionary<string, object> dict, List<string> columns)
        {
            foreach (var column in columns)
            {
                Console.Write($"{column}: {dict[column]}, ");
            }
            Console.WriteLine();
        }
        public static void PrintDictList(List<Dictionary<string, object>> list, List<string> columns)
        {
            foreach (var column in columns)
            {
                Console.Write($"{column}\t\t");
            }
            Console.WriteLine();
            foreach (var dict in list)
            {
                foreach (var column in columns)
                {
                        Console.Write($"{dict[column]}\t\t");
                }
                Console.WriteLine();
            }
        }

        public static string? RequestInput(string msg = "")
        {
            Console.Write(msg);
            return Console.ReadLine();
        }

        public static void GetValues1(ref Dictionary<UInt16, object> values, in BaseModel model)
        {
            Console.WriteLine("Enter values to edit");
            Console.Write("Enter column: ");
            var column = Console.ReadLine();
            if(column == string.Empty) return;
            Console.Write("Enter value: ");
            var value = Console.ReadLine();
            values.Add(model.GetEnum(column), value);
        }

        public static void ShowException(Exception ex, string msg = "")
        {
            if(msg != string.Empty) Console.WriteLine(msg);
            Console.WriteLine(ex.Message);
        }
    }
}