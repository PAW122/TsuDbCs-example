using System;

class Program
{
    static void Main()
    {
        TsunamiClient.InitNetworkManager(5855, null);
        Console.WriteLine("▶ Save...");
        int rc = TsunamiClient.Save("hello", "test", new byte[] {3,2,1}, 3);
        Console.WriteLine("Save rc = " + rc);
        if (rc != 0)
        {
            Console.WriteLine("❌ Save failed");
            return;
        }

        Console.WriteLine("▶ Read...");
        try
        {
            var data = TsunamiClient.ReadBytes("hello", "test");
            Console.WriteLine("✅ Read success: " + BitConverter.ToString(data));
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Read failed: " + ex.Message);
        }

        Console.WriteLine("Naciśnij Enter aby zakończyć...");
        Console.ReadLine();
    }
}
