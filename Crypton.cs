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
            int numberOfColumns = 0; //количество столбцов
            int numberOfMatrices = 0;//количество матриц
            int index = 0;           //индекс для заполнения матрицы
            SortedDictionary<char, string> dictonaryColumns = new SortedDictionary<char, string>();//словарь сортированных по ключу столбцов 
            char[] charArray = key.ToCharArray();//массив char, в котором находится ключ 
            int keyLenght = key.Length;//длина ключа
            ICollection<string> columns = dictonaryColumns.Values;//коллекция значей (столбцов)
            char[,] charMatrice = new char[keyLenght, keyLenght]; //матрица размером длина ключа * длина ключа

            numberOfMatrices = OriginalText.Length / (keyLenght * keyLenght);
            if (numberOfMatrices * keyLenght * keyLenght < OriginalText.Length)
                numberOfMatrices++;
            while (OriginalText.Length < numberOfMatrices * keyLenght * keyLenght)
                OriginalText += " ";
            numberOfColumns = OriginalText.Length / keyLenght;
            for (int i = 0; i < numberOfColumns; i++)
            {
                index = 0;
                dictonaryColumns.Clear();
                //сортировка по ключу
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
                //записываем столбы матрицу
                foreach (string c in columns)
                {
                    for (int i1 = 0; i1 < keyLenght; i1++)
                    {
                        charMatrice[i1, index] = c[i1];
                    }
                    index++;
                }
                //записываем в EncryptText
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
            int index = 0;
            int val = 0;
            int numberOfColumns = 0;
            Dictionary<char, string> dictonaryColumns = new Dictionary<char, string>();
            char[] charArrayKey = key.ToCharArray();
            List<string> listColumns = new List<string>(key.Length);
            char[,] charMatrice = new char[key.Length, key.Length]; //матрица размером длина ключа * длина ключа

            numberOfColumns = EncryptText.Length / key.Length;

            for (int i = 0; i < numberOfColumns; i++)
            {
                index = 0;
                dictonaryColumns.Clear();
                listColumns.Clear();
                for (int j = 0; j < key.Length; j++)
                {
                    if (i + j < numberOfColumns)
                    {
                        listColumns.Add(EncryptText.Substring(val, key.Length));
                        val += key.Length;
                    }
                    else
                        break;
                }
                i = val / key.Length - 1;
                //записываем столбы матрицу
                foreach (string l in listColumns)
                {
                    for (int i1 = 0; i1 < key.Length; i1++)
                    {
                        charMatrice[i1, index] = l[i1];
                    }
                    index++;
                }
                dictonaryColumns = ReadFromEncryptText(key, charMatrice);
                foreach (char c in charArrayKey)
                {
                    if (dictonaryColumns.ContainsKey(c))
                        OriginalText += dictonaryColumns[c];
                }
            }
            WriteOfTextInFile("text.txt", OriginalText);
        }
        /// <summary>
        /// Считывание из зашифрованного текста в массив
        /// </summary>
        /// <param name="keyLengt"></param>
        /// <returns></returns>
        private Dictionary<char, string> ReadFromEncryptText(string key, char[,] charMatrice)
        {
            string str = "";
            char[] charArraySort = key.ToCharArray();
            Array.Sort(charArraySort);
            Dictionary<char, string> dictionaryColumns = new Dictionary<char, string>(key.Length);
            for (int i = 0; i < key.Length; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    str += charMatrice[i, j];
                }
                dictionaryColumns.Add(charArraySort[i], str);
                str = "";
            }
            return dictionaryColumns;
        }
        /// <summary>
        /// Ввод слова-ключа
        /// </summary>
        /// <returns></returns>
        public string TestKey()
        {
            string key;
            bool flag;

            do
            {
                flag = false;
                key = "";
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
                operation = Console.ReadLine();
                switch(operation)
                {
                    case "E":
                        SetColorConsole(true);
                        Console.WriteLine("Шифрование текста");
                        OpenFileText("text.txt");
                        Console.WriteLine("Исходный текст:");
                        SetColorConsole(false);
                        Console.WriteLine(OriginalText);
                        SetColorConsole(true);
                        Encryption(TestKey());
                        Console.WriteLine("Зашифрованный текст:");
                        SetColorConsole(false);
                        Console.WriteLine(EncryptText);
                        OriginalText = "";
                        //flag = false;
                        break;
                    case "D":
                        SetColorConsole(true);
                        Console.WriteLine("Дешифрование текста");
                        OpenFileText("encrypt_text.txt");
                        Console.WriteLine("Исходный текст:");
                        SetColorConsole(false);
                        Console.WriteLine(EncryptText);
                        SetColorConsole(true);
                        Decryption(TestKey());
                        Console.WriteLine("Расшифрованный текст:");
                        SetColorConsole(false);
                        Console.WriteLine(OriginalText);
                        EncryptText = "";
                        //flag = false;
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
        private void SetColorConsole(bool fl)
        {
            if (fl)
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            else
                Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
