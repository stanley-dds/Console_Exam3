using System.Data;
using System.Runtime.InteropServices;

namespace dtp15_todolist
{
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        private static string fileInUse;
        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "aktiv";
                case Waiting: return "väntande";
                case Ready: return "avklarad";
                default: return "(felaktig)";
            }
        }
        public static string getFileInUse() { return fileInUse; }
        public class TodoItem
        {
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem(int priority, string task)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = "";
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }

        //This method has been modified
        // Added: 1) default argument, 2) check if file exists, 3) clearing data from the previous file.
        public static void ReadListFromFile(string nameOfFile = "TomkisToDo.lis")
        {
          //  string todoFileName = "todo.lis";
            Console.Write($"Läser från fil {nameOfFile} ... ");
            if (File.Exists(nameOfFile))
            {
                fileInUse = nameOfFile;
                StreamReader sr = new StreamReader(nameOfFile);
                 int numRead = 0;

           
                list.Clear();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    TodoItem item = new TodoItem(line);
                    list.Add(item);
                    numRead++;
                }
                sr.Close();
                Console.WriteLine($"Läste {numRead} rader.");
            }
            else Console.WriteLine($"Filen {nameOfFile} finns inte! Ange ett annat namn");
        }
        public static async void WriteListToFile(string nameOfFile = "TomkisToDo.lis")
        {
            Console.Write($"Sparas till default filen {nameOfFile} ... \n");
            if (File.Exists(nameOfFile))
            {
                if (nameOfFile == fileInUse)
                {
                    using (StreamWriter sr = new StreamWriter(nameOfFile))
                    {
                        foreach (TodoItem item in list)
                        {
                            sr.WriteLine($"{item.status}|{item.priority}|{item.task}|{item.taskDescription}");
                            Console.WriteLine($"Saved: {item.status}|{item.priority}|{item.task}|{item.taskDescription}");

                        }
                        sr.Close();
                    }

                    
                }
                else Console.WriteLine($"ERROR! En annan fil är i bruk! Spara uppgifterna till [{fileInUse}] istället!");

            }
            else  Console.WriteLine($"Filen {nameOfFile} finns inte! Ange ett annat namn");

        }


        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintTodoList(bool verbose = false, string sortedTasks = "allt")
        {
            PrintHead(verbose);            
            if (sortedTasks == "allt")
            {
                foreach (TodoItem item in list)
                {
                    item.Print(verbose);
                }
            }
            else if (sortedTasks == "aktiva")
            {
                foreach (TodoItem item in list)
                {
                    if(item.status == 1) item.Print(verbose);
                }
            }
            else if (sortedTasks == "väntande")
            {
                foreach (TodoItem item in list)
                {
                    if (item.status == 2) item.Print(verbose);
                }
            }
            else if (sortedTasks == "klara")
            {
                foreach (TodoItem item in list)
                {
                    if (item.status == 3)  item.Print(verbose);
                }
            }



            PrintFoot(verbose);
        }
        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:\n");
            Console.WriteLine("ladda                   Skriv ladda filnamn.lis för specifierad val av filen");
            Console.WriteLine("hjälp                   lista denna hjälp");
            Console.WriteLine("lista                   lista att-göra-listan");
            Console.WriteLine("sluta                   spara att-göra-listan och sluta");
        }

        public static bool ChangeStatus(string[] command)
        {
            if (command[0] == "aktivera")
            {
                string subcommand = " ";
                for (int i = 1; i < command.Length; i++)
                { 
                    subcommand += command[i] + " ";
                }
                subcommand = subcommand.Trim();
                foreach (TodoItem item in list)
                {
                    if(item.task == subcommand)
                    {
                        item.status = 1; Console.WriteLine($"status for {item.task.ToUpper()} changed");
                        return true;
                    }
                }
            }
            else if (command[0] == "vänta")
            { }
            else if (command[0] == "klar")
            { }
            return false;
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");
         //   Todo.ReadListFromFile();
            Todo.PrintHelp();
            string command;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "hjälp"))
                {
                    Todo.PrintHelp();
                }
                else if (MyIO.Equals(command, "sluta"))
                {
                    Console.WriteLine("Hej då!");
                    break;
                }
                else if (MyIO.Equals(command, "lista"))
                {
                    if (MyIO.HasArgument(command, "allt")) { } // command has both LISTA and ALLT
                    else if (MyIO.HasArgument(command, "väntande")) { } // command has both LISTA and VÄNTANDE
                    else if (MyIO.HasArgument(command, "klara")) { } // command has both LISTA and KLARA

                    else if (!MyIO.HasArgument(command)) Todo.PrintTodoList(verbose: false, "aktiva"); // visar bara aktiva, kommando.Length <2


                    else Console.WriteLine($"ERROR! Okänt kommando: {command}"); // has to be LISTA and dsrgtd

                }
                else if (MyIO.Equals(command, "beskriv"))
                {
                    // Todo.PrintTodoList(verbose: true); transfer this to command "beskriv allt"
                    if (!MyIO.HasArgument(command)) { Todo.PrintTodoList(verbose: true, "aktiva"); } // show active with description
                    else if (MyIO.HasArgument(command, "allt")) Todo.PrintTodoList(verbose: true); // show all with description
                    else Console.WriteLine($"ERROR! Okänt kommando: {command}");

                }
                else if (MyIO.Equals(command, "ladda"))
                {
                    if (MyIO.HasArgument(command)) { }
                    else if (!MyIO.HasArgument(command)) Todo.ReadListFromFile();
                }
                else if (MyIO.Equals(command, "spara"))
                {
                    string fileName = Todo.getFileInUse();
                    if (fileName != null)
                    {
                        if (MyIO.HasArgument(command)) { }
                        else if (!MyIO.HasArgument(command))
                        {
                            Todo.WriteListToFile();
                        }
                    }
                    else Console.WriteLine("Ingen fil var laddade! Ladda filen first!");


                }
                else if (MyIO.Equals(command, "aktivera"))
                {
                    if (MyIO.HasArgument(command)) { }
                    else
                    { Console.WriteLine($"ERROR! Skriv aktivera och ett korrekt namn på uppgiften!"); }
                   //
                }
                else if (MyIO.Equals(command, "klar"))
                { }
                else if (MyIO.Equals(command, "vänta"))
                { }

                else
                {
                    Console.WriteLine($"Okänt kommando: {command}");
                }
            }
            while (true);
        }
    }
    class MyIO
    {
        static public string ReadCommand(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        static public bool Equals(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords[0] == expected) return true;
            }
            return false;
        }
        static public bool HasArgument(string rawCommand, string expected = "")
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords.Length < 2) return false;

          //   if (cwords[1] == expected) return true; // other commands except "ladda" and "spara"

                if (cwords[0] == "ladda")
                { 
                    expected = cwords[1]; 
                    Todo.ReadListFromFile(expected);
                    return true;
                }
                if (cwords[0] == "spara")
                {
                    expected = cwords[1]; // name of file
                    Todo.WriteListToFile(expected);                    
                    return true;
                }
                if (cwords[0] == "lista")
                {
                    if (cwords[1] == "allt")
                    { Todo.PrintTodoList(verbose: false); return true; }
                    if (cwords[1] == "väntande")
                    { Todo.PrintTodoList(verbose: false, "väntande"); return true; }
                    if (cwords[1] == "klara")
                    { Todo.PrintTodoList(verbose: false, "klara"); return true; }

                    if ((cwords.Length >= 2) && (cwords[1] != "allt") && (cwords[1] != "väntande") && (cwords[1] != "klara")) return false;
                  //  if (expected == "")
                   // { Todo.PrintTodoList(verbose: false, "aktiva"); return true; }
                }
                if (cwords[0] == "beskriv")
                {
                    if (cwords[1] == "allt") { return true; }
                    if ( (cwords.Length >= 2) && (cwords[1] != "allt")) return false;
                       
                }
                if (cwords[0] == "aktivera" || cwords[0] == "klar" || cwords[0] == "vänta")
                {
                    if (Todo.ChangeStatus(cwords)) return true;
                    else Console.WriteLine("status was not changed");
                    
                }



                return false;
            }
        }
    }
}
