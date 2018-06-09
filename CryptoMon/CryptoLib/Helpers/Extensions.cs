using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Starts at the top of the BlockingCollection, breaks as soon as it finds an instance where the predicate is true.
        /// </summary>        
        /// <returns>Removes each instance of the blocking collection, ceases execution once it finds a true instance.</returns>
        public static void TakeAndBreak<T>(this BlockingCollection<T> collection, Func<T, bool> predicate)
        {            
            while (true)
            {
                try
                {
                    if(collection == null || collection.Count == 0)                    
                        break;                    
                    T value = collection.ElementAt<T>(0);                    
                    if (!predicate(value))  //evaluate the item at the top of the list against the predicate, determine if it needs to be removed.
                        collection.TryTake(out value);                                     
                    else
                        break; //When the predicate ceases to be true, end execution early. 
                }
                catch
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Dequeues objects from a ConcurrentQueue and returns an IEnumerable.
        /// </summary>
        /// <returns>The object at the top of the queue.</returns>
        public static IEnumerable<T> DequeueExisting<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
                yield return item;
        }        
    }
}
