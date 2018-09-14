using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Basic_Accounting_System
{
    /*************************************************************************************
    About: This program is written as part of the fulfilment for the ‘Unit18: Procedural Programming (L4)’
    Course - HND in ‘Computing and Systems Development’ at Icon College, London.
    Date : ‘Put date here’
    By : Put your name here. Student ID: Put your student ID Here
    *************************************************************************************/

    // to repesent information about employee
    struct EmployeeInfo{
        public String id,pwd, name, job_title;
        public float rate;
    };
    // to represent information about invoice
    struct Invoice{
        public String id, mnth;
        public float amt;
        public bool accepted;
    };


    class Program
    {
        //employee file name
        static String EMPLOYEE_FILE = "emp_info.txt";
        //invoice file name
        static String INVOICES_FILE = "invoice_file.txt";

        static Hashtable employee_collection = new Hashtable(); //all employee collection
        static Hashtable invoice_collection = new Hashtable();  //all invoice collection


        static bool user_is_employee;   //true if user is employee
        static String user_id;  //contains user id

        static void Main(string[] args)
        {
            ReadDetails();
            bool login_successful = false;
            int option;
            while (true)
            {
                //print intitial menu
                option = InitialMenu();
                if (option == 3)
                    break;
                if (option == 1)
                {
                    user_is_employee = false;
                }
                else
                {
                    user_is_employee = true;
                }

                login_successful = LoginUser(); //read username and password of user
                if (login_successful)
                    break;
                else
                {
                    Console.ReadLine();
                }
                Console.Clear();
            }
            if (login_successful)
            {
                if (option == 1)
                    EmployerMenu(); //employeer Logged In
                else
                    EmployeeMenu(); //employee Logged In
            }
            WriteDataToFile(); // write data
            Console.Write("press Input to exit..");
            Console.Read();
        }

        public static bool LoginUser()
        {
            Console.Clear();
            string u_id, password;
            Console.Write("Input Username :: ");
            u_id = Console.ReadLine();
            if (u_id == "")
            {
                Console.WriteLine("Username Cannot Be Empty");
                return false;
            }
            Console.Write("Input Password :: ");
            password = Console.ReadLine();
            if (password == "")
            {
                Console.WriteLine("Password Cannot Be Empty");
                return false;
            }
            if (!user_is_employee)
            {
                if (u_id == "admin" && password == "admin")
                {
                    user_id = u_id;
                    return true;

                }
                else
                {
                    Console.WriteLine("Incorrect User Id/Password");
                    return false;
                }
            }

            else
            {
                if (!employee_collection.ContainsKey(u_id))
                {
                    Console.WriteLine("User Name Not Found...");
                    return false;
                }
                else
                {
                    EmployeeInfo e = (EmployeeInfo)employee_collection[u_id];
                    if (e.pwd != password)
                    {
                        Console.WriteLine("Incorrect Password..");
                        return false;
                    }
                    else
                    {
                        user_id = u_id;
                        return true;
                    }
                }
            }
        }

        //this method is used load data into hashtable
        public static void ReadDetails()
        {
            employee_collection = ReadEmployeeInfrormationFromFile();
            invoice_collection = ReadInvoiceFromFile();
        }

        //this method is used write data to file
        public static void WriteDataToFile()
        {
            WriteEmployeeToFile(employee_collection);
            WriteInvoiceToFile(invoice_collection);
        }

        //this method is used display menu if user is employer
        public static void EmployerMenu()
        {
            int option = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1 :: Register new employee\n2 :: Confirm the monthly payment\n3 :: List all employee id and name\n4 :: Exit");
                Console.Write("Input Option :: ");
                try
                {
                    option = int.Parse(Console.ReadLine());
                    if (option <= 0 || option > 4)
                        throw new Exception();
                    else if (option == 4)
                    {
                        break;
                    }
                    else
                    {
                        switch (option)
                        {
                            case 1: Console.Clear(); NewEmployee();
                                break;
                            case 2: Console.Clear(); EmployeeInvoice();
                                break;
                            case 3: Console.Clear(); PrintEmployees();
                                break;
                        }
                        Console.ReadLine();
                        Console.Clear();

                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Incorrect option..\nhit return to continue..\n");
                    Console.ReadLine();
                    continue;

                }
            }
        }

        //this method is used accept employee invoice
        public static void EmployeeInvoice()
        {
            Console.WriteLine("Invoices");
            Console.WriteLine("*************************\n");
            if (invoice_collection.Count <= 0)
            {
                Console.WriteLine("No Invoices..\nhit return to continue..");
                return;
            }
            else
            {
                Invoice invoice = new Invoice();
                EmployeeInfo employee = new EmployeeInfo();
                Hashtable temp_collection = new Hashtable();
                int i = 1;
                Console.WriteLine("Sr. No.    Employee Id    Employee Name    Amount    Month");
                Console.WriteLine("*******    ***********    *************    ******    *****");
                foreach (DictionaryEntry dict_entry in invoice_collection)
                {
                    invoice = (Invoice)dict_entry.Value;
                    if (invoice.accepted == true)
                        continue;
                    else
                    {
                        employee = (EmployeeInfo)employee_collection[dict_entry.Key];
                        char[] value_array = i.ToString().ToCharArray();
                        AlignOutput(value_array, 11);
                        value_array = invoice.id.ToCharArray();
                        AlignOutput(value_array, 15);
                        value_array = employee.name.ToCharArray();
                        AlignOutput(value_array, 17);
                        value_array = (invoice.amt + "").ToCharArray();
                        AlignOutput(value_array, 10);
                        value_array = invoice.mnth.ToCharArray();
                        AlignOutput(value_array, 14);
                        Console.Write("\n");
                        temp_collection.Add(i, invoice.id);
                        i++;
                    }
                }
                if (i == 1)
                {
                    Console.WriteLine("No Invoice Found..");
                    return;
                }
                Console.Write("\nInput Employee Invoice No. To Accept :: ");
                int invoiceNo = 0;
                try
                {
                    invoiceNo = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect Invoice No. \nNo Match..");
                    return;
                }
                if (!temp_collection.ContainsKey(invoiceNo))
                {
                    Console.WriteLine("Incorrect Invoice No. \nNo Match..");
                    return;
                }
                invoice = (Invoice)invoice_collection[(String)temp_collection[invoiceNo]];
                invoice.accepted = true;
                invoice_collection.Remove(invoice.id);
                invoice_collection.Add(invoice.id, invoice);
                Console.WriteLine("Accepted..\nhit return to continue..");

            }
        }

        //this method is used print list of all employees 
        public static void PrintEmployees()
        {
            Console.WriteLine("\nList of Employees");
            Console.WriteLine("*******************");
            Console.WriteLine("Employee Id    Employee Name    Job Title    Daily Rate");
            Console.WriteLine("***********    *************    *********    **********");
            foreach (DictionaryEntry d in employee_collection)
            {
                EmployeeInfo e = (EmployeeInfo)d.Value;
                char[] val = e.id.ToCharArray();
                AlignOutput(val, 15);
                val = e.name.ToCharArray();
                AlignOutput(val, 17);
                val = e.job_title.ToCharArray();
                AlignOutput(val, 13);
                val = (e.rate + "").ToCharArray();
                AlignOutput(val, 9);
                Console.Write("\n");
            }
            Console.WriteLine("\nhit return to continue..");
        }

        public static void AlignOutput(char[] val, int width)
        {
            if (val.Length > width - 1)
            {
                for (int i = 0; i < width - 3; i++)
                    Console.Write(val[i]);
                for (int i = 0; i < 2; i++)
                    Console.Write(".");
                Console.Write(" ");
            }
            else
            {
                for (int i = 0; i < val.Length; i++)
                    Console.Write(val[i]);
                for (int i = 0; i < width - val.Length; i++)
                    Console.Write(" ");
            }
        }

        //this method is used register new employee
        public static void NewEmployee()
        {
            EmployeeInfo e = new EmployeeInfo();
            try
            {

                Console.Write("Input Employee Name : ");
                e.name = Console.ReadLine();
                if (e.name == "")
                {
                    Console.WriteLine("\nEmployee Name Cannot Be Empty");
                    throw new Exception();
                }
                Console.Write("Input Job Title : ");
                e.job_title = Console.ReadLine();
                if (e.job_title == "")
                {
                    Console.WriteLine("\nJob Title Cannot Be Empty");
                    throw new Exception();
                }
                Console.Write("Input Daily Rate : ");
                try
                {
                    e.rate = float.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("\nIncorrect Daily Rate");
                    throw new Exception();
                }
                Console.Write("Input Emplyoee Id : ");
                e.id = Console.ReadLine();
                if (e.id == "")
                {
                    Console.WriteLine("\nEmployee Id Cannot be empty");
                    throw new Exception();
                }
                else if (employee_collection.ContainsKey(e.id))
                {
                    Console.WriteLine("\nEmployee Id Already Exists. No Match");
                    throw new Exception();
                }
                Console.Write("Input Password : ");
                e.pwd = Console.ReadLine();
                if (e.pwd == "")
                {
                    Console.WriteLine("\nPassword Cannot Be Empty");
                    throw new Exception();
                }
            }
            catch
            {
                Console.WriteLine("No Match....\n hit return to continue..");
                return;
            }
            employee_collection.Add(e.id, e);
            Console.WriteLine("Employee Added..");

        }

        //this method is used to generate employee menu
        public static void EmployeeMenu()
        {
            int option = 0;
            while (true)
            {
                Console.Clear();
                Console.Write("1 :: Check Payment\n2 :: Invoice Employer\n3 :: Exit\nPlease Input Your option :- ");
                try
                {
                    option = int.Parse(Console.ReadLine());
                    if (option <= 0 || option > 3)
                        throw new Exception();
                    else if (option == 3)
                    {
                        break;
                    }
                    else
                    {
                        switch (option)
                        {
                            case 1: Console.Clear(); PaymentCheck();
                                break;
                            case 2: Console.Clear(); GenerateInvoice();
                                break;
                        }
                        Console.ReadLine();
                        Console.Clear();
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Incorrect option..\nhit return to continue..\n");
                    Console.ReadLine();
                    continue;

                }
            }

        }

        //this method is used to check payment
        public static void PaymentCheck()
        {
            Console.WriteLine("\nInvoice List");
            Console.WriteLine("************");
            Invoice invoice;
            int invoiceCount = 0;
            Console.WriteLine("Month    Amount   Status");
            Console.WriteLine("*****    ******   ******");
            foreach (DictionaryEntry d in invoice_collection)
            {
                invoice = (Invoice)d.Value;
                if (invoice.id == user_id)
                {
                    invoiceCount++;
                    char[] val = invoice.mnth.ToCharArray();
                    AlignOutput(val, 9);
                    val = (invoice.amt + "").ToCharArray();
                    AlignOutput(val, 9);
                    val = (invoice.accepted ? "Pending" : "Accepted").ToCharArray();
                    AlignOutput(val, 15);
                    Console.Write("\n");
                }
            }
            if (invoiceCount == 0)
                Console.WriteLine("\nNo Invoice Found..");
            Console.WriteLine("\nhit return to continue...");
        }

        //this method is used to generate invoice
        public static void GenerateInvoice()
        {
            Invoice invoice = new Invoice();
            try
            {
                invoice.id = user_id;
                Console.Write("\nInput Days :: ");
                int noOfdays = 0;
                try
                {
                    noOfdays = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect Days");
                    throw new Exception();
                }
                if (noOfdays == 0)
                {
                    Console.WriteLine("Days Cannot Be Zero");
                    throw new Exception();
                }
                String month;
                Console.Write("Input Month :: ");
                month = Console.ReadLine();
                if (month == "")
                {
                    Console.WriteLine("Incorrect Month..");
                    throw new Exception();
                }
                float dailyRate = 0;
                Console.Write("Input Daily Rate :: ");
                try
                {
                    dailyRate = float.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Incorrect Rate");
                    throw new Exception();
                }
                if (dailyRate == 0)
                {
                    Console.WriteLine("Cannot Be Zero");
                    throw new Exception();
                }
                invoice.amt = dailyRate * noOfdays;
                invoice.mnth = month;


            }
            catch
            {
                Console.WriteLine("No Match....\n hit return to continue..");
                return;
            }
            invoice_collection.Remove(user_id);
            invoice_collection.Add(user_id, invoice);
            Console.WriteLine("Invoice Generated..");
            Console.WriteLine("\nhit return to continue..");
        }

        //this method is used to print initial menu on screen
        public static int InitialMenu()
        {
            bool do_loop = true;
            int option = 0;
            while (do_loop)
            {
                Console.Clear();

                Console.WriteLine("1 :: Employer\n2 :: Employee\n3 :: Exit\n");
                Console.Write("Input Option :: ");
                try
                {
                    option = int.Parse(Console.ReadLine());
                    if (option <= 0 || option > 3)
                        throw new Exception();

                }
                catch (Exception)
                {
                    Console.WriteLine("Incorrect option..\nhit return to continue..\n");
                    Console.ReadLine();

                }
                if (option <= 0 || option > 3)
                    do_loop = true;
                else
                    do_loop = false;
            }
            return option;
        }

        //this method is used read employee details
        private static Hashtable ReadEmployeeInfrormationFromFile()
        {
            Hashtable employee_collection = new Hashtable();
            //if employee file doesnot exist
            if (!File.Exists(EMPLOYEE_FILE))
                return employee_collection;
            else{ // read file
                StreamReader stream_reader = new StreamReader(new FileStream(EMPLOYEE_FILE, FileMode.Open));
                String line;
                String[] valueArr;
                EmployeeInfo employee = new EmployeeInfo();
                while ((line = stream_reader.ReadLine()) != null)    {
                    valueArr = line.Split(new char[] { ':' }, 5);
                    employee.id = valueArr[0];
                    employee.pwd = valueArr[1];
                    employee.name = valueArr[2];
                    employee.job_title = valueArr[3];
                    employee.rate = float.Parse(valueArr[4]);
                    employee_collection.Add(employee.id, employee);
                }
                stream_reader.Close();
            }
            return employee_collection;
        }

        //this method is used write employee details to the file
        private static Boolean WriteEmployeeToFile(Hashtable employee_collection)
        {
            bool res = true;

            //if file doesnot exist return hashtable
            if (employee_collection.Count == 0)
                return res;
            else // read data
            {
                StreamWriter stream_writer = null;
                try
                {
                    stream_writer = new StreamWriter(new FileStream(EMPLOYEE_FILE, FileMode.Create));
                    string line;
                    EmployeeInfo employee = new EmployeeInfo();
                    foreach (DictionaryEntry dict_entry in employee_collection)
                    {
                        employee = (EmployeeInfo)dict_entry.Value;
                        line = employee.id + ":" + employee.pwd + ":" + employee.name + ":" + employee.job_title + ":" + employee.rate;
                        stream_writer.WriteLine(line);
                    }
                }
                catch (Exception)
                {
                    res = false;
                }
                finally
                {
                    if (stream_writer != null)
                        stream_writer.Close();
                }
            }
            return res;
        }

        //this method is used read employee invoice details
        private static Hashtable ReadInvoiceFromFile()
        {
            Hashtable invoice_collection = new Hashtable();
           
            if (!File.Exists(INVOICES_FILE))
                return invoice_collection;
            else // read file
            {
                StreamReader stream_reader = new StreamReader(new FileStream(INVOICES_FILE, FileMode.Open));
                String line;
                String[] value_array;
                Invoice employee = new Invoice();
                for (; (line = stream_reader.ReadLine()) != null; )
                {
                    value_array = line.Split(new char[] { ':' }, 5);
                    employee.id = value_array[0];
                    employee.amt = float.Parse(value_array[1]);
                    employee.mnth = value_array[2];
                    employee.accepted = (value_array[3] == "1") ? true : false;
                    invoice_collection.Remove(employee.id);
                    invoice_collection.Add(employee.id, employee);
                }
                stream_reader.Close();
            }
            return invoice_collection;
        }

        //this method is used write write employee invoice details to the file
        private static Boolean WriteInvoiceToFile(Hashtable invoice_collection)
        {
            bool result = true;

            //if employee file doesnot exist return empty hashtable
            if (invoice_collection.Count == 0)
                return result;
            else // read data from hashtable and write it the file
            {
                StreamWriter stream_writer = null;
                try
                {
                    stream_writer = new StreamWriter(new FileStream(INVOICES_FILE, FileMode.Create));
                    string line;
                    Invoice employee = new Invoice();
                    foreach (DictionaryEntry d in invoice_collection)
                    {
                        employee = (Invoice)d.Value;
                        line = employee.id + ":" + employee.amt + ":" + employee.mnth + ":" + ((employee.accepted == true) ? "1" : "0");
                        stream_writer.WriteLine(line);
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
                finally
                {
                    if (stream_writer != null)
                        stream_writer.Close();
                }
            }
            return result;
        }

    }
}
