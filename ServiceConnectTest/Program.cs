using ObjectLayer;
using ServiceConnect;
using System;


namespace ServiceConnectTest
{
    class Program
    {

        static void Main(string[] args)
        {
            FillData();
            Console.ReadKey();
        }

        private async static void FillData()
        {
            HttpServiceAsyncRequest request = new HttpServiceAsyncRequest();
        }
    }

}

