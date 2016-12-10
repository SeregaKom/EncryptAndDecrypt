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
            int numberOfMatrices = 0;
            int index = 0;
            SortedDictionary<char, string> dictonaryColumns = new SortedDictionary<char, string>();
            char[] charArray = key.ToCharArray();
            int keyLenght = key.Length;
            ICollection<char> keys = dictonaryColumns.Keys;//коллекция ключей
            ICollection<string> columns = dictonaryColumns.Values;
            char[,] charMatrice = new char[keyLenght, keyLenght];

            numberOfMatrices = OriginalText.Length / (keyLenght * keyLenght);
            if (numberOfMatrices * keyLenght * keyLenght < OriginalText.Length)
                numberOfMatrices++;
            while (OriginalText.Length < numberOfMatrices * keyLenght * keyLenght) 
            {
                OriginalText += " ";
            }
            numberOfColumns = OriginalText.Length / keyLenght;
            for (int i = 0; i < numberOfColumns; i++)
            {
                index = 0;
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

                foreach (string c in columns)
                {
                    for (int i1 = 0; i1 < keyLenght; i1++)
                    {
                        charMatrice[i1, index] = c[i1];
                    }
                    index++;
                }
                WriteInEncryptText(keyLenght, charMatrice);
            }
            WriteOfTextInFile("encrypt_text.txt", EncryptText);
        }
        /// <summary>
        /// Запись текста из массива в зашифрованный текст
        /// </summary>
        /// <param name="keyLenght"></param>
        /// <param name="charMatrice"></param>
        public void WriteInEncryptText(int keyLenght, char[,] charMatrice)
        {
            for (int i = 0; i < keyLenght; i++)
            {
                for (int j = 0; j < keyLenght; j++)
                {
                    EncryptText += charMatrice[i, j];
                }
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
                key = "рев";// Console.ReadLine();
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
        /// <summary>
        /// Выбор операции (Шифрование или Дешифрование)
        /// </summary>
        public void ChoiceOfOperation()
        {
            bool flag = true;
            string operation = "";
            do
            {
                Console.WriteLine("Выберите вид операции (Шифрование = E, Расширование = D):");
                operation = "E";// Console.ReadLine();
                switch(operation)
                {
                    case "E":
                        Console.WriteLine("Шифрование текста");
                        OpenFileText("text.txt");
                        Console.WriteLine("Исходный текст: \n" + OriginalText);
                        Encryption(TestKey());
                        Console.WriteLine("Зашифрованный текст: \n{0}", EncryptText);
                        flag = false;
                        break;
                    case "D":
                        Console.WriteLine("Расшифрование текста");
                        OpenFileText("encrypt_text.txt");
                        Console.WriteLine("Зашифрованный текст: \n" + EncryptText);
                        Decryption(TestKey());
                        Console.WriteLine("Расшифрованный текст: \n{0}", OriginalText);
                        flag = false;
                        break;
                }
            } while (flag);
        }
        /// <summary>
        /// Открытие файла 
        /// </summary>
        /// <param name="typeCryption"></param>
        public void OpenFileText(string nameFile)
        {
            using (StreamReader sr = new StreamReader(nameFile))
            {
                if (sr != null)
                {
                    if (nameFile == "text.txt")
                    {
                        OriginalText = sr.ReadToEnd();
                    }
                    else
                        EncryptText = sr.ReadToEnd();
                }
                else
                    Console.WriteLine("Error!!! File text not found!");
            }
        }
        /// <summary>
        /// Запись в файл
        /// </summary>
        /// <param name="nameFile"></param>
        /// <param name="typeFile"></param>
        public void WriteOfTextInFile(string nameFile, string typeFile)
        {
            using (StreamWriter sw = new StreamWriter(nameFile))
            {
                if (sw != null)
                {
                    for (int i = 0; i < typeFile.Length; i++)
                        sw.Write(typeFile[i]);
                }
                else
                    Console.WriteLine("Error!!! File for writer not found!");
            }
        }
    }
}
