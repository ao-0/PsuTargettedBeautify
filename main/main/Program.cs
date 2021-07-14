using System;
using System.IO;
using System.Text.RegularExpressions;

namespace main
{
    class Program
    {
        static dynamic lua = new DynamicLua.DynamicLua();
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("No file found");
                    Console.WriteLine("Press enter to close...");
                    Console.ReadLine();
                }

                else
                {

                    string text = File.ReadAllText(args[0]);
                    string path = Path.GetDirectoryName(args[0]);
                    string getscript = ConverScript(text);
                    File.WriteAllTextAsync(path + @"\output.txt", getscript);
                    Console.ReadLine();
                }
            }
            catch (Exception exe)
            {
                Console.WriteLine(exe.Message);
            }
            //    string Path = @"C:\Users\XE\Documents\GitHub\UniversalConstantDumperV2\main\main\bin\Release\netcoreapp3.1\text.txt";
            //string script = System.IO.File.ReadAllText(Path);

            //string result = ConverScript(script);
            //Console.WriteLine(result);
            //    Console.ReadLine();
            return;
        }


        public static string GetText(Match match)
        {
            return ByteToString(match.Value);
        }

        static string ConverScript(string arg)
        {
            MatchEvaluator bytetostring = new MatchEvaluator(GetText);
            string result = Regex.Replace(arg, @"(.)\+=", @"$1=$1+");
            result = Regex.Replace(result, @"(.)\-=", @"$1=$1-");
            result = Regex.Replace(result, @"(.)\*=", @"$1=$1*");
            result = Regex.Replace(result, @"(.)\\=", @"$1=$1\");
            result = Regex.Replace(result, @"(\\\d{1,3})", bytetostring);


            result = PSUMathSolver(result);

            return result;
        }


        static string PSUMathSolver(string arg)
        {
            string result = arg;
            var matches = Regex.Matches(result, "(#\\(\"(.*?)\"\\))");
            foreach (Match match in matches)
            {

                GroupCollection groups = match.Groups;
                int lenghtofstring = groups[2].Value.Length;
                result = result.Replace(groups[1].Value.ToString(), lenghtofstring.ToString());
            }
            matches = Regex.Matches(result, "((\\d+)\\s*([+/*-])\\s*(\\d+))");
            foreach (Match match in matches)
            {
                int eval = 0;
                GroupCollection groups = match.Groups;
                switch (groups[3].Value)
                {
                    case ("+"):
                        eval = int.Parse(groups[2].Value) + int.Parse(groups[4].Value);
                        break;

                    case ("-"):
                        eval = int.Parse(groups[2].Value) - int.Parse(groups[4].Value);
                        break;

                    case ("*"):
                        eval = int.Parse(groups[2].Value) * int.Parse(groups[4].Value);
                        break;

                    case ("/"):
                        eval = int.Parse(groups[2].Value) / int.Parse(groups[4].Value);
                        break;

                }
                result = result.Replace(groups[1].Value.ToString(), eval.ToString());
            }



            matches = Regex.Matches(result, "(\\({1}(\\d+)\\){1}\\s*([+/*-])\\s*(\\d+))");
            foreach (Match match in matches)
            {
                int eval = 0;
                GroupCollection groups = match.Groups;
                switch (groups[3].Value)
                {
                    case ("+"):
                        eval = int.Parse(groups[2].Value) + int.Parse(groups[4].Value);
                        break;

                    case ("-"):
                        eval = int.Parse(groups[2].Value) - int.Parse(groups[4].Value);
                        break;

                    case ("*"):
                        eval = int.Parse(groups[2].Value) * int.Parse(groups[4].Value);
                        break;

                    case ("/"):
                        eval = int.Parse(groups[2].Value) / int.Parse(groups[4].Value);
                        break;

                }
                result = result.Replace(groups[1].Value.ToString(), eval.ToString());
            }

            result = Regex.Replace(result, "#{}\\s*[+/*-]\\s*(\\d+)", @"$1");
            return result;
        }

        static string ByteToString(string arg)
        {
            var result = lua(@$"return tostring('{arg}')");
            return result.ToString();
        }
    }
}
