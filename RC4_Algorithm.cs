using System;
using System.IO;

class RC4
{
    private static byte[] KeyStream(byte[] key, int length)
    {
        byte[] S = new byte[256];
        byte[] keyStream = new byte[length];

        for (int i = 0; i < 256; i++)
        {
            S[i] = (byte)i;
        }

        int j = 0;
        for (int i = 0; i < 256; i++)
        {
            j = (j + S[i] + key[i % key.Length]) % 256;
            byte temp = S[i];
            S[i] = S[j];
            S[j] = temp;
        }

        int x = 0, y = 0;
        for (int n = 0; n < length; n++)
        {
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;
            byte temp = S[x];
            S[x] = S[y];
            S[y] = temp;

            keyStream[n] = S[(S[x] + S[y]) % 256];
        }

        return keyStream;
    }

    public static string Encrypt(string plaintext, byte[] key)
    {
        byte[] keyStream = KeyStream(key, System.Text.Encoding.UTF8.GetBytes(plaintext).Length);
        byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        byte[] cipherBytes = new byte[plainBytes.Length];

        for (int i = 0; i < plainBytes.Length; i++)
        {
            cipherBytes[i] = (byte)(plainBytes[i] ^ keyStream[i]);
        }

        return Convert.ToBase64String(cipherBytes);
    }

    public static string Decrypt(string ciphertext, byte[] key)
    {
        byte[] keyStream = KeyStream(key, Convert.FromBase64String(ciphertext).Length);
        byte[] cipherBytes = Convert.FromBase64String(ciphertext);
        byte[] plainBytes = new byte[cipherBytes.Length];

        for (int i = 0; i < cipherBytes.Length; i++)
        {
            plainBytes[i] = (byte)(cipherBytes[i] ^ keyStream[i]);
        }

        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите приватный ключ (например, введите 'hello' без кавычек):");
        string inputKey = Console.ReadLine();
        
        byte[] userKey = System.Text.Encoding.UTF8.GetBytes(inputKey);

        Console.WriteLine("Выберите режим: 1 - Шифрование, 2 - Расшифрование");
        int mode = int.Parse(Console.ReadLine());
        
        string logFilePath = "log.txt";
        using (StreamWriter writer = File.AppendText(logFilePath))
        {
            writer.WriteLine($"Режим: {(mode == 1 ? "Шифрование" : "Расшифрование")}");
        }

        if (mode == 1)
        {
            Console.WriteLine("Введите строку для шифрования:");
            string input = Console.ReadLine();
            string encrypted = RC4.Encrypt(input, userKey);

            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine($"Входные данные: {input}");
                writer.WriteLine($"Зашифрованная строка: {encrypted}");
            }

            Console.WriteLine($"Зашифрованная строка: {encrypted}");
        }
        else if (mode == 2)
        {
            Console.WriteLine("Введите зашифрованную строку для расшифрования:");
            string input = Console.ReadLine();
            string decrypted = RC4.Decrypt(input, userKey);

            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine($"Зашифрованная строка: {input}");
                writer.WriteLine($"Расшифрованная строка: {decrypted}");
            }

            Console.WriteLine($"Расшифрованная строка: {decrypted}");
        }
        else
        {
            Console.WriteLine("Неправильно выбран режим. Завершение программы.");
        }
    }
}
