using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BankOfGb
{
    enum AccountType { Saving =1,Current,FixedDeposit}
 //------------------- Transaction Class ------------------------   
    class Transaction
    {
        public string TransactionID {get; private set;}
        public DateTime Date {get; private set;}
        public string Type {get; private set;}
        public decimal Amount {get; private set;}
        public decimal BalanceAfter{get; private set;}
        
        public Transaction(string type,decimal amount,decimal balanceafter)
        {
            TransactionID = Guid.NewGuid().ToString().Substring(0,8);
            Date =DateTime.Now;
            Type =type;
            Amount= amount;
            BalanceAfter=balanceafter;
        }
        
        public void ShowTransaction()
        {
            Console.WriteLine($"{Date} | {TransactionID} | {Type} | Amount : {Amount:C} | Balance : {BalanceAfter:C}");
        }
    }
    
//--------------------- DepositRequest Class ---------------------
    class DepositRequest
    {
        public int AccountNumber{get;set;}
        public decimal Amount {get; set;}
        public string Status {get; set;}
        
        public DepositRequest(int accNo, decimal amount)
        {
            AccountNumber = accNo;
            Amount =amount;
            Status ="Pending";
        }
    }
//----------------------- BankAccount Cla---------------------------
   class BankAccount
   {
       public int AccountNumber {get; private set;}
       public string FullName {get; private set;}
       public int Age {get; private set;}
       public string Gender {get; private set;}
       public string Email {get; private set;}
       public string Phone {get; private set;}
       public string Address {get; private set;}
       public string Password {get; private set;}
       public decimal Balance {get; private set;}
       public AccountType Type {get; private set;}
       
       private List<Transaction>transactions = new List<Transaction>();
       private static Random random = new Random();
       
       public BankAccount(string fullName,int age,string gender,string email,string phone,string address,string password,AccountType type,decimal initialBalance)
       {
           AccountNumber = random.Next(100000,999999);
           FullName =fullName;
           Age =age ;
           Gender = gender;
           Email = email;
           Phone = phone;
           Address = address;
           Password = password;
           Type = type;
           Balance = initialBalance;
           transactions.Add(new Transaction("Initial Deposit",initialBalance,Balance));
       }
       
       public bool ValidatePassword(string input)=>Password ==input;
       
//------------- Balance Cheack  Method-----------------------       
       public void CheckBalance()
       {
           Console.WriteLine($@"
           Account Number: {AccountNumber}
           Holder        : {FullName}
           Balance       : {Balance}
           ");
       }
//--------------- Deposit Method --------------------------
       public void Deposit(decimal amount)
       {
           Balance +=amount;
           transactions.Add(new Transaction("Deposit",amount,Balance));
           Console.WriteLine($@"
           Deposited {amount}. 
           Available Balance : {Balance}
           ");
       }
  
  //------------------ Withdraw Method------------------     
       public void Withdraw(decimal amount)
       {
           if(amount <= 0)
           {
               Console.WriteLine("Amount must be positive !");
               return;
           }
           if(Balance >= amount)
           {
               Balance -= amount;
               transactions.Add(new Transaction("Withdrawal",amount,Balance));
               
               Console.WriteLine($@"
               Withdrawn {amount:C}
               Available Balance : {Balance:C}
               ");
           }
           else
           {
               Console.WriteLine("Insufficient Balance!");
           }
       }
//---------------------- Transfer Method       
       public void Transfer(BankAccount target,decimal amount)
       {
           if(amount <=0)
           {
               Console.WriteLine("Amount must be positive !");
               return;
           }
           
           if(Balance >= amount)
           {
               Balance -=amount;
               target.Balance += amount;
               transactions.Add(new Transaction($"Transfer to {target.AccountNumber}",amount,Balance));
               
               target.transactions.Add(new Transaction($"Transfer From {AccountNumber}",amount,target.Balance));
               Console.WriteLine($@"
               Transferred {amount:C} To {target.FullName}.
               Avalible Balance : {Balance:C}
               ");
           }
           else
           {
               Console.WriteLine("Insufficient balance to transfer!");
           }
       }
//--------------------- Interest method-----------
       public void AddInterest(decimal ratePercent)
       {
           if(Type == AccountType.Saving)
           {
               decimal interest = Balance * ratePercent /100;
               Balance +=interest;
               transactions.Add(new Transaction($"Interest {ratePercent}%",interest,Balance));
               Console.WriteLine($@"
               Interest added    : {interest:C}
               Available Balance : {Balance:C}
               ");
           }
       }
//--------------------- Show Details --------------
       public void ShowDetails()
       {
           Console.WriteLine($@"
           ------ Bank Of GB Account Details -------
           Account Number : {AccountNumber}
           Full Name      : {FullName}
           Age            : {Age}
           Gender         : {Gender}
           Email          : {Email}
           Phone          : {Phone}
           Address        : {Address}
           Account Type   : {Type}
           Balance        : {Balance:C}
           ");
       }
       
//---------------------- Show Transaction method
       public void ShowTransactions()
       {
           Console.WriteLine($"\n --- Transaction History for {FullName} ----");
           foreach(var t in transactions) t.ShowTransaction();
       }
   }
   
// ------------- Main Method Class -------------------------
   class Program
   {
       static List<BankAccount> accounts = new List<BankAccount>();
       static List<DepositRequest> pendingDeposits = new List <DepositRequest>();
       
       static void Main(String [] args)
       {
           Console.WriteLine("******** Welcome TO Bank Of GB ********");
           bool running = true;
           
           while (running)
           {
               Console.WriteLine("\n --- Main Menu ---");
               Console.WriteLine("1. Login Existing Account");
               Console.WriteLine("2. Create New Account ");
               Console.WriteLine("3. Bank Employee Login");
               Console.WriteLine("0. Exit");
               Console.Write("Select Option : ");
               string choice =Console.ReadLine();
               
               switch(choice)
               {
                   case "1":UserLogin();break;
                   case "2":CreateAccount();break;
                   case "3":EmployeeLogin();break;
                   case "0":running =false;break;
                   default:Console.WriteLine("Invaild Option!");break;
               }
           }
       }
//-------------------- Acccount Create -------------
       static void CreateAccount()
       {
           //---------Name 
           Console.Write("Enter full Name: ");
           string name =Console.ReadLine();
           //---------Age
           int age;
           while(true)
           {
               Console.Write("Enter Age:");
               if(int.TryParse(Console.ReadLine(),out age) && age>=18) break;
               Console.WriteLine("Age must be at least 18.");
           }
           //----------Gender
           Console.Write("Enter Gender: ");
           string gender =Console.ReadLine();
           
           //---------Email
           string email;
           while(true)
           {
               Console.Write("Enter Email :");
               email =Console.ReadLine();
               
               if(Regex.IsMatch(email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$")) break;
               Console.WriteLine("Invaild email format");
           }
           
           //---------Phone
           string phone;
           while(true)
           {
               Console.Write("Enter Phone Number:");
               phone = Console.ReadLine();
               
               if(Regex.IsMatch(phone,@"^\d{10}$"));break;
               Console.WriteLine("Invalid Phone number");
           }
           
           //----------Address
           Console.Write("Enter Address : ");
           string address =Console.ReadLine();
           //-----------Password
           Console.Write("set Password :");
           string password=Console.ReadLine();
           //----------Account Type
           Console.WriteLine("Select Account Type : 1 Saving | 2 Current | 3 FixDeposit");
           AccountType type =(AccountType)int.Parse(Console.ReadLine());
           //------------ InitialDeposit
           decimal initialDeposit;
           while(true)
           {
               Console.Write("Initial Deposit (min 500): ");
               initialDeposit=decimal.Parse(Console.ReadLine());
               
               if(initialDeposit >=500)break;
               Console.WriteLine("Minimum initial deposit is 500");
           }
           
           BankAccount account = new BankAccount(name,age,gender,email,phone,address,password,type,initialDeposit);
           accounts.Add(account);
           Console.WriteLine($"Account created successfully ! Account Number: {account.AccountNumber}");
       }
//----------------- Login 
       static BankAccount Login()
       {
           Console.Write("Enter Account Number:");
           int accNo =int.Parse(Console.ReadLine());
           
           var account = accounts.Find(a => a.AccountNumber == accNo);
           
           if(account == null)
           {
               Console.WriteLine("Account not Found!");
               return null;
           }
           
           Console.Write("Enter Password:");
           string pwd =Console.ReadLine();
           
           if(account.ValidatePassword(pwd)) return account;
           else{Console.WriteLine("Incorrect password!"); return null;}
       }
    //-----------user Menu   
       static void UserLogin()
       {
           var user = Login();
           if(user == null )return;
           
           bool running = true;
           
           while(running)
           {
               Console.WriteLine($"\n --- User Menu ({user.FullName})---");
               Console.WriteLine("1. Request Deposit");
               Console.WriteLine("2. Withdraw Money");
               Console.WriteLine("3. Transfer Money");
               Console.WriteLine("4. Check Balance");
               Console.WriteLine("5. Show Account Details");
               Console.WriteLine("6. Transaction Histroy");
               Console.WriteLine("0. Logout");
               Console.Write("Select Option: ");
               string choice = Console.ReadLine();
               
               switch(choice)
               {
                   case "1":UserDeposit(user);break;
                   case "2":Console.Write("Amount: "); 
                   user.Withdraw(decimal.Parse(Console.ReadLine()));
                   break;
                   case "3":
                   Console.Write("Target Account Number:"); 
                   int targetAcc = int.Parse(Console.ReadLine());
                  var target =accounts.Find(a => a.AccountNumber == targetAcc);
                  
                  if(target != null)
                  {
                      Console.Write("Amount: ");
                      decimal amt = decimal.Parse(Console.ReadLine());
                      user.Transfer(target,amt);
                  }
                  else Console.WriteLine("Target account not found!");
                  break;
                  
                  case "4":user.CheckBalance();break;
                  case "5":user.ShowDetails();break;
                  case "6":user.ShowTransactions();break;
                  case "0":running=false;break;
                  default: Console.WriteLine("Invalid Option");break;
               }
           }
       }
//---------------- User Deposit method
       static void UserDeposit(BankAccount user)
       {
           Console.Write("Enter deposit amount: ");
           decimal amount =decimal.Parse(Console.ReadLine());
           
           if(amount <=0){Console.WriteLine("Amount must be positive"); return;}
           
           pendingDeposits.Add(new DepositRequest(user.AccountNumber,amount));
           Console.WriteLine($"Deposit request of {amount:C} Submitted. Wait for bank approval");
       }
     
//-----------------Bank Admin login Method and menu 
       static void EmployeeLogin()
       {
           Console.Write("Enter Employee Id :");
           string empid =Console.ReadLine();
           
           Console.Write("Enter Employee Password :");
           string empPwd =Console.ReadLine();
           
           if(empid == "Admin" && empPwd == "admin123")
           {
               bool running = true;
               
               while(running)
               {
                   Console.WriteLine("\n --- Employee Menu ---");
                   Console.WriteLine("1. View All Accounts");
                   Console.WriteLine("2. Search Account by Number");
                   Console.WriteLine("3. Add Interest to Account");
                   Console.WriteLine("4. Delete Account");
                   Console.WriteLine("5. Approve Deposit Requests");
                   Console.WriteLine("0. Logout");
                   Console.Write("Select option:");
                   string choice = Console.ReadLine();
                   
                   switch(choice)
                   {
                       case "1":
                       foreach(var acc in accounts)acc.ShowDetails();break;
                       case "2":
                       Console.Write("Account Number:");
                       int searchAcc = int.Parse(Console.ReadLine());
                       var accFound =accounts.Find(a=>a.AccountNumber == searchAcc);
                       
                       if(accFound != null) accFound.ShowDetails();
                       else Console.WriteLine("Account not found!");break;
                       
                       case "3":
                       Console.Write("Account Number:");
                       int accInt =int.Parse(Console.ReadLine());
                       
                       var accInterest = accounts.Find(a=>a.AccountNumber == accInt);
                       
                       if(accInterest != null)
                       {
                           Console.Write("Enter Interest Rate (%):");
                           decimal rate =decimal.Parse(Console.ReadLine());
                           
                           accInterest.AddInterest(rate);
                       }
                       else Console.WriteLine("Account not found !"); break;
                       
                       case "4":
                       Console.Write("Account Number:");
                       int accDel =int.Parse(Console.ReadLine());
                       
                       var accToDel = accounts.Find(a=>a.AccountNumber == accDel);
                       if(accToDel != null)
                       {
                           accounts.Remove(accToDel);
                           Console.WriteLine("Account Deleted Successfully!");
                       }
                       else Console.WriteLine("Account not found!");break;
                       case "5":ApproveDepositRequests();break;
                       case "0": running =false; break;
                       default:Console.WriteLine("Invalid Option");break;
                   }
               }
           }
           else Console.WriteLine("Invalid Employee Credentials !");
       }
//--------------- deposit approve method       
       static void ApproveDepositRequests()
       {
           if(pendingDeposits.Count ==0)
           {
               Console.WriteLine("No Pending Deposit Request.");
               return;
           }
           for(int i = 0; i<pendingDeposits.Count;i++)
           {
               var req = pendingDeposits[i];
               Console.WriteLine($"{i +1}.Account:{req.AccountNumber}, Amount: {req.Amount:C}, Status: {req.Status}");
           }
           
           Console.Write("Enter Request number to Approve:");
           int choice =int.Parse(Console.ReadLine())-1;
           
           if(choice >=0 && choice < pendingDeposits.Count)
           {
               var request = pendingDeposits[choice];
               var account =accounts.Find(a=>a.AccountNumber == request.AccountNumber);
               if(account !=null)
               {
                   account.Deposit(request.Amount);
                   request.Status="Approved";
                   pendingDeposits.RemoveAt(choice);
                   Console.WriteLine("Deposit Approved Successfully!");
               }
               else Console.WriteLine("Account not found!");
           }
           else Console.WriteLine("Invalid selection.");
       }
   }
}
