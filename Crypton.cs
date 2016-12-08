using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EncryptAndDecryptFileTXT
{
    class Crypton
    {
        public string OriginalText { get; set; } //строка, в которой содержится текст из файла txt
        public string EncryptText { get; set; }  //строка, в которую записывается зашифрованный текст

        public Crypton() { }
        /// <summary>
        /// Шифрование
        /// </summary>
        public void Encryption(string key)
        {
            int val = 0;
            int numberOfColumns = 0;
            SortedDictionary<char, string> dictonaryColumns = new SortedDictionary<char, string>();
            char[] charArray = key.ToCharArray();
            ICollection<char> keys = dictonaryColumns.Keys;//коллекция ключей
            
            numberOfColumns = OriginalText.Length / key.Length;
            for (int i = 0; i < numberOfColumns; i++)
            {
                dictonaryColumns.Clear();
                for (int j = 0; j < key.Length; j++)
                {
                    if (i + j < numberOfColumns)
                    {
                        dictonaryColumns.Add(charArray[j], OriginalText.Substring(val, key.Length));
                        val += key.Length;
                    }
                    else
                        break;
                }
                i = val / key.Length - 1;
                foreach (char k in keys)
                    EncryptText += dictonaryColumns[k];
            }
            if (OriginalText.Length > numberOfColumns * key.Length)
            {
                EncryptText += OriginalText.Substring(numberOfColumns * key.Length);
            }
            using (StreamWriter sw = File.CreateText("encrypt_text.txt"))
            {
                if (sw != null)
                {
                    for (int i = 0; i < EncryptText.Length; i++)
                        sw.Write(EncryptText[i]);
                }
                else
                    Console.WriteLine("Error!!! File for writer not found!");
            }
        }
        /// <summary>
        /// Расшифрование текста
        /// </summary>
        /// <param name="key"></param>
        public void Decryption(string key)
        {
            //Дешифрование будет проводиться обратным путем, только нужно расставить столбцы в соответствии с ключом, а это проблема еще та
            int val = 0;
            int numberOfColumns = 0;
            Dictionary<char, string> dictonaryColumns = new Dictionary<char, string>();
            char[] charArray = key.ToCharArray();
            Array.Sort(charArray);
            char[] charArrayKey = key.ToCharArray();

            numberOfColumns = EncryptText.Length / key.Length;
            for (int i = 0; i < numberOfColumns; i++)
            {
                dictonaryColumns.Clear();
                for (int j = 0; j < key.Length; j++)
                {
                    if (i + j < numberOfColumns)
                    {
                        dictonaryColumns.Add(charArray[j], EncryptText.Substring(val, key.Length));
                        val += key.Length;
                    }
                    else
                        break;
                }
                i = val / key.Length - 1;
                foreach (char c in charArrayKey)
                {
                    if (dictonaryColumns.ContainsKey(c))
                        OriginalText += dictonaryColumns[c];
                }
            }
            if (EncryptText.Length > numberOfColumns * key.Length)
            {
                OriginalText += EncryptText.Substring(numberOfColumns * key.Length);
            }
            using (StreamWriter sw = new StreamWriter("text.txt"))
            {
                if (sw != null)
                {
                    for (int i = 0; i < OriginalText.Length; i++)
                        sw.Write(OriginalText[i]);
                }
                else
                    Console.WriteLine("Error!!! File for writer not found!");
            }
        }
        /// <summary>
        /// Ввод слова-ключа
        /// </summary>
        /// <returns></returns>
        public string TestKey()
        {
            string key = "";
            bool flag = false;

            do
            {
                Console.WriteLine("Введите слово-ключ, не содержащее одинаковый буквы:");
                key = Console.ReadLine();
                for (int i = 0; i < key.Length; i++)
                {
                    if (flag != true)
                    {
                        for (int j = i + 1; j < key.Length; j++)
                        {
                            if (key[i] == key[j])
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
            } while (flag);
            return key;
        }
        public void ChoiceOfOperation()
        {
            bool flag = true;
            string operation = "";
            do
            {
                Console.WriteLine("Выберите вид операции (Шифрование = E, Расширование = D):");
                operation = Console.ReadLine();
                switch(operation)
                {
                    case "E":
                        Console.WriteLine("Шифрование текста");
                        OpenFileText("Encryption");
                        Encryption(TestKey());
                        Console.WriteLine("Зашифрованный текст: \n{0}", EncryptText);
                        flag = false;
                        break;
                    case "D":
                        Console.WriteLine("Расшифрование текста");
                        OpenFileText("Decryption");
                        Decryption(TestKey());
                        Console.WriteLine("Расшифрованный текст: \n{0}", OriginalText);
                        flag = false;
                        break;
                }
            } while (flag);
        }
        public void OpenFileText(string typeCryption)
        {
            if (typeCryption == "Encryption") 
            {
                using (StreamReader sr = new StreamReader("text.txt")) 
                {
                    if (sr != null)
                    {
                        OriginalText = sr.ReadToEnd();
                        Console.WriteLine("Оригинальный текст: \n{0}", OriginalText);
                    }
                    else
                        Console.WriteLine("Error!!! File text.txt not found!");
                }
            }
            else
            {
                if(typeCryption == "Decryption")
                {
                    using (StreamReader sr = new StreamReader("encrypt_text.txt"))
                    {
                        if (sr != null)
                        {
                            EncryptText = sr.ReadToEnd();
                            Console.WriteLine("Зашифрованный текст: \n{0}", EncryptText);
                        }
                        else
                            Console.WriteLine("Error!!! File encrypt_text.txt not found!");
                    }
                }
            }
        }
    }
}
