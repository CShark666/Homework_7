using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static Semaphore barberReady = new Semaphore(0, 1);
    static Semaphore chairsAvailable;
    static object chairLock = new object();
    static Queue<int> waitingRoom = new Queue<int>();
    static int totalChairs = 6;


    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        chairsAvailable = new Semaphore(totalChairs, totalChairs);
        Thread barberThread = new Thread(Barber);
        barberThread.Start();

        Random random = new Random();
        int visitorId = 1;

        while (true)
        {
            Thread.Sleep(random.Next(1000, 5000));
            Thread visitorThread = new Thread(() => Visitor(visitorId++));
            visitorThread.Start();
        }
    }

    static void Barber()
    {
        while (true)
        {

            barberReady.WaitOne();

            int visitorId;
            lock (chairLock)
            {
                if (waitingRoom.Count > 0)
                {
                    visitorId = waitingRoom.Dequeue(); 
                    chairsAvailable.Release();
                }
                else
                {
                    continue;
                }
            }

            Console.WriteLine($"Перукар стриже відвідувача {visitorId}.");
            Thread.Sleep(3000);
            Console.WriteLine($"Відвідувач {visitorId} пострижений і покидає перукарню.");
        }
    }

    static void Visitor(int visitorId)
    {
        Console.WriteLine($"Відвідувач {visitorId} зайшов до перукарні.");


        if (chairsAvailable.WaitOne(0))
        {
            lock (chairLock)
            {
                waitingRoom.Enqueue(visitorId);
            }
            Console.WriteLine($"Відвідувач {visitorId} чекає на свою чергу.");


            barberReady.Release();
        }
        else
        {
            Console.WriteLine($"Відвідувач {visitorId} покидає перукарню, бо немає вільних крісел.");
        }
    }
}
