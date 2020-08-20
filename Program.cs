using System;
using System.Collections.Generic;
using System.IO;

namespace c_assignment_crud_AdolfStary
{
    class Program
    {

        /*

            Title: C# Introduction Assignment - CRUD
            Purpose: Application that will maintain a list of items and allow the user to manipulate that list.

            Author: Adolf Stary
            Last modified: Aug 14, 2020

        */
        static bool escapeRequested = false;
        static bool shutdown = false;
        static bool isBeingEdited = false;
        static bool editFailed = false;
        static int i = 0;
        static List<string> salaryList = new List<string>();
        static int menuSelected = 0;
        // Removed due to challenge - Dynamic list
        // static int listSize = 10;
        

        static void Main(string[] args)
        {
            // Welcomes user
            Welcome();        
            
            // Keeps returning to main menu until user chooses to quit
            do
            {

                DisplayMenu();

                switch(menuSelected)
                {
                    case 1: DisplayList();
                        break;
                    case 2: EditList();
                        break;
                    case 3: NewList();
                        break;
                    case 4: ImportFromFile(salaryList);
                        break;
                    case 5: ExportToFile(salaryList);
                        break;
                    case 10: shutdown = true;
                        break;
                    default:
                        break;
                }

            }while(!shutdown);

            Autosave();
        }


        //////////////////////////////////
        //////////////////////////////////
        // Methods
        //////////////////////////////////
        //////////////////////////////////

        //////////////////////////////////////////////////////
        // Autosave - asks user if they want to save list into file before exiting
        //////////////////////////////////////////////////////
        static void Autosave()
        {

            Console.WriteLine("Would you like to save your list before existing? (\'Y\' for yes, any other key for no)");
            if (Console.ReadKey().Key == ConsoleKey.Y) 
            {

                Console.WriteLine();
                ExportToFile(salaryList);
                
            }

        }

        //////////////////////////////////////////////////////
        // Displays current list of items, notifies user if list is empty.
        //////////////////////////////////////////////////////
        static void DisplayList()
        {

            if (salaryList.Count == 0) Console.WriteLine("Current list is empty.");
            else
            {
                Console.WriteLine("---------------------------------------\nThis is your current list:\n---------------------------------------\n");

                // Writes out all the items in the list
                foreach(string item in salaryList) Console.WriteLine((salaryList.IndexOf(item)+1) + ". " + item);
            }
            
        }


        //////////////////////////////////////////////////////
        // Allows user to edit list
        //////////////////////////////////////////////////////
        static void EditList()
        {

            ConsoleKey editSelection;
            bool validInput;
            
            // Asks user what editing action to perform, whether delete or edit
            Console.Write("---------------------------\nWhat type of edit would you like to perform?\nA - Delete an entry\nB - Edit an entry\nC - Add an entry\nR - Return to main menu\n");

            // Loops until valid entry is given
            do
            {
                // Reads input
                editSelection = Console.ReadKey().Key;
                Console.WriteLine();
                validInput = true;

                // Decides based on input
                if (editSelection == ConsoleKey.A) DeleteEntry();
                else if (editSelection == ConsoleKey.B) EditEntry();
                else if (editSelection == ConsoleKey.C) RepeatedEntry();
                else if (editSelection == ConsoleKey.R) Console.WriteLine("----------------------------------\nReturning to main menu...");
                else
                {
                    Console.Write("Invalid entry, try again: ");
                    validInput = false;
                }

            }while(!validInput);


        }


        //////////////////////////////////////////////////////
        // Delete Entry
        //////////////////////////////////////////////////////
        static void DeleteEntry()
        {

            string inputAsString;
            int selection;
            bool validInput = true;

            // Shows user the list so he can make a decision without scrolling
            DisplayList();

            // Code only runs if list is not empty
            if (salaryList.Count > 0)
            {
                
                // Loops until valid entry is given
                do
                {

                    if (!validInput)
                    {
                        Console.Write("Wrong input. ");
                    }
                    Console.Write("Enter number of the entry you want to delete: ");
                    inputAsString = Console.ReadLine();
                    validInput = int.TryParse(inputAsString, out selection);

                    // If selection is higher than amount of items or smaller than 1st position, it is also an invalid entry
                    if (selection > salaryList.Count || selection < 1) validInput = false;


                }while(!validInput);

                // Making sure user wants to delete entry, reminds which entry is being deleted, specific key needs to be pressed
                Console.Write($"Are you sure you want to delete entry #{selection}? (Y for yes, any other key for no): ");

                // Deletion process
                if (Console.ReadKey().Key == ConsoleKey.Y) 
                {
                    salaryList.RemoveAt(selection-1);
                    Console.WriteLine($"\nEntry #{selection} was deleted. Returning to main menu...");
                }
                else Console.WriteLine("Returning to main menu...\n---------------------------");

                
            }

        }


        //////////////////////////////////////////////////////
        // Edits a single entry
        //////////////////////////////////////////////////////
        static void EditEntry()
        {
            string inputAsString;
            int selection;
            bool validInput = true;


            // Shows list to user so they don't need to scroll
            DisplayList();

            // Script only runs if list is not empty
            if (salaryList.Count > 0)
            {
                
                // Loops until proper input is received
                do
                {

                    if (!validInput)
                    {
                        Console.Write("Wrong input. ");
                    }
                    Console.Write("Enter number of the entry you want to replace: ");
                    inputAsString = Console.ReadLine();
                    validInput = int.TryParse(inputAsString, out selection);

                    // Selection higher than number of items and smaller than 1st entry is invalid input
                    if (selection > salaryList.Count || selection < 1) validInput = false;

                }while(!validInput);

                // Translates selection into actual index number, sets switch logic within InputEntry to that an entry is being edited
                i = selection - 1;
                isBeingEdited = true;
                
                // Repeats InputEntry method using fail switch in case of duplication error
                do
                {
                    editFailed = false;
                    InputEntry();

                }while(editFailed);

                // Resets switch logic
                isBeingEdited = false;
                

                Console.WriteLine("Entry has been edited. Returning to main menu...");
                
            }

        }


        //////////////////////////////////////////////////////
        // Calls out welcome message
        //////////////////////////////////////////////////////
        static void Welcome()
        {
            Console.WriteLine("Hello, this app was developed to keep track of salaries within your company. ");
        }


        //////////////////////////////////////////////////////
        // Takes name of the employee as input, returns string value
        //////////////////////////////////////////////////////
        static string InputName()
        {
            
            bool nameInputValid = true;
            string inputAsString;

            // Loops until proper input is received
            do
            {

                nameInputValid = true;
                Console.Write($"Please enter the name of the person #{i+1}, or type 'done' to exit: ");
                inputAsString = Console.ReadLine();

                // Runs through the input to check whether it's a word/name, will fail if there are any special characters or numbers
                foreach (char letter in inputAsString)
                {
                        
                        if (!(Char.IsLetter(letter) || Char.IsWhiteSpace(letter))) nameInputValid = false;

                }

                // Stops asking for inputs if 'done' is received, case insensitive
                if (inputAsString.ToLower().Trim() == "done")
                {
                    escapeRequested = true;
                    nameInputValid = true;
                }
                else if (inputAsString.Trim() == "")
                {
                    nameInputValid = false;
                    Console.Write("Wrong input. ");
                }
                else if (nameInputValid)
                {
                    inputAsString = inputAsString.Trim(); // If everything is right, trims the input
                }
                else Console.Write("Wrong input. ");
                


            }while(!nameInputValid);
            
            return inputAsString;

        }


        //////////////////////////////////////////////////////
        // Complete input entry including salary and name
        //////////////////////////////////////////////////////
        static void InputEntry()
        {

            string name, listEntry;
            int salary = 0;
            escapeRequested = false;
            if (!isBeingEdited) i = salaryList.Count;

            // Gets name of the employee
            name = InputName();
            if(!escapeRequested) salary = InputSalary(name); // If user didn't decide to end the sequence early it follows with asking employee's salary

            // This code only runs if user didn't end input sequence early in either previous Input methods.
            if(!escapeRequested)
            {
                
                // Appends string to be saved and trims it, just in case
                listEntry =  (name + ": " + salary).Trim();

                // Calls Check for Duplicate method to check whether parameter listEntry is already in the list
                if (CheckForDuplicate(listEntry))
                {
                    
                    if(isBeingEdited) salaryList.RemoveAt(i); // This line only runs if the entry is being edited, deletes the old one

                    salaryList.Add(listEntry);

                    i++;                      
                    
                    salaryList.Sort(); // Sorts the list after each entry as requested in requirements

                }
                else
                {
                    Console.WriteLine("WARNING: Person with the same name is already in the system. Ignoring entry...");
                    if (isBeingEdited) editFailed = true; // Throws an edit fail for the Edit methods due to duplicate being in the list already
                }
                
            }

        }


        //////////////////////////////////////////////////////
        // Checks for duplicate, in list - case insensitive, inspired by https://stackoverflow.com/questions/1857513/get-substring-everything-before-certain-char
        //////////////////////////////////////////////////////
        static bool CheckForDuplicate(string listEntry)
        {

            bool noDuplicate = true;
            string name;
            int splitIndex;

            // Trim entry to just the name
            splitIndex = listEntry.IndexOf(":");
            listEntry = listEntry.Substring(0, splitIndex);

            foreach(string item in salaryList)
            {

                // Get only the name out of the existing item
                splitIndex = item.IndexOf(":");
                name = item.Substring(0, splitIndex);               

                // Check for case insensitive NAME duplicate
                if (name.ToLower() == listEntry.ToLower()) noDuplicate = false;

            }

            return noDuplicate;

        }



        //////////////////////////////////////////////////////
        // Allows user to input a salary
        //////////////////////////////////////////////////////
        static int InputSalary(string name)
        {

            bool validInput = true;
            string inputAsString;
            int inputAsInt;

            // Loops until proper input is received
            do
            {
                
                Console.Write($"Please enter the salary for {name}, or type 'done' to exit: ");
                inputAsString = Console.ReadLine();

                validInput = int.TryParse(inputAsString, out inputAsInt);

                if (inputAsInt < 0) validInput = false; // We don't want people to be paying for working

                // If user types done, entry exits into main menu, if his employee has to pay to work, program looks down on user and forbids such behavior to be documented.
                if (inputAsString.ToLower().Trim() == "done")
                {
                    escapeRequested = true;
                    validInput = true;
                }
                else if(validInput == false)
                {
                    if (inputAsInt < 0) Console.Write("I hope you're not charging that person to work for you, that's not right.\n");
                    else Console.Write("Wrong input. ");
                }
                


            }while(!validInput);

            return inputAsInt;
        }


        //////////////////////////////////////////////////////
        // Displays menu choices
        //////////////////////////////////////////////////////
        static void DisplayMenu()
        {

            ConsoleKey menuChoice;
            bool validInput;

            // Menu header and options
            Console.WriteLine("--------------------------------------------------\nWelcome to the main menu, press the number of your choice.\n--------------------------------------------------\nA - Display list of salaries\nB - Edit list\nC - New list\nD - Import data from file\nE - Export data into file\nQ - Exit Program");

            // Loops until proper input is received
            do
            {
                validInput = true;
                menuChoice = Console.ReadKey().Key;
                Console.Write("\n");

                // translates keys pressed into numeric value which, operates switch in Main()
                if (menuChoice == ConsoleKey.A) menuSelected = 1;
                else if (menuChoice == ConsoleKey.B) menuSelected = 2;
                else if (menuChoice == ConsoleKey.C) menuSelected = 3;
                else if (menuChoice == ConsoleKey.D) menuSelected = 4;
                else if (menuChoice == ConsoleKey.E) menuSelected = 5;
                else if (menuChoice == ConsoleKey.Q) menuSelected = 10;
                else
                {
                    validInput = false;
                    Console.Write("Invalid menu choice, please choose a menu item by corresponding letter: ");
                }



            }while(!validInput);
            
        }
        

        //////////////////////////////////////////////////////
        // Asks user if they want to erase current list, and clears it
        //////////////////////////////////////////////////////
        static void DeleteList()
        {

            // Only runs if list is not empty
            if (salaryList.Count > 0)
            {

                escapeRequested = false;
                Console.Write("WARNING: This action will erase existing list, are you sure you want to continue? (Y for yes, any other key for no): ");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    salaryList.Clear();         // Clears the list
                    i = 0;                      // Resets position counter
                }
                else escapeRequested = true;
                Console.WriteLine();
                
            }

        }


        //////////////////////////////////////////////////////
        // Exports list into a file
        //////////////////////////////////////////////////////
        static void ExportToFile(List<string> theList)
        {
                // Only runs if list has entries, if it doesn't lets user know that list is empty and skips code.
                if (salaryList.Count > 0)
                {
                    ConsoleKey input = ConsoleKey.A;
                    // Asks user if they want to overwrite file if the export is not being run by autosave
                    if (!shutdown)
                    {
                        if (File.Exists("data.bat")) Console.Write("Exporting data will overwrite the old data, do you wish to continue? (\'Y\' for yes, any other key for no): ");
                        input = Console.ReadKey().Key;
                    }


                    if(input == ConsoleKey.Y || shutdown)
                    {
                        // Deletes old file
                        if (File.Exists("data.bat")) File.Delete("data.bat");
                        
                        TextWriter dataFile;
                        dataFile = new StreamWriter("data.bat");

                        // Saves the list into 'data.bat'
                        foreach(string item in salaryList)
                        {
                            dataFile.WriteLine(item);
                        }
                        
                        dataFile.Close();
                        Console.Write("\nList was successfully saved into \'data.bat\' file. ");
                        // Output message based on whether method is being used by autosave or manual export
                        if (shutdown) Console.WriteLine("Exiting...");
                        else Console.WriteLine("Returning to main menu...");
                    

                    }
                    else Console.WriteLine("\nReturning to main menu...");

                }
                else Console.WriteLine("List is empty, nothing to export. Returning to main menu...");

        }

        //////////////////////////////////////////////////////
        // Imports data into list
        //////////////////////////////////////////////////////
        static void ImportFromFile(List<string> theList)
        {

            TextReader dataFile;
            // Checks for existing file, exits if import file doesn't exist, runs else if it does
            if(!File.Exists("data.bat")) Console.WriteLine("Data file \'data.bat\' doesn't exist in root folder. Returning to main menu...");
            else
            {

                string loadedData;
                dataFile = new StreamReader("data.bat");
                salaryList.Clear(); // Clears the list in the program to receive new data
                
                // Loads data from the file into list.
                do
                {
                    loadedData = dataFile.ReadLine();
                    if (loadedData != null) salaryList.Add(loadedData);
                    
                }while(loadedData != null);


                dataFile.Close();
                Console.WriteLine("Data was successfully loaded from \'data.bat\' file. Returning to main menu...");

            }
            
        }

        //////////////////////////////////////////////////////
        // Creates new list
        //////////////////////////////////////////////////////
        static void NewList()
        {

            // If user wants a new list, warns user, asks for confirmation, deletes list to make a new one, only runs if list is not empty
            DeleteList();            

            RepeatedEntry();

        }


        //////////////////////////////////////////////////////
        // Repeated input entry, keeps running InputEntry() until 'done' is typed in.
        //////////////////////////////////////////////////////
        static void RepeatedEntry()
        {
            
            escapeRequested = false;


            while(!escapeRequested /* List amount removed due to challenge of dynamic lists -- && i < listSize -- */)
            {
                
                InputEntry();

            }

        }


    }
}
