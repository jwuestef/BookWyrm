using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookWyrm.Data.DataContexts;
using BookWyrm.Data.Models;

namespace BookWyrm.Data.Services
{
    public static class ChronJobService
    {

        // Each day we need to calculate the daily late fees
        // Can not use scheduler like Quartz.NET for this because visitors may not hit the site every day...
        // ...or they may hit it multiple times in a day
        // We call a method from application start to see if today's calculations have been run
        // When we run a calculation, add today's date into a table in the DB


        public const double DAILY_LATE_FEE = 0.25;



        // See if today's calculations have been run, and if not, call the method to run the calculations
        // This should be called on application startup and/or on the home controller
        public static void CheckIfCalculationsNeeded()
        {
            // Query database and get the most recent date we ran the calculations, always stored at index 1
            using (TransactionDb _transactionDb = new TransactionDb())
            {
                ChronJob foundChrobJob = _transactionDb.ChronJobs.Find(1);

                // If the date is today, we already ran the calculations today... don't run them again
                string lastRunDate = foundChrobJob.DateRan.ToString("MM.dd.yyyy");
                string today = DateTime.Now.ToString("MM.dd.yyyy");

                if (lastRunDate == today)
                    return;

                DateTime originalDateRan = foundChrobJob.DateRan;

                // Change the DateRan to today, stop anyone else from the RunCalculations method
                foundChrobJob.DateRan = DateTime.Now;
                foundChrobJob.RandomKey = Guid.NewGuid();
                _transactionDb.SaveChanges();

                RunCalculations(originalDateRan, foundChrobJob.RandomKey.ToString());
            }
        }



        // Run the calculations and add today's date to the tracking table
        private static void RunCalculations(DateTime DateLastRun, string randomKey)
        {
            // First - VERY IMPORTANT... while very unlikely, it is technically possible that a 2nd user hit...
            // ...the CheckIfCalculationsNeeded() method while the first was in between CHECKING the value of DateRan...
            // ...and UPDATING the value of DateRan
            // ...which means the 2nd user would otherwise be calling this function a 2nd time, duplicating fees
            // So, we need handle that possibility here, give this async function a pause...
            // ...and then after the pause, check to see if the value of the RandomKey in the database is the same as...
            // ...the randomKey passed to this function
            // If it's the same, then no one else (no 2nd user) has changed the value in the database
            // If it's different, that means a 2nd user is also trying to call this function...
            // ...so stop this execution, and then when the 2nd user tries this check, their randomKey will match...
            // ...they will be allowed to proceed

            // Delay execution for a few moments
            Thread.Sleep(3000);

            // Query database and get the current value of the RandomKey
            using (TransactionDb _transactionDb = new TransactionDb())
            {
                ChronJob foundChrobJob = _transactionDb.ChronJobs.Find(1);

                // If the RandomKey from the database doesn't match the randomKey passed to this function, stop
                if (foundChrobJob.RandomKey.ToString() != randomKey)
                    return;
            }

            // If we made it here, our randomKey matched the one in the database, it's safe to calculate fees now
            // Loop over all entries in the Borrowing table that haven't been returned yet

            using (BookDb _bookDb = new BookDb())
            {
                // Find all books that have not been returned, and that are past the due date
                // The .ToList() method is necessary since we'll use _bookDb later inside this loop, If we didn't trigger this
                //  ...immediate execution, then we'd get an error later since we'd still be using the _bookDb during the iteration
                var unreturnedLateBooks = _bookDb.Borrowings.Where(bw =>
                    (bw.CheckInDateTime == null)
                    &&
                    (DbFunctions.TruncateTime(bw.DueDate) < DbFunctions.TruncateTime(DateTime.Now))
                ).ToList();

                foreach (Borrowing unreturnedLateBook in unreturnedLateBooks)
                {
                    // For this book, determine whether the due date, or the DateLastRan, is more recent
                    // Then calculate the number of days since that date (hopefully only 1, unless no one visits the site often)
                    // Finally, calculate the late fees based off that number of days

                    // If the due date is more recent, then base the fees off the due date... otherwise base it off the DateLastRun
                    DateTime dateThatLateFeesNeedToBeCalculatedFrom = unreturnedLateBook.DueDate.Date > DateLastRun.Date ?
                        unreturnedLateBook.DueDate.Date : DateLastRun.Date;

                    double numberOfDaysOfFees = (DateTime.Now.Date - dateThatLateFeesNeedToBeCalculatedFrom).TotalDays;
                    double amountOfNewFees = Math.Round(numberOfDaysOfFees * DAILY_LATE_FEE, 2);

                    // Now add this entry to the Transactions table and then update the balance for this user
                    using (TransactionDb _transactionDb = new TransactionDb())
                    {
                        // Get the details of this book, for the notes section of the transaction
                        Book foundBook = _bookDb.Books.Where(bk => bk.BookId == unreturnedLateBook.BookId).FirstOrDefault();
                        if (foundBook == null)
                            throw new KeyNotFoundException("Book Id not found, unable to create notes");

                        string notesToAdd = "[" + DateTime.Now.ToShortDateString() + "] " + amountOfNewFees.ToString("C") +
                            " late fee created for unreturned overdue book: \"" + foundBook.Title + "\", due on " +
                            unreturnedLateBook.DueDate.ToShortDateString() + ".";

                        // Add the transaction to the database
                        Transaction newTransaction = new Transaction()
                        {
                            TransactionId = Guid.NewGuid(),
                            PersonId = unreturnedLateBook.UserId,
                            DateApplied = DateTime.Now,
                            Amount = amountOfNewFees,
                            BookId = unreturnedLateBook.BookId,
                            Notes = notesToAdd
                        };

                        _transactionDb.Transactions.Add(newTransaction);
                        _transactionDb.SaveChanges();

                        // Now, for that user, update their balance
                        RecalculateBalanceForUser(unreturnedLateBook.UserId);
                    }
                }

            }
        }





        private static void RecalculateBalanceForUser(string UserId)
        {
            using (IdentityDb _identityDb = new IdentityDb())
            {
                // Find the user
                ApplicationUser foundUser = _identityDb.Users.Where(u => u.Id == UserId).FirstOrDefault();
                if (foundUser == null)
                    throw new KeyNotFoundException("UserId not found in database, unable to update user's balance");

                // Now search the Transaction table for all transactions for this user, and add up the amounts
                double updatedBalance = 0;
                using (TransactionDb _transactionDb = new TransactionDb())
                {
                    IQueryable<Transaction> allTransactionsForUser = _transactionDb.Transactions.Where(t => t.PersonId == UserId);
                    foreach (Transaction eachTransaction in allTransactionsForUser)
                    {
                        updatedBalance += eachTransaction.Amount;
                    }
                }

                // Update the user's balance
                foundUser.Balance = updatedBalance;
                _identityDb.SaveChanges();
            }
        }





    }
}
